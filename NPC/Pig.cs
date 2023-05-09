using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : WeakAnimal
{
    // update가 부모 스크립트에도 있기때문에 자식스크립트에서 똑같은 함수를 생성시키면 여기내용만 실행됨
    // 함수가 중복됨 
    protected override void Update()
    {
        base.Update();
        // 조건까진 못가져옴
        if (theViewAngle.View() && !isDead)
            Run(theViewAngle.GetTargetPos());
    }
    protected override void ReSet()
    {
        base.ReSet();
        RandomAction();
    }
    private void RandomAction()
    {
        RandomSound();

        int _random = Random.Range(0, 4);

        if (_random == 0)
            Wait();
        else if (_random == 1)
            Eat();
        else if (_random == 2)
            Peek();
        else if (_random == 3)
            TryWalk();
    }
    private void Wait()
    {
        currentTime = waitTime;
        Debug.Log("대기");
    }
    private void Eat()
    {
        currentTime = waitTime;
        anim.SetTrigger("Eat");
        Debug.Log("풀뜯기");
    }
    private void Peek()
    {
        currentTime = waitTime;
        anim.SetTrigger("Peek");
        Debug.Log("두리번");
    }

 
}
