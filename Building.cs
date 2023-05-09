using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    // 빌딩의 타입 설치가능한 위치구분
    public Type type;
    public enum Type
    {
        Nomal,
        Wall,
        Foundation,
        Pillar
    }

}
