using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    //풀 체력
    [SerializeField]
    private int hp;
    [SerializeField]
    private GameObject go_hit_effect_prefab;

    [SerializeField]
    private float destroyTime; // 이펙트 삭제 시간

    [SerializeField]
    private float force; // 폭발력 세기

    [SerializeField]
    public Item item_leaf;
    [SerializeField]
    private int leaCount;
    private Inventory theInven;

    private Rigidbody[] rigidbodys;
    private BoxCollider[] boxColliders;

    [SerializeField]
    private string hit_sound;
    void Start()
    {
        //근데 왜 트렌스폼이지 많이쓰긴하네
        rigidbodys = this.transform.GetComponentsInChildren<Rigidbody>();
        boxColliders = transform.GetComponentsInChildren<BoxCollider>();
        theInven = FindObjectOfType<Inventory>();
    }

    public void Damage()
    {
        hp--;
        Hit();

        if (hp <= 0)
            Destruction();
    }
    private void Destruction()
    {
        theInven.AcquireItem(item_leaf, leaCount);
        for (int i = 0; i < rigidbodys.Length; i++)
        {
            rigidbodys[i].useGravity = true;
            //힘,위치,반경
            rigidbodys[i].AddExplosionForce(force, transform.position, 1f);
            boxColliders[i].enabled = true;
        }

        Destroy(this.gameObject, destroyTime);
    }
    private void Hit()
    {
        SoundManager.instance.PlaySE(hit_sound);
        var clone = Instantiate(go_hit_effect_prefab, transform.position + Vector3.up, Quaternion.identity);

        Destroy(clone,destroyTime);
    }
}
