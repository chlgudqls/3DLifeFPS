using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayAndNight : MonoBehaviour
{
    [SerializeField] private float secondPerRealTimeSecond; // ���� ���迡���� 100�� = ���� ������ 1��

    [SerializeField] private float fogDensityCalc; // ������ ����

    [SerializeField] private float nightFogDensity; // �� ������ Fog �е�
    private float dayFogDensity; // �� ������ Fog �е�
    private float currentFogDensity; // ���
    void Start()
    {
        dayFogDensity = RenderSettings.fogDensity;
    }

    void Update()
    {
        transform.Rotate(Vector3.right, 0.1f * secondPerRealTimeSecond * Time.deltaTime);

        //�ذ� ��������
        if (transform.eulerAngles.x >= 170)
            GameManager.isNight = true;
        // ���⼭ 10���� 350���� �ٲ����� �ضߴ� �ð��� ���� ��Ȯ�ϰ� �������ϴµ�
        // �������� ��ũ��Ʈ���� �����̾ȵ� �׷��� 350�� ����
        // 350���� ���۵Ǵ°Ű� ��ȣ���مf�µ� ����Ƽ�󿡼� �������̳��� 350�����Ȱ��� ���������� ����� �ٽú�ȯ�ϴ�
        // ����Ŭ�� ���ư����� ���� ������ ���̶� ����Ƽ���̶� ��¦ �ٸ�
        else if (transform.eulerAngles.x >= 350)
            GameManager.isNight = false;

        if (GameManager.isNight)
        {
            // ��� �����ϴµ� �����Ѱ��� �����ʰ��ϰ� �ִ��μ� 2������������ ������Ű��
            if(currentFogDensity <= nightFogDensity)
            {
                currentFogDensity += 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            }
        }
        else
        {
            // �ּڰ����� Ŭ���� ���ҽ�Ű��
            if(currentFogDensity >= dayFogDensity)
            {
                currentFogDensity -= 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            }
        }
    }
}
