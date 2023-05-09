using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : CloseWeaponController
{
    public static bool isActivate = false;
    public static Item currentKit; // 설치하려는 킷 (연금 테이블)

    private bool isPreview = false;

    private GameObject go_Preiview; // 설치할 키트 프리뷰
    private Vector3 previewPos; // 설치할 키트 위치
    [SerializeField] private float rangeAdd; // 건축시 추가 사정거리

    [SerializeField]
    private QuickSlotController theQuickSlot;
    void Update()
    {
        if (isActivate && !Inventory.inventoryActivated)
        {
            if(currentKit == null)  
            {
                if (QuickSlotController.go_HandItem == null)
                    TryAttack();
                else
                    TryEating(); 
            }
            else
            {
                if (!isPreview)
                    InstallPreviewKit();

                    Build();
                    PreviewPositionUpdate();
            }
        }
    }
    private void InstallPreviewKit()
    {
        isPreview = true;
        go_Preiview = Instantiate(currentKit.kitPreviewPrefab, transform.position, Quaternion.identity);
    }
    private void PreviewPositionUpdate()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentcloseWeapon.range + rangeAdd, layerMask))
        {
            previewPos = hitInfo.point;
            go_Preiview.transform.position = previewPos;
        }
    }
    private void Build()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if(go_Preiview.GetComponent<PreviewObject>().isBuildable())
            {
                theQuickSlot.DecreaseSelectedItem(); // 슬롯 아이템 개수 -1
                GameObject temp = Instantiate(currentKit.kitPrefab, previewPos, Quaternion.identity);
                temp.name = currentKit.itemName;
                Destroy(go_Preiview);
                currentKit = null; 
                isPreview = false;
            }
        }
    }
    public void Cancel()
    {
        Destroy(go_Preiview);
        currentKit = null; 
        isPreview = false;
    }
    private void TryEating()
    {
        if (Input.GetButtonDown("Fire1") && !theQuickSlot.GetIsCoolTime())
        {
            currentcloseWeapon.anim.SetTrigger("Eat");
            theQuickSlot.DecreaseSelectedItem();
        }
    }

    protected override IEnumerator HitCoroutine()
    {
        while (isSwing)
        {
            if (CheckObject())
            {
                //false여서 실행후 true로 계속 hit되는거 1번 hit되게 막아줌
                isSwing = false;
                //충돌했음
                //충돌하면 데미지를 입힐건지 나무를벨건지 여기서 정함
                //그래서 이걸 상속받은곳에서 구현시킴
                Debug.Log(hitInfo.transform.name);
            }
            yield return null;
        }
    }

    public override void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        base.CloseWeaponChange(_closeWeapon);
        isActivate = true;
    }
}
