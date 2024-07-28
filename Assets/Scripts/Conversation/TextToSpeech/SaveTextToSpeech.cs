using UnityEngine;
using System.IO;
using SpeechLib;

public class SaveTextToSpeech : MonoBehaviour
{
    public AudioClip TextToSpeechToByteArray(string text)
    {
        string tempFileName = "temp.wav";
        SpVoice voice = new SpVoice();
        SpFileStream fileStream = new SpFileStream();

        ISpeechObjectTokens availableVoices = voice.GetVoices();
        voice.Voice = availableVoices.Item(0);

        fileStream.Format.Type = SpeechAudioFormatType.SAFT44kHz16BitStereo;
        fileStream.Open(tempFileName, SpeechStreamFileMode.SSFMCreateForWrite, false);

        voice.AudioOutputStream = fileStream;
        voice.Speak(text, SpeechVoiceSpeakFlags.SVSFDefault);

        fileStream.Close();
        byte[] fileBytes = File.ReadAllBytes(tempFileName);
        File.Delete(tempFileName);

        return OpenWavParser.ByteArrayToAudioClip(fileBytes);
    }
}
