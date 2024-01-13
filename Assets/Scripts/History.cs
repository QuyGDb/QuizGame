using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class History
{
    //cái thuộc tính cần thiết để hiện thị history trong game được join từ 2 collection 
    public string TestID;
    public int NumberOfCorrectAns;
    public int NumberOfWrongAns;
    public int Score;
    public DateTime ExaminationTime;
    public string ExaminationID;
    public bool IsDone;
}
