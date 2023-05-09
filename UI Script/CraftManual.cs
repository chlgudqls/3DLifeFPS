using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ��ġ�Ұǹ��� �������� ��Ƽ� ���ٴµ�
[System.Serializable]
public class Craft
{
    public string craftName; // �̸�
    public Sprite craftImage; // �̹���
    public string craftDesc; // ����
    public string[] craftNeedItem; // �ʿ��� ������
    public int[] craftNeedItemCount; // �ʿ��� �������� ����
    public GameObject go_Prefab; // ���� ��ġ�� ������
    public GameObject go_PreviewPrefab; // �̸����� ������
}

public class CraftManual : MonoBehaviour
{
    // ���º���
    private bool isActivated = false;
    private bool isPreviewActivated = false;

    [SerializeField] GameObject go_BaseUI; // �⺻ ���̽�  UI

    private int tabNumber = 0;
    private int page = 1;
    private int selectedSlotNumber;
    private Craft[] craft_selectedTab;

    [SerializeField] private Craft[] craft_Fire; // ��ںҿ� ��
    [SerializeField] private Craft[] craft_Build; // ����� �� 

    private GameObject go_PreView; // �̸����� �������� ���� ����
    private GameObject go_Prefab; // ���� ������ �������� ���� ����

    //�̰ǹ��� �ǹ����������� �޾ƿ�
    [SerializeField]
    private Transform tf_Player;

    // Raycast �ʿ亯������ �浹��������, �ν��� layer, ��������
    private RaycastHit hitInfo;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float range;

    // �ʿ��� UI Slot ���
    [SerializeField] private GameObject[] go_Slots;
    [SerializeField] private Image[] image_Slot;
    [SerializeField] private Text[] text_SlotName;
    [SerializeField] private Text[] text_SlotDesc;
    [SerializeField] private Text[] text_SlotNeedItem;

    // �ʿ��� ������Ʈ
    private Inventory theInventory;
    void Start()
    {
        theInventory = FindObjectOfType<Inventory>();
        tabNumber = 0;
        page = 1;
        TabSlotSetting(craft_Fire);
        // page �� ��� �Ϸ��� 
    }

    public void TabSetting(int _tabNumber)
    {
        tabNumber = _tabNumber;
        page = 1;

        switch (tabNumber)
        {
            case 0:
                TabSlotSetting(craft_Fire); //�� ����
                break;
            case 1:
                TabSlotSetting(craft_Build); //���� ����
                break;
        }
    }
    private void ClearSlot()
    {
        for (int i = 0; i < go_Slots.Length; i++)
        {
            image_Slot[i].sprite = null;
            text_SlotName[i].text = "";
            text_SlotDesc[i].text = "";
            text_SlotNeedItem[i].text = "";
            go_Slots[i].SetActive(false);
        }
    }

    private void TabSlotSetting(Craft[] _craft_tab)
    {
        ClearSlot();

        craft_selectedTab = _craft_tab;

        int startSlotNumber = (page - 1) * go_Slots.Length;

        for (int i = startSlotNumber; i < craft_selectedTab.Length; i++)
        {
            if (i == page * go_Slots.Length)
                break;

            // �̷� ������ �ݺ����� ó���ε�
            go_Slots[i - startSlotNumber].SetActive(true);
              
            image_Slot[i - startSlotNumber].sprite = craft_selectedTab[i].craftImage;
            text_SlotName[i - startSlotNumber].text = craft_selectedTab[i].craftName;
            text_SlotDesc[i - startSlotNumber].text = craft_selectedTab[i].craftDesc;

            for (int x = 0; x < craft_selectedTab[i].craftNeedItem.Length; x++)
            {
                // �׳� = �����ϸ� �������� ������� += ���ְ� \n 
                text_SlotNeedItem[i - startSlotNumber].text += craft_selectedTab[i].craftNeedItem[x];
                text_SlotNeedItem[i - startSlotNumber].text += "x " + craft_selectedTab[i].craftNeedItemCount[x] + "\n";
            }
        }
    }

