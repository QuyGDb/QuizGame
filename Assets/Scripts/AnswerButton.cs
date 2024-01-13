using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour
{
    public TextMeshProUGUI AnswerText;
    public Button compBtn;
    public  void SetAnswerText(string text)
    {
        if(AnswerText != null)
        {
            AnswerText.text = text;
        }
    }
}
