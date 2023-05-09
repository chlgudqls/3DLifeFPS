using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] private string fireName; // ���� �̸�. ����, ��ں�, ȭ�Ժ�

    [SerializeField] private int damage; // ���� ������

    [SerializeField] private float damageTime; // �������� �� ������
    private float currentDamageTime;

    [SerializeField] private float durationTime; // ���� ���ӽð�
    private float currentDurationTime;

    [SerializeField] private ParticleSystem ps_Flame; // ��ƼŬ �ý���

    // �ʿ��� ������Ʈ
    private StatusController thePlayerStatus;

    // ���º��� 
    private bool isFire = true;

    void Start()
    {
        thePlayerStatus = FindObjectOfType<StatusController>();
        currentDurationTime = durationTime;
    }
    void Update()
    {
        if(isFire)
        {
            ElapseTime();
        }
    }

    private void ElapseTime()
    {
        currentDurationTime -= Time.deltaTime;

        if (currentDamageTime > 0)
            currentDamageTime -= Time.deltaTime;

        if(currentDurationTime <= 0)
        {
            // �Ҳ�
            Off();
        }
    }
    private void Off()
    {
        ps_Flame.Stop();
        isFire = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (isFire && other.transform.tag == "Player")
        {
            if (currentDamageTime <= 0)
            {
                other.GetComponent<Burn>().StartBurning();
                thePlayerStatus.DecreaseHP(damage);
                currentDamageTime = damageTime;
            }
        }
    }
    public bool GetIsFire()
    {
        return isFire;
    }
}
