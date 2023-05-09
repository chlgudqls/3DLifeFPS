using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongAnimal : Animal
{
    [SerializeField] protected int attackDamage; // ���ݵ�����
    [SerializeField] protected float attackDelay; // ���ݵ�����
    [SerializeField] protected LayerMask targetMask; // Ÿ�� ����ũ


    [SerializeField] protected float ChaseTime; // �� �߰� �ð�
    protected float currentChaseTime; // ���
    [SerializeField] protected float chaseDelayTime; // �߰� ������
    public void Chase(Vector3 _targetPos)
    {
        isChasing = true;
        isRunning = true;
        // ��ġ �ٲ㵵 ���������
        //direction = Quaternion.LookRotation(_targetPos - transform.position).eulerAngles;
        destination = _targetPos;

        nav.speed = runSpeed;
        anim.SetBool("Running", isRunning);
        nav.SetDestination(destination);
    }
    public override void Damage(int _dmg, Vector3 _targetPos)
    {
        base.Damage(_dmg, _targetPos);
        if (!isDead)
            Chase(_targetPos);
    }

    //�� �Լ��� �þ߿� �ѹ� �����Ǹ� ��ġ while Ƚ����ŭ �������鼭 ��� �߰���
    // ������ 2��°�� �����ϸ� �� �ڷ�ƾ ����
    protected IEnumerator ChaseTargetCoroutine()
    {
        currentChaseTime = 0;

        while (currentChaseTime < ChaseTime)
        {
            Chase(theViewAngle.GetTargetPos());
            // 3������ �Ÿ����ְ�
            if (Vector3.Distance(transform.position, theViewAngle.GetTargetPos()) <= 3f)
            {
                //�þ߰� �ȿ��ְ�, �۶� viewdistance�� �ɸ����
                if (theViewAngle.View())
                {
                    Debug.Log("�÷��̾� ���� �õ�");
                    StartCoroutine(AttackCoroutine());
                }
            }

            yield return new WaitForSeconds(chaseDelayTime);
            currentChaseTime += chaseDelayTime;
        }
        isRunning = false;
        isChasing = false;
        anim.SetBool("Running", isRunning);
        nav.ResetPath();
    }

    protected IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        nav.ResetPath();
        currentChaseTime = ChaseTime;
        yield return new WaitForSeconds(0.5f);
        // ���ڱ� �� �ٶ󺸰��ϴ°��� 
        transform.LookAt(theViewAngle.GetTargetPos());
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);
        RaycastHit _hit;

        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out _hit, 3, targetMask))
        {
            Debug.Log("�÷��̾ �����߽��ϴ�");
            thePlayerStatus.DecreaseHP(attackDamage);
        }
        else
        {
            Debug.Log("�÷��̾� ������!!!");
        }

        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
        StartCoroutine(ChaseTargetCoroutine());
    }
}
