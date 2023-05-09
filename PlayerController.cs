using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    static public bool isActivated = true;

    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float crouchSpeed;
    [SerializeField]
    private float swimSpeed;
    [SerializeField]
    private float swimFastSpeed;
    [SerializeField]
    private float upSwimSpeed;


    //여기에 대입한다고함    
    //이걸 만들어야 편리한듯
    //move함수를 변경하지않아도 된다고함
    private float applySpeed;


    [SerializeField]
    private float jumpForce;

    //상태 변수
    //이게 바뀔때마다 크로스헤어 변경
    private bool isWalk = false;
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;

    //움직임 체크 변수
    //전 프레임의 현재위치와 현재 프레임의 현재위치를 비교
    private Vector3 lastPos;

    //앉았을때 얼마나 앉을지 결정하는 변수
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;

    //땅 착지 여부
    private CapsuleCollider capsuleCollider;

    [SerializeField]
    //카메라 민감도
    private float lookSensitivity;

    [SerializeField]
    //카메라각도 제한
    private float cameraRotationLimit;
    //각도
    private float currentCameraRotationX = 0;

    [SerializeField]
    private Camera theCamera;
    private Rigidbody myRigid;
    private GunController theGunController;
    private CrossHair theCrosshair;
    private StatusController theStatusController;
    void Start()
    {

        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        theGunController = FindObjectOfType<GunController>();
        theCrosshair = FindObjectOfType<CrossHair>();
        theStatusController = FindObjectOfType<StatusController>();


        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
    }
 
    void Update()
    {
        if (isActivated && GameManager.canPlayerMove)
        {
            WaterCheck();
            IsGround();
            TryJump();
            if(!GameManager.isWater)
            {
                TryRun();
            }
            TryCrouch();
            Move();
            MoveCheck();
            CameraRotation();
            CharacterRotation();
        }
    }
    private void WaterCheck()
    {
        if (GameManager.isWater)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
                applySpeed = swimFastSpeed;
            else
                applySpeed = swimSpeed;

        }
    }
    //앉기 시도
    private void TryCrouch()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }
    //앉기 실행
    private void Crouch()
    {
        //누를때마다 바꿈 스위치느낌
        isCrouch = !isCrouch;
        theCrosshair.CrouchingAnimation(isCrouch);
        if (isCrouch)
        {
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else
        {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }
        //캐릭터가 앉는건데 카메라도 같이 움직이게하려고
        //y값을 변경하려함
        theCamera.transform.localPosition = new Vector3(theCamera.transform.localPosition.x, applyCrouchPosY, theCamera.transform.localPosition.z);
    }
    //서서히 앉기
    IEnumerator CrouchCoroutine()
    {
        float _posY = theCamera.transform.localPosition.y;

        while(_posY != applyCrouchPosY)
        {
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.3f);
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);
            yield return null;
        }
    }
    //땅 감지여부를 raycast로 true false 확인
    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.3f);
        theCrosshair.JumpingAnimation(!isGround);
    }
    //점프 시도
    private void TryJump()  
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround && theStatusController.GetCurrentSP() > 0 && !GameManager.isWater)
            jump();
        else if (Input.GetKey(KeyCode.Space) && GameManager.isWater)
            UpSwim();
    }
    private void UpSwim()
    {
        myRigid.velocity = transform.up * upSwimSpeed;
    }
    //점프 실행
    private void jump()
    {
        if (isCrouch)
            Crouch();

        theStatusController.DecreaseStamina(100);

        myRigid.velocity = transform.up * jumpForce;
    }
    //달리기 시도
    private void TryRun()
    {
        if(Input.GetKey(KeyCode.LeftShift) && theStatusController.GetCurrentSP() > 0)
        {
            Running();
        }
        if(Input.GetKeyUp(KeyCode.LeftShift) || theStatusController.GetCurrentSP() <= 0)
        {
            RunningCancel();
        }
    }
    //달리기 실행
    private void Running()
    {
        if (isCrouch)
            Crouch();
            
        theGunController.CancelFineSight();
        theStatusController.DecreaseStamina(10);

        isRun = true;
        theCrosshair.RunningAnimation(isRun);
        applySpeed = runSpeed;
    }
    //달리기 취소
    private void RunningCancel()
    {
        isRun = false;
        theCrosshair.RunningAnimation(isRun);
        applySpeed = walkSpeed;
    }
    //이동
    //매프레임마다 실행되기때문에 이것만으로 움직이는지 확인하기 어려움
    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHrizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHrizontal + _moveVertical).normalized * applySpeed;
        //현재위치에 입력값을 더해준다 순간이동 방지하기위해서 time 곱해줌 update안에 1초가 들어있으니 1초동안 실행되는느낌
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }
    private void MoveCheck()
    {
        //달릴땐 체크안해도됨
        //이유가 달릴땐 크로스헤어 안써서
        //걷고있는거 체크하는거니까 굳이 false일때 실행되지않도록
        if(!isRun && !isCrouch && isGround)
        {
            //이게 다르다는건 움직였다는거니까 true 로 바꿈
            //아주 작은 차이가 있을수있음 경사로같은거 때문에
            //if(lastPos != transform.position)
            //그래서 이 사이거리체크
            if (Vector3.Distance(lastPos,transform.position) >= 0.01f)
                isWalk = true;
            else
                isWalk = false;

                theCrosshair.WalkingAnimation(isWalk);
                lastPos = transform.position;
        }
    }
    //rigid를 활용한 플레이어 좌우 회전
    private void CharacterRotation()
    {
        //좌우 캐릭터 회전
        float _yRotation = Input.GetAxisRaw("Mouse X");
        //오일러값
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        //로테이션은 쿼터니언값 구한 백터값을 쿼터니언으로변경
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
        //Debug.Log("?"+myRigid.rotation);
        //Debug.Log("!"+myRigid.rotation.eulerAngles);
    }
    //localEulerAngles에 vector값을 대입한 상하 회전
    private void CameraRotation()
    {
        if (!pauseCameraRotation)
        {
        //상하 카메라 회전
        //마우스포인트위치 y값만
        float _xRotaion = Input.GetAxisRaw("Mouse Y");
        //이동수치조정
        float _cameraRotationX = _xRotaion * lookSensitivity;
        //여기서 생각좀
        currentCameraRotationX -= _cameraRotationX;
        //max값조정
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        //입력하는만큼 카메라한테 값전달
        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }

    }
    //나무를 바라보는 동안에 잠시 꺼준다고함
    private bool pauseCameraRotation = false;

    //여기 어려움 다시 봐야됨
    //_target 바라볼대상 
    public IEnumerator TreeLookCoroutine(Vector3 _target)
    {
        pauseCameraRotation = true;

        //나무위치와 플레이어 위치 어느곳을 바라볼건지
        Quaternion direction = Quaternion.LookRotation(_target - theCamera.transform.position);
        Vector3 eulerValue = direction.eulerAngles;
        //뭔말 반복문에 쓴다고함
        float destinationX = eulerValue.x;

        while(Mathf.Abs(destinationX - currentCameraRotationX) >= 0.5f)
        {
            eulerValue = Quaternion.Lerp(theCamera.transform.localRotation, direction, 0.3f).eulerAngles;
            theCamera.transform.localRotation = Quaternion.Euler(eulerValue.x, 0f, 0f);
            currentCameraRotationX = theCamera.transform.localEulerAngles.x;
            yield return null;
        }

        pauseCameraRotation = false;
    }
    
    // 그냥 이것만 넘겨도 상관없는지
    public bool GetRun()
    {
        return isRun;
    }
}
