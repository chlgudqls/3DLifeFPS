using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//클래스에는 이런식으로 붙임
[System.Serializable]
public class Sound
{
    public string name; // 곡의 이름
    public AudioClip clip; // 곡
}
public class SoundManager : MonoBehaviour
{
    static public SoundManager instance;

    #region singleton
    //객체 생성시 최초 1회실행
    //2번째 씬에서 awake안에 있는게 다시 실행됨 그래서 삭제
    //처음엔 널이니까 값넣어주고 다음씬으로 이동해도 그대로 유지
    //널이아닌경우
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //한번만 생성하기 때문
            //새로운 씬에서 파괴방지
            DontDestroyOnLoad(gameObject);
        }
        else
            //새로운게 삭제됨
            Destroy(this.gameObject);
    }

    #endregion singleton
    void Start()
    {
        playSoundName = new string[audioSourceEffects.Length];
    }
    //effect 관리플레이어
    public AudioSource[] audioSourceEffects;
    //Bgm 관리플레이어
    public AudioSource audioSourceBgm;

    //재생중인 오디오이름
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
                    //여기서 비어있는 플레이어들이 걸러짐
                    if (!audioSourceEffects[j].isPlaying)
                    {
                        playSoundName[j] = effectSounds[i].name;
                        audioSourceEffects[j].clip = effectSounds[i].clip;
                        audioSourceEffects[j].Play();
                        //재생했으면 더이상 for문이 돌지 않게해야됨
                        return;
                    }
                }
                //일치하는게 있는데 비어있는 플레이어가 없으면 return실행안함
                Debug.Log("모든 가용 Audio Source가 사용중입니다");
                return;
            }
        }
        //여기로 빠지면 일치하는 클립이 없다는거 돌 for문이 없으니 return쓰지않음
        Debug.Log(_name + "사운드가 Sound클래스에 등록되지 않았습니다.");
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
        Debug.Log("재생중인 " + _name + "사운드가 없습니다.");
    }
}
