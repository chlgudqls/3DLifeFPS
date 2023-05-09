using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongAnimal : Animal
{
    [SerializeField] protected int attackDamage; // 공격데미지
    [SerializeField] protected float attackDelay; // 공격딜레이
    [SerializeField] protected LayerMask targetMask; // 타겟 마스크


    [SerializeField] protected float ChaseTime; // 총 추격 시간
    protected float currentChaseTime; // 계산
    [SerializeField] protected float chaseDelayTime; // 추격 딜레이
    public void Chase(Vector3 _targetPos)
    {
        isChasing = true;
        isRunning = true;
        // 위치 바꿔도 상관없는지
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

    //이 함수는 시야에 한번 포착되면 위치 while 횟수만큼 가져오면서 계속 추격함
    // 끝나고 2번째로 포착하면 또 코루틴 실행
    protected IEnumerator ChaseTargetCoroutine()
    {
        currentChaseTime = 0;

        while (currentChaseTime < ChaseTime)
        {
            Chase(theViewAngle.GetTargetPos());
            // 3이하의 거리에있고
            if (Vector3.Distance(transform.position, theViewAngle.GetTargetPos()) <= 3f)
            {
                //시야각 안에있고, 뛸때 viewdistance에 걸린경우
                if (theViewAngle.View())
                {
                    Debug.Log("플레이어 공격 시도");
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
        // 갑자기 왜 바라보게하는거지 
        transform.LookAt(theViewAngle.GetTargetPos());
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);
        RaycastHit _hit;

        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out _hit, 3, targetMask))
        {
            Debug.Log("플레이어가 적중했습니다");
            thePlayerStatus.DecreaseHP(attackDamage);
        }
        else
        {
            Debug.Log("플레이어 빗나감!!!");
        }

        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
        StartCoroutine(ChaseTargetCoroutine());
    }
}
