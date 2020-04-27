﻿using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController2D))]
[RequireComponent(typeof(Animator))]
public class PlayerBehaviour : MonoBehaviour
{
    public enum PlayableCharacter
    {
        Seri, Ires
    }

    public SpriteRenderer spriteRenderer;
    public Damageable damageable;
    public Dashable dashable;
    public PlayerInput playerInput;

    public float moveSpeed;
    public float groundAcceleration;
    public float groundDeceleration;

    public float dashDistance;
    public float timeToDashPoint;

    public float jumpHeight;
    public float timeToJumpApex;
    [Range(0f, 1f)] public float airborneAccelProportion;
    [Range(0f, 1f)] public float airborneDecelProportion;

    public Vector2 wallLeapVelocity;
    public float slidingSpeed;
    public float slidingByWaitTime;
    public float maxSlidingSpeed;
    [Range(0f, 1f)] public float slidingSpeedDecelProportion;

    public bool spriteOriginallyFacesRight;
    public PlayableCharacter playableCharacter;

    private float m_JumpVelocity;
    private float m_CurrentGravity;
    private float m_OriginallyGravity;
    private float m_OriginallyAirborneAccelProp;
    private bool m_UseableXAxis = true;

    private float m_DashVelocity;
    private float m_DashDeceleration;

    private float m_CurrentTimeToWaitSliding = 0f;
    private bool m_Slidingable = false;
    private float m_TimeToLeapHeight;

    private Vector2 m_MoveVector;
    private Animator m_Animator;
    private PlayerController2D m_PlayerController2D;
    private Vector2 m_DashDirection;
    private bool m_IsParabolaDash = false;
    private WaitForSeconds m_WallLeapingEndWait;
    private Coroutine m_WallLeapCoroutine;
    private Coroutine m_JumpPadCoroutine;
    private BoxCollider2D m_Box;
    private float m_BoxOriginallyOffsetSign;

    private readonly int m_HashGroundedPara = Animator.StringToHash("Grounded");
    private readonly int m_HashDashingPara = Animator.StringToHash("Dashing");
    private readonly int m_HashGrabbingPara = Animator.StringToHash("Grabbing");
    private readonly int m_HashCrouchingPara = Animator.StringToHash("Crouching");
    private readonly int m_HashRespawnPara = Animator.StringToHash("Respawn");
    private readonly int m_HashDeadPara = Animator.StringToHash("Dead");
    private readonly int m_HashHorizontalPara = Animator.StringToHash("Horizontal");
    private readonly int m_HashVerticalPara = Animator.StringToHash("Vertical");

    private const float m_GroundedStickingVelocityModifier = 30f;

    public float JumpVelocity { get { return m_JumpVelocity; } }
    public Vector2 MoveVector { get { return m_MoveVector; } }

    void Awake()
    {
        m_PlayerController2D = GetComponent<PlayerController2D>();
        m_Animator = GetComponent<Animator>();
        m_Box = GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SceneLinkedSMB<PlayerBehaviour>.Initialise(m_Animator, this);
        PlayableCharacterFactory.Initialise(this, GetComponent<PlayerInput>(), m_Box);

        m_PlayerController2D.SetBoxOffset();

        if (spriteOriginallyFacesRight)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            m_PlayerController2D.UpdateBoxOffset(1f);
        }

        m_BoxOriginallyOffsetSign = Mathf.Sign(m_PlayerController2D.BoxOffset.x);

        m_CurrentGravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        m_OriginallyGravity = m_CurrentGravity;
        m_JumpVelocity = Mathf.Abs(m_CurrentGravity) * timeToJumpApex;

        m_OriginallyAirborneAccelProp = airborneAccelProportion;

        m_TimeToLeapHeight = -wallLeapVelocity.y / m_CurrentGravity;

        m_DashDeceleration = (2 * dashDistance) / Mathf.Pow(timeToDashPoint, 2);
        m_DashVelocity = m_DashDeceleration * timeToDashPoint;

