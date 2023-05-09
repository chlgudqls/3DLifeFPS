using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    [SerializeField] private int damage;
    // 일정시간이 지나면 콜라이더 false 시키려는듯
    [SerializeField] private float finishTime;

    private bool isHurt = false;
    private bool isActivated = false;

    public IEnumerator ActivatedTrapCoroutine()
    {
        isActivated = true;

        yield return new WaitForSeconds(finishTime);
        isActivated = false;
        // 피해가들어오면 true가되고 부딪히면 false가됨
        isHurt = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(isActivated)
        {
            if(!isHurt)
            {
                isHurt = true;
                if (other.transform.name == "Player")
                    other.transform.GetComponent<StatusController>().DecreaseHP(damage);
            }
        }
    }
}
