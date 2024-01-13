using System.Collections.Generic;
using UnityEngine;

public class QuizManager : MonoBehaviour
{
    public static QuizManager Instance;
    public List<QuestionData> questionData;
    public QuestionData currentQuestion;
    public List<QuestionData> selectedQuestions = new List<QuestionData>();
    public List<QuestionData> correctQuestions = new List<QuestionData>();
    public List<QuestionData> WrongQuestions = new List<QuestionData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void GetRandomQuestions(int numQuestions)
    {


        if (questionData != null && questionData.Count > numQuestions)
        {
            for (int i = 0; i < numQuestions; i++)
            {
                int randIdx = Random.Range(0, questionData.Count);
                selectedQuestions.Add(questionData[randIdx]);
                questionData.RemoveAt(randIdx);
            }
        }
    }
    //lấy từ câu hỏi từ selected questions
    public QuestionData SetupQuestions()
    {
        if (selectedQuestions != null && selectedQuestions.Count > 0)
        {

            currentQuestion = selectedQuestions[0];
            selectedQuestions.RemoveAt(0);

            return currentQuestion;
        }
        return null;
    }

}