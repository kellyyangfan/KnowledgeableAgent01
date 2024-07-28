using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;


public class AzureRealTime : MonoBehaviour
{
    
    SpeechConfig config;
    AudioConfig audioInput;
    SpeechRecognizer recognizer;
    PushAudioInputStream pushStream;

    public event EventHandler completeSpeech;

    public string SubscriptionKey;
    public string Region;

    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public string completeMessage;
    [HideInInspector] public DateTime startTime;

    private object threadLocker = new object();
    private bool recognitionStarted = false;
    private bool firstRecongition = true;
    
    private string message;
    private string lastMessage;
    
    int lastSample = 0;


    void Start()
    {
        message = "";
        config = SpeechConfig.FromSubscription(SubscriptionKey, Region);
        config.SpeechRecognitionLanguage = "en-US";
        //config.SpeechRecognitionLanguage = "es-MX";


        pushStream = AudioInputStream.CreatePushStream();
        audioInput = AudioConfig.FromStreamInput(pushStream);
        recognizer = new SpeechRecognizer(config, audioInput);

        recognizer.Recognizing += RecognizingHandler;
        recognizer.Recognized += RecognizedHandler;
        recognizer.Canceled += CanceledHandler;
        audioSource = GameObject.Find("SpeechRealTime").GetComponent<AudioSource>();
    }


    private byte[] ConvertAudioClipDataToInt16ByteArray(float[] data)
    {
        MemoryStream dataStream = new MemoryStream();
        int x = sizeof(Int16);
        Int16 maxValue = Int16.MaxValue;
        int i = 0;
        while (i < data.Length)
        {
            dataStream.Write(BitConverter.GetBytes(Convert.ToInt16(data[i] * maxValue)), 0, x);
            ++i;
        }
        byte[] bytes = dataStream.ToArray();
        dataStream.Dispose();
        return bytes;
    }

    private void RecognizingHandler(object sender, SpeechRecognitionEventArgs e)
    {
        lock (threadLocker)
        {
            message = e.Result.Text;
            if (firstRecongition)
            {
                startTime = System.DateTime.Now;
                firstRecongition = false;
            }
            // Debug.Log("1. RecognizingHandler: " + message);
        }
    }

    private void RecognizedHandler(object sender, SpeechRecognitionEventArgs e)
    {
        lock (threadLocker)
        {
            message = e.Result.Text;
            completeMessage = completeMessage + message;
            // Debug.Log("2. RecognizedHandler: " + message);
        }
    }

    private void CanceledHandler(object sender, SpeechRecognitionCanceledEventArgs e)
    {
        lock (threadLocker)
        {
            message = e.ErrorDetails.ToString();
            Debug.Log("CanceledHandler: " + message);
        }
    }

    // We activate or deactivate the recognition
    public async void RecognitionActivate()
    {
        if (recognitionStarted)
        {
            StopCoroutine(CheckMessageChange());
            await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(true);


            //StartCoroutine(StopRecordMicrophone());

            
            if (Microphone.IsRecording(Microphone.devices[0]))
            {
               // Debug.Log("Microphone.End: " + Microphone.devices[0]);
                Microphone.End(null);
                lastSample = 0;
            }

            lock (threadLocker)
            {
                recognitionStarted = false;
            }
        }
        else
        {
            StartCoroutine(CheckMessageChange());
            
            if (!Microphone.IsRecording(Microphone.devices[0]))
            {
                audioSource.clip = Microphone.Start(Microphone.devices[0], true, 3000, 16000);
                //Debug.Log("Microphone.Start: " + Microphone.devices[0]);
                //Debug.Log("audioSource.clip channels: " + audioSource.clip.channels);
                //Debug.Log("audioSource.clip frequency: " + audioSource.clip.frequency);
            }

            
            await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false); 
            lock (threadLocker)
            {
                recognitionStarted = true;
            }
            
        }
    }


    private IEnumerator StopRecordMicrophone()
    {
        yield return new WaitForEndOfFrame();
        if (Microphone.IsRecording(Microphone.devices[0]))
        {
            // Debug.Log("Microphone.End: " + Microphone.devices[0]);
            Microphone.End(null);
            lastSample = 0;
        }

        lock (threadLocker)
        {
            recognitionStarted = false;
        }
    }


    public void clearVar()
    {
        completeMessage = "";
        message = "";
        lastMessage = "";
        firstRecongition = true;
}

    // Check if there is another thing to send
    private IEnumerator CheckMessageChange()
    {
        bool continueCheck = true;
        while (continueCheck)
        {
            lastMessage = message;
            yield return new WaitForSeconds(1.3f);
            if (lastMessage == message && lastMessage != "")
            {
                completeSpeech?.Invoke(this, EventArgs.Empty);
                continueCheck = false;
            }
        }
    }

    void Disable()
    {
        recognizer.Recognizing -= RecognizingHandler;
        recognizer.Recognized -= RecognizedHandler;
        recognizer.Canceled -= CanceledHandler;
        pushStream.Close();
        recognizer.Dispose();
    }

    void FixedUpdate()
    {
        if (Microphone.IsRecording(Microphone.devices[0]) && recognitionStarted == true)
        {
            int pos = Microphone.GetPosition(Microphone.devices[0]);
            int diff = pos - lastSample;

            if (diff > 0)
            {
                float[] samples = new float[diff * audioSource.clip.channels];
                audioSource.clip.GetData(samples, lastSample);
                byte[] ba = ConvertAudioClipDataToInt16ByteArray(samples);
                if (ba.Length != 0)
                {
                    //Debug.Log("pushStream.Write pos:" + Microphone.GetPosition(Microphone.devices[0]).ToString() + " length: " + ba.Length.ToString());
                    pushStream.Write(ba);
                }
            }
            lastSample = pos;
        }
    }
}
