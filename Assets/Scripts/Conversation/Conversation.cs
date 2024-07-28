using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace OpenAI
{
    public class Conversation : MonoBehaviour
    {
        [SerializeField] private ChatGPT chatgpt;
        [SerializeField] private Whisper whisper;

        // For add the text into the Canva
        //[SerializeField] private Text messagePlayer;
        //[SerializeField] private Text messageChatGPT;
        
        [SerializeField] private SaveTextToSpeech saveTextToSpeech;
        [SerializeField] private AudioSource audiosource;
        [SerializeField] private int MaxDurationAudio;
        [SerializeField] private Animator animator;

        private bool isRecording = false;
        private bool isReadyToRecord = true;
        

        void Start()
        {
            // Subscription for the correct use of this project
            whisper.transcriptWhisper += startChatGPT;
            chatgpt.answerChatGPT += voice;
        }

        private void startChatGPT(object sender, EventArgs e)
        {
            //messagePlayer.text = whisper.messagePlayer;
            chatgpt.inputChat = whisper.messagePlayer;
            chatgpt.SendReply();
           
        }


        // Here is where the animation takes place
        private void voice(object sender, EventArgs e)
        {
            //messageChatGPT.text = chatgpt.lastMessage;
            string animationN = selectAnimation();
            animator.SetBool(animationN, true);
            var clip = saveTextToSpeech.TextToSpeechToByteArray(chatgpt.lastMessage);
            audiosource.clip = clip;
            StartCoroutine(WaitForAudioToFinish(audiosource, animationN));
        }

        // If you want to select a diferent animation for each answer could be posible
        private string selectAnimation()
        {
            int randomNumber = UnityEngine.Random.Range(1, 4);
            if (randomNumber == 1)
            {
                return "IsTalking";
            }
            else if (randomNumber == 2)
            {
                return "IsTalking";
            }
            else
            {
                return "IsTalking";
            }
        }


        IEnumerator WaitForAudioToFinish(AudioSource audioSource, string animationN)
        {
            audioSource.Play();
            yield return new WaitWhile(() => audioSource.isPlaying);
            isRecording = false;
            isReadyToRecord = true;
            animator.SetBool(animationN, false);
        }



        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (isReadyToRecord)
                {
                    if (!isRecording)
                    {
                        Debug.Log("Start to recording");
                        //Debug.Log("----------------------------------------");
                        whisper.StartRecording();
                        isRecording = true;
                    }
                    else
                    {
                        Debug.Log("Stop to recording");
                        //Debug.Log("----------------------------------------");
                        whisper.EndRecording();
                        isRecording = false;
                        isReadyToRecord = false;
                    }
                }
                else
                {                    
                    Debug.Log("Not ready to record");
                    //Debug.Log("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                }
            }
        }
    }
}