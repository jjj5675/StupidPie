using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class DataSaver : MonoBehaviour
{
    public SaveData datas;
    public void AutoSaver()
    {
        datas.iresKeySet = (int)SceneController.Instance.gameObject.GetComponent<ControllerSets>().iresKeySet;
        datas.SeriKeySet = (int)SceneController.Instance.gameObject.GetComponent<ControllerSets>().seriKeySet;
        datas.rootCell = GetComponent<TransitionPoint>().transitionCell.cellNum;
        string jsonData = JsonUtility.ToJson(datas, true);
        string path = Path.Combine(Application.dataPath, "SaveData.json");
        File.WriteAllText(path, jsonData);
    }
}

[System.Serializable]
public class SaveData
{
    public int rootCell;
    public int iresKeySet;
    public int SeriKeySet;
}
