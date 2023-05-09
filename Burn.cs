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

    [SerializeField] private GameObject flame_prefab; // �� ������ �����ջ���
    private GameObject go_tempFlame; // �׸��� ����

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
        // �÷��̾��� ��ġ���� �������ѾߵǴ� �÷��̾������
        // deltatime���� ������Ÿ��, ���ӽð� ���ҽ��Ѽ�  0���� �׷� currenttime�� 0����Ŭ������ ������ �������ı� 
        // ���ӽð��� Ŀ�ߵ� �̷��� ����Ǹ� ���ӽð� ���ҵǰ� ���ӽð����� ���̹���Ÿ���� 0�ϵ��Ȱ����ϸ鼭 0�����۾����� damage����
        if (isBurning)
        {
            currentDurationTime -= Time.deltaTime;
            if(currentDamageTime > 0)
                currentDamageTime -= Time.deltaTime;

            if(currentDamageTime <= 0)
            {
                // ������ ����
                Damage();
            }
            if(currentDurationTime <= 0)
            {
                // ���� ��
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

