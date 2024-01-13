using System.Collections.Generic;

[System.Serializable]
public class QuestionData
{
    public string question;
    public string correctAnswer;
    public List<string> answers = new List<string>();
    public string questionID;
    public void AddAnswer(string answer)
    {
        if (answer != correctAnswer)
        {
            answers.Add(answer);
        }
    }
}
