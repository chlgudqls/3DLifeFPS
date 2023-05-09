using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    //Ǯ ü��
    [SerializeField]
    private int hp;
    [SerializeField]
    private GameObject go_hit_effect_prefab;

    [SerializeField]
    private float destroyTime; // ����Ʈ ���� �ð�

    [SerializeField]
    private float force; // ���߷� ����

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
        //�ٵ� �� Ʈ���������� ���̾����ϳ�
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
            //��,��ġ,�ݰ�
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
