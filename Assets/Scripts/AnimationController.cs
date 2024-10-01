using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
//=====================================================================================
// Written by FreeKnight 2023/12/08
//=====================================================================================
// �������ƶ�������
//=====================================================================================
public class AnimationController : MonoBehaviour
{
    public enum CharacterState
    {
        None,
        Idle,
        Walk,
        Run,
        Crouch,
        Rise,
        Fall,
        Attack
    }

    [Header("Controls")]
    public string XAxis = "Horizontal";
    public string YAxis = "Vertical";
    public string JumpButton = "Jump";
    public string FireButton = "Fire1";

    [Header("Jumping")] 
    public float jumpSpeed = 4f;                    // ��Ծʱ���ϼ��ٶ�
    public float forceCrouchVelocity = 4;           // �Ӹߴ�����ʱ��ǿ�ơ��¶ס��ĸ߶ȣ�Ҳ���ǡ�Ӳ��½����������
    public float forceCrouchDuration = 0.5f;
    public float minimumJumpDuration = 0.5f;
    public float jumpInterruptFactor = 0.5f;

    [Header("Moving")]
    public float gravityScale = 2f;                 // ������Ӱ����Ծ�ٶ�
    public float runToMove = 0.5f;                  // ��·->�ܲ�����ֵ
    public float walkSpeed = 0.15f;                 // �ƶ��ٶ�
    public float runSpeed = 0.7f;                   // �ܶ��ٶ�

    private CharacterController controller;
    private bool wasGrounded = false;
    private Vector2 input = default(Vector2);
    private Vector3 velocity = default(Vector3);
    private float forceCrouchEndTime = 0f;          // ���¶ס���Ӧ�����¡���������ʱ�䡣
    private float minimumJumpEndTime = 0f;
    private CharacterState previousState, currentState;

    private Spine.AnimationState animationState;
    private SkeletonAnimation skeletonAnimation;
    private ExposedList<Spine.Animation> animations;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        skeletonAnimation = GetComponent<SkeletonAnimation>();
        animationState = skeletonAnimation.AnimationState;
        animations = skeletonAnimation.Skeleton.Data.Animations;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        float dt = Time.deltaTime;
        bool isGrounded = controller.isGrounded;
        bool isLanded = (!wasGrounded) && isGrounded;   // ˲ʱ��Ϊ����½

        input.x = Input.GetAxis(XAxis);
        input.y = Input.GetAxis(YAxis);
        bool bIsInputJumpStop = Input.GetButtonUp(JumpButton);
        bool bIsInputJumpStart = Input.GetButtonDown(JumpButton);
        bool bIsAttackButtonDown = Input.GetButton(FireButton);
        bool bIsShouldDoCrouch = (isGrounded && input.y < -0.5f) || (forceCrouchEndTime > Time.time);
        bool bIsShouldDoJump = false;
        bool bIsShouldDoJumpInterrupt = false;
        bool bIsHardLand = false;

        if (isLanded)
        {
            if(-velocity.y > forceCrouchVelocity)
            {
                bIsHardLand = true;
                bIsShouldDoCrouch = true;
                forceCrouchEndTime = Time.time + forceCrouchDuration;
            }
        }

        if (!bIsShouldDoCrouch)
        {
            if (isGrounded)
            {
                if (bIsInputJumpStart)
                {
                    bIsShouldDoJump = true;
                }
            }
            else
            {
                bIsShouldDoJumpInterrupt = bIsInputJumpStop && Time.time < minimumJumpEndTime;
            }
        }

        Vector3 gravityDeltaVelocity = Physics.gravity * gravityScale * dt;

        if (bIsShouldDoJump)
        {
            velocity.y = jumpSpeed;
            minimumJumpEndTime = Time.time + minimumJumpDuration;
        } else if (bIsShouldDoJumpInterrupt)
        {
            if(velocity.y > 0)
            {
                velocity.y *= jumpInterruptFactor;
            }
        }

        velocity.x = 0;
        if (!bIsShouldDoCrouch)
        {
            if (input.x != 0)
            {
                velocity.x = Mathf.Abs(input.x) > runToMove ? runSpeed : walkSpeed;
                velocity.x *= Mathf.Sign(input.x);
            }
        }

        if (!isGrounded)
        {
            if (wasGrounded)
            {
                if (velocity.y < 0)
                    velocity.y = 0;
            }
            else
            {
                velocity += gravityDeltaVelocity;
            }
        }

        controller.Move(velocity * dt);
        wasGrounded = isGrounded;

        CualAnimationState(isGrounded, bIsShouldDoCrouch, bIsAttackButtonDown);

        bool bIsStateChanged = previousState != currentState;
        previousState = currentState;
        if (bIsStateChanged)
        {
            HandleStateChanged();
        }

        if(input.x != 0)        // �����ƶ�
        {
            skeletonAnimation.Skeleton.ScaleX = input.x > 0 ? 1f : -1f;
        }

        // fire events
        if (bIsShouldDoJump)
        {
            // TODO:
        }
        if (isLanded)
        {
            if (bIsHardLand)
            {
                // TODO:
            }
            else
            {
                // TODO:
            }
        }
        
    }

    void HandleStateChanged()
    {
        string stateName = null;
        switch (currentState)
        {
            case CharacterState.Idle:
                stateName = "idle";
                break;
            case CharacterState.Walk:
                stateName = "walk";
                break;
            case CharacterState.Run:
                stateName = "run";
                break;
            case CharacterState.Crouch:
                stateName = "crouch";
                break;
            case CharacterState.Rise:
                stateName = "rise";
                break;
            case CharacterState.Fall:
                stateName = "fall";
                break;
            case CharacterState.Attack:
                stateName = "attack";
                break;
            default:
                break;
        }

        foreach (var animation in animations)
        {
            if(animation.Name == stateName)
            {
                animationState.SetAnimation(0, stateName, true);
            }
        }
    }

    void CualAnimationState(bool bIsGrounded, bool bIsShouldDoCrouch, bool bIsShuoldAttack)
    {
        if (bIsShuoldAttack)
        {
            currentState = CharacterState.Attack;
            return;
        }
        if (bIsGrounded)
        {
            if (bIsShouldDoCrouch)
            {
                currentState = CharacterState.Crouch;
            }
            else
            {
                if (input.x == 0)
                {
                    currentState = CharacterState.Idle;
                }
                else
                {
                    currentState = Mathf.Abs(input.x) > runToMove ? CharacterState.Run : CharacterState.Walk;
                }
            }
        }
        else
        {
            currentState = velocity.y > 0 ? CharacterState.Rise : CharacterState.Fall;
        }
    }
}
