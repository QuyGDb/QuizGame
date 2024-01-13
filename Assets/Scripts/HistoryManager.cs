using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HistoryManager : MonoBehaviour
{
    public static HistoryManager Instance;
    public Button historyBtnPrefab;
    public GameObject content;
    public Dictionary<string, object> history;
    public List<Button> historyBtnList = new List<Button>();
    public List<History> historyList = new List<History>();
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
    public IEnumerator GenerateHistory()
    {

        var historySort = historyList.OrderByDescending(temp => temp.ExaminationTime).ToList();
        foreach (var history in historySort)
        {
            Button historyBtn = Instantiate(historyBtnPrefab, content.transform);
            TextMeshProUGUI Text = historyBtn.GetComponentInChildren<TextMeshProUGUI>();

            Text.text = $"    Exanimation Time: {history.ExaminationTime}";
            Text.text += $"\n    Number Of Correct Answer : {history.NumberOfCorrectAns}";
            Text.text += $"\n    Number Of Wrong Answer : {history.NumberOfWrongAns}";
            Text.text += $"\n    Score: {history.Score}";
            Text.text += $"\n    Is Done: {history.IsDone}";
            historyBtnList.Add(historyBtn);
        }
        GameUIManager.Instance.LoadingPanel.SetActive(false);
        yield return null;
    }
    public void DestroyButton()
    {
        int temp = historyBtnList.Count;
        for (int i = 0; i < temp; i++)
        {
            Debug.Log("duoc goi");
            Destroy(historyBtnList[0].gameObject);
            historyBtnList.RemoveAt(0);
            historyList.Clear();
        }
    }

    //}
}
