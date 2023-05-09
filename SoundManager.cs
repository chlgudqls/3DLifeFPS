using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ŭ�������� �̷������� ����
[System.Serializable]
public class Sound
{
    public string name; // ���� �̸�
    public AudioClip clip; // ��
}
public class SoundManager : MonoBehaviour
{
    static public SoundManager instance;

    #region singleton
    //��ü ������ ���� 1ȸ����
    //2��° ������ awake�ȿ� �ִ°� �ٽ� ����� �׷��� ����
    //ó���� ���̴ϱ� ���־��ְ� ���������� �̵��ص� �״�� ����
    //���̾ƴѰ��
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //�ѹ��� �����ϱ� ����
            //���ο� ������ �ı�����
            DontDestroyOnLoad(gameObject);
        }
        else
            //���ο�� ������
            Destroy(this.gameObject);
    }

    #endregion singleton
    void Start()
    {
        playSoundName = new string[audioSourceEffects.Length];
    }
    //effect �����÷��̾�
    public AudioSource[] audioSourceEffects;
    //Bgm �����÷��̾�
    public AudioSource audioSourceBgm;

    //������� ������̸�
    public string[] playSoundName;

    public Sound[] effectSounds;
    public Sound[] bgmSounds;
    public void PlaySE(string _name)
    {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            if(_name == effectSounds[i].name)
            {
                for (int j = 0; j < audioSourceEffects.Length; j++)
                {
                    //���⼭ ����ִ� �÷��̾���� �ɷ���
                    if (!audioSourceEffects[j].isPlaying)
                    {
                        playSoundName[j] = effectSounds[i].name;
                        audioSourceEffects[j].clip = effectSounds[i].clip;
                        audioSourceEffects[j].Play();
                        //��������� ���̻� for���� ���� �ʰ��ؾߵ�
                        return;
                    }
                }
                //��ġ�ϴ°� �ִµ� ����ִ� �÷��̾ ������ return�������
                Debug.Log("��� ���� Audio Source�� ������Դϴ�");
                return;
            }
        }
        //����� ������ ��ġ�ϴ� Ŭ���� ���ٴ°� �� for���� ������ return��������
        Debug.Log(_name + "���尡 SoundŬ������ ��ϵ��� �ʾҽ��ϴ�.");
    }
    public void StopAllSE()
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].Stop();
        }
    }
    public void StopSE(string _name)
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            if(_name == playSoundName[i])
            {
                audioSourceEffects[i].Stop();
                return;
            }
        }
        Debug.Log("������� " + _name + "���尡 �����ϴ�.");
    }
}
