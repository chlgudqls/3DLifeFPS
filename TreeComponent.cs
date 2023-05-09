using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeComponent : MonoBehaviour
{
    //깎일 나무 조각들
    [SerializeField]
    private GameObject[] go_treePieces;
    [SerializeField]
    private GameObject go_treeCenter;
    //통나무
    [SerializeField]
    private GameObject go_Log_Prefabs;

    //쓰러질때 랜덤으로 가해질 힘의 세기
    [SerializeField]
    private float force;
    //얜 뭐지 자식트리 
    [SerializeField]
    private GameObject go_ChildTree;

    [SerializeField]
    private CapsuleCollider parentCol;
    [SerializeField]
    private CapsuleCollider childCol;
    [SerializeField]
    private Rigidbody childRigid;
    //파편
    [SerializeField]
    private GameObject go_hit_effect_prefab;

    //파편 제거 시간
    [SerializeField]
    private float debrisDestroyTime;
    //나무 제거 시간
    [SerializeField]
    private float destroyTime;

    //필요한 사운드
    [SerializeField]
    private string chop_sound;
    [SerializeField]
    private string falldown_sound;
    [SerializeField]
    private string logChange_sound;
    
    //적중당한 위치를 어떻게 넘기는거지
    public void Chop(Vector3 _pos, float angleY)
    {
        Hit(_pos);
        SoundManager.instance.PlaySE(chop_sound);
        AngleCalc(angleY);

        //이 위치에서 리턴시키니까 이 아래가 실행이 안되는거네 
        //계속 true여서 리턴하다가 모두 null이면 for문빠져나와서
        //false 반환하면 CheckTreePieces 이 리턴안하니까 아래 로직을 사용할수있음
        if (CheckTreePieces())
            return;

        //그러면 나무를 쓰러트림
        FallDownTree();
    }

    //적중 이펙트
    private void Hit(Vector3 _pos)
    {
        SoundManager.instance.PlaySE(chop_sound);

        GameObject clone = Instantiate(go_hit_effect_prefab, _pos, Quaternion.Euler(Vector3.zero));
        Destroy(clone, debrisDestroyTime);
    }
    private void AngleCalc(float _angleY)
    {
        Debug.Log(_angleY);
        if (0 <= _angleY && _angleY <= 70)
            DestroyPiece(2);
        else if (70 < _angleY && _angleY <= 140)
            DestroyPiece(3);
        else if (140 < _angleY && _angleY <= 210)
            DestroyPiece(4);
        else if (210 < _angleY && _angleY <= 280)
            DestroyPiece(0);
        else if (280 < _angleY && _angleY <= 360)
            DestroyPiece(1);

    }
    private void DestroyPiece(int _num)
    {
        //없는데 삭제하라고 할수없음
        if(go_treePieces[_num].gameObject != null)
        {
            GameObject clone = Instantiate(go_hit_effect_prefab, go_treePieces[_num].transform.position, Quaternion.Euler(Vector3.zero));
            Destroy(clone, debrisDestroyTime);
            Destroy(go_treePieces[_num].gameObject);
        }
    }
    private bool CheckTreePieces()
    {
        for (int i = 0; i < go_treePieces.Length; i++)
        {
            if (go_treePieces[i].gameObject != null)
                return true;
        }
        return false;
    }
    private void FallDownTree()
    {
        SoundManager.instance.PlaySE(falldown_sound);
        Destroy(go_treeCenter);

        parentCol.enabled = false;
        childCol.enabled = true;
        childRigid.useGravity = true;

        //안기울수가있어서
        //랜덤으로 기울어지게함
        //힘을 랜덤으로
        childRigid.AddForce(Random.Range(-force, force), 0f, Random.Range(-force, force));

        //Destroy(go_ChildTree.gameObject, destroyTime);
        StartCoroutine(LogCoroutine());
    }
    IEnumerator LogCoroutine()
    {
        yield return new WaitForSeconds(destroyTime);

        SoundManager.instance.PlaySE(logChange_sound);

        Instantiate(go_Log_Prefabs, go_ChildTree.transform.position + (go_ChildTree.transform.up * 3f), Quaternion.LookRotation(go_ChildTree.transform.up));
        Instantiate(go_Log_Prefabs, go_ChildTree.transform.position + (go_ChildTree.transform.up * 6f), Quaternion.LookRotation(go_ChildTree.transform.up));
        Instantiate(go_Log_Prefabs, go_ChildTree.transform.position + (go_ChildTree.transform.up * 9f), Quaternion.LookRotation(go_ChildTree.transform.up));

        Destroy(go_ChildTree.gameObject);
    }
    public Vector3 GetTreeCenterPosition()
    {
        return go_treeCenter.transform.position;
    }
}
