using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private GameObject answerPanel;
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private GameObject numOfQuestions;
    [SerializeField] private GameObject DetailsUI;
    public Button saveButton;
    public GameObject LoadingPanel;
    public TextMeshProUGUI confirmSave;
    public GameObject backBtn;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }

    }
    public void ClearScreen()
    {
        questionPanel.SetActive(false);
        answerPanel.SetActive(false);
        dialogPanel.SetActive(false);
        numOfQuestions.SetActive(false);
        DetailsUI.SetActive(false);
    }
    public void SelectQuestioPackageScreen()
    {
        LoadingPanel.SetActive(false);
        numOfQuestions.SetActive(false);
        questionPanel.SetActive(true);
        answerPanel.SetActive(true);
        DetailsUI.SetActive(true);
        backBtn.SetActive(false);
    }
    public void GameOverScreen()
    {
        DetailsUI.SetActive(false);

    }
    public void NewGame()
    {
        ClearScreen();
        numOfQuestions.SetActive(true);
        GameController.Instance.dialog.Show(false);
        confirmSave.SetText("");
        backBtn.SetActive(true);
    }
    public void ConfirmSave()
    {
        confirmSave.SetText("saved successfully");
    }

}

