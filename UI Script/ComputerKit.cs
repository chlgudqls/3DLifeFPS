using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// kit �� ���� 2�� ���߰� �̰� ��� ����Ұ���
[System.Serializable]
public class Kit
{
    public string kitName;
    public string kitDescription;
    public string[] needItemName;
    public int[] needItemNumber;

    public GameObject go_Kit_Prefab;
}
public class ComputerKit : MonoBehaviour
{
    [SerializeField] private Kit[] kits;
    [SerializeField] private Transform tf_ItemAppear; // ������ ������ ��ġ
    [SerializeField] private GameObject go_BaseUI; 

    private bool isCraft = false; // �ߺ� ���� ����
    public bool isPowerOn = false; // ���� ��������

    // �ʿ��� ������Ʈ
    private Inventory theInven;

    [SerializeField] private ComputerToolTip theToolTip;

    private AudioSource theAudio;
    [SerializeField] private AudioClip sound_ButtonClick;
    [SerializeField] private AudioClip sound_Beep;
    [SerializeField] private AudioClip sound_Activated;
    [SerializeField] private AudioClip sound_Output;
    void Start()
    {
        // Ŀ���� �߾ӿ� ��ġ ���������ʰ�
        //Cursor.lockState = CursorLockMode.Locked;
        // Ŀ�� �Ⱥ���
        //Cursor.visible = false;

        theInven = FindObjectOfType<Inventory>();
        theAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(isPowerOn && Input.GetKeyDown(KeyCode.Escape))
            PowerOff();
    }

    public void PowerOn()
    {
        GameManager.isOpenComputerKit = true;

        isPowerOn = true;
        go_BaseUI.SetActive(true);
    }
    private void PowerOff()
    {
        GameManager.isOpenComputerKit = false;

        HideToolTip();

        isPowerOn = false;
        go_BaseUI.SetActive(false);
    }
    public void ShowToolTip(int _buttonNum)
    {
        theToolTip.ShowToolTip(kits[_buttonNum].kitName, kits[_buttonNum].kitDescription, 
            kits[_buttonNum].needItemName, kits[_buttonNum].needItemNumber);
    }
    public void HideToolTip()
    {
        theToolTip.HideToolTip();
    }
    private void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }

    // �Ѿ���� ���Գѹ��� �̿��ؼ� kit�迭�� �����Ѵٰ��� �ε����� ��
    // ��ư���� ��ȣ�Ѱܼ� index�� �����ϴ°� ��κ� �����
    public void ClickButton(int _slotNumber)
    {
        PlaySE(sound_ButtonClick);
        if (!isCraft)
        {
            if (!CheckIngredient(_slotNumber)) // ��� üũ
                return;

            isCraft = true;
            UseIngredient(_slotNumber); // ��� ��� 

            StartCoroutine(CraftCoroutine(_slotNumber)); // ŰƮ ����
        }
    }
    private bool CheckIngredient(int _slotNumber)
    {
        // �����ϳ��� �ʿ��� �������� 1���� �ִ°Ծƴ� �׷��� for������ �ϳ����Ѱܼ� ����ϴ°���
        for (int i = 0; i < kits[_slotNumber].needItemName.Length; i++)
        {
             if (theInven.GetItemCount(kits[_slotNumber].needItemName[i]) < kits[_slotNumber].needItemNumber[i])
            {
                PlaySE(sound_Beep);
                return false;
            }
        }
        return true;
    }
    private void UseIngredient(int _slotNumber)
    {
        for (int i = 0; i < kits[_slotNumber].needItemName.Length; i++)
            theInven.SetItemCount(kits[_slotNumber].needItemName[i], kits[_slotNumber].needItemNumber[i]);
    }
    IEnumerator CraftCoroutine(int _slotNumber)
    {
        PlaySE(sound_Activated);
        yield return new WaitForSeconds(3f);
        PlaySE(sound_Output);

        Instantiate(kits[_slotNumber].go_Kit_Prefab, tf_ItemAppear.position, Quaternion.identity);
        isCraft = false;
    }
}
