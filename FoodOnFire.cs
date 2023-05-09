using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodOnFire : MonoBehaviour
{
    [SerializeField] private float time; // 익히는데 걸리는 시간
    private float currentTime;

    private bool done; // 익힘끝

    [SerializeField] private GameObject go_CookedItemPrefab; // 익혀진 or 탄 아이템 교체

    private void OnTriggerStay(Collider other)
    {
        if(other.transform.tag == "Fire" && !done)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= time)
            {
                done = true;
                Instantiate(go_CookedItemPrefab, transform.position, Quaternion.Euler(transform.eulerAngles));
                Destroy(gameObject);
            }
        }
    }
}
