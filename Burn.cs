using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : MonoBehaviour
{
    private bool isBurning = false;

    [SerializeField] private int damage;

    [SerializeField] private float damageTime;
    private float currentDamageTime;

    [SerializeField] private float durationTime;
    private float currentDurationTime;

    [SerializeField] private GameObject flame_prefab; // 불 붙으면 프리팹생성
    private GameObject go_tempFlame; // 그릇은 뭐지

    public void StartBurning()
    {
        if (!isBurning)
        {
            go_tempFlame = Instantiate(flame_prefab, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            go_tempFlame.transform.SetParent(transform);
        }
            isBurning = true;
            currentDurationTime = durationTime;
    }

    void Update()
    {
        if (isBurning)
            ElapseTime();
    }
    private void ElapseTime()
    {
        // 플레이어의 위치에서 생성시켜야되니 플레이어가져오고
        // deltatime으로 데미지타임, 지속시간 감소시켜서  0으로 그럼 currenttime이 0보다클때실행 끝나면 프리팹파괴 
        // 지속시간이 커야됨 이러면 실행되면 지속시간 감소되고 지속시간동안 데이미지타임이 0일동안감소하면서 0보다작아지면 damage입음
        if (isBurning)
        {
            currentDurationTime -= Time.deltaTime;
            if(currentDamageTime > 0)
                currentDamageTime -= Time.deltaTime;

            if(currentDamageTime <= 0)
            {
                // 데미지 입힘
                Damage();
            }
            if(currentDurationTime <= 0)
            {
                // 불을 끔
                Off();
            }
        }
    }
    private void Damage()
    {
        currentDamageTime = damageTime;
        GetComponent<StatusController>().DecreaseHP(damage);
    }
    private void Off()
    {
        isBurning = false;
        Destroy(go_tempFlame);
    }
}

