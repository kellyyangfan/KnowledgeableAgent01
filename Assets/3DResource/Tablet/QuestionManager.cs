using UnityEngine;
using TMPro;

public class QuestionManager : MonoBehaviour
{
    public TMP_Text questionText; // Referencia al componente TextMeshPro
    private int currentQuestionIndex = 0; // Índice de la pregunta actual
    [SerializeField] private string[] questions = { // Arreglo de preguntas
        "1. What term describes small, self-funded game development, and how does it differ from mainstream game development?",
        "2. How has the role of a single programmer in video game development changed from the 1980s to the present day?",
        "3. Who was responsible for creating \"Tennis for Two\" in 1958, and what unique display technology did it use?",
        "4. What was the significance of the game \"Space Invaders\" in the context of the video game industry's history?",
        "5. What are the roles that exists in game development?",
        "6. What is the Game design document?"
    };

    void Start()
    {
        UpdateQuestion(); // Actualiza la pregunta al inicio
    }

    public void NextQuestion()
    {
        currentQuestionIndex++; // Incrementa el índice
        if (currentQuestionIndex >= questions.Length)
        {
            currentQuestionIndex = 0; // Vuelve al inicio si se pasa del límite
        }
        UpdateQuestion(); // Actualiza la pregunta
    }

    public void PrevQuestion()
    {
        currentQuestionIndex--; // Incrementa el índice
        if (currentQuestionIndex < 0)
        {
            currentQuestionIndex = questions.Length - 1; // Vuelve al inicio si se pasa del límite
        }
        UpdateQuestion(); // Actualiza la pregunta
    }

    void UpdateQuestion()
    {
        questionText.text = questions[currentQuestionIndex]; // Cambia el texto de la pregunta
    }
}