    public void SlotClick(int _slotNumber)
    {
        // ���๰�� 5���̻��̸� �ʿ������� ��ȣ�� 5�̻�Ѿ�� �ʿ��� ���ƴѰ� 2���������� ��ư�� ��ȣ�� �ö�
        // 4,5,6 ��ȣ�� �ٿ��شٴ°žƴѰ� 2�������� ���� 
        // craft ���� �迭�� 4�� �̻��� ������ �Ѱ��ִ� slot �ѹ�����  �������� ���������� �������� ���� �ѹ��׸�ŭ������Ű��
        // craft�� ������� �迭index�� ��ġ��
        selectedSlotNumber = _slotNumber + (page - 1) * go_Slots.Length;

        // ��ᰡ������ preview ���� ����
        // ��ᰡ �ִ��� boolŸ�� �Լ������ؼ� �׾ȿ��� ������ ���δ�
        if (!CheckIngredient())
            return;

        go_PreView = Instantiate(craft_selectedTab[selectedSlotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);
        go_Prefab = craft_selectedTab[selectedSlotNumber].go_Prefab;

        GameManager.isOpenCraftManual = false;

        isPreviewActivated = true;
        go_BaseUI.SetActive(false);
    }
    private bool CheckIngredient()
    {
        // ���õȽ��Գѹ� page���� ���������ϰ� ��ȯ�Ѱ��� �Ѱܹ��� 
        // ������������� �˼��ִ¼��� �׽����� needItem�� check 
        for (int i = 0; i < craft_selectedTab[selectedSlotNumber].craftNeedItem.Length; i++)
        {
            if(theInventory.GetItemCount(craft_selectedTab[selectedSlotNumber].craftNeedItem[i]) < craft_selectedTab[selectedSlotNumber].craftNeedItemCount[i])
                return false;
        }
        // �ɸ��°� �������� true�ε� ����� ������ item���ִٰ���  �׷� ���Ÿ��°��� for���� 
        return true;
    }
    private void UseIngredient()
    {
        for (int i = 0; i < craft_selectedTab[selectedSlotNumber].craftNeedItem.Length; i++)
        {
            theInventory.SetItemCount(craft_selectedTab[selectedSlotNumber].craftNeedItem[i], craft_selectedTab[selectedSlotNumber].craftNeedItemCount[i]);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !isPreviewActivated)
            Window();

        if (isPreviewActivated)
            PreviewPositionUpdate();

        if (Input.GetButtonDown("Fire1"))
            Build();

        if (Input.GetKeyDown(KeyCode.Escape))
            Cancel();
    }
    private void Build()
    {
        if (isPreviewActivated && go_PreView.GetComponent<PreviewObject>().isBuildable())
        {
            UseIngredient();
            Instantiate(go_Prefab, go_PreView.transform.position , go_PreView.transform.rotation);
            Destroy(go_PreView);
            isActivated = false;
            isPreviewActivated = false;
            go_PreView = null;
            go_Prefab = null;
        }
    }
    private void PreviewPositionUpdate()
    {
        if(Physics.Raycast(tf_Player.position, tf_Player.forward,out hitInfo, range, layerMask))
        {
            if(hitInfo.transform != null)
            {
                Vector3 _location = hitInfo.point;

                if (Input.GetKeyDown(KeyCode.Q))
                    go_PreView.transform.Rotate(0f, -90f, 0f);
                else if (Input.GetKeyDown(KeyCode.E))
                    go_PreView.transform.Rotate(0f, +90f, 0f);

                // �ٷ� ���������ʰ� ��ĭ���̵���Ŵ 
                // �� y�� �� �̼���Ʈ������
                _location.Set(Mathf.Round(_location.x), Mathf.Round(_location.y / 0.1f) * 0.1f, Mathf.Round(_location.z));
                go_PreView.transform.position = _location;
            }
        }
    }
    // ��� ��ư�� ������ 
    private void Cancel()
    {
        if(isPreviewActivated)
            Destroy(go_PreView);

        isPreviewActivated = false;
        isActivated = false;

        GameManager.isOpenCraftManual = false;


        go_BaseUI.SetActive(false);
        go_PreView = null;
        go_Prefab = null;
    }

    public void RightPageSetting()
    {
        if (page < (craft_selectedTab.Length / go_Slots.Length) + 1)
            page++;
        else
            page = 1;

        TabSlotSetting(craft_selectedTab);
    }
    public void LeftPageSetting()
    {
        if (page != 1)
            page--;
        else
            page = (craft_selectedTab.Length / go_Slots.Length) + 1;

        TabSlotSetting(craft_selectedTab);

    }
    private void Window()
    {
        if (!isActivated)
            OpenWindow();
        else
            CloseWindow();
    }
    private void OpenWindow()
    {
        GameManager.isOpenCraftManual = true;

        isActivated = true;
        go_BaseUI.SetActive(true);
    }
    private void CloseWindow()
    {
        GameManager.isOpenCraftManual = false;

        isActivated = false;
        go_BaseUI.SetActive(false);
    }

}
 