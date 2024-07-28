using System;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;

// using UnityEngine.UI;

namespace OpenAI
{
    public class SecondConversation : MonoBehaviour
    {
        [SerializeField] private AzureTextToSpeech azureTextToSpeech;
        [SerializeField] private StreamingChatGPT streamingChatGPT;
        [SerializeField] private AzureRealTime speechAzure;
        [SerializeField] private AudioSource audiosource;
        [SerializeField] private Animator animator;
        [HideInInspector] private string animationN;
        [HideInInspector] private string dataConversation;
        [HideInInspector] public int limitPreconversation;

        List<double> conversationalPause = new List<double>();
        private string newsGPT = "";
        private DateTime startTime;

        // For add the text into the Canva
        //[SerializeField] private Text messagePlayer;
        //[SerializeField] private Text messageAlbert;

        void Start()
        {
            StartTimer();
            //Subscription for the correct use of this project
            speechAzure.RecognitionActivate();
            speechAzure.completeSpeech += startChatGPT;
            streamingChatGPT.updateSpeech += addSpeech;
            streamingChatGPT.startSpeech += voice;
            azureTextToSpeech.completeSpeech += createSpeechCheck;
            limitPreconversation = (streamingChatGPT.dialogueExamples.Count * 2) + 1;
        }


        private void startChatGPT(object sender, EventArgs e)
        {
            //Debug.Log("-------------- Start Speech To Text STT --------------");
            // Ejecuta RecognitionActivate en segundo plano
            speechAzure.RecognitionActivate();

            streamingChatGPT.inputChat = speechAzure.completeMessage;
            streamingChatGPT.SendMessage();
            speechAzure.clearVar();
        }
       

        private void addSpeech(object sender, EventArgs e)
        {
            if (streamingChatGPT.updateSpeechString != "")
            {
                //Debug.Log("+++ new text");
                newsGPT += streamingChatGPT.updateSpeechString;
            }
        }

        // Here is where the animation takes place
        private void voice(object sender, EventArgs e)
        {
            //Debug.Log("-------------- Voice started GPT --------------");
            stopTimer();
            animationN = selectAnimation();
            animator.SetBool(animationN, true);
            newsGPT += streamingChatGPT.updateSpeechString;
            azureTextToSpeech.characterSpeech = newsGPT;
            newsGPT = "";
            azureTextToSpeech.startTextToSpeech();
            // StartCoroutine(playAudioToFinish(animationN));
        }

        private void createSpeechCheck(object sender, EventArgs e)
        {
            if (newsGPT != "" || streamingChatGPT.lastMessage == false)
            {
                //newsGPT += streamingChatGPT.updateSpeechString;
                azureTextToSpeech.characterSpeech = newsGPT;
                newsGPT = "";
                azureTextToSpeech.startTextToSpeech();
                //Debug.Log("New text to TTS");
            }
            else
            {
                //Debug.Log("Finished speech, stop everything");
                StartTimer();
                animator.SetBool(animationN, false);
                streamingChatGPT.ClearStrings();
                //streamingChatGPT.LogMessages();
                speechAzure.RecognitionActivate();
            }
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

        public void StartTimer()
        {
            startTime = System.DateTime.Now;
        }

        public void stopTimer()
        {
            TimeSpan difference = speechAzure.startTime - startTime;
            TimeSpan absoluteDifference = difference.Duration();
            conversationalPause.Add(absoluteDifference.TotalSeconds);
            
        }

        void OnApplicationQuit()
        {
            saveData();

        }

        private int CountWords(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            // Separar el texto en palabras usando espacios y otros caracteres comunes de separación
            char[] delimiters = new char[] { ' ', '\r', '\n', '\t', ',', '.', ';', ':', '!', '?' };
            string[] words = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            return words.Length;
        }

        private void IncrementNumPlayer()
        {
            string path = "Results\\numPlayer.txt";
            using (StreamReader reader = new StreamReader(path))
            {
                string line = reader.ReadLine();
                int numPlayer = int.Parse(line) + 1;
                string newLine = numPlayer.ToString();
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.WriteLine(newLine);
                }
            }
        }

        private void CreateText(string fileContent,string fileName)
        {
            string path = Path.Combine(Application.dataPath, "Results\\Players");
            Directory.CreateDirectory(path);

            string fullPath = Path.Combine(path, fileName + ".txt");
            File.WriteAllText(fullPath, fileContent);

            Debug.Log("Conversation saved! :)");
        }

        private void updateNumberPlayer()
        {
            string fileName = "numPlayer";
            string filePath = "Results";

            string fullPath = Path.Combine(Application.dataPath, filePath);
            fullPath = Path.Combine(fullPath, fileName + ".txt");

            if (File.Exists(fullPath))
            {
                string fileContent = File.ReadAllText(fullPath);
                int numPlayer = int.Parse(fileContent) + 1;
                fileContent = numPlayer.ToString();
                CreateText(dataConversation, fileContent);
                File.WriteAllText(fullPath, fileContent);
                // Debug.Log("Number of the file updated");
            }
            else
            {
                string newFullPath = Path.Combine(Application.dataPath, "Results");
                Directory.CreateDirectory(newFullPath);

                newFullPath = Path.Combine(newFullPath, fileName + ".txt");
                File.WriteAllText(newFullPath, "1");
                CreateText(dataConversation, "1");
                //Debug.Log("You didn´t have a counting files, so i created");
            }
        }

        void saveData()
        {
            dataConversation += "Pause duration between character finishing speaking and player's first response\n";
            foreach (var timeC in conversationalPause)
            {
                dataConversation += timeC.ToString() + "\n";
            }

            double minValue = conversationalPause.Min();
            double maxValue = conversationalPause.Max();
            double averageValue = conversationalPause.Average();

            // Mostrando los resultados
            dataConversation += "Lowest value: " + minValue + "\n";
            dataConversation += "Higher value: " + maxValue + "\n";
            dataConversation += "Average: " + averageValue + "\n";

            int iterationCount = 0;
            foreach (var message in streamingChatGPT.messages)
            {
                iterationCount++;
                dataConversation += "\n" + message.Role + " - " + message.Content + "\n";
                dataConversation += "Number of character - " + CountWords(message.Content) + "\n";
                if (iterationCount == limitPreconversation)
                {
                    dataConversation += "\n -------------- Here the real conversation begins --------------\n";
                }
            }
            dataConversation += "\n" + "Total time " + Time.time + "\n";

            Debug.Log(dataConversation);

            //IncrementNumPlayer();
            // CreateText(dataConversation);
            updateNumberPlayer();
            dataConversation = "";

        }
    }
}