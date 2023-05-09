using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    //컨트롤러가 중복됨
    //활성화 비활성화한거뭐지
    public static bool isActivate = false;
    //현재 장착된 총
    [SerializeField]
    private Gun currentGun;

    //연사속도 계산
    private float currentFireRate;

    //상태변수
    private bool isReload = false;
    [HideInInspector]
    public bool isFineSightMode = false;

    //본래 포지션 값
    private Vector3 originPos;

    private AudioSource audioSource;

    //레이저 충돌 정보
    private RaycastHit hitInfo;
    [SerializeField] private LayerMask layerMask;

    //필요한 컴포넌트
    [SerializeField]
    private Camera theCam;
    public CrossHair theCrossHair;

    //피격 이펙트
    [SerializeField]
    private GameObject hit_effect_prefab;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //originPos = transform.localPosition;
        theCrossHair = FindObjectOfType<CrossHair>();
        //처음실행할때 기본값넣어줌 처음엔 널이라서

    }
    void Update()
    {
        if(isActivate)
        {
            GundFireRateCalc();
            //TryFire();
            TryReload();
            TryFineSight();
        }
    }
    //연사속도 재계산
    private void GundFireRateCalc()
    {
        if (currentFireRate > 0)
            currentFireRate -= Time.deltaTime;

    }
    //발사 시도
    private void TryFire()
    {
        if(Input.GetButton("Fire1") && currentFireRate <= 0 && !isReload)
        {
            Fire();
        }
    }
    //총알 발사를 발사전 발사후로 나눴음
    private void Fire()
    {
        if (!isReload)
        {
            if (currentGun.currentBulletCount > 0)
                Shoot();
            else
            {
                CancelFineSight();
                StartCoroutine(ReloadCoroutine());
            }
        }
    }
    private void Shoot()
    {
        theCrossHair.FireAnimation();
        //총알발사할때 이뤄지는 일들 병렬처리하는느낌
        currentGun.currentBulletCount--;
        currentFireRate = currentGun.fireRate; //연사속도 재계산
        PlaySE(currentGun.fire_Sound);
        currentGun.muzzleFlash.Play();
        Hit();
        //총기반동 코루틴 실행
        //아무튼 중복코루틴 방지해결은 stop코루틴을 쓰면된다
        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutine());
        //Debug.Log("발사");
    }
    private void Hit()
    {
        //가운데애 광선쏨
        if (Physics.Raycast(theCam.transform.position, theCam.transform.forward + 
            //앞의 정확도는 기본값 뒤의 정확도는 인스펙터창에서 수정가능
            new Vector3(Random.Range(-theCrossHair.GetAccuracy() - currentGun.accuracy, theCrossHair.GetAccuracy() + currentGun.accuracy)
            , Random.Range(-theCrossHair.GetAccuracy() - currentGun.accuracy, theCrossHair.GetAccuracy() + currentGun.accuracy),0)
            , out hitInfo, currentGun.range, layerMask))
        {
            //Debug.Log(hitInfo.transform.name);
            //hitinfo의 표면값인데 그표현방향으로 바라봄
            GameObject clone = Instantiate(hit_effect_prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(clone, 2f);
        }
    }
    //재장전중에 총알못쏘게 코루틴으로 바꾼다고함
    private void TryReload()
    {
        if(Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount)
        {
            CancelFineSight();
            StartCoroutine(ReloadCoroutine());
        }
    }
    //재장전
    IEnumerator ReloadCoroutine()
    {
        if(currentGun.carryBulletCount > 0)
        {
            isReload = true;
            currentGun.anim.SetTrigger("Reload");

            //10발일때 재장전했다면 현재총알이 가비지가 될수있음 버리지않기위해서
            currentGun.carryBulletCount += currentGun.currentBulletCount;
            //현재총알 넣어주고 0으로 초기화
            currentGun.currentBulletCount = 0;
            
            yield return new WaitForSeconds(currentGun.reloadTime);

            if(currentGun.carryBulletCount >= currentGun.reloadBulletCount)
            {
                currentGun.currentBulletCount = currentGun.reloadBulletCount;
                currentGun.carryBulletCount -= currentGun.reloadBulletCount;
            }
            else
            {
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;
            }
            isReload = false;
        }
        else
        {
            //하는 이유 뭘 넣을지 모름 이안에 유아이,소리를 넣을수있음
            Debug.Log("소유한 총알이 없습니다.");
        }
    }
    //정조준시도
    private void TryFineSight() 
    {
        if(Input.GetButtonDown("Fire2") && !isReload)
        {
            FineSight();
        }
    }
    //정조준 취소
    public void CancelFineSight()
    {
        if (isFineSightMode)
            FineSight();
    }
    public void CancelReload()
    {
        if (isReload)
        {
            //캔슬 두개 비교해보기
            StopAllCoroutines();
            isReload = false;
        }
            
    }
    //정조준 로직 가동
    private void FineSight()
    {
        isFineSightMode = !isFineSightMode;
        currentGun.anim.SetBool("FineSightMode", isFineSightMode);
        theCrossHair.FineSightAnimation(isFineSightMode);

        if (isFineSightMode)
        {
            //코루틴 실행하기전에 진행중이던 코루틴스탑 이전 코루틴이 끝나지않아서 버그발생했음
            StopAllCoroutines();
            StartCoroutine(FineSightActivateCoroutine());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FineSightDeActivateCoroutine());
        }
    }
    //정조준 활성화
    IEnumerator FineSightActivateCoroutine()
    {
        while(currentGun.transform.localPosition != currentGun.fineSightOriginPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.2f);
            yield return null;
        }
    }
    //정조준 비활성화
    IEnumerator FineSightDeActivateCoroutine()
    {
        while (currentGun.transform.localPosition != originPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
            yield return null;
        }
    }
    //반동 코루틴
    //코루틴이 병렬처리에 유리 , 대기시간도 줄수있기때문에 사용
    IEnumerator RetroActionCoroutine()
    {
        Vector3 recoilBack = new Vector3(currentGun.retroActionForce, originPos.y, originPos.z);
        Vector3 retroActionRecoilBack = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOriginPos.y, currentGun.fineSightOriginPos.z);

        if(!isFineSightMode)
        {
            //반동이 다시 실행될때 원래 값으로 되돌린다? 지우고보기
            //2번 실행될떄 무슨 부자연스러운 문제가 있나봄
            currentGun.transform.localPosition = originPos;

            //반동 시작
            //조준이랑 다르게 반복문에서 빠져나가게 소숫점뺏음
            while (currentGun.transform.localPosition.x <= currentGun.retroActionForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }
            // 원위치
            while (currentGun.transform.localPosition != originPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
                yield return null;
            }
        }
        else
        {
            currentGun.transform.localPosition = currentGun.fineSightOriginPos;

            while (currentGun.transform.localPosition.x <= currentGun.retroActionFineSightForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null;
            }
            while (currentGun.transform.localPosition != currentGun.fineSightOriginPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.1f);
                yield return null;
            }
        }
    }
    private void PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }
    //private인 건을 반환시킬수있게 만듬
    public Gun GetGun()
    {
        return currentGun;
    }
    public bool GetFineSightMode()
    {
        return isFineSightMode;
    }
    public void GunChange(Gun _gun)
    {
        if (WeaponManager.currentWeapon != null)
            WeaponManager.currentWeapon.gameObject.SetActive(false);

        currentGun = _gun;
        //객체니까 겟컴포넌트로 transform가져오면되네
        WeaponManager.currentWeapon = currentGun.GetComponent<Transform>();
        //
        WeaponManager.currentWeaponAnim = currentGun.anim;
        //정조준중에 총에서 다른무기로가면 위치가바뀔수있다고하는데 모르겠다
        currentGun.transform.localPosition = Vector3.zero;
        currentGun.gameObject.SetActive(true);
        isActivate = true;
    }
}
