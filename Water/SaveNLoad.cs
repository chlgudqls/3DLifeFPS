using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPos;
    public Vector3 playerRot;

    // 매개 변수로 넘기는것들 slot 에서 받아와서 저장되었다가 꺼내쓸때 넘기는것들
    public List<int> invenArrayNumber = new List<int>();
    public List<string> invenItemName = new List<string>();
    public List<int> invenItemNumber = new List<int>();
}
public class SaveNLoad : MonoBehaviour
{
    private SaveData saveData = new SaveData();

    // 경로라고함
    private string SAVE_DATA_DIRECTORY;
    private string SAVE_FILENAME = "SaveFile.txt";

    private PlayerController thePlayer;
    private Inventory theInven;
    void Start()
    {
        // dataPath - 게임폴더  
        SAVE_DATA_DIRECTORY = Application.dataPath + "/Saves/";

        if (!Directory.Exists(SAVE_DATA_DIRECTORY))
            Directory.CreateDirectory(SAVE_DATA_DIRECTORY);
    }

    public void SaveData()
    {
        thePlayer = FindObjectOfType<PlayerController>();
        theInven = FindObjectOfType<Inventory>();

        saveData.playerPos = thePlayer.transform.position;
        saveData.playerRot = thePlayer.transform.eulerAngles;

        Slot[] slots = theInven.GetSlots();
        for (int i = 0; i < slots.Length; i++)
        {
            if(slots[i].item != null)
            {
                saveData.invenArrayNumber.Add(i);
                saveData.invenItemName.Add(slots[i].item.itemName);
                saveData.invenItemNumber.Add(slots[i].itemCount);
            }
        }

        // 저장할 값을 제이슨으로 변환
        // 제이슨이 텍스트들인가봄 
        string json = JsonUtility.ToJson(saveData);

        // 물리적인 파일로 저장
        // 텍스트를 전부 써버림  - 기억시킨다고함 SAVE_DATA_DIRECTORY 이 위치에   어떤 텍스트를 json을 
        File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME, json);

        Debug.Log("저장완료");
        Debug.Log(json);
    }
    public void LoadData()
    {
        if (File.Exists(SAVE_DATA_DIRECTORY + SAVE_FILENAME))
        {
            // 해당경로의 json텍스트를 읽어온다
            string loadJson = File.ReadAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME);
            // 제이슨을 다시 SaveData 형식으로 변환한다
            // 전역변수선언한거 그대로 사용
            saveData = JsonUtility.FromJson<SaveData>(loadJson);

            // 읽어오고 역변환하고 다시 대입한다
            thePlayer = FindObjectOfType<PlayerController>();
            theInven = FindObjectOfType<Inventory>();

            thePlayer.transform.position = saveData.playerPos;
            thePlayer.transform.eulerAngles = saveData.playerRot;

            for (int i = 0; i < saveData.invenItemName.Count; i++)
                theInven.LoadToInven(saveData.invenArrayNumber[i], saveData.invenItemName[i], saveData.invenItemNumber[i]);

            Debug.Log("로드완료");
        }
        else
            Debug.Log("세이브 파일이 없습니다");
    }
}
