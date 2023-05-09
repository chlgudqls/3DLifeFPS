using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Twig : MonoBehaviour
{
    [SerializeField]
    private int hp; // 체력

    [SerializeField]
    private float destroyTime; // 이펙트 삭제 시간

    //작은 나무가지 조각들
    [SerializeField]
    private GameObject go_little_Twig;
    //타격 이펙트
    [SerializeField]
    private GameObject go_hit_effect_prefab;

    //회전값 변수
    private Vector3 originRot;
    private Vector3 wantedRot;
    private Vector3 currentRot;

    //필요한 사운드 이름
    [SerializeField]
    private string hit_Sound;
    [SerializeField]
    private string broken_Sound;
    void Start()
    {
        originRot = transform.rotation.eulerAngles;
        currentRot = originRot;
    }
    //데미지의 매개변수가 왜 트랜스폼 플레이어인가
    public void Damage(Transform _playerTf)
    {
        hp--;
        Hit();                                                                              
        StartCoroutine(HitSwayCoroutine(_playerTf));
        if(hp <= 0)
        {
            Destruction(); // 파괴
        }
    }
    private void Hit()
    {
        SoundManager.instance.PlaySE(hit_Sound);

        GameObject clone = Instantiate(go_hit_effect_prefab,
                                        gameObject.GetComponent<BoxCollider>().bounds.center + (Vector3.up * 0.5f),
                                        Quaternion.identity);

        Destroy(clone, destroyTime);
    }
    //매개변수가 왜 트랜스폼 플레이어인가
    //필요하니까 겠지
    IEnumerator HitSwayCoroutine(Transform _target)
    {
        Vector3 direction = (_target.position - transform.position).normalized;

        //이건 또 뭔가
        Vector3 rotationDir = Quaternion.LookRotation(direction).eulerAngles;

        CheckDirection(rotationDir);

        while (!CheckThreshold())
        {
            currentRot = Vector3.Lerp(currentRot, wantedRot, 0.25f);
            transform.rotation = Quaternion.Euler(currentRot);
            yield return null;
        }

        //끝났다는게 뭐지 일단 하는데
        wantedRot = originRot;

        while (!CheckThreshold())
        {
            currentRot = Vector3.Lerp(currentRot, wantedRot, 0.15f);
            transform.rotation = Quaternion.Euler(currentRot);
            yield return null;
        }
    }
    private bool CheckThreshold()
    {
        if (Mathf.Abs(wantedRot.x - currentRot.x) <= 0.5f && Mathf.Abs(wantedRot.z - currentRot.z) <= 0.5f)
            return true;
        return false;

    }
    private void CheckDirection(Vector3 _rotationDir)
    {
        Debug.Log(_rotationDir);
        //왜 y값을 가지고 그러지 y값이 때리는 위치확인이 쉽네
        //각도마다 눕히는방향을 어떻게 알지
        if (_rotationDir.y > 180)
        {
            if (_rotationDir.y > 300)
                wantedRot = new Vector3(-50f, 0f, -50f);
            else if (_rotationDir.y > 240)
                wantedRot = new Vector3(0f, 0f, -50f);
            else
                wantedRot = new Vector3(50f, 0f, -50f);
        }
        if (_rotationDir.y <= 180)
        {
            //180보다 작은것중에 값들이 알아서 들어갈거임
            if (_rotationDir.y < 60)
                wantedRot = new Vector3(-50f, 0f, 50f);
            else if (_rotationDir.y < 120)
                wantedRot = new Vector3(0f, 0f, 50f);
            else
                wantedRot = new Vector3(50f, 0f, 50f);
        }
    }
    private void Destruction()
    {
        SoundManager.instance.PlaySE(broken_Sound);

        GameObject clone1 = Instantiate(go_little_Twig,
                                gameObject.GetComponent<BoxCollider>().bounds.center + (Vector3.up * 0.5f),
                                Quaternion.identity);

        GameObject clone2 = Instantiate(go_little_Twig,
                        gameObject.GetComponent<BoxCollider>().bounds.center - (Vector3.up * 0.5f),
                        Quaternion.identity);

        Destroy(clone1, destroyTime);
        Destroy(clone2, destroyTime);
        Destroy(this.gameObject);
    }
}
