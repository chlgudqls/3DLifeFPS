using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : CloseWeaponController
{
    public static bool isActivate = false;
    public static Item currentKit; // ��ġ�Ϸ��� Ŷ (���� ���̺�)

    private bool isPreview = false;

    private GameObject go_Preiview; // ��ġ�� ŰƮ ������
    private Vector3 previewPos; // ��ġ�� ŰƮ ��ġ
    [SerializeField] private float rangeAdd; // ����� �߰� �����Ÿ�

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
                theQuickSlot.DecreaseSelectedItem(); // ���� ������ ���� -1
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
                //false���� ������ true�� ��� hit�Ǵ°� 1�� hit�ǰ� ������
                isSwing = false;
                //�浹����
                //�浹�ϸ� �������� �������� ������������ ���⼭ ����
                //�׷��� �̰� ��ӹ��������� ������Ŵ
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
