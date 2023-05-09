using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���ӿ�����Ʈ�� ���� �ʿ���� ��ũ��Ʈ�� ����� ���ؼ� ScriptableObject ����
// �������̺��� ��ӹ޴� Ŭ������ �ݵ�� ���ӿ�����Ʈ�� �ٿ���� ȿ��������
// ScriptableObject �� ���� �Ⱥٿ��� ��������
// ������Ŭ������ ����������ٰ���
[CreateAssetMenu(fileName = "New Item" , menuName = "New Item/item")]
public class Item : ScriptableObject
{
    public string itemName; // �������� �̸�
    [TextArea]
    public string itemDesc; // �������� ����
    public ItemType itemType; // �������� ����
    public Sprite itemImage; // �������� �̹���
    public GameObject itemPrefab; // �������� ������

    public GameObject kitPrefab; // ŰƮ ������
    public GameObject kitPreviewPrefab; // ŰƮ ������ ������

    // ����Ÿ�Ե��� �� �����ϳ��� ��ü�Ѵٰ� ������
    public string weaponType;  // ���� ����

    public enum ItemType
    {
        Equipment,
        Used,
        Ingredient,
        Kit,
        ETC
    }

}
