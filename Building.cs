using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    // ������ Ÿ�� ��ġ������ ��ġ����
    public Type type;
    public enum Type
    {
        Nomal,
        Wall,
        Foundation,
        Pillar
    }

}
