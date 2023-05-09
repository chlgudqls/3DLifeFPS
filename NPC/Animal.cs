using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    [SerializeField] public string animalName; // 동물의 이름
    [SerializeField] protected int hp; // 동물들의 체력

    [SerializeField] protected Item item_Prefab; // 아이템
    [SerializeField] public int itemNumber; // 아이템의 획득 개수

    [SerializeField] protected float walkSpeed; // 걷기 스피드
    [SerializeField] protected float runSpeed; // 뛰기 스피드
    //[SerializeField] protected float turningSpeed; // 회전 스피드
    //protected float applySpeed;

    //protected Vector3 direction; // 방향
    protected Vector3 destination; // 방향변수를 수정함 목적지

    // 상태변수
    protected bool isAction; // 행동중인지 아닌지 판별
    protected bool isWalking; // 걷는중인지 아닌지 판별
    protected bool isRunning; // 뛰는지 판별
    protected bool isChasing; // 추격중인지 판별
    protected bool isAttacking; //공격중인지 판별
    public bool isDead; // 죽었는지 판별

    [SerializeField] protected float walkTime; // 걷는 시간
    [SerializeField] protected float waitTime; // 대기 시간
    [SerializeField] protected float runTime; // 뛰기 시간
    protected float currentTime;

    // 필요한 컴포넌트
    [SerializeField] protected Animator anim;
    [SerializeField] protected Rigidbody rigid;
    [SerializeField] protected BoxCollider boxCol;
    protected AudioSource theAudio;
    protected NavMeshAgent nav;
    protected FieldOfViewAngle theViewAngle;
    protected StatusController thePlayerStatus;

    [SerializeField] protected AudioClip[] sound_Nomal; // 일상사운드 꿀소리 하나보다 꿀꿀, 꿀꿀꿀 여러소리 내게함
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
            // 리셋이 되면 안되는 행위들은 막아줌
            if (currentTime <= 0 && !isChasing && !isAttacking)
                ReSet();
        }
    }
    // 상태변수가 이미 true인데 true 또 대입해주면 또 if문 안으로 들어가서 실행함
    protected virtual void ReSet()
    {
        // isAction 이 굳이 false가 되지않아도 처음에 true로 ElapseTime 탄후 여기서 true가되면 또 타고 무한반복
        // 이런것도되네
        isWalking = false; isAction = true; isRunning = false;
        nav.speed = walkSpeed;
        // 계속 move중인데 다른행동하고있음 move도 패턴이니까 초기화필요
        nav.ResetPath();
        anim.SetBool("Walking", isWalking); anim.SetBool("Running", isRunning);
        // y축만 변경시키면 방향이 달라짐
        //direction.Set(0f, Random.Range(0f, 360f), 0f);
        destination.Set(Random.Range(-destinationX, destinationX), 0f, Random.Range(-destinationZ, destinationZ));
        //RandomAction(); // 다음 랜덤 행동 개시

    }


    protected void TryWalk()
    {
        nav.speed = walkSpeed;
        currentTime = walkTime;
        isWalking = true;
        anim.SetBool("Walking", isWalking);
        Debug.Log("걷기");
    }

    public virtual void Damage(int _dmg, Vector3 _targetPos)
    {
        if (!isDead)
        {
            hp -= _dmg;

            if (hp <= 0)
            {
                Debug.Log("체력 0 이하");
                Dead();
                //PlaySE(sound_Dead);
                return;
            }
            PlaySE(sound_Hurt);
            anim.SetTrigger("Hurt");
            //Run(_targetPos);
        }
    }
    // 죽었으니 모든걸 초기화 죽었으면 상태를 다 꺼줌
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
