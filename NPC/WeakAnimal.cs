using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakAnimal : Animal
{
    public void Run(Vector3 _targetPos)
    {
        // 위치 바꿔도 상관없는지
        //direction = Quaternion.LookRotation(_targetPos - transform.position).eulerAngles;
        destination = new Vector3(transform.position.x - _targetPos.x, 0f, transform.position.z - _targetPos.z).normalized;

        nav.speed = runSpeed;

        currentTime = runTime;
        isWalking = false;
        isRunning = true;
        anim.SetBool("Running", isRunning);
    }
    public override void Damage(int _dmg, Vector3 _targetPos)
    {
        base.Damage(_dmg, _targetPos);
        if (!isDead)
            Run(_targetPos);
    }
}
