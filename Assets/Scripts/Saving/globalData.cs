using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalData : MonoBehaviour
{
    [System.Serializable]
    public class Data{
        public int currSave;
    }

    public Data data = new Data();

    public void SaveToJson()
    {
        string json = JsonUtility.ToJson(data);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/globalData.json", json);
    }

    public void LoadFromJson()
    {
        string path = Application.persistentDataPath + "/globalData.json";
        if(System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            data = JsonUtility.FromJson<Data>(json);
        }
    }
}
