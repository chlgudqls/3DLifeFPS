using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigWild : StrongAnimal
{
    protected override void Update()
    {
        base.Update();
        if (theViewAngle.View() && !isDead && !isAttacking)
        {
            // update 안에있기때문에 시야안에있으면 계속 돌기때문에  4초후에 update로 돌아오면 stop해줌 코루틴한개만 실행시킴
            StopAllCoroutines();
            StartCoroutine(ChaseTargetCoroutine());
        }
    }
}
