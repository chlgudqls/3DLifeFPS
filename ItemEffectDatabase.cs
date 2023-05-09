using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//커스텀클래스
// 부위에 따라 수치도 바뀜 둘다배열
// 아이템 네임은 같지만 부위가 다를수있음
[System.Serializable]
public class ItemEffect
{
    public string itemName; // 아이템의 이름 // 아이템클래스의 아이템네임과 똑같이해서 키값으로 사용할거임
    [Tooltip("HP, SP, DP, HUNGRY, THIRSTY, SATISFY만 가능합니다")]
    public string[] part; // 부위
    public int[] num; // 수치
}
public class ItemEffectDatabase : MonoBehaviour
{
    [SerializeField]
    private ItemEffect[] itemEffects;

    // 필요한 컴포넌트
    [SerializeField]
    private StatusController thePlayerStatus;
    [SerializeField]
    private WeaponManager theWeaponManager;
    [SerializeField]
    private SlotToolTip theSlotToolTip;
    [SerializeField]
    private QuickSlotController theQuickSlotController;
    // case안의 문자열을 const 로 상수로 선언해서 사용
    // enum이랑 살짝 다르네
    private const string HP = "HP", SP = "SP", DP = "DP", HUNGRY = "HUNGRY", THIRSTY = "THIRSTY", SATISFY = "SATISFY";

    // QuickSlotController 징검다리
    public void IsActivateQuickSlot(int _num)
    {
        theQuickSlotController.IsActivateQuickSlot(_num);
    }


    // SlotToolTip 징검다리
    public void ShowToolTip(Item _item, Vector3 _pos)
    {
        theSlotToolTip.ShowToolTip(_item , _pos);
    }
    // SlotToolTip 징검다리
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
                    // 파트갯수만큼돌려서 for 한번돌때 한번씩 실행되니까 파트길이만큼 돌린것중에 문자열에 case에 딱 맞으면 아이템사용됨
                    // 틀리면 default로 이동
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
                                Debug.Log("잘못된 Status 부위. HP, SP, DP, HUNGRY, THIRSTY, SATISFY만 가능합니다");
                                break;
                        }
                        Debug.Log(_item.itemName + " 을 사용했습니다");
                    }
                    // 일치하는거 하나 걸렸으면 리턴 네임이 일치하는거 또 찾을필요없음
                    return;
                }
            } 
            Debug.Log("ItemEffectDatabase에 일치하는 itemName이 없습니다");
        }
    }
}
