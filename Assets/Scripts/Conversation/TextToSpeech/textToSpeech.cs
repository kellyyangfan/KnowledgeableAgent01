using UnityEngine;
using SpeechLib;


public class textToSpeech : MonoBehaviour
{

    SpVoice voice = new SpVoice();

    
    public void startVoice(string message)
    {
        ISpeechObjectTokens availableVoices = voice.GetVoices();
        voice.Voice = availableVoices.Item(1);

        Debug.Log("Starting text to Speech...");
        voice.Speak(message, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
    }
}
