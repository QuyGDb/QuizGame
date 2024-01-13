using System;
using TMPro;
using UnityEngine;

public class Dialog : MonoBehaviour
{
    public TextMeshProUGUI dialogText;
    public void Show(bool isShow)
    {
        gameObject.SetActive(isShow);

    }
    public void SetDialogContent(int status)
    {

        if (dialogText != null)
        {

            dialogText.text = $"   User: {AuthManager.Instance.User.DisplayName}";
            dialogText.text += $"\n   Exanimation Time: {DateTime.UtcNow.AddHours(7f)}";
            if (status == 1)
            {
                dialogText.text += $"\n   Number Of Correct Answer : {QuizManager.Instance.correctQuestions.Count + 1}";
            }
            else
            {
                dialogText.text += $"\n   Number Of Correct Answer : {QuizManager.Instance.correctQuestions.Count}";
            }
            if (status == 2)
            {
                dialogText.text += $"\n   Number Of Wrong Answer : {QuizManager.Instance.WrongQuestions.Count + 1}";
            }
            else
            {
                dialogText.text += $"\n   Number Of Wrong Answer : {QuizManager.Instance.WrongQuestions.Count}";
            }
            dialogText.text += $"\n   Score: {Score.Instance.GetScore()}";
            dialogText.text += $"\n   Remaining Time: {GameController.Instance.CurrentTime}";
            dialogText.text += "\n";
        }
    }
}
