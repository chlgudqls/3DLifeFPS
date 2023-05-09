using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayAndNight : MonoBehaviour
{
    [SerializeField] private float secondPerRealTimeSecond; // 게임 세계에서의 100초 = 현실 세계의 1초

    [SerializeField] private float fogDensityCalc; // 증감량 비율

    [SerializeField] private float nightFogDensity; // 밤 상태의 Fog 밀도
    private float dayFogDensity; // 낮 상태의 Fog 밀도
    private float currentFogDensity; // 계산
    void Start()
    {
        dayFogDensity = RenderSettings.fogDensity;
    }

    void Update()
    {
        transform.Rotate(Vector3.right, 0.1f * secondPerRealTimeSecond * Time.deltaTime);

        //해가 지고있음
        if (transform.eulerAngles.x >= 170)
            GameManager.isNight = true;
        // 여기서 10에서 350으로 바꾼이유 해뜨는 시간을 좀더 정확하게 계산려고하는데
        // 음수값은 스크립트에서 적용이안됨 그래서 350을 설정
        // 350부터 시작되는거고 기호도바꿧는데 유니티상에선 일정값이나면 350까진안가고 음수값에서 양수로 다시변환하는
        // 싸이클이 돌아갈거임 대충 로직의 값이랑 유니티값이랑 살짝 다름
        else if (transform.eulerAngles.x >= 350)
            GameManager.isNight = false;

        if (GameManager.isNight)
        {
            // 계속 증가하는데 설정한값을 넘지않게하고 최댓값인셈 2보다작을동안 증가시키고
            if(currentFogDensity <= nightFogDensity)
            {
                currentFogDensity += 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            }
        }
        else
        {
            // 최솟값보다 클동안 감소시키고
            if(currentFogDensity >= dayFogDensity)
            {
                currentFogDensity -= 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            }
        }
    }
}
