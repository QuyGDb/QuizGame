using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.XR;
public class FirestoreManager : MonoBehaviour
{
    public FirebaseFirestore db;
    private DocumentReference testDoc;
    private DocumentReference examinationDoc;
    private DocumentReference historyDoc;
    private DocumentReference RegistrationInfo;

    private DocumentSnapshot UserSnap;
    private DocumentSnapshot historySnapshot;
    public List<DocumentReference> examList;

    public static FirestoreManager Instance;

    public int periodOfTime;
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
    public IEnumerator GetQuestions()
    {
        Query query = db.Collection("Questions");
        Task<QuerySnapshot> task = query.GetSnapshotAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted || task.IsCanceled)
        {
            // Xử lý lỗi khi lấy dữ liệu không thành công
            Debug.LogError("Failed to fetch questions!");
            yield break;
        }

        QuerySnapshot snapshot = task.Result;

        foreach (DocumentSnapshot docSnap in snapshot.Documents)
        {
            if (docSnap.Exists)
            {
                QuestionData questionData = new QuestionData();

                questionData.question = docSnap.GetValue<string>("question");
                questionData.correctAnswer = docSnap.GetValue<string>("correctAnswer");
                questionData.AddAnswer(docSnap.GetValue<string>("answerA"));
                questionData.AddAnswer(docSnap.GetValue<string>("answerB"));
                questionData.AddAnswer(docSnap.GetValue<string>("answerC"));
                questionData.AddAnswer(docSnap.GetValue<string>("answerD"));
                questionData.questionID = docSnap.GetValue<string>("questionID");
                // Thêm câu hỏi vào QuizManager
                QuizManager.Instance.questionData.Add(questionData);

            }
        }
    }
    public void GetQuestionEvent()
    {
        StartCoroutine(GetQuestions());
        UILoginManager.instance.loginButton.onClick.RemoveAllListeners();
    }
    public IEnumerator CreateHistory()
    {
        if (db != null)
        {
            DocumentSnapshot docRef = null;

            Query UserQuery = db.Collection("User").WhereEqualTo("id_user", RegistrationInfo.Id);
            var UserTask = UserQuery.GetSnapshotAsync();
            yield return new WaitUntil(() => UserTask.IsCompleted);
            if (UserTask.IsCanceled || UserTask.IsFaulted)
            {
                yield break;
            }
            QuerySnapshot User = UserTask.Result;
            foreach (var item in User)
            {
                docRef = item;
            }
            var History = new Dictionary<string, object>
        {
             {"id_user", docRef.Reference},
        };

            var HistoryAdd = db.Collection("History").AddAsync(History);
            yield return new WaitUntil(() => HistoryAdd.IsCompleted);
            if (HistoryAdd.IsFaulted || HistoryAdd.IsCanceled)
            {
                yield break;
            }
            Debug.Log("History doc save to firestore");
            historyDoc = HistoryAdd.Result;
            string HistoryID = historyDoc.Id;
            var updateTask = historyDoc.UpdateAsync("id_History", HistoryID);
            yield return new WaitUntil(() => updateTask.IsCompleted);
            if (updateTask.IsFaulted)
            {
                Debug.LogError("Error updating document: " + updateTask.Exception);
                yield break;
            }
            Debug.Log("History update with id");

        }
    }
    public IEnumerator SaveRegistrationInfo(string password)
    {
        if (db != null)
        {
            var dataDict = new Dictionary<string, object>
            {
                { "name", AuthManager.Instance.User.DisplayName },
                { "mail", AuthManager.Instance.User.Email },
                {"password", password },
            };

            var addTask = db.Collection("User").AddAsync(dataDict);

            yield return new WaitUntil(() => addTask.IsCompleted);
            RegistrationInfo = addTask.Result;
            if (addTask.IsFaulted)
            {
                Debug.LogError("Error saving registration info: " + addTask.Exception);
                yield break;
            }

            Debug.Log("Registration info saved to Firestore!");
            string userRegister = RegistrationInfo.Id;
            var updateTask = RegistrationInfo.UpdateAsync("id_user", userRegister);
            yield return new WaitUntil(() => updateTask.IsCompleted);
            if (updateTask.IsFaulted)
            {
                Debug.LogError("Error updating document: " + updateTask.Exception);
                yield break;
            }
            Debug.Log("update id_user");
            yield return StartCoroutine(CreateHistory());
        }
        else
        {
            Debug.LogError("Firestore is not initialized!");
        }

    }
    private IEnumerator CreateTest(int time)
    {
        if (db != null)
        {

            var TestDict = new Dictionary<string, object>
        {
            {"numberOfQuestions", QuizManager.Instance.selectedQuestions.Count + 1 },
            {"periodOfTime", time }
        };

            var TestAdd = db.Collection("Test").AddAsync(TestDict);
            yield return new WaitUntil(() => TestAdd.IsCompleted);
            if (TestAdd.IsFaulted || TestAdd.IsCanceled)
            {
                yield break;
            }
            Debug.Log("Test doc save to firestore");
            testDoc = TestAdd.Result;
            string testID = testDoc.Id;
            var updateTask = testDoc.UpdateAsync("testID", testID);
            yield return new WaitUntil(() => updateTask.IsCompleted);
            if (updateTask.IsFaulted)
            {
                Debug.LogError("Error updating document: " + updateTask.Exception);
                yield break;
            }
            Debug.Log("Test update with id");

            foreach (var questionTemp in QuizManager.Instance.selectedQuestions)
            {
                var testDict = new Dictionary<string, DocumentReference>()
                {
                    {"testID", testDoc },
                    {"questionID", FirebaseFirestore.DefaultInstance.Document($"Questions/{questionTemp.questionID}")  }

                };
                var addTestDetailsTask = db.Collection("TestDetails").AddAsync(testDict);
                yield return new WaitUntil(() => addTestDetailsTask.IsCompleted);

                if (addTestDetailsTask.IsFaulted)
                {
                    Debug.LogError("Error saving data: " + addTestDetailsTask.Exception);
                    yield break;
                }
                Debug.Log("TestDetails added");
            }

            GameUIManager.Instance.SelectQuestioPackageScreen();
            GameController.Instance.SetTime(time);
        }
        else
        {
            Debug.Log("Firestore is not initialized!");
        }
    }
    public void CreateTestEvent(int time)
    {
        GameUIManager.Instance.LoadingPanel.SetActive(true);
        StartCoroutine(CreateTest(time));
    }
    public IEnumerator GetUser()
    {
        if (db != null)
        {

            string mail = AuthManager.Instance.User.Email;
            Query UserQuery = db.Collection("User").WhereEqualTo("mail", mail);
            var UserTask = UserQuery.GetSnapshotAsync();
            yield return new WaitUntil(() => UserTask.IsCompleted);
            if (UserTask.IsCanceled || UserTask.IsFaulted)
            {
                Debug.Log("loi getuser");
                yield break;
            }
            QuerySnapshot User = UserTask.Result;
            foreach (var item in User)
            {
                UserSnap = item;
            }
            Debug.Log(UserSnap.Reference);
            yield return StartCoroutine(GetHistory());
        }
    }
    public void GetUserEvent()
    {
        StartCoroutine(GetUser());
    }
    public IEnumerator GetHistory()
    {
        if (db != null)
        {
            Query HistoryQuery = db.Collection("History").WhereEqualTo("id_user", UserSnap.Reference);
            var HistoryTask = HistoryQuery.GetSnapshotAsync();
            yield return new WaitUntil(() => HistoryTask.IsCompleted);
            if (HistoryTask.IsCanceled || HistoryTask.IsFaulted)
            {
                Debug.Log("Loi lay data");
                yield break;
            }
            QuerySnapshot history = HistoryTask.Result;
            foreach (var historySnap in history)
            {
                historySnapshot = historySnap;
                Debug.Log(historySnap.Reference);
            }
            yield return StartCoroutine(MakeHistory());
        }
    }
    public void GetHistoryEvent()
    {
        StartCoroutine(GetHistory());
    }
    public IEnumerator Examination()
    {
        if (db != null)
        {
            HistoryManager.Instance.DestroyButton();
            var Examination = new Dictionary<string, object>
        {
            {"isDone", QuizManager.Instance.selectedQuestions.Count >0 ? false : true },
            {"testID", testDoc },
            {"examinationTime", DateTime.UtcNow.AddHours(7f)},
        };
            var ExaminatonAdd = db.Collection("Examination").AddAsync(Examination);
            yield return new WaitUntil(() => ExaminatonAdd.IsCompleted);
            if (ExaminatonAdd.IsFaulted || ExaminatonAdd.IsCanceled)
            {
                yield break;
            }
            Debug.Log("Examinaton doc save to firestore");
            examinationDoc = ExaminatonAdd.Result;
            string ExaminationID = examinationDoc.Id;
            var updateTask = examinationDoc.UpdateAsync("examinationID", ExaminationID);
            yield return new WaitUntil(() => updateTask.IsCompleted);
            if (updateTask.IsFaulted)
            {
                Debug.LogError("Error updating document: " + updateTask.Exception);
                yield break;
            }

            Debug.Log("Examinaton update with id");
            yield return StartCoroutine(Exam());
        }
    }
    public void SaveExaminationEvent()
    {
        GameUIManager.Instance.LoadingPanel.SetActive(true);
        StartCoroutine(Examination());
        GameUIManager.Instance.saveButton.onClick.RemoveAllListeners();
    }

    IEnumerator Exam()
    {
        if (db != null)
        {

            var Exam = new Dictionary<string, object>
        {
            {"examinationID", examinationDoc },
            {"id_History",  historySnapshot.Reference},
            {"id_user", UserSnap.Reference},
            {"numberOfCorrectAns", QuizManager.Instance.correctQuestions.Count },
            {"numberOfWrongAns", QuizManager.Instance.WrongQuestions.Count },
            {"score", Score.Instance.GetScore() },

        };

            var ExamAdd = db.Collection("Exam").AddAsync(Exam);
            yield return new WaitUntil(() => ExamAdd.IsCompleted);
            if (ExamAdd.IsFaulted || ExamAdd.IsCanceled)
            {
                yield break;
            }
            Debug.Log("Exam doc save to firestore");
            yield return StartCoroutine(MakeHistory());
        }
    }

    public void SaveExam()
    {
        StartCoroutine(Exam());
    }

    IEnumerator MakeHistory()
    {
        if (db != null)
        {
            //HistoryManager.Instance.DestroyButton();
            Query examQuey = db.Collection("Exam").WhereEqualTo("id_History", historySnapshot.Reference);
            var examTask = examQuey.GetSnapshotAsync();
            yield return new WaitUntil(() => examTask.IsCompleted);
            if (examTask.IsCanceled || examTask.IsFaulted)
            {
                Debug.Log("Loi lay exam");
                yield break;
            }
            QuerySnapshot examQuerySnap = examTask.Result;
            foreach (var exam in examQuerySnap)
            {
                DocumentReference examinationRef = exam.GetValue<DocumentReference>("examinationID");
                var examinationTask = examinationRef.GetSnapshotAsync();
                yield return new WaitUntil(() => examinationTask.IsCompleted);
                if (examinationTask.IsCanceled || examinationTask.IsFaulted)
                {
                    Debug.Log("loi examination");
                    yield break;
                }
                var examinationSnap = examinationTask.Result;
                var testRef = examinationSnap.GetValue<DocumentReference>("testID");
                var testTask = testRef.GetSnapshotAsync();
                yield return new WaitUntil(() => testTask.IsCompleted);
                if (testTask.IsCanceled || testTask.IsFaulted)
                {
                    Debug.Log("loi test");
                    yield break;
                }
                var testSnap = testTask.Result;
                History history = new History();
                history.NumberOfCorrectAns = exam.GetValue<int>("numberOfCorrectAns");
                history.NumberOfWrongAns = exam.GetValue<int>("numberOfWrongAns");
                history.IsDone = examinationSnap.GetValue<bool>("isDone");
                history.Score = exam.GetValue<int>("score");
                history.ExaminationID = examinationSnap.Id;
                history.TestID = testSnap.Id;
                history.ExaminationTime = examinationSnap.GetValue<DateTime>("examinationTime");
                HistoryManager.Instance.historyList.Add(history);

            }
            Debug.Log("make history ");
            yield return StartCoroutine(HistoryManager.Instance.GenerateHistory());
        }
    }
}
