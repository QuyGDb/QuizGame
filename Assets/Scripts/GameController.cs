using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    private QuestionData newQuestion;
    [SerializeField] private int numOfQuestions;
    private int timeQuestion;
    private int currentTime;
    public int CurrentTime { get { return currentTime; } }

    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private AnswerButton[] answerButtons;

    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private GameObject dialogPanel;
    public Dialog dialog;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI questionPerQuestionsText;
    private int countQuestions = 1;
    private bool isTotalOfQuestionsAssigned = false;
    public int totalOfQuestions = 0;
    public int TimeQuestion { get => timeQuestion; set => timeQuestion = value; }


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

    public void CreateQuestion()
    {

        newQuestion = QuizManager.Instance.SetupQuestions();
        if (newQuestion != null)
        {
            ShuffleAnswer();
            questionText.SetText(newQuestion.question);
            string[] wrongAnswers = new string[] { newQuestion.answers[0], newQuestion.answers[1], newQuestion.answers[2] };
            var AnswerButtons = answerButtons;
            if (AnswerButtons != null && AnswerButtons.Length > 0)
            {

                int wrongAnswerCount = 0;
                for (int i = 0; i < AnswerButtons.Length; i++)
                {
                    int answerId = i;
                    if (AnswerButtons[i].CompareTag("correctAnswer"))
                    {
                        AnswerButtons[i].SetAnswerText(newQuestion.correctAnswer);
                    }
                    else
                    {
                        AnswerButtons[i].SetAnswerText(wrongAnswers[wrongAnswerCount]);
                        wrongAnswerCount++;
                    }
                    AnswerButtons[answerId].compBtn.onClick.RemoveAllListeners();
                    AnswerButtons[answerId].compBtn.onClick.AddListener(() => CheckRightAnswerEvent(AnswerButtons[answerId]));
                }
            }
        }
        else if (newQuestion == null)
        {
            Sound.Instance.EndGameSound();

            StopAllCoroutines();
            GameUIManager.Instance.GameOverScreen();
            dialog.SetDialogContent(2);
            dialog.Show(true);
        }

    }
    public void BeginExam(int numOfQuestions)
    {
        QuizManager.Instance.correctQuestions.Clear();
        QuizManager.Instance.WrongQuestions.Clear();
        GameUIManager.Instance.saveButton.onClick.AddListener(() => FirestoreManager.Instance.SaveExaminationEvent());
        scoreText.SetText("");
        countQuestions = 1;
        dialogPanel.SetActive(false);
        QuizManager.Instance.GetRandomQuestions(numOfQuestions);
        CreateQuestion();

        isTotalOfQuestionsAssigned = false;
        questionPerQuestionsText.SetText($"{countQuestions++}/{numOfQuestions}");

        Score.Instance.ResetScore();

    }
    public void SetTime(int time)
    {
        currentTime = time;
        FirestoreManager.Instance.periodOfTime = time;
        StartCoroutine(TimeCountingDown());
    }
    public void CheckRightAnswerEvent(AnswerButton answerButton)
    {


        // Kiểm tra xem đã gán hay chưa trước khi gán giá trị
        if (!isTotalOfQuestionsAssigned)
        {
            totalOfQuestions = QuizManager.Instance.selectedQuestions.Count + 1;
            isTotalOfQuestionsAssigned = true; // Đánh dấu đã gán giá trị
        }

        if (answerButton.CompareTag("correctAnswer"))
        {

            if (QuizManager.Instance.selectedQuestions.Count == 0)
            {
                Score.Instance.IncreaseScore(10);
                questionPerQuestionsText.SetText($"{countQuestions++}/{totalOfQuestions}");
                StopAllCoroutines();
                scoreText.SetText(Score.Instance.GetScore().ToString());

                GameUIManager.Instance.GameOverScreen();
                StopAllCoroutines();
                Sound.Instance.EndGameSound();
                dialog.SetDialogContent(1);
                dialog.Show(true);
            }
            else
            {
                CreateQuestion();
                Score.Instance.IncreaseScore(10);
                scoreText.SetText(Score.Instance.GetScore().ToString());
                questionPerQuestionsText.SetText($"{countQuestions++}/{totalOfQuestions}");
            }
            Sound.Instance.CorrectSound();
            StartCoroutine(WaitOneSecond());
            QuizManager.Instance.correctQuestions.Add(newQuestion);

        }
        else
        {
            Sound.Instance.WrongSound();
            StartCoroutine(WaitOneSecond());
            CreateQuestion();
            scoreText.SetText("Wrong");
            questionPerQuestionsText.SetText($"{countQuestions++}/{totalOfQuestions}");
            QuizManager.Instance.WrongQuestions.Add(newQuestion);
        }
    }
    IEnumerator TimeCountingDown()
    {
        while (currentTime > 0)
        {
            yield return new WaitForSeconds(1);
            currentTime--;
            timeText.SetText(currentTime.ToString());
        }

        dialog.SetDialogContent(3);
        dialog.Show(true);
        GameUIManager.Instance.GameOverScreen();
        StopAllCoroutines();
    }
    public void ShuffleAnswer()
    {

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i])
            {
                answerButtons[i].tag = "Untagged";
            }
        }
        int RandIdx = UnityEngine.Random.Range(0, answerButtons.Length);
        if (answerButtons[RandIdx])
        {
            answerButtons[RandIdx].tag = "correctAnswer";
        }
    }
    IEnumerator WaitOneSecond()
    {
        yield return new WaitForSeconds(1f);
        // Code sau khi chờ 1 giây
    }

}
