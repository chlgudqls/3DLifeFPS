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


    //���⿡ �����Ѵٰ���    
    //�̰� ������ ���ѵ�
    //move�Լ��� ���������ʾƵ� �ȴٰ���
    private float applySpeed;


    [SerializeField]
    private float jumpForce;

    //���� ����
    //�̰� �ٲ𶧸��� ũ�ν���� ����
    private bool isWalk = false;
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;

    //������ üũ ����
    //�� �������� ������ġ�� ���� �������� ������ġ�� ��
    private Vector3 lastPos;

    //�ɾ����� �󸶳� ������ �����ϴ� ����
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;

    //�� ���� ����
    private CapsuleCollider capsuleCollider;

    [SerializeField]
    //ī�޶� �ΰ���
    private float lookSensitivity;

    [SerializeField]
    //ī�޶󰢵� ����
    private float cameraRotationLimit;
    //����
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
    //�ɱ� �õ�
    private void TryCrouch()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }
    //�ɱ� ����
    private void Crouch()
    {
        //���������� �ٲ� ����ġ����
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
        //ĳ���Ͱ� �ɴ°ǵ� ī�޶� ���� �����̰��Ϸ���
        //y���� �����Ϸ���
        theCamera.transform.localPosition = new Vector3(theCamera.transform.localPosition.x, applyCrouchPosY, theCamera.transform.localPosition.z);
    }
    //������ �ɱ�
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
    //�� �������θ� raycast�� true false Ȯ��
    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.3f);
        theCrosshair.JumpingAnimation(!isGround);
    }
    //���� �õ�
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
    //���� ����
    private void jump()
    {
        if (isCrouch)
            Crouch();

        theStatusController.DecreaseStamina(100);

        myRigid.velocity = transform.up * jumpForce;
    }
    //�޸��� �õ�
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
    //�޸��� ����
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
    //�޸��� ���
    private void RunningCancel()
    {
        isRun = false;
        theCrosshair.RunningAnimation(isRun);
        applySpeed = walkSpeed;
    }
    //�̵�
    //�������Ӹ��� ����Ǳ⶧���� �̰͸����� �����̴��� Ȯ���ϱ� �����
    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHrizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHrizontal + _moveVertical).normalized * applySpeed;
        //������ġ�� �Է°��� �����ش� �����̵� �����ϱ����ؼ� time ������ update�ȿ� 1�ʰ� ��������� 1�ʵ��� ����Ǵ´���
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }
    private void MoveCheck()
    {
        //�޸��� üũ���ص���
        //������ �޸��� ũ�ν���� �ȽἭ
        //�Ȱ��ִ°� üũ�ϴ°Ŵϱ� ���� false�϶� ��������ʵ���
        if(!isRun && !isCrouch && isGround)
        {
            //�̰� �ٸ��ٴ°� �������ٴ°Ŵϱ� true �� �ٲ�
            //���� ���� ���̰� ���������� ���ΰ����� ������
            //if(lastPos != transform.position)
            //�׷��� �� ���̰Ÿ�üũ
            if (Vector3.Distance(lastPos,transform.position) >= 0.01f)
                isWalk = true;
            else
                isWalk = false;

                theCrosshair.WalkingAnimation(isWalk);
                lastPos = transform.position;
        }
    }
    //rigid�� Ȱ���� �÷��̾� �¿� ȸ��
    private void CharacterRotation()
    {
        //�¿� ĳ���� ȸ��
        float _yRotation = Input.GetAxisRaw("Mouse X");
        //���Ϸ���
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        //�����̼��� ���ʹϾ� ���� ���Ͱ��� ���ʹϾ����κ���
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
        //Debug.Log("?"+myRigid.rotation);
        //Debug.Log("!"+myRigid.rotation.eulerAngles);
    }
    //localEulerAngles�� vector���� ������ ���� ȸ��
    private void CameraRotation()
    {
        if (!pauseCameraRotation)
        {
        //���� ī�޶� ȸ��
        //���콺����Ʈ��ġ y����
        float _xRotaion = Input.GetAxisRaw("Mouse Y");
        //�̵���ġ����
        float _cameraRotationX = _xRotaion * lookSensitivity;
        //���⼭ ������
        currentCameraRotationX -= _cameraRotationX;
        //max������
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        //�Է��ϴ¸�ŭ ī�޶����� ������
        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }

    }
    //������ �ٶ󺸴� ���ȿ� ��� ���شٰ���
    private bool pauseCameraRotation = false;

    //���� ����� �ٽ� ���ߵ�
    //_target �ٶ󺼴�� 
    public IEnumerator TreeLookCoroutine(Vector3 _target)
    {
        pauseCameraRotation = true;

        //������ġ�� �÷��̾� ��ġ ������� �ٶ󺼰���
        Quaternion direction = Quaternion.LookRotation(_target - theCamera.transform.position);
        Vector3 eulerValue = direction.eulerAngles;
        //���� �ݺ����� ���ٰ���
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
    
    // �׳� �̰͸� �Ѱܵ� ���������
    public bool GetRun()
    {
        return isRun;
    }
}
