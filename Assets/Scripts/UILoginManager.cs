using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILoginManager : MonoBehaviour
{
    public static UILoginManager instance;
    [SerializeField] private GameObject loginCanvas;
    [SerializeField] private GameObject gameCanvas;
    //Screen object variables
    [SerializeField] private GameObject loginUI;
    [SerializeField] private GameObject registerUI;
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject exitPanel;
    [SerializeField] private GameObject historyScrollView;
    [SerializeField] private GameObject settingUI;
    [SerializeField] private GameObject settingCanvas;
    public Button loginButton;
    bool isSettingScreen = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }


    }
    private void Start()
    {
        loginButton.onClick.AddListener(() => FirestoreManager.Instance.GetQuestionEvent());
    }

    //Functions to change the login screen UI

    public void ClearScreen() //Turn off all screens
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        menuUI.SetActive(false);
        historyScrollView.SetActive(false);

    }

    public void LoginScreen() //Back button
    {
        ClearScreen();
        loginUI.SetActive(true);
    }
    public void RegisterScreen() // Regester button
    {
        ClearScreen();
        registerUI.SetActive(true);
    }
    public void PlayModeScreen()
    {
        ClearScreen();
        menuUI.SetActive(true);
    }
    public void ExitGame()
    {
        exitPanel.SetActive(true);
    }
    public IEnumerator ConfirmAndQuit()
    {
        if (QuizManager.Instance.selectedQuestions.Count > 0)
        {
            yield return StartCoroutine(FirestoreManager.Instance.Examination());
        }
        // Sau khi hàm SaveExaminationEvent() hoàn thành, sẽ chờ 1 khoảng thời gian rồi mới thoát ứng dụng
        yield return new WaitForSeconds(1f); // Đợi 1 giây
        Application.Quit();
    }
    public void ConfirmAndQuitEvent()
    {
        StartCoroutine(ConfirmAndQuit());
    }
    public void Cancel()
    {
        exitPanel.SetActive(false);
    }
    public void MainScreen()
    {
        loginCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        ClearScreen();
        menuUI.SetActive(true);
        GameUIManager.Instance.confirmSave.SetText("");
    }
    public void GameScreen()
    {
        loginCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        GameUIManager.Instance.backBtn.SetActive(true);
        GameUIManager.Instance.NewGame();
    }
    public void HistoryScreen()
    {
        ClearScreen();
        historyScrollView.SetActive(true);
    }
    public void SettingScreen()
    {

        if (isSettingScreen == false)
        {

            settingCanvas.SetActive(true);
            settingUI.SetActive(true);
            isSettingScreen = true;
        }
        else
        {
            settingCanvas.SetActive(false);
            settingUI.SetActive(false);
            isSettingScreen = false;
        }
    }

}