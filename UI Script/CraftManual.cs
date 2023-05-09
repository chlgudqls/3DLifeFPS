using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 설치할건물의 정보들을 담아서 쓴다는데
[System.Serializable]
public class Craft
{
    public string craftName; // 이름
    public Sprite craftImage; // 이미지
    public string craftDesc; // 설명
    public string[] craftNeedItem; // 필요한 아이템
    public int[] craftNeedItemCount; // 필요한 아이템의 개수
    public GameObject go_Prefab; // 실제 설치될 프리팹
    public GameObject go_PreviewPrefab; // 미리보기 프리팹
}

public class CraftManual : MonoBehaviour
{
    // 상태변수
    private bool isActivated = false;
    private bool isPreviewActivated = false;

    [SerializeField] GameObject go_BaseUI; // 기본 베이스  UI

    private int tabNumber = 0;
    private int page = 1;
    private int selectedSlotNumber;
    private Craft[] craft_selectedTab;

    [SerializeField] private Craft[] craft_Fire; // 모닥불용 탭
    [SerializeField] private Craft[] craft_Build; // 건축용 탭 

    private GameObject go_PreView; // 미리보기 프리팹을 담을 변수
    private GameObject go_Prefab; // 실제 생성될 프리팹을 담을 변수

    //이건뭐임 건물생성때문에 받아옴
    [SerializeField]
    private Transform tf_Player;

    // Raycast 필요변수선언 충돌정보저장, 인식할 layer, 광선길이
    private RaycastHit hitInfo;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float range;

    // 필요한 UI Slot 요소
    [SerializeField] private GameObject[] go_Slots;
    [SerializeField] private Image[] image_Slot;
    [SerializeField] private Text[] text_SlotName;
    [SerializeField] private Text[] text_SlotDesc;
    [SerializeField] private Text[] text_SlotNeedItem;

    // 필요한 컴포넌트
    private Inventory theInventory;
    void Start()
    {
        theInventory = FindObjectOfType<Inventory>();
        tabNumber = 0;
        page = 1;
        TabSlotSetting(craft_Fire);
        // page 는 어떻게 하려나 
    }

    public void TabSetting(int _tabNumber)
    {
        tabNumber = _tabNumber;
        page = 1;

        switch (tabNumber)
        {
            case 0:
                TabSlotSetting(craft_Fire); //불 세팅
                break;
            case 1:
                TabSlotSetting(craft_Build); //건축 세팅
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

            // 이런 패턴의 반복문은 처음인데
            go_Slots[i - startSlotNumber].SetActive(true);
              
            image_Slot[i - startSlotNumber].sprite = craft_selectedTab[i].craftImage;
            text_SlotName[i - startSlotNumber].text = craft_selectedTab[i].craftName;
            text_SlotDesc[i - startSlotNumber].text = craft_selectedTab[i].craftDesc;

            for (int x = 0; x < craft_selectedTab[i].craftNeedItem.Length; x++)
            {
                // 그냥 = 대입하면 이전것이 사라지니 += 해주고 \n 
                text_SlotNeedItem[i - startSlotNumber].text += craft_selectedTab[i].craftNeedItem[x];
                text_SlotNeedItem[i - startSlotNumber].text += "x " + craft_selectedTab[i].craftNeedItemCount[x] + "\n";
            }
        }
    }

    public void SlotClick(int _slotNumber)
    {
        // 건축물이 5개이상이면 필요할지도 번호가 5이상넘어갈때 필요한 계산아닌가 2페이지부터 버튼의 번호도 올라감
        // 4,5,6 번호도 붙여준다는거아닌가 2페이지로 가면 
        // craft 탭의 배열이 4개 이상이 됐으니 넘겨주는 slot 넘버에서  페이지가 얼만지모르지만 페이지에 따라서 넘버그만큼증가시키면
        // craft에 만들어진 배열index와 일치함
        selectedSlotNumber = _slotNumber + (page - 1) * go_Slots.Length;

        // 재료가없으면 preview 생성 막음
        // 재료가 있는지 bool타입 함수생성해서 그안에서 계산까지 전부다
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
        // 선택된슬롯넘버 page별로 숫자증가하게 변환한값을 넘겨받음 
        // 몇번쨰슬롯인지 알수있는숫자 그슬롯의 needItem을 check 
        for (int i = 0; i < craft_selectedTab[selectedSlotNumber].craftNeedItem.Length; i++)
        {
            if(theInventory.GetItemCount(craft_selectedTab[selectedSlotNumber].craftNeedItem[i]) < craft_selectedTab[selectedSlotNumber].craftNeedItemCount[i])
                return false;
        }
        // 걸리는게 없었으면 true인데 여기로 빠지면 item이있다고함  그럼 뭘거르는거지 for에서 
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

                // 바로 대입하지않고 한칸씩이동시킴 
                // 저 y가 왜 미세컨트롤이지
                _location.Set(Mathf.Round(_location.x), Mathf.Round(_location.y / 0.1f) * 0.1f, Mathf.Round(_location.z));
                go_PreView.transform.position = _location;
            }
        }
    }
    // 취소 버튼을 누르면 
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
 