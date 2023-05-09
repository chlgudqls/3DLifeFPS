using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Water : MonoBehaviour
{
    [SerializeField] private float waterDrag; // 물속 중력
    private float originDrag;

    [SerializeField] private Color waterColor; // 물속 색깔
    [SerializeField] private float waterFogDensity; // 물 탁함 정도

    [SerializeField] private Color waterNightColor; // 밤 상태의 물속 색깔
    [SerializeField] private float waterNightFogDensity;

    private Color originColor;
    private float originFogDensity;

    [SerializeField] private Color originNightColor;
    [SerializeField] private float originNigihtFogDensity;

    [SerializeField] private string sound_WaterOut;
    [SerializeField] private string sound_WaterIn;
    [SerializeField] private string sound_Breathe;

    [SerializeField] private float breatheTime;
    private float currentBreatheTime;

    [SerializeField] private float totalOxygen;
    private float currentOxygen;
    private float temp;

    [SerializeField] private GameObject go_BaseUI;
    [SerializeField] private Text text_currentOxygen;
    [SerializeField] private Text text_totalOxygen;
    [SerializeField] private Image image_gauge;

    private StatusController thePlayerStat;

    void Start()
    {
        originColor = RenderSettings.fogColor;
        originFogDensity = RenderSettings.fogDensity;

        originDrag = 0;
        thePlayerStat = FindObjectOfType<StatusController>();
        currentOxygen = totalOxygen;
        text_totalOxygen.text = totalOxygen.ToString();
    }

    void Update()
    {
        currentBreatheTime += Time.deltaTime;
        // update에서 주기적으로 사운드 발생시킬건데 프레임마다 발생시키면 안되니까 로직추가
        if (GameManager.isWater)
        {
            if(currentBreatheTime >= breatheTime)
            {
                SoundManager.instance.PlaySE(sound_Breathe);
                currentBreatheTime = 0;
            }
        }
        DecreaseOxygen();
    }
    private void DecreaseOxygen()
    {
        if(GameManager.isWater)
        {
            currentOxygen -= Time.deltaTime;
            text_currentOxygen.text = Mathf.RoundToInt(currentOxygen).ToString();
            // fillAmount 는 % 즉 0 ~ 1사이 그래서 이런식으로 백분율로 계산 
            image_gauge.fillAmount = currentOxygen / totalOxygen;

            if (currentOxygen <= 0)
            {
                temp += Time.deltaTime; 
                if(temp >= 1)
                {
                    thePlayerStat.DecreaseHP(1);
                    temp = 0;
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            GetWater(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            GetOutWater(other);
        }
    }
    private void GetWater(Collider _player)
    {
         SoundManager.instance.PlaySE(sound_WaterIn);

        go_BaseUI.SetActive(true);
        GameManager.isWater = true;
        // 닿은 collider를 넘긴이유 그 컴포넌트의 drag를 바꾸기위해서 물속에 들어왔으니까
        _player.GetComponent<Rigidbody>().drag = waterDrag;

        if (!GameManager.isNight)
        {
            RenderSettings.fogColor = waterColor;
            RenderSettings.fogDensity = waterFogDensity;
        }
        else
        {
            RenderSettings.fogColor = waterNightColor;
            RenderSettings.fogDensity = waterNightFogDensity;
        }
    }
    private void GetOutWater(Collider _player)
    {
        if(GameManager.isWater)
        {
            go_BaseUI.SetActive(false);
            currentOxygen = totalOxygen;
            SoundManager.instance.PlaySE(sound_WaterOut);

            GameManager.isWater = false;
            _player.GetComponent<Rigidbody>().drag = originDrag;

            if (!GameManager.isNight)
            {
                RenderSettings.fogColor = originColor;
                RenderSettings.fogDensity = originFogDensity;
            }
            else
            {
                RenderSettings.fogColor = originNightColor;
                RenderSettings.fogDensity = originNigihtFogDensity;
            }
        }
    }
}
