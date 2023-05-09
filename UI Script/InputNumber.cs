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
        // ���ڸ� ���;ߵǳ�
        int num;
        if(text_Input.text != "")
        {
            if (CheckNumber(text_Input.text))
            {
                // ��μ� ��ȯ�Ҽ��ְԵȴ�
                num = int.Parse(text_Input.text);
                //���� �������� �� ������ �������ƾߵ�
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
        // �������� �ϳ��� �����Ŷ�
    IEnumerator DropItemCoroutine(int _num)
    {
        for (int i = 0; i < _num; i++)
        {
            if(DragSlot.instance.dragSlot.item.itemPrefab != null)
                Instantiate(DragSlot.instance.dragSlot.item.itemPrefab, thePlayer.transform.position + thePlayer.transform.forward, Quaternion.identity);
            DragSlot.instance.dragSlot.SetSlotCount(-1);
            yield return new WaitForSeconds(0.05f);
        }
        // �������� �Է��� ���ڸ�ŭ ����Ʈ���� �κ� �Ѱ��� ����Ʈ���� �� ���ټ��� ����
        // �������� ��� ����ְ� �� �������� ��� ������ ������� �տ� �� �������ı� 
        if(_num == int.Parse(text_Preview.text))
            if (QuickSlotController.go_HandItem != null)
                Destroy(QuickSlotController.go_HandItem);

        DragSlot.instance.dragSlot = null;
        go_Base.SetActive(false);
        activated = false;
    }
    private bool CheckNumber(string _argString)
    {
        // �ѱ��ھ� ��ȯ�ؼ� �˻��Ϸ��µ�
        char[] _tempCharArray = _argString.ToCharArray();
        // Ȯ���ϱ����� ���ڶ�� �����ϰ� �����Ѵٰ���
        // ���� �����ϸ� true�� �ٲ�ϱ� 
        // �˻��ϰ� ������ �Ѿ�°� ���ڰ� �ƴϴ� false�� �ٲ�
        bool isNumber = true;
        for (int i = 0; i < _tempCharArray.Length; i++)
        {
            // for���� ������ if�������� üũ�ϴ� ������ ����
            // �����ڵ�� �񱳰����ϴٰ���   �ƽ�Ű�ڵ�ǥ�����ǰ�
            if (_tempCharArray[i] >= 48 && _tempCharArray[i] <= 57)
                // �ɸ��°� ������ �ѹ������� �Ʒ��� ���������ʴ´ٰ���
                // �ɸ��°� �Ѱ��� �־����� isNumber�� true�� ��ȯ
                // ��� ���ڰ� ���ڿ��� isNumber�� true��� ��ȯ�ؾߵǳ�
                continue;

            //�ɸ��°� ���ų� �ϳ��� ���ڰ��ִٸ�
            isNumber = false;
        }
        return isNumber;
    }
}
