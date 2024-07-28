
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


namespace OpenAI
{
    public class ChatGPT : MonoBehaviour
    {
        [HideInInspector] public string inputChat;
        [HideInInspector] public string lastMessage;
        public event EventHandler answerChatGPT;
        private OpenAIApi openai = new OpenAIApi();


        private List<ChatMessage> messages = new List<ChatMessage>();
        [SerializeField] private string prompt;

        void Start()
        {
            changeBehaviour(1);
        }

        public void changeBehaviour(int whatBehaviour)
        {

            Debug.Log("Behaviour");
            Debug.Log(whatBehaviour);

            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            messages.Clear();
            if (sceneIndex == 1)
            {
                if (whatBehaviour == 1)
                {
                    // Inoccent -- Normal behavior
                    prompt = "You are in a pub, facing questions about a recent car accident that occurred in a random city. Various vehicles from random companies are involved in the incident. Set random city fot this crash. People around you are questioning you.. Respond to the questions in informal manner, you are guilty";  //maintaining your stance of innocence at all times";
                }
                else if (whatBehaviour == 2)
                {
                    // Maintain your stance, despite your guilt -- Normal behavior
                    prompt = "You are in a pub, facing questions about a recent car accident that occurred in a random city. Various vehicles from random companies are involved in the incident. In this case, you are actually at fault for what happened, but people around you are questioning you. Respond to the questions in an informal manner, while attempting to maintain your stance, despite your guilt";
                }
                else if (whatBehaviour == 3)
                {
                    // Inoccent -- Good pleasend
                    prompt = "You are in a pub, facing questions about a recent car accident that occurred in a random city. Various vehicles from random companies are involved in the incident. Although you bear no fault in what happened, people around you are questioning you. Respond to the questions in an informal, yet pleasant manner, while maintaining your stance of innocence at all times";
                }
                else if (whatBehaviour == 4)
                {
                    // Maintain your stance, despite your guilt -- Good pleasend
                    prompt = "You are in a pub, facing questions about a recent car accident that occurred in a random city. Various vehicles from random companies are involved in the incident. In this case, you are actually at fault for what happened. However, people around you are questioning you. Respond to the questions in an informal, yet pleasant manner, while maintaining your stance of innocence, despite your guilt";
                }
                else if (whatBehaviour == 5)
                {
                    // Inoccent -- Unpleasend
                    prompt = "You are in a pub, facing questions about a recent car accident that occurred in a random city. Various vehicles from random companies are involved in the incident. Although you bear no fault in what happened, people around you are questioning you. Respond to the questions in an informal, yet unpleasant manner, while maintaining your stance of innocence at all times";
                }
                else if (whatBehaviour == 6)
                {
                    // Maintain your stance, despite your guilt -- Unpleasend
                    prompt = "You are in a pub, facing questions about a recent car accident that occurred in a random city. Various vehicles from random companies are involved in the incident. In this case, you are actually at fault for what happened. However, people around you are questioning you. Respond to the questions in an informal, yet unpleasant manner, while maintaining your stance of innocence, despite your guilt";
                }

            }
            else if (sceneIndex == 2)
            {
                if (whatBehaviour == 1)
                {
                    // Innocent -- Normal behavior
                    prompt = "You are in a museum, facing questions about a recent car accident that occurred in a random city. Various vehicles from random companies are involved in the incident. Although you bear no fault in what happened, people around you are questioning you. Respond to the questions in an informal manner, maintaining your stance of innocence at all times";
                }
                else if (whatBehaviour == 2)
                {
                    // Maintain your stance, despite your guilt -- Normal behavior
                    prompt = "You are in a museum, facing questions about a recent car accident that occurred in a random city. Various vehicles from random companies are involved in the incident. In this case, you are actually at fault for what happened, but people around you are questioning you. Respond to the questions in an informal manner, while attempting to maintain your stance, despite your guilt";
                }
                else if (whatBehaviour == 3)
                {
                    // Innocent -- Good pleasant
                    prompt = "You are in a museum, facing questions about a recent car accident that occurred in a random city. Various vehicles from random companies are involved in the incident. Although you bear no fault in what happened, people around you are questioning you. Respond to the questions in an informal, yet pleasant manner, while maintaining your stance of innocence at all times";
                }
                else if (whatBehaviour == 4)
                {
                    // Maintain your stance, despite your guilt -- Good pleasant
                    prompt = "You are in a museum, facing questions about a recent car accident that occurred in a random city. Various vehicles from random companies are involved in the incident. In this case, you are actually at fault for what happened. However, people around you are questioning you. Respond to the questions in an informal, yet pleasant manner, while maintaining your stance of innocence, despite your guilt";
                }
                else if (whatBehaviour == 5)
                {
                    // Innocent -- Unpleasant
                    prompt = "You are in a museum, facing questions about a recent car accident that occurred in a random city. Various vehicles from random companies are involved in the incident. Although you bear no fault in what happened, people around you are questioning you. Respond to the questions in an informal, yet unpleasant manner, while maintaining your stance of innocence at all times";
                }
                else if (whatBehaviour == 6)
                {
                    // Maintain your stance, despite your guilt -- Unpleasant
                    prompt = "You are in a museum, facing questions about a recent car accident that occurred in a random city. Various vehicles from random companies are involved in the incident. In this case, you are actually at fault for what happened. However, people around you are questioning you. Respond to the questions in an informal, yet unpleasant manner, while maintaining your stance of innocence, despite your guilt";
                }

            }
            else
            {
                if (whatBehaviour == 1)
                {
                    // Innocent -- Serious behavior
                    prompt = "You are albert Einstein, i write a book about the relativity theory give me feedback, you're personality is fun and you like to make jokes";


                }
                else if (whatBehaviour == 2)
                {
                    // Maintain your stance, despite your guilt -- Serious behavior
                    prompt = "You are in an interrogation room, facing questions about a recent car accident that occurred in a random city. Various vehicles from random companies are involved in the incident. In this case, you are actually at fault for what happened but you are trying to hide it, but authorities are questioning you. Respond to the questions in a serious and formal manner, while attempting to maintain your stance, despite your guilt";
                }
                else if (whatBehaviour == 3)
                {
                    // Innocent -- Serious and respectful
                    prompt = "You are in an interrogation room, facing questions about a recent car accident that occurred in a random city. Various vehicles from random companies are involved in the incident. Although you bear no fault in what happened, authorities are questioning you. Respond to the questions in a serious, yet respectful manner, while maintaining your stance of innocence at all times";
                }
                else if (whatBehaviour == 4)
                {
                    // Maintain your stance, despite your guilt -- Serious and respectful
                    prompt = "You are in an interrogation room, facing questions about a recent car accident that occurred in a random city. Various vehicles from random companies are involved in the incident. In this case, you are actually at fault for what happened. However, authorities are questioning you. Respond to the questions in a serious, yet respectful manner, while maintaining your stance of innocence, despite your guilt";
                }
                else if (whatBehaviour == 5)
                {
                    // Innocent -- Serious but terse
                    prompt = "You are in an interrogation room, facing questions about a recent car accident that occurred in a random city. Various vehicles from random companies are involved in the incident. Although you bear no fault in what happened, authorities are questioning you. Respond to the questions in a serious, but terse manner, while maintaining your stance of innocence at all times";
                }
                else if (whatBehaviour == 6)
                {
                    // Maintain your stance, despite your guilt -- Serious but terse
                    prompt = "You are in an interrogation room, facing questions about a recent car accident that occurred in a random city. Various vehicles from random companies are involved in the incident. In this case, you are actually at fault for what happened. However, authorities are questioning you. Respond to the questions in a serious, but terse manner, while maintaining your stance of innocence, despite your guilt";
                }

            }
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                changeBehaviour(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                changeBehaviour(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                changeBehaviour(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                changeBehaviour(4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                changeBehaviour(5);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                changeBehaviour(6);
            }
        }



        public async void SendReply()
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputChat
            };

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputChat;

            messages.Add(newMessage);
            inputChat = "";

            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                //Model = "gpt-3.5-turbo-0613",
                Model = "gpt-4-0613",

                Messages = messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();
                messages.Add(message);
                lastMessage = message.Content.ToString();
                answerChatGPT?.Invoke(this, EventArgs.Empty);



                if (messages.Count == 0)
                {
                    Debug.Log("No messages in the list.");
                    return;
                }

                foreach (var messageA in messages)
                {
                    Debug.Log(messageA.Role + " \nSaid\n " + message.Content); // Accediendo directamente a la propiedad content de ChatMessage
                }

            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }
        }
    }
}