using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeComponent : MonoBehaviour
{
    //���� ���� ������
    [SerializeField]
    private GameObject[] go_treePieces;
    [SerializeField]
    private GameObject go_treeCenter;
    //�볪��
    [SerializeField]
    private GameObject go_Log_Prefabs;

    //�������� �������� ������ ���� ����
    [SerializeField]
    private float force;
    //�� ���� �ڽ�Ʈ�� 
    [SerializeField]
    private GameObject go_ChildTree;

    [SerializeField]
    private CapsuleCollider parentCol;
    [SerializeField]
    private CapsuleCollider childCol;
    [SerializeField]
    private Rigidbody childRigid;
    //����
    [SerializeField]
    private GameObject go_hit_effect_prefab;

    //���� ���� �ð�
    [SerializeField]
    private float debrisDestroyTime;
    //���� ���� �ð�
    [SerializeField]
    private float destroyTime;

    //�ʿ��� ����
    [SerializeField]
    private string chop_sound;
    [SerializeField]
    private string falldown_sound;
    [SerializeField]
    private string logChange_sound;
    
    //���ߴ��� ��ġ�� ��� �ѱ�°���
    public void Chop(Vector3 _pos, float angleY)
    {
        Hit(_pos);
        SoundManager.instance.PlaySE(chop_sound);
        AngleCalc(angleY);

        //�� ��ġ���� ���Ͻ�Ű�ϱ� �� �Ʒ��� ������ �ȵǴ°ų� 
        //��� true���� �����ϴٰ� ��� null�̸� for���������ͼ�
        //false ��ȯ�ϸ� CheckTreePieces �� ���Ͼ��ϴϱ� �Ʒ� ������ ����Ҽ�����
        if (CheckTreePieces())
            return;

        //�׷��� ������ ����Ʈ��
        FallDownTree();
    }

    //���� ����Ʈ
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
        //���µ� �����϶�� �Ҽ�����
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

        //�ȱ������־
        //�������� ����������
        //���� ��������
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
