using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    //��Ʈ�ѷ��� �ߺ���
    //Ȱ��ȭ ��Ȱ��ȭ�ѰŹ���
    public static bool isActivate = false;
    //���� ������ ��
    [SerializeField]
    private Gun currentGun;

    //����ӵ� ���
    private float currentFireRate;

    //���º���
    private bool isReload = false;
    [HideInInspector]
    public bool isFineSightMode = false;

    //���� ������ ��
    private Vector3 originPos;

    private AudioSource audioSource;

    //������ �浹 ����
    private RaycastHit hitInfo;
    [SerializeField] private LayerMask layerMask;

    //�ʿ��� ������Ʈ
    [SerializeField]
    private Camera theCam;
    public CrossHair theCrossHair;

    //�ǰ� ����Ʈ
    [SerializeField]
    private GameObject hit_effect_prefab;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //originPos = transform.localPosition;
        theCrossHair = FindObjectOfType<CrossHair>();
        //ó�������Ҷ� �⺻���־��� ó���� ���̶�

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
    //����ӵ� ����
    private void GundFireRateCalc()
    {
        if (currentFireRate > 0)
            currentFireRate -= Time.deltaTime;

    }
    //�߻� �õ�
    private void TryFire()
    {
        if(Input.GetButton("Fire1") && currentFireRate <= 0 && !isReload)
        {
            Fire();
        }
    }
    //�Ѿ� �߻縦 �߻��� �߻��ķ� ������
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
        //�Ѿ˹߻��Ҷ� �̷����� �ϵ� ����ó���ϴ´���
        currentGun.currentBulletCount--;
        currentFireRate = currentGun.fireRate; //����ӵ� ����
        PlaySE(currentGun.fire_Sound);
        currentGun.muzzleFlash.Play();
        Hit();
        //�ѱ�ݵ� �ڷ�ƾ ����
        //�ƹ�ư �ߺ��ڷ�ƾ �����ذ��� stop�ڷ�ƾ�� ����ȴ�
        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutine());
        //Debug.Log("�߻�");
    }
    private void Hit()
    {
        //����� ������
        if (Physics.Raycast(theCam.transform.position, theCam.transform.forward + 
            //���� ��Ȯ���� �⺻�� ���� ��Ȯ���� �ν�����â���� ��������
            new Vector3(Random.Range(-theCrossHair.GetAccuracy() - currentGun.accuracy, theCrossHair.GetAccuracy() + currentGun.accuracy)
            , Random.Range(-theCrossHair.GetAccuracy() - currentGun.accuracy, theCrossHair.GetAccuracy() + currentGun.accuracy),0)
            , out hitInfo, currentGun.range, layerMask))
        {
            //Debug.Log(hitInfo.transform.name);
            //hitinfo�� ǥ�鰪�ε� ��ǥ���������� �ٶ�
            GameObject clone = Instantiate(hit_effect_prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(clone, 2f);
        }
    }
    //�������߿� �Ѿ˸���� �ڷ�ƾ���� �ٲ۴ٰ���
    private void TryReload()
    {
        if(Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount)
        {
            CancelFineSight();
            StartCoroutine(ReloadCoroutine());
        }
    }
    //������
    IEnumerator ReloadCoroutine()
    {
        if(currentGun.carryBulletCount > 0)
        {
            isReload = true;
            currentGun.anim.SetTrigger("Reload");

            //10���϶� �������ߴٸ� �����Ѿ��� �������� �ɼ����� �������ʱ����ؼ�
            currentGun.carryBulletCount += currentGun.currentBulletCount;
            //�����Ѿ� �־��ְ� 0���� �ʱ�ȭ
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
            //�ϴ� ���� �� ������ �� �̾ȿ� ������,�Ҹ��� ����������
            Debug.Log("������ �Ѿ��� �����ϴ�.");
        }
    }
    //�����ؽõ�
    private void TryFineSight() 
    {
        if(Input.GetButtonDown("Fire2") && !isReload)
        {
            FineSight();
        }
    }
    //������ ���
    public void CancelFineSight()
    {
        if (isFineSightMode)
            FineSight();
    }
    public void CancelReload()
    {
        if (isReload)
        {
            //ĵ�� �ΰ� ���غ���
            StopAllCoroutines();
            isReload = false;
        }
            
    }
    //������ ���� ����
    private void FineSight()
    {
        isFineSightMode = !isFineSightMode;
        currentGun.anim.SetBool("FineSightMode", isFineSightMode);
        theCrossHair.FineSightAnimation(isFineSightMode);

        if (isFineSightMode)
        {
            //�ڷ�ƾ �����ϱ����� �������̴� �ڷ�ƾ��ž ���� �ڷ�ƾ�� �������ʾƼ� ���׹߻�����
            StopAllCoroutines();
            StartCoroutine(FineSightActivateCoroutine());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FineSightDeActivateCoroutine());
        }
    }
    //������ Ȱ��ȭ
    IEnumerator FineSightActivateCoroutine()
    {
        while(currentGun.transform.localPosition != currentGun.fineSightOriginPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.2f);
            yield return null;
        }
    }
    //������ ��Ȱ��ȭ
    IEnumerator FineSightDeActivateCoroutine()
    {
        while (currentGun.transform.localPosition != originPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
            yield return null;
        }
    }
    //�ݵ� �ڷ�ƾ
    //�ڷ�ƾ�� ����ó���� ���� , ���ð��� �ټ��ֱ⶧���� ���
    IEnumerator RetroActionCoroutine()
    {
        Vector3 recoilBack = new Vector3(currentGun.retroActionForce, originPos.y, originPos.z);
        Vector3 retroActionRecoilBack = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOriginPos.y, currentGun.fineSightOriginPos.z);

        if(!isFineSightMode)
        {
            //�ݵ��� �ٽ� ����ɶ� ���� ������ �ǵ�����? �������
            //2�� ����ɋ� ���� ���ڿ������� ������ �ֳ���
            currentGun.transform.localPosition = originPos;

            //�ݵ� ����
            //�����̶� �ٸ��� �ݺ������� ���������� �Ҽ�������
            while (currentGun.transform.localPosition.x <= currentGun.retroActionForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }
            // ����ġ
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
    //private�� ���� ��ȯ��ų���ְ� ����
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
        //��ü�ϱ� ��������Ʈ�� transform��������ǳ�
        WeaponManager.currentWeapon = currentGun.GetComponent<Transform>();
        //
        WeaponManager.currentWeaponAnim = currentGun.anim;
        //�������߿� �ѿ��� �ٸ�����ΰ��� ��ġ���ٲ���ִٰ��ϴµ� �𸣰ڴ�
        currentGun.transform.localPosition = Vector3.zero;
        currentGun.gameObject.SetActive(true);
        isActivate = true;
    }
}
