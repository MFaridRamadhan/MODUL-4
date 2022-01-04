using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class ThirdPersonController : MonoBehaviour
{
    public Camera MyCamera;
    public float Speed = 2f;
    public float SprintSpeed = 5f;
    public float AnimationBlendedSpeed = 2f;
    public float RotationSpeed = 15;
    public float JumpSpeed = 15;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private FixedJoystick joystick;
    [SerializeField] private Animator MyAnimator;


    CharacterController MyController;
    

    float mDesiredRotation = 0f;
    float mDesireAnimationSpeed = 0f;
    bool mSprinting = false;

    float mSpeedY = 0f;
    float mGravity = -7.81f;

    bool mJumping = false;
    // Start is called before the first frame update
    void Start()
    {
        MyController = GetComponent<CharacterController>();
        MyAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        //joystick = GameObject.FindWithTag("Joystick").GetComponent<FixedJoystick>();
        if(gameObject.name.Contains("Fixed Joystick"))
        {
            joystick = GameObject.FindWithTag("Joystick").GetComponent<FixedJoystick>();
        }

    }
        
    
    // Update is called once per frame
    void Update()
    {
        
        

        MyInput();
    }

    void MyInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump") && !mJumping)
        {
            mJumping = true;
            MyAnimator.SetTrigger("Jump");

            mSpeedY += JumpSpeed;
        }
        if (!MyController.isGrounded)
        {
            mSpeedY +=  mGravity * Time.deltaTime;
        }
        else if(mSpeedY < 0)
        {
            mSpeedY = 0;
        }
        MyAnimator.SetFloat("SpeedY", mSpeedY / JumpSpeed);
        if(mJumping && mSpeedY < 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, .5f, LayerMask.GetMask("Default")))
            {
                mJumping = false;
                MyAnimator.SetTrigger("Land");

            }
        }
        
        mSpeedY += mGravity * Time.deltaTime;

        mSprinting = Input.GetKey(KeyCode.LeftShift);
        Vector3 movement = new Vector3(x, 0, z).normalized;
    
        Vector3 rotatedMovement = Quaternion.Euler(0, MyCamera.transform.rotation.eulerAngles.y, 0) * movement;
        Vector3 verticalMovement = Vector3.up * mSpeedY;
        MyController.Move((verticalMovement + (rotatedMovement * ( mSprinting ? SprintSpeed : Speed))) * Time.deltaTime);

        if(rotatedMovement.magnitude > 0)
        {
            mDesiredRotation = Mathf.Atan2(rotatedMovement.x, rotatedMovement.z) * Mathf.Rad2Deg;
            mDesireAnimationSpeed = mSprinting ? 1 : .5f;
        }
        else
        {
            mDesireAnimationSpeed = 0;
        }

        MyAnimator.SetFloat("Speed", Mathf.Lerp(MyAnimator.GetFloat("Speed"), mDesireAnimationSpeed, AnimationBlendedSpeed * Time.deltaTime));

        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, mDesiredRotation, 0);
        transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, RotationSpeed * Time.deltaTime);
    }
}
