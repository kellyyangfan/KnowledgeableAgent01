using System;
using UnityEngine;
using System.Threading;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;

public class AzureTextToSpeech : MonoBehaviour
{
    // Hook up the four properties below with a Text, InputField, Button and AudioSource object in your UI.

    [HideInInspector] public string characterSpeech;
    public AudioSource audioSource;
    public event EventHandler completeSpeech;

    public string SubscriptionKey;
    public string Region;
    private const int SampleRate = 24000;
    private bool audioSourceNeedStop;
    private SpeechConfig speechConfig;
    private SpeechSynthesizer synthesizer;

  

    public void startTextToSpeech()
    {
        // We can't await the task without blocking the main Unity thread, so we'll call a coroutine to
        // monitor completion and play audio when it's ready.
        if(characterSpeech != "")
        {
            var speakTask = synthesizer.StartSpeakingTextAsync(characterSpeech);
            StartCoroutine(SpeakRoutine(speakTask));
        }
        else
        {
            StartCoroutine(Waiting());
        }
    }

    IEnumerator Waiting()
    {
        yield return new WaitForSeconds(1.5f);
        completeSpeech?.Invoke(this, EventArgs.Empty);
    }



    IEnumerator SpeakRoutine(Task<SpeechSynthesisResult> speakTask)
    {
        var startTime = DateTime.Now;

        while (!speakTask.IsCompleted)
        {
            yield return null;
        }

        var result = speakTask.Result;
        {
            if (result.Reason == ResultReason.SynthesizingAudioStarted)
            {
                // Native playback is not supported on Unity yet (currently only supported on Windows/Linux Desktop).
                // Use the Unity API to play audio here as a short term solution.
                // Native playback support will be added in the future release.
                var audioDataStream = AudioDataStream.FromResult(result);
                while (!audioDataStream.CanReadData(4092 * 2)) // audio clip requires 4096 samples before it's ready to play
                {
                    yield return null;
                }

                var isFirstAudioChunk = true;
                var audioClip = AudioClip.Create(
                    "Speech",
                    SampleRate * 600, // Can speak 10mins audio as maximum
                    1,
                    SampleRate,
                    true,
                    (float[] audioChunk) =>
                    {
                        var chunkSize = audioChunk.Length;
                        var audioChunkBytes = new byte[chunkSize * 2];
                        var readBytes = audioDataStream.ReadData(audioChunkBytes);
                        if (isFirstAudioChunk && readBytes > 0)
                        {
                            var endTime = DateTime.Now;
                            var latency = endTime.Subtract(startTime).TotalMilliseconds;
                            isFirstAudioChunk = false;
                        }

                        for (int i = 0; i < chunkSize; ++i)
                        {
                            if (i < readBytes / 2)
                            {
                                audioChunk[i] = (short)(audioChunkBytes[i * 2 + 1] << 8 | audioChunkBytes[i * 2]) / 32768.0F;
                            }
                            else
                            {
                                audioChunk[i] = 0.0f;
                            }
                        }

                        if (readBytes == 0)
                        {
                            Thread.Sleep(220); // Leave some time for the audioSource to finish playback
                            audioSourceNeedStop = true;
                            
                        }
                    });

                audioSource.clip = audioClip;
                audioSource.Play();
            }
        }
    }


    void Start()
    {
        // For the salsa Speech


        // Creates an instance of a speech config with specified subscription key and service region.
        speechConfig = SpeechConfig.FromSubscription(SubscriptionKey, Region);

        // The default format is RIFF, which has a riff header.
        // We are playing the audio in memory as audio clip, which doesn't require riff header.
        // So we need to set the format to raw (24KHz for better quality).
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw24Khz16BitMonoPcm);
        //speechConfig.SpeechSynthesisVoiceName = "en-GB-RyanNeural";
        speechConfig.EndpointId = "e1c07780-adee-4f61-b68a-44b69c0cc5e4";
        //speechConfig.SpeechSynthesisVoiceName = "en-US-EmmaNeural";
        //speechConfig.SpeechSynthesisVoiceName = "es-MX-BeatrizNeural";

        speechConfig.SpeechSynthesisVoiceName = "Albert EinsteinNeural";


        // SpeechServiceConnection_SynthVoice
        //speechConfig.SpeechSynthesisVoiceName("Albert EinsteinNeural");
        // Creates a speech synthesizer.
        // Make sure to dispose the synthesizer after use!
        synthesizer = new SpeechSynthesizer(speechConfig, null);

        synthesizer.SynthesisCanceled += (s, e) =>
        {
            var cancellation = SpeechSynthesisCancellationDetails.FromResult(e.Result);
        };
        
    }

    void Update()
    {

        if (audioSourceNeedStop)
        {
            audioSource.Stop();
            audioSourceNeedStop = false;
            completeSpeech?.Invoke(this, EventArgs.Empty);
        }
    }

    void OnDestroy()
    {
        if (synthesizer != null)
        {
            synthesizer.Dispose();
        }
    }
}
// </code>
