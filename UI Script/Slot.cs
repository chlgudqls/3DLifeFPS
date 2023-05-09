using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour , IPointerEnterHandler , IPointerExitHandler , IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Item item; // 획득한 아이템
    public int itemCount; // 획득한 아이템의 개수
    public Image itemImage; // 아이템의 이미지

    [SerializeField] private bool isQuickSlot; // 퀵슬롯 여부판단  
    [SerializeField] private int quickNumber; // 퀵슬롯 번호

    [SerializeField]
    private Text text_Count;
    [SerializeField]
    private GameObject go_CountImage;

    //장착을 하려면 웨폰매니저가 필요하다고함
    // ItemEffectDatabase랑 겹치는 부분이있음 슬롯이 많아질수록 성능에 부하걸리기때문에 변수옮김
    //private WeaponManager theWeaponManager;
    //private Rect baseRect;
    private InputNumber theInputNumber;
    private ItemEffectDatabase theItemEffectDatabase;

    [SerializeField]
    private RectTransform baseRect;
    [SerializeField]
    private RectTransform quickSlotBaseRect; // 퀵슬롯의 영역 

    void Start()
    {
        // 부모의 부모의 랙트트랜스폼을 가져와서 쓰기때문에 
        // 서로 옮겨야되는데 퀵슬롯의 부모의 부모는 다른값이기때문에 아무튼 계산이다르다
        //baseRect = transform.parent.parent.GetComponent<RectTransform>().rect;
        //theWeaponManager = FindObjectOfType<WeaponManager>();
        theInputNumber = FindObjectOfType<InputNumber>();
        // 아이템을 이걸로 다관리 슬롯은 무리
        theItemEffectDatabase = FindObjectOfType<ItemEffectDatabase>();
    }

    //이미지 투명도 조절 인벤토리에 아이템들어오고 나가면 이미지 알파값 조절
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }
    // E키를 눌러서 아이템 획득하면 아이템이 인벤토리에 추가   
    // 넘어오면 아이템타입을 받아서 유아이 이미지에 아이템 이미지추가
    // 넘어온 아이템 숫자도 유아이에 추가
    public void AddItem(Item _item , int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = _item.itemImage;

        if(item.itemType != Item.ItemType.Equipment)
        {
            go_CountImage.SetActive(true);
            text_Count.text = itemCount.ToString();
        }
        else
        {
            //왜 바꾸지
            //순서가 틀리면 오류가 남
            text_Count.text = "0";
            go_CountImage.SetActive(false);
        }

        SetColor(1);
    }

    // 넘어오는 수에 따라서 아이템 텍스트유아이 변경
     public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        //아이템 모두 사용되면 유아이 원복시킴
        if (itemCount <= 0)
            //슬롯을 다시비움 넘어오는 아이템 다 사라지면
            ClearSlot();
    }
    private void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        go_CountImage.SetActive(false);
        text_Count.text = "0";
    }

    // 이스크립트의 객체에 마우스를 input하면 이벤트실행
    public void OnPointerClick(PointerEventData eventData)
    {
        // 마우스 이벤트가 뭔지 우클릭일때 실행하겠다
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(item != null)
            {
                //if(item.itemType == Item.ItemType.Equipment)
                //{
                    //장착
                    //StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(item.weaponType, item.itemName));
                //}
                //else
                //{
                    //소모
                    theItemEffectDatabase.UseItem(item);
                if (item.itemType == Item.ItemType.Used)
                    SetSlotCount(-1);
                //}
            }
        }
    }

    //마우스를 좌클릭해서 처음드래그할때 발생하는 이벤트
    //매개 변수로 넘어오는건 마우스클릭좌표가 클릭한 슬롯에 함수로 들어온다
    //들어왔으니 공유자원에 이 슬롯을 넣고 이미지알파값올리고 마우스포지션을 넣으면 마우스에 붙음
    //발생한다는건 슬롯이 선택됐다는거 선택된게 이함수를 실행한다
    //어떤 슬롯인지 구분할필요없이 클릭한게 해당슬롯
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(item != null && Inventory.inventoryActivated)
        {
            //드래그한 슬롯의 정보를 드래그슬롯에 drag 타입 변수에 넣음 정보가담겼겠지
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage);

            //본래 슬롯위치는 제자리에 두고 드래그슬롯을 옮김
            DragSlot.instance.transform.position = eventData.position;
        }
    }
    //드래그중인거
    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    //디버그로 어떨때 호출되는지 OnEndDrag , OnDrop 차이 알아보면됨
    //드래그가 끝났을때
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log(baseRect.rect.xMin);
        Debug.Log(baseRect.rect.xMax);
        Debug.Log(baseRect.rect.yMax);
        Debug.Log(baseRect.rect.yMin);
        //Debug.Log(baseRect.width);
        //Debug.Log(baseRect.height);
        Debug.Log("OnEndDrag 호출됨");
        //즉 바깥지점
        //인벤토리바깥 지점의 최솟값,최댓값을 이용함 
        //if(DragSlot.instance.transform.localPosition.x < baseRect.xMin || DragSlot.instance.transform.localPosition.x > baseRect.xMax
        //    || DragSlot.instance.transform.localPosition.y < baseRect.yMin || DragSlot.instance.transform.localPosition.y > baseRect.yMax)
            //인벤토리 안으로 바꿈
        if (!((DragSlot.instance.transform.localPosition.x > baseRect.rect.xMin && DragSlot.instance.transform.localPosition.x < baseRect.rect.xMax
            && DragSlot.instance.transform.localPosition.y > baseRect.rect.yMin && DragSlot.instance.transform.localPosition.y < baseRect.rect.yMax)
            ||
            (DragSlot.instance.transform.localPosition.x > quickSlotBaseRect.rect.xMin && DragSlot.instance.transform.localPosition.x < quickSlotBaseRect.rect.xMax
            && DragSlot.instance.transform.localPosition.y > (quickSlotBaseRect.transform.localPosition.y - 500) - quickSlotBaseRect.rect.yMax
            && DragSlot.instance.transform.localPosition.y < (quickSlotBaseRect.transform.localPosition.y - 500) - quickSlotBaseRect.rect.yMin)))
        {
            //아직 구현이 안된기능은 디버그 처리 
            Debug.Log("인벤토리 영역을 벗어났음");
            if(DragSlot.instance.dragSlot != null)
            {
                //Debug.Log(quickSlotBaseRect.rect.xMin);
                //Debug.Log(quickSlotBaseRect.rect.xMax);
                //Debug.Log(quickSlotBaseRect.transform.localPosition.y);
                //Debug.Log(quickSlotBaseRect.rect.yMin);
                //Debug.Log(quickSlotBaseRect.rect.yMax);
                // 꼭짓점의 위치값으로 계산한거임 heigh이 100 이니까 계산하면 맞음
                Debug.Log((quickSlotBaseRect.transform.localPosition.y -500) - quickSlotBaseRect.rect.yMin); // -450
                Debug.Log((quickSlotBaseRect.transform.localPosition.y - 500) - quickSlotBaseRect.rect.yMax); // -550
                theInputNumber.Call();
            }
            //Instantiate(DragSlot.instance.dragSlot.item.itemPrefab, theWeaponManager.transform.position + theWeaponManager.transform.forward, Quaternion.identity);
            //DragSlot.instance.dragSlot.ClearSlot();
        }
        else
        {
            //transform.position = originPos;
            DragSlot.instance.SetColor(0);
            DragSlot.instance.dragSlot = null;
        }
    }

    //떨어트렸을때겠지
    //드랍을감지함 해당하는슬롯이 감지하게됨 아이템이 떨어지는 슬롯 
    //감지를 했으니 자기 슬롯에 아이템을 더하고 들어온 슬롯에 내아이템을 넣고
    //위치감지가아닌 드랍감지로 슬롯끼리 서로 바꿈 이 슬롯스크립트를 이용해서
    //스크립트하나에 슬롯하나 붙어있다는거
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop 호출됨");
        if(DragSlot.instance.dragSlot != null)
        {
            ChangeSlot();

            // 어떻게 바꾸겠단거지
            if(isQuickSlot)  // 인벤토리에서 퀵슬롯 or 퀵슬롯에서 퀵슬롯
            {
                //  활성화된 퀵슬롯인지 판단 true일때 교체
                // 드랍시점에 퀵넘버를 넘겨준다  그래서 드랍된곳의 퀵넘버가 넘어가면 비교할때 false가되서 교체진행되지않는다
                theItemEffectDatabase.IsActivateQuickSlot(quickNumber);
            }
            else // 인벤토리에서 인벤토리 or 퀵슬롯에서 인벤토리
            {
                // 퀵슬롯에서 인벤토리 이동
                if (DragSlot.instance.dragSlot.isQuickSlot)
                    // 활성화된 퀵슬롯인지 판단 true일때 교체
                    theItemEffectDatabase.IsActivateQuickSlot(DragSlot.instance.dragSlot.quickNumber);
            }
        }
    }
    //이것은 슬롯들중에 떨어트림을 감지한 슬롯이 이함수를 실행하겠지 이벤트를
    //공유자원 이용해서 드랍할때 
    private void ChangeSlot()
    {
        // 가만히있던게 쫓겨남
        Item _tempItem = item;
        int _tempItemCount = itemCount;

        // 침범하고 점령
        // 옮기던 슬롯의 정보를 점령할 위치에 추가
        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);

        if(_tempItem != null)
        {
            //본진에서 dragSlot 위치로 데이터 교환
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount);
        }
        //근데 점령한곳이 아무도없었다면 본진을 바꿀필요도없음
        else
        {
            //옮겼으니 이전위치 초기화
            DragSlot.instance.dragSlot.ClearSlot();
        }
    }

    // 마우스가 슬롯에 들어갈때 발동
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(item != null)
            theItemEffectDatabase.ShowToolTip(item , transform.position);
    }

    // 마우스가 슬롯에서 빠져나갈때 발동
    public void OnPointerExit(PointerEventData eventData)
    {
        theItemEffectDatabase.HideToolTip();
    }
    // 퀵넘버가 private 니까 public 함수로 넘겨준거고 그걸 drag에서 써야되니까 
    public int QuickSlotNumber()
    {
        return quickNumber;
    }
}
