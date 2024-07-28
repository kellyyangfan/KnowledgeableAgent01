using System;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Threading;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

namespace OpenAI
{
    public class StreamingChatGPT : MonoBehaviour
    {
        [SerializeField] private string APIkey;
        [SerializeField] private string APIOrg;

        [HideInInspector] public string inputChat;
        [HideInInspector] public string realGPT;
        [HideInInspector] public string pastAnswer;
        [HideInInspector] public string newSpeech;
        [HideInInspector] public string spokenSpeech;
        [HideInInspector] public string updateSpeechString;

        [HideInInspector] public bool lastMessage = false;

        public event EventHandler startSpeech;
        public event EventHandler updateSpeech;
        bool continueCheck;

        private OpenAIApi openai;

        private CancellationTokenSource token = new CancellationTokenSource();
        
        [SerializeField] private string prompt;
        [SerializeField] private string knowledge;
        [HideInInspector] private string knowledgeBank;

        [HideInInspector] public List<ChatMessage> messages = new List<ChatMessage>();
        public List<UserAssistantPair> dialogueExamples = new List<UserAssistantPair>();

        void Start()
        {
            openai = new OpenAIApi(APIkey);
        }

        public void SendMessage()
        {
            //Debug.Log("--------------- Check sender ---------------");
            StartCoroutine(CreateAudioSender());
            //var message = new List<ChatMessage>

            if (messages.Count == 0)
            {
                var behaviour = new ChatMessage()
                {
                    Role = "system",
                    Content = prompt + "\n" + knowledgeBank
                };
                messages.Add(behaviour);

                foreach (UserAssistantPair pair in dialogueExamples)
                {
                    var examplesChat1 = new ChatMessage()
                    {
                        Role = "user",
                        Content = pair.userAnswer
                    };
                    messages.Add(examplesChat1);

                    var examplesChat2 = new ChatMessage()
                    {
                        Role = "assistant",
                        Content = pair.assistantAnswer
                    };
                    messages.Add(examplesChat2);
                }

            }

            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputChat
            };
            messages.Add(newMessage);
            openai.CreateChatCompletionAsync(new CreateChatCompletionRequest()
            {
                //Model = "gpt-3.5-turbo-0301",
                Model = "gpt-4-0613",
                Messages = messages,
                Stream = true
            }, HandleResponse, finishedAnswer, token);
        }

        public void LogMessages()
        {
            if (messages.Count == 0)
            {
                Debug.Log("No messages in the list.");
                return;
            }

            foreach (var message in messages)
            {
                Debug.Log(message.Role + " \nSaid\n " + message.Content);
            }
        }



        private IEnumerator CreateAudioSender()
        {
            continueCheck = true;
            while (continueCheck)
            {
                //Debug.Log("-------------Reload Audio----------------");
                yield return new WaitForSeconds(0.33f);
                string localRealGPT;


                // Access realGPT securely

                localRealGPT = realGPT; 
                

                string[] realGPTWords = localRealGPT.Split(' ');
                string[] spokenSpeechWords = spokenSpeech.Split(' ');

                // If spokenSpeech is empty, add first word automatically
                if (string.IsNullOrEmpty(spokenSpeech) && realGPTWords.Length > 0)
                {
                    spokenSpeech = realGPTWords[0];
                    if (realGPTWords[0] != "" )
                    {
                        updateSpeechString = realGPTWords[0];
                        startSpeech?.Invoke(this, EventArgs.Empty);
                    }

                }
                else if (realGPTWords.Length > spokenSpeechWords.Length) // If there is a new word
                {
                    List<string> newWords = realGPTWords.ToList().GetRange(spokenSpeechWords.Length, realGPTWords.Length - spokenSpeechWords.Length - 1); // Delete the last word
                    spokenSpeech += (string.IsNullOrEmpty(spokenSpeech) ? "" : " ") + string.Join(" ", newWords);
                    updateSpeechString = string.Join(" ", newWords);
                    updateSpeech?.Invoke(this, EventArgs.Empty);
                    //Debug.Log(string.Join(" ", newWords));
                }
            }
        }

        private void finishedAnswer()
        {
            continueCheck = false;
            StopCoroutine(CreateAudioSender());
            StartCoroutine(FinishedAnswerCoroutine());
        }

        private IEnumerator FinishedAnswerCoroutine()
        {
            yield return new WaitForSeconds(0.75f);  // Wait

            string localRealGPT;
            localRealGPT = realGPT;
            string[] realGPTWords = localRealGPT.Split(' ');
            string[] spokenSpeechWords = spokenSpeech.Split(' ');



            //string remainingWords = string.Join(" ", realGPT.Split(' ').Skip(spokenSpeech.Split(' ').Length));

            List<string> newWords = realGPTWords.ToList().GetRange(spokenSpeechWords.Length, realGPTWords.Length - spokenSpeechWords.Length);
            spokenSpeech += (string.IsNullOrEmpty(spokenSpeech) ? "" : " ") + string.Join(" ", newWords);
            updateSpeechString = string.Join(" ", newWords);
            updateSpeech?.Invoke(this, EventArgs.Empty);

                /*
                 * spokenSpeech += " " + remainingWords;
                 * updateSpeechString = remainingWords;
                 * updateSpeech?.Invoke(this, EventArgs.Empty);
                */
            
            if (!lastMessage)
            {
                var gptMessage = new ChatMessage()
                {
                    Role = "assistant",
                    Content = spokenSpeech
                };
                lastMessage = true;
                messages.Add(gptMessage);
            }
            else
            {
                // Debug.Log("---- Repeated end ----");
                messages.RemoveAt(messages.Count - 1);
                var gptMessage = new ChatMessage()
                {
                    Role = "assistant",
                    Content = spokenSpeech
                };
                lastMessage = true;
                messages.Add(gptMessage);
                
                token.Cancel();
                openai = new OpenAIApi();
                token = new CancellationTokenSource();
            }

        }
        

        private void HandleResponse(List<CreateChatCompletionResponse> responses)
        {

            realGPT = string.Join("", responses.Select(r => r.Choices[0].Delta.Content));
            
        }

        public void ClearStrings()
        {
            inputChat = "";
            realGPT = "";
            pastAnswer = "";
            newSpeech = "";
            spokenSpeech = "";
            updateSpeechString = "";
            lastMessage = false;
        }

        private void OnDestroy()
        {
            token.Cancel();
        }
    }
}


[System.Serializable]
public class UserAssistantPair
{
    public string userAnswer;
    public string assistantAnswer;
}