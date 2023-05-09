using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    [SerializeField] public string animalName; // ������ �̸�
    [SerializeField] protected int hp; // �������� ü��

    [SerializeField] protected Item item_Prefab; // ������
    [SerializeField] public int itemNumber; // �������� ȹ�� ����

    [SerializeField] protected float walkSpeed; // �ȱ� ���ǵ�
    [SerializeField] protected float runSpeed; // �ٱ� ���ǵ�
    //[SerializeField] protected float turningSpeed; // ȸ�� ���ǵ�
    //protected float applySpeed;

    //protected Vector3 direction; // ����
    protected Vector3 destination; // ���⺯���� ������ ������

    // ���º���
    protected bool isAction; // �ൿ������ �ƴ��� �Ǻ�
    protected bool isWalking; // �ȴ������� �ƴ��� �Ǻ�
    protected bool isRunning; // �ٴ��� �Ǻ�
    protected bool isChasing; // �߰������� �Ǻ�
    protected bool isAttacking; //���������� �Ǻ�
    public bool isDead; // �׾����� �Ǻ�

    [SerializeField] protected float walkTime; // �ȴ� �ð�
    [SerializeField] protected float waitTime; // ��� �ð�
    [SerializeField] protected float runTime; // �ٱ� �ð�
    protected float currentTime;

    // �ʿ��� ������Ʈ
    [SerializeField] protected Animator anim;
    [SerializeField] protected Rigidbody rigid;
    [SerializeField] protected BoxCollider boxCol;
    protected AudioSource theAudio;
    protected NavMeshAgent nav;
    protected FieldOfViewAngle theViewAngle;
    protected StatusController thePlayerStatus;

    [SerializeField] protected AudioClip[] sound_Nomal; // �ϻ���� �ܼҸ� �ϳ����� �ܲ�, �ܲܲ� �����Ҹ� ������
    [SerializeField] protected AudioClip sound_Hurt;
    [SerializeField] protected AudioClip sound_Dead;


    [SerializeField] float destinationX;
    [SerializeField] float destinationZ;
    void Start()
    {
        thePlayerStatus = FindObjectOfType<StatusController>();
        nav = GetComponent<NavMeshAgent>();
        theAudio = GetComponent<AudioSource>();
        theViewAngle = GetComponent<FieldOfViewAngle>();
        currentTime = waitTime;
        isAction = true;
    }

    protected virtual void Update()
    {
        if (!isDead)
        {
            Move();
            //Rotation();
            ElapseTime();
        }
    }
    protected void Move()
    {
        if (isWalking || isRunning)
            //rigid.MovePosition(transform.position + (transform.forward * applySpeed * Time.deltaTime));
            nav.SetDestination(transform.position + destination * 5f);
    }
    //protected void Rotation()
    //{
    //    if (isWalking || isRunning)
    //    {
    //        Vector3 rotation = Vector3.Lerp(transform.eulerAngles, new Vector3(0f, direction.y, 0f), turningSpeed); ;
    //        rigid.MoveRotation(Quaternion.Euler(rotation));
    //    }
    //}
    protected void ElapseTime()
    {
        if (isAction)
        {
            currentTime -= Time.deltaTime;
            // ������ �Ǹ� �ȵǴ� �������� ������
            if (currentTime <= 0 && !isChasing && !isAttacking)
                ReSet();
        }
    }
    // ���º����� �̹� true�ε� true �� �������ָ� �� if�� ������ ���� ������
    protected virtual void ReSet()
    {
        // isAction �� ���� false�� �����ʾƵ� ó���� true�� ElapseTime ź�� ���⼭ true���Ǹ� �� Ÿ�� ���ѹݺ�
        // �̷��͵��ǳ�
        isWalking = false; isAction = true; isRunning = false;
        nav.speed = walkSpeed;
        // ��� move���ε� �ٸ��ൿ�ϰ����� move�� �����̴ϱ� �ʱ�ȭ�ʿ�
        nav.ResetPath();
        anim.SetBool("Walking", isWalking); anim.SetBool("Running", isRunning);
        // y�ุ �����Ű�� ������ �޶���
        //direction.Set(0f, Random.Range(0f, 360f), 0f);
        destination.Set(Random.Range(-destinationX, destinationX), 0f, Random.Range(-destinationZ, destinationZ));
        //RandomAction(); // ���� ���� �ൿ ����

    }


    protected void TryWalk()
    {
        nav.speed = walkSpeed;
        currentTime = walkTime;
        isWalking = true;
        anim.SetBool("Walking", isWalking);
        Debug.Log("�ȱ�");
    }

    public virtual void Damage(int _dmg, Vector3 _targetPos)
    {
        if (!isDead)
        {
            hp -= _dmg;

            if (hp <= 0)
            {
                Debug.Log("ü�� 0 ����");
                Dead();
                //PlaySE(sound_Dead);
                return;
            }
            PlaySE(sound_Hurt);
            anim.SetTrigger("Hurt");
            //Run(_targetPos);
        }
    }
    // �׾����� ���� �ʱ�ȭ �׾����� ���¸� �� ����
    protected void Dead()
    {
        PlaySE(sound_Dead);
        isWalking = false;
        isRunning = false;
        isChasing = false;
        isAttacking = false;
        isDead = true;
        nav.ResetPath();
        anim.SetTrigger("Dead");
    }
    protected void RandomSound()
    {
        int random = Random.Range(0, 3);
        PlaySE(sound_Nomal[random]);
    }
    protected void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }

    public Item GetItem()
    {
        this.gameObject.tag = "Untagged";
        Destroy(this.gameObject, 3f);
        return item_Prefab;
    }
}
