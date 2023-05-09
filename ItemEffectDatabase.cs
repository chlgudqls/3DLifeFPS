using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ŀ����Ŭ����
// ������ ���� ��ġ�� �ٲ� �Ѵٹ迭
// ������ ������ ������ ������ �ٸ�������
[System.Serializable]
public class ItemEffect
{
    public string itemName; // �������� �̸� // ������Ŭ������ �����۳��Ӱ� �Ȱ����ؼ� Ű������ ����Ұ���
    [Tooltip("HP, SP, DP, HUNGRY, THIRSTY, SATISFY�� �����մϴ�")]
    public string[] part; // ����
    public int[] num; // ��ġ
}
public class ItemEffectDatabase : MonoBehaviour
{
    [SerializeField]
    private ItemEffect[] itemEffects;

    // �ʿ��� ������Ʈ
    [SerializeField]
    private StatusController thePlayerStatus;
    [SerializeField]
    private WeaponManager theWeaponManager;
    [SerializeField]
    private SlotToolTip theSlotToolTip;
    [SerializeField]
    private QuickSlotController theQuickSlotController;
    // case���� ���ڿ��� const �� ����� �����ؼ� ���
    // enum�̶� ��¦ �ٸ���
    private const string HP = "HP", SP = "SP", DP = "DP", HUNGRY = "HUNGRY", THIRSTY = "THIRSTY", SATISFY = "SATISFY";

    // QuickSlotController ¡�˴ٸ�
    public void IsActivateQuickSlot(int _num)
    {
        theQuickSlotController.IsActivateQuickSlot(_num);
    }


    // SlotToolTip ¡�˴ٸ�
    public void ShowToolTip(Item _item, Vector3 _pos)
    {
        theSlotToolTip.ShowToolTip(_item , _pos);
    }
    // SlotToolTip ¡�˴ٸ�
    public void HideToolTip()
    {
        theSlotToolTip.HideToolTip();
    }
    public void UseItem(Item _item)
    {
        if (_item.itemType == Item.ItemType.Equipment)
        {
            StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(_item.weaponType, _item.itemName));
        }
        else if (_item.itemType == Item.ItemType.Used)
        {
            for (int x = 0; x < itemEffects.Length; x++)
            {
                if(itemEffects[x].itemName == _item.itemName)
                {
                    // ��Ʈ������ŭ������ for �ѹ����� �ѹ��� ����Ǵϱ� ��Ʈ���̸�ŭ �������߿� ���ڿ��� case�� �� ������ �����ۻ���
                    // Ʋ���� default�� �̵�
                    for (int y = 0; y < itemEffects[x].part.Length; y++)
                    {
                        switch (itemEffects[x].part[y])
                        {
                            case HP:
                                thePlayerStatus.IncreaseHP(itemEffects[x].num[y]);
                                break;
                            case SP:
                                thePlayerStatus.IncreaseSP(itemEffects[x].num[y]);
                                break;
                            case DP:
                                thePlayerStatus.IncreaseDP(itemEffects[x].num[y]);
                                break;
                            case HUNGRY:
                                thePlayerStatus.IncreaseHungry(itemEffects[x].num[y]);
                                break;
                            case THIRSTY:
                                thePlayerStatus.IncreaseThirsty(itemEffects[x].num[y]);
                                break;
                            case SATISFY:
                                break;
                            default:
                                Debug.Log("�߸��� Status ����. HP, SP, DP, HUNGRY, THIRSTY, SATISFY�� �����մϴ�");
                                break;
                        }
                        Debug.Log(_item.itemName + " �� ����߽��ϴ�");
                    }
                    // ��ġ�ϴ°� �ϳ� �ɷ����� ���� ������ ��ġ�ϴ°� �� ã���ʿ����
                    return;
                }
            } 
            Debug.Log("ItemEffectDatabase�� ��ġ�ϴ� itemName�� �����ϴ�");
        }
    }
}
