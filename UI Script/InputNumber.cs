using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputNumber : MonoBehaviour
{
    private bool activated = false;
    
    [SerializeField]
    private Text text_Preview;
    [SerializeField]
    private Text text_Input;
    [SerializeField]
    private InputField if_text;

    [SerializeField]
    private GameObject go_Base;

    [SerializeField]
    private ActionController thePlayer;

    void Update()
    {
        if(activated)
        {
            if (Input.GetKeyDown(KeyCode.Return))
                OK();
            else if (Input.GetKeyDown(KeyCode.Escape))
                Cancel();
        }
    }
    public void Call()
    {
        go_Base.SetActive(true);
        activated = true;
        if_text.text = "";
        text_Preview.text = DragSlot.instance.dragSlot.itemCount.ToString();
    }
    public void Cancel()
    {
        go_Base.SetActive(false);
        activated = false;
        DragSlot.instance.SetColor(0);
        DragSlot.instance.dragSlot = null;
    }
    public void OK()
    {
        DragSlot.instance.SetColor(0);
        // 숫자만 들어와야되네
        int num;
        if(text_Input.text != "")
        {
            if (CheckNumber(text_Input.text))
            {
                // 비로소 변환할수있게된다
                num = int.Parse(text_Input.text);
                //버린 아이템의 총 개수를 넘지말아야됨
                if (num > DragSlot.instance.dragSlot.itemCount)
                    num = DragSlot.instance.dragSlot.itemCount;
            }
            else
                num = 1;
        }
        else
            num = int.Parse(text_Preview.text);

        StartCoroutine(DropItemCoroutine(num));
    }
        // 아이템을 하나씩 버릴거라서
    IEnumerator DropItemCoroutine(int _num)
    {
        for (int i = 0; i < _num; i++)
        {
            if(DragSlot.instance.dragSlot.item.itemPrefab != null)
                Instantiate(DragSlot.instance.dragSlot.item.itemPrefab, thePlayer.transform.position + thePlayer.transform.forward, Quaternion.identity);
            DragSlot.instance.dragSlot.SetSlotCount(-1);
            yield return new WaitForSeconds(0.05f);
        }
        // 아이템을 입력한 숫자만큼 떨어트리는 부분 한개만 떨어트려도 다 없앨수가 있음
        // 아이템을 모두 들고있고 그 아이템의 모든 갯수를 버릴경우 손에 든 아이템파괴 
        if(_num == int.Parse(text_Preview.text))
            if (QuickSlotController.go_HandItem != null)
                Destroy(QuickSlotController.go_HandItem);

        DragSlot.instance.dragSlot = null;
        go_Base.SetActive(false);
        activated = false;
    }
    private bool CheckNumber(string _argString)
    {
        // 한글자씩 변환해서 검사하려는듯
        char[] _tempCharArray = _argString.ToCharArray();
        // 확인하기전에 숫자라고 간주하고 시작한다고함
        // 맞음 시작하면 true로 바뀌니까 
        // 검사하고 없으면 넘어온게 숫자가 아니다 false로 바꿈
        bool isNumber = true;
        for (int i = 0; i < _tempCharArray.Length; i++)
        {
            // for문을 돌려서 if문내용을 체크하는 로직이 많네
            // 유니코드로 비교가능하다고함   아스키코드표같은건가
            if (_tempCharArray[i] >= 48 && _tempCharArray[i] <= 57)
                // 걸리는게 있으면 한번더실행 아래는 실행하지않는다고함
                // 걸리는게 한개라도 있었으면 isNumber가 true를 반환
                // 모든 글자가 숫자여야 isNumber를 true라고 반환해야되네
                continue;

            //걸리는게 없거나 하나라도 문자가있다면
            isNumber = false;
        }
        return isNumber;
    }
}
