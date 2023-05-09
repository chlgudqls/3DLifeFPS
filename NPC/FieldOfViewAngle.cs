using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class FieldOfViewAngle : MonoBehaviour
{
    [SerializeField] private float viewAngle; // 시야각 (120도)
    [SerializeField] private float viewDistance; // 시야거리 (10미터)
    [SerializeField] private LayerMask targetMask; // 타겟 마스크 (플레이어)

    // 플레이어의 위치를 보내줌 왜인진 모름
    // 시야각에 들어온 플레이어의 위치를 이 스크립트에서 리턴해줘야되나봄
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
    //    // 대충 y값을 바꿔야 좌우측값이 변한다는뜻
    //    _angle += transform.eulerAngles.y;
    //    return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    //}
    public bool View()
    {
        // 좌 우측 limit 경계를 구함
        // 각도를 이용해서 점을구함 sin ,cos 사용
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
                            // 플레이어가 시야각안에 나타났으면 취할행동
                            // 시야각에 들어오면 동물 타입에따라서 다른 행동 취하게 할거라서 bool타입으로 return 시킨다고함
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
                    Debug.Log("돼지가 주변에서 뛰고있는 플레이어의 움직임을 파악했습니다");
                    return true;
                }
            }
        }
        return false;  
    }
    private float CalPathLength(Vector3 _targetPos)
    {
        // 설명에 네베이게이션에 시스템에의해 계산된 경로를 저장
        NavMeshPath _path = new NavMeshPath();
        //경로를 이용해서 거리를 구함
        // 설명에 특정지역에 대한 경로를 계산해서 _path에 저장시킴
        nav.CalculatePath(_targetPos, _path);

        // 경로의 코너 point를 알수있음 내장 프로퍼티 거기에 자신과 목표지점의 point 추가
        Vector3[] _wayPoint = new Vector3[_path.corners.Length + 2];

        _wayPoint[0] = transform.position;
        _wayPoint[_path.corners.Length + 1] = _targetPos;

        //뭐지 이건
        float _pathLegnth = 0;
        for (int i = 0; i < _path.corners.Length; i++)
        {
            _wayPoint[i + 1] = _path.corners[i]; // 웨이포인트에 경로를 넣음
            // 각 점들을 구함과 동시에 점 사이의 거리를 계속 더해주면서 경로를 계산했음
            // 마지막위치는?
            _pathLegnth += Vector3.Distance(_wayPoint[i], _wayPoint[i + 1]);
            //Debug.Log(i);
            // 내가 따로 추가함 인덱스가 11까지있는데 10번반복이면 마지막위치 거리계산은 안함
            if(_path.corners.Length-1 == i)
                _pathLegnth += Vector3.Distance(_wayPoint[i + 1], _wayPoint[_path.corners.Length + 1]);
            //Debug.Log(i+1);
        }
        // 길이를 구했으니 결과리턴
        return _pathLegnth;
    }
}
