using System;
using UnityEngine;
using UnityEngine.UI;

namespace OpenAI
{
    public class Whisper : MonoBehaviour
    {
        private AudioClip clip;
        private string fileName;
        private OpenAIApi openai = new OpenAIApi();
        
        public event EventHandler transcriptWhisper;

        [SerializeField] private Dropdown dropdown;
        [HideInInspector] public string messagePlayer;
        [HideInInspector] public int maxDuration = 40;

        private void Start()
        {
            // Whisper
            #if UNITY_WEBGL && !UNITY_EDITOR
            dropdown.options.Add(new Dropdown.OptionData("Microphone not supported on WebGL"));
            #else
            foreach (var device in Microphone.devices)
            {
                dropdown.options.Add(new Dropdown.OptionData(device));
            }
            dropdown.onValueChanged.AddListener(ChangeMicrophone);
            var index = PlayerPrefs.GetInt("user-mic-device-index");
            dropdown.SetValueWithoutNotify(index);
            #endif
        }

        private void ChangeMicrophone(int index)
        {
            PlayerPrefs.SetInt("user-mic-device-index", index);
        }
        
        public void StartRecording()
        {
            messagePlayer = String.Empty;
            var index = PlayerPrefs.GetInt("user-mic-device-index");
            #if !UNITY_WEBGL
            clip = Microphone.Start(dropdown.options[index].text, false, maxDuration, 44100);
            #endif
        }

        public async void EndRecording()
        {
            messagePlayer = "Transcripting...";
            
            #if !UNITY_WEBGL
            Microphone.End(null);
            #endif
            fileName = $"{System.Guid.NewGuid().ToString()}.wav";

            byte[] data = OpenWavParser.AudioClipToByteArray(clip);
            //byte[] data = SaveWav.Save(fileName, clip);


            var req = new CreateAudioTranscriptionsRequest
            {
                FileData = new FileData() { Data = data, Name = fileName }, //"audio.wav" },
                //File = "C:\\temp\\New" + fileName,
                Model = "whisper-1",
                Language = "en"
            };
            var res = await openai.CreateAudioTranscription(req);
            messagePlayer = res.Text;

            transcriptWhisper?.Invoke(this, EventArgs.Empty);
            Debug.Log(res.Text);
        }
    }
}