        m_WallLeapingEndWait = new WaitForSeconds(m_TimeToLeapHeight);
    }

    void FixedUpdate()
    {
        m_PlayerController2D.Move(m_MoveVector * Time.deltaTime);
        m_Animator.SetFloat(m_HashHorizontalPara, m_MoveVector.x);
        m_Animator.SetFloat(m_HashVerticalPara, m_MoveVector.y);
    }

    public void TeleportToColliderBottom()
    {
        Vector2 colliderBottom = m_PlayerController2D.Rigidbody2D.position + m_PlayerController2D.BoxOffset
            + Vector2.down * (m_Box.size.y * 0.5f);
        m_PlayerController2D.Teleport(colliderBottom);
    }

    public void SetMoveVector(Vector2 newMovement)
    {
        m_MoveVector = newMovement;
    }

    public void SetVerticalMovement(float newVerticalMovement)
    {
        m_MoveVector.y = newVerticalMovement;
    }

    public void SetHorizontalMovement(float newHorizontalMovement)
    {
        m_MoveVector.x = newHorizontalMovement;
    }

    public void GroundedHorizontalMovement()
    {
        
        float desiredSpeed = playerInput.Horizontal.Value * moveSpeed;
        float acceleration = playerInput.Horizontal.ReceivingInput ? groundAcceleration : groundDeceleration;
        m_MoveVector.x = Mathf.MoveTowards(m_MoveVector.x, desiredSpeed, acceleration * Time.deltaTime);

        //if (m_CharacterController2D.collisionFlags.CheckForHorizontal())
        //{
        //    m_MoveVector.x = (m_CharacterController2D.sideRaycastDistance + m_Box.size.x * 0.5f) * PlayerInput.Instance.Horizontal.Value;
        //}
    }

    public void GroundedVerticalMovement()
    {
        m_MoveVector.y += m_CurrentGravity * Time.deltaTime;

        if (m_MoveVector.y < m_CurrentGravity * Time.deltaTime * m_GroundedStickingVelocityModifier)
        {
            m_MoveVector.y = m_CurrentGravity * Time.deltaTime * m_GroundedStickingVelocityModifier;
        }
    }

    public bool CheckForJumpInput()
    {
        return playerInput.Jump.Down;
    }

    public bool CheckForUseableXAxis()
    {
        return m_UseableXAxis;
    }

    public bool CheckForGrounded()
    {
        bool grounded = m_PlayerController2D.collisionFlags.IsGrounded;

        m_Animator.SetBool(m_HashGroundedPara, grounded);

        return grounded;
    }

    public bool CheckForCurrentGravity()
    {
        if (Mathf.Approximately(m_CurrentGravity, m_OriginallyGravity))
        {
            return true;
        }

        return false;
    }

    //이제 플레이어의 레이캐스트 위치까지 고려할것
    public bool CheckForSide()
    {
        return m_PlayerController2D.collisionFlags.CheckForWidth();
    }

    public void AirborneHorizontalMovement()
    {
        float desiredSpeed = playerInput.Horizontal.Value * moveSpeed;

        float acceleration;

        if (!playerInput.Horizontal.ReceivingInput)
            acceleration = groundAcceleration * airborneAccelProportion;
        else
            acceleration = groundDeceleration * airborneDecelProportion;

        m_MoveVector.x = Mathf.MoveTowards(m_MoveVector.x, desiredSpeed, acceleration * Time.deltaTime);


        //if (m_CharacterController2D.collisionFlags.CheckForHorizontal())
        //{
        //    m_MoveVector.x = (m_CharacterController2D.sideRaycastDistance + m_Box.size.x * 0.5f) * PlayerInput.Instance.Horizontal.Value;
        //}
    }

    public void AirborneVerticalMovement()
    {
        if (m_PlayerController2D.collisionFlags.IsCeilinged && !m_PlayerController2D.collisionFlags.inContactJumppad && m_MoveVector.y > 0)
        {
            m_MoveVector.y = 0;
        }

        m_MoveVector.y += m_CurrentGravity * Time.deltaTime;
    }

    public bool CheckForDashInput()
    {
        bool inputDash = playerInput.Dash.Down;

        if (inputDash)
        {
            dashable.UpdateDashable();
        }

        return inputDash;
    }

    public void OnDash(Dashable dashable)
    {
        dashable.EnableDashability(timeToDashPoint, m_PlayerController2D.collisionFlags.IsGrounded);
        CalculateDashVelocity(out m_MoveVector);
        UpdateFacing();
        m_Animator.SetBool(m_HashDashingPara, true);
    }

    void CalculateDashVelocity(out Vector2 movement)
    {
        bool inputNone = playerInput.CheckAxisInputsNone();
        Vector2 dashDirection = playerInput.AxisInputsValue();
        dashDirection.Normalize();
        m_DashDirection = dashDirection;

        if (inputNone)
        {
            movement = m_DashVelocity * GetFacing() * Vector2.right;
            m_DashDirection.x = Mathf.Sign(movement.x);
        }
        else
        {
            movement = m_DashVelocity * dashDirection;
        }

        if (!Mathf.Approximately(m_DashDirection.x, 0) && !Mathf.Approximately(m_DashDirection.y, 0))
        {
            m_IsParabolaDash = true;
        }
        else
        {
            m_IsParabolaDash = false;
        }
    }

    public void ResetDashState()
    {
        dashable.SetState(Dashable.DashState.Ready);
    }

    public bool CheckForDashing()
    {
        if (dashable.CurrentDashState == Dashable.DashState.Cooldown)
        {
            return DashingEnd();
        }

        if (!m_IsParabolaDash)
        {
            if (m_PlayerController2D.collisionFlags.CheckForWidth())
            {
                return DashingEnd();
            }

            bool airborneDashing = dashable.CurrentDashState == Dashable.DashState.AirborneDashing;
            bool grounded = m_PlayerController2D.collisionFlags.IsGrounded;

            if (airborneDashing && grounded && m_DashDirection.y < 0)
            {
                return DashingEnd();
            }

            if (m_PlayerController2D.collisionFlags.IsCeilinged)
            {
                return DashingEnd();
            }
        }

        if (m_PlayerController2D.collisionFlags.inContactJumppad)
        {
            return DashingEnd();
        }


        m_Animator.SetBool(m_HashDashingPara, true);
        return true;
    }

    bool DashingEnd()
    {
        dashable.DisableDashability();
        m_Animator.SetBool(m_HashDashingPara, false);
        return false;
    }

    public void DashDecelMovement()
    {
        m_MoveVector += m_DashDeceleration * (-m_DashDirection) * Time.deltaTime;
    }

    public void UpdateFacing()
    {
        bool faceLeft = playerInput.Horizontal.Value < 0f;
        bool faceRight = playerInput.Horizontal.Value > 0f;

        if (faceLeft)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);

            if (spriteOriginallyFacesRight)
            {
                m_PlayerController2D.UpdateBoxOffset(-m_BoxOriginallyOffsetSign);
            }
            else
            {
                m_PlayerController2D.UpdateBoxOffset(m_BoxOriginallyOffsetSign);
            }
        }
        else if (faceRight)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);

            if (spriteOriginallyFacesRight)
            {
                m_PlayerController2D.UpdateBoxOffset(m_BoxOriginallyOffsetSign);
            }
            else
            {
                m_PlayerController2D.UpdateBoxOffset(-m_BoxOriginallyOffsetSign);
            }
        }
    }

    public void UpdateFacing(bool faceLeft)
    {
        if (faceLeft)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);

            if (spriteOriginallyFacesRight)
            {
                m_PlayerController2D.UpdateBoxOffset(-m_BoxOriginallyOffsetSign);
            }
            else
            {
                m_PlayerController2D.UpdateBoxOffset(m_BoxOriginallyOffsetSign);
            }
        }
        else
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);

            if (spriteOriginallyFacesRight)
            {
                m_PlayerController2D.UpdateBoxOffset(m_BoxOriginallyOffsetSign);
            }
            else
            {
                m_PlayerController2D.UpdateBoxOffset(-m_BoxOriginallyOffsetSign);
            }
        }
    }

    public float GetFacing()
    {
        return transform.localScale.x > 0 ? -1f : 1f;
    }

    public void CheckForGrabbingWall()
    {
        if(m_Animator.GetBool(m_HashDashingPara))
        {
            m_Animator.SetBool(m_HashGrabbingPara, false);
            return;
        }
        if (m_PlayerController2D.collisionFlags.IsGrounded)
        {
            m_CurrentTimeToWaitSliding = 0f;
            m_Animator.SetBool(m_HashGrabbingPara, false);
            return;
        }
        if (!m_PlayerController2D.collisionFlags.CheckForWidth())
        {
            m_Animator.SetBool(m_HashGrabbingPara, false);
            return;
        }
        if (m_PlayerController2D.collisionFlags.inContactJumppad)
        {
            m_Animator.SetBool(m_HashGrabbingPara, false);
            return;
        }
        if (m_MoveVector.y > 0)
        {
            m_Animator.SetBool(m_HashGrabbingPara, false);
            return;
        }

        if (!m_Slidingable)
        {
            m_MoveVector.y = 0;
        }

        WaitForSliding();

        m_Animator.SetBool(m_HashGrabbingPara, true);
    }

    void WaitForSliding()
    {
        if (m_CurrentTimeToWaitSliding <= slidingByWaitTime)
        {
            m_Slidingable = false;

            m_CurrentTimeToWaitSliding += Time.deltaTime;

            if (m_CurrentTimeToWaitSliding >= slidingByWaitTime)
            {
                m_Slidingable = true;
            }
        }
    }

    public void GrabbingWallHorizontalMovement()
    {
        m_MoveVector.x = playerInput.Horizontal.Value;
    }

    public void SlidingMovement()
    {
        if (m_Slidingable)
        {
            float deceleration = slidingSpeed * slidingSpeedDecelProportion;
            m_MoveVector.y += (-slidingSpeed + deceleration) * Time.deltaTime;
            m_MoveVector.y = Mathf.Clamp(m_MoveVector.y, -maxSlidingSpeed, m_MoveVector.y);
        }
    }

    public void WallLeapMovement()
    {
        int wallDirection = m_PlayerController2D.collisionFlags.IsLeftSide ? -1 : 1;

        if (playerInput.Horizontal.Value == wallDirection)
        {
            m_MoveVector.x = -wallDirection * wallLeapVelocity.x;
            m_MoveVector.y = wallLeapVelocity.y;
            UpdateFacing(-wallDirection < 0 ? true : false);

            m_UseableXAxis = false;

            if (m_WallLeapCoroutine != null)
            {
                StopCoroutine(m_WallLeapCoroutine);
            }

            m_WallLeapCoroutine = StartCoroutine(WaitForWallLeapingEnd(m_WallLeapingEndWait));
        }
    }

    IEnumerator WaitForWallLeapingEnd(WaitForSeconds waitForSeconds)
    {
        yield return waitForSeconds;
        m_UseableXAxis = true;
    }

    public void CheckForCrouching()
    {
        //m_Animator.SetBool(m_HashCrouchingPara, PlayerInput.Instance.Vertical.Value < 0f);
    }

    public void Respawn(bool resetHealth)
    {
        m_Animator.SetTrigger(m_HashRespawnPara);

        GameObjectTeleporter.Teleport(gameObject, Vector2.zero);

        if (resetHealth)
        {
            damageable.SetHealth(damageable.startingHealth);
        }
    }

    public void OnHurt(Damageable damageable, Damager damager)
    {
        if (!playerInput.HaveControl)
        {
            return;
        }

        damageable.EnableInvulnerability();
    }

    public void OnDie()
    {
        m_Animator.SetTrigger(m_HashDeadPara);
        StartCoroutine(DieRespawnCoroutine(true));
    }

    // fadeduration * 2 < invulnerabilityDuration
    IEnumerator DieRespawnCoroutine(bool resetHealth)
    {
        playerInput.ReleaseControl();
        yield return ScreenFader.FadeSceneOut();
        Respawn(resetHealth);
        yield return ScreenFader.FadeSceneIn();
        playerInput.GainControl();
    }

    public bool CheckForJumpPadCollisionEnter()
    {
        if (m_PlayerController2D.collisionFlags.inContactJumppad && m_PlayerController2D.ContactCollider != null)
        {
            JumpPad jumpPad;

            if (PhysicsHelper.TryGetJumpPad(m_PlayerController2D.ContactCollider, out jumpPad))
            {
                if (!jumpPad.EventFired)
                {
                    StartStraightMoving(jumpPad.targetPosition, jumpPad.timeToPoint);
                    jumpPad.OnLaunch(m_Box);

                    if (!jumpPad.useOnlyVertically)
                    {
                        m_UseableXAxis = false;
                    }

                    if (m_JumpPadCoroutine != null)
                    {
                        StopCoroutine(m_JumpPadCoroutine);
                    }

                    m_JumpPadCoroutine = StartCoroutine(WaitForJumpingPadEnd(jumpPad.timeToPoint, jumpPad.useOnlyVertically));

                    return true;
                }
            }
        }

        return false;
    }

    IEnumerator WaitForJumpingPadEnd(float waitForSec, bool useOnlyVertically)
    {
        yield return new WaitForSeconds(waitForSec);
        m_CurrentGravity = m_OriginallyGravity;
        m_UseableXAxis = true;

        if (!useOnlyVertically)
        {
            airborneAccelProportion = 0.01f;
        }

        yield return new WaitForSeconds(0.1f);

        airborneAccelProportion = m_OriginallyAirborneAccelProp;
    }

    //피직스헬퍼로 플레이어가 충돌안했는데 점프대에서 캐처했다면 무시
    public void StartStraightMoving(Vector2 target, float time)
    {
        bool horizontally = !Mathf.Approximately(target.x, 0);
        float displacementY = target.y - m_PlayerController2D.Rigidbody2D.position.y;
        float acceleration = -(2 * displacementY) / Mathf.Pow(time, 2);
        Vector2 velocity = Vector2.zero;

        velocity.y = -(acceleration * time);

        if (horizontally)
        {
            float displacementX = target.x - m_PlayerController2D.Rigidbody2D.position.x;
            velocity.x = (displacementX / time);

            float launchDirection = Mathf.Sign(velocity.x);
            UpdateFacing(launchDirection < 0);
        }

        m_MoveVector = velocity;
        m_CurrentGravity = acceleration;
    }

    //public void StartParabolaMoving(Vector2 target, float height)
    //{
    //    float gravity = m_CurrentGravity;
    //    float displacementY = target.y - m_PlayerController2D.Rigidbody2D.position.y;
    //    float displacementX = target.x - m_PlayerController2D.Rigidbody2D.position.x;

    //    float timeUp = Mathf.Sqrt(-2 * height / gravity);
    //    float timeDown = Mathf.Sqrt(2 * (displacementY - height) / gravity);
    //    float totalTime = timeUp + timeDown;

    //    float velocityX = (displacementX / totalTime);
    //    float velocityY = Mathf.Sqrt(-2 * gravity * height);
    //    Vector2 totalVelocity = new Vector2(velocityX, velocityY);

    //    float launchDirection = Mathf.Sign(velocityX);
    //    UpdateFacing(launchDirection < 0);

    //    m_MoveVector = totalVelocity;

    //}
}