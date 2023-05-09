using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class FieldOfViewAngle : MonoBehaviour
{
    [SerializeField] private float viewAngle; // �þ߰� (120��)
    [SerializeField] private float viewDistance; // �þ߰Ÿ� (10����)
    [SerializeField] private LayerMask targetMask; // Ÿ�� ����ũ (�÷��̾�)

    // �÷��̾��� ��ġ�� ������ ������ ��
    // �þ߰��� ���� �÷��̾��� ��ġ�� �� ��ũ��Ʈ���� ��������ߵǳ���
    private PlayerController thePlayer;
    //private Pig thePig;
    private NavMeshAgent nav;

    void Start()
    {
        thePlayer = FindObjectOfType<PlayerController>();
        //thePig = GetComponent<Pig>();
        nav = GetComponent<NavMeshAgent>();
    }
    public Vector3 GetTargetPos()
    {
        return thePlayer.transform.position;
    }
    //void Update()
    //{
    //    View();
    //}
    //private Vector3 BoundaryAngle(float _angle)
    //{
    //    // ���� y���� �ٲ�� �¿������� ���Ѵٴ¶�
    //    _angle += transform.eulerAngles.y;
    //    return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    //}
    public bool View()
    {
        // �� ���� limit ��踦 ����
        // ������ �̿��ؼ� �������� sin ,cos ���
        //Vector3 _leftBoundary = BoundaryAngle(-viewAngle * 0.5f);
        //Vector3 _rightBoundary = BoundaryAngle(viewAngle * 0.5f);

        //Debug.DrawRay(transform.position + transform.up, _leftBoundary, Color.red);
        //Debug.DrawRay(transform.position + transform.up, _rightBoundary, Color.red);

        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance, targetMask);

        for (int i = 0; i < _target.Length; i++)
        {
            // 1.
            Transform _tartgetTf = _target[i].transform;
            if (_tartgetTf.name == "Player")
            {
                Vector3 direction = (_tartgetTf.position - transform.position).normalized;
                float _angle = Vector3.Angle(direction, transform.forward);
                if(_angle < viewAngle * 0.5f)
                {
                    RaycastHit _hit;
                    if (Physics.Raycast(transform.position + transform.up, direction, out _hit, viewDistance))
                    {
                        if(_hit.transform.name == "Player")
                        {
                            Debug.DrawRay(transform.position + transform.up, direction, Color.blue);
                            // �÷��̾ �þ߰��ȿ� ��Ÿ������ �����ൿ
                            // �þ߰��� ������ ���� Ÿ�Կ����� �ٸ� �ൿ ���ϰ� �ҰŶ� boolŸ������ return ��Ų�ٰ���
                            //thePig.Run(_hit.transform.position); 
                            return true;
                        }
                    }
                }
            }
            // 2.
            if (thePlayer.GetRun())
            {
                if(CalPathLength(thePlayer.transform.position) <= viewDistance)
                {
                    Debug.Log("������ �ֺ����� �ٰ��ִ� �÷��̾��� �������� �ľ��߽��ϴ�");
                    return true;
                }
            }
        }
        return false;  
    }
    private float CalPathLength(Vector3 _targetPos)
    {
        // ���� �׺��̰��̼ǿ� �ý��ۿ����� ���� ��θ� ����
        NavMeshPath _path = new NavMeshPath();
        //��θ� �̿��ؼ� �Ÿ��� ����
        // ���� Ư�������� ���� ��θ� ����ؼ� _path�� �����Ŵ
        nav.CalculatePath(_targetPos, _path);

        // ����� �ڳ� point�� �˼����� ���� ������Ƽ �ű⿡ �ڽŰ� ��ǥ������ point �߰�
        Vector3[] _wayPoint = new Vector3[_path.corners.Length + 2];

        _wayPoint[0] = transform.position;
        _wayPoint[_path.corners.Length + 1] = _targetPos;

        //���� �̰�
        float _pathLegnth = 0;
        for (int i = 0; i < _path.corners.Length; i++)
        {
            _wayPoint[i + 1] = _path.corners[i]; // ��������Ʈ�� ��θ� ����
            // �� ������ ���԰� ���ÿ� �� ������ �Ÿ��� ��� �����ָ鼭 ��θ� �������
            // ��������ġ��?
            _pathLegnth += Vector3.Distance(_wayPoint[i], _wayPoint[i + 1]);
            //Debug.Log(i);
            // ���� ���� �߰��� �ε����� 11�����ִµ� 10���ݺ��̸� ��������ġ �Ÿ������ ����
            if(_path.corners.Length-1 == i)
                _pathLegnth += Vector3.Distance(_wayPoint[i + 1], _wayPoint[_path.corners.Length + 1]);
            //Debug.Log(i+1);
        }
        // ���̸� �������� �������
        return _pathLegnth;
    }
}
