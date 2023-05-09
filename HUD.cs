using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private GunController theGunController;
    //건컨트롤러의 건스크립트를 쓸거임
    private Gun currentGun;

    [SerializeField]
    private GameObject go_BulletHUD;
    //텍스트에 총알 개수 반영
    [SerializeField]
    private Text[] text_Bullet; 

  
    void Update()
    {
        CheckBullet();
    }
    private void CheckBullet()
    {
        currentGun = theGunController.GetGun();
        text_Bullet[0].text = currentGun.carryBulletCount.ToString();
        text_Bullet[1].text = currentGun.reloadBulletCount.ToString();
        text_Bullet[2].text = currentGun.currentBulletCount.ToString();
    }
}
