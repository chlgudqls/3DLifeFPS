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
            // update �ȿ��ֱ⶧���� �þ߾ȿ������� ��� ���⶧����  4���Ŀ� update�� ���ƿ��� stop���� �ڷ�ƾ�Ѱ��� �����Ŵ
            StopAllCoroutines();
            StartCoroutine(ChaseTargetCoroutine());
        }
    }
}
