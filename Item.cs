using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 게임오브젝트에 붙일 필요없는 스크립트를 만들기 위해서 ScriptableObject 쓴다
// 모노비헤이비어로 상속받는 클레스는 반드시 게임오브젝트에 붙여줘야 효력이있음
// ScriptableObject 는 굳이 안붙여도 쓸수있음
// 아이템클레스를 여러개만든다고함
[CreateAssetMenu(fileName = "New Item" , menuName = "New Item/item")]
public class Item : ScriptableObject
{
    public string itemName; // 아이템의 이름
    [TextArea]
    public string itemDesc; // 아이템의 설명
    public ItemType itemType; // 아이템의 유형
    public Sprite itemImage; // 아이템의 이미지
    public GameObject itemPrefab; // 아이템의 프리팹

    public GameObject kitPrefab; // 키트 프리팹
    public GameObject kitPreviewPrefab; // 키트 프리뷰 프리팹

    // 무기타입들을 이 변수하나로 대체한다고 설명함
    public string weaponType;  // 무기 유형

    public enum ItemType
    {
        Equipment,
        Used,
        Ingredient,
        Kit,
        ETC
    }

}
