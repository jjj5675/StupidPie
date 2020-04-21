using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
[RequireComponent(typeof(Animator))]
public class PlayerCharacter : MonoBehaviour
{
    static public PlayerCharacter PlayerInstance;

    public float moveSpeed;
    public float groundAcceleration;
    public float groundDeceleration;

    public float dashDistance;
    public float timeToDashPoint;

    public float jumpHeight;
    public float timeToJumpApex;
    public bool spriteOriginallyFacesRight;

    public Vector2 wallLeapVelocity;
    public float slidingSpeed;
    public float slidingByWaitTime;
    public float maxSlidingSpeed;
    [Range(0f, 1f)] public float slidingSpeedDecelProportion;

    public SpriteRenderer spriteRenderer;
    public Damageable damageable;
    public Dashable dashable;

    private float m_JumpVelocity;
    private float m_VerticalAcceleration;
    private float m_OriginalGravity;

    private float m_DashVelocity;
    private float m_DashDeceleration;

    private bool m_Slidingable;
    private float m_TimeToLeapHeight;
    private bool m_IsLeaping;

    private bool m_UseableXAxis;
    private float m_LaunchingEndTime;

    private Vector2 m_MoveVector;
    private Animator m_Animator;
    private CharacterController2D m_CharacterController2D;
    private Vector2 m_DashDirection;
    private Coroutine m_WallTimerCoroutine;
    private WaitForSeconds m_WallSlidingByWait;
    private WaitForSeconds m_WallLeapingEndWait;
    private Coroutine m_LaunchingTimerCoroutine;
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
    private readonly int m_HashLaunchingPara = Animator.StringToHash("Launching");
    private readonly int m_HashLeapingPara = Animator.StringToHash("Leaping");

    private const float m_GroundedStickingVelocityModifier = 3f;

    public float JumpVelocity { get { return m_JumpVelocity; } }
    public float VerticalAcceleration { get { return m_VerticalAcceleration; } }
    public Vector2 MoveVector { get { return m_MoveVector; } }

    void Awake()
    {
        PlayerInstance = this;
        m_CharacterController2D = GetComponent<CharacterController2D>();
        m_Animator = GetComponent<Animator>();
        m_Box = GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SceneLinkedSMB<PlayerCharacter>.Initialise(m_Animator, this);

        spriteOriginallyFacesRight = true;

        m_CharacterController2D.SetBoxOffset();

        if (spriteOriginallyFacesRight)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            m_CharacterController2D.UpdateBoxOffset(1f);
        }

        m_BoxOriginallyOffsetSign = Mathf.Sign(m_CharacterController2D.BoxOffset.x);

        m_VerticalAcceleration = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        m_OriginalGravity = m_VerticalAcceleration;
        m_JumpVelocity = Mathf.Abs(m_VerticalAcceleration) * timeToJumpApex;

        m_TimeToLeapHeight = -wallLeapVelocity.y / m_VerticalAcceleration;

        m_DashDeceleration = (2 * dashDistance) / Mathf.Pow(timeToDashPoint, 2);
        m_DashVelocity = m_DashDeceleration * timeToDashPoint;

        m_WallSlidingByWait = new WaitForSeconds(slidingByWaitTime);
        m_WallLeapingEndWait = new WaitForSeconds(m_TimeToLeapHeight);
    }

    void FixedUpdate()
    {
        m_CharacterController2D.Move(m_MoveVector * Time.deltaTime);
        m_Animator.SetFloat(m_HashHorizontalPara, m_MoveVector.x);
        m_Animator.SetFloat(m_HashVerticalPara, m_MoveVector.y);
    }

    public void TeleportToColliderBottom()
    {
        Vector2 colliderBottom = m_CharacterController2D.Rigidbody2D.position + m_CharacterController2D.BoxOffset 
            + Vector2.down * (m_Box.size.y * 0.5f);
        m_CharacterController2D.Teleport(colliderBottom);
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
        float desiredSpeed = PlayerInput.Instance.Horizontal.Value * moveSpeed;
        float acceleration = PlayerInput.Instance.Horizontal.ReceivingInput ? groundAcceleration : groundDeceleration;
        m_MoveVector.x = Mathf.MoveTowards(m_MoveVector.x, desiredSpeed, acceleration * Time.deltaTime);

        //if (m_CharacterController2D.collisionFlags.CheckForHorizontal())
        //{
        //    m_MoveVector.x = (m_CharacterController2D.sideRaycastDistance + m_Box.size.x * 0.5f) * PlayerInput.Instance.Horizontal.Value;
        //}
    }

    public void GroundedVerticalMovement()
    {
        m_MoveVector.y += m_VerticalAcceleration * Time.deltaTime;

        if (m_MoveVector.y < m_VerticalAcceleration * Time.deltaTime * m_GroundedStickingVelocityModifier)
        {
            m_MoveVector.y = m_VerticalAcceleration * Time.deltaTime * m_GroundedStickingVelocityModifier;
        }
    }

    public bool CheckForJumpInput()
    {
        if (!Mathf.Approximately(m_VerticalAcceleration, m_OriginalGravity))
        {
            return false;
        }

        return PlayerInput.Instance.Jump.Down;
    }

    public bool CheckForGrounded()
    {
        bool grounded = m_CharacterController2D.collisionFlags.IsGrounded;

        m_Animator.SetBool(m_HashGroundedPara, grounded);

        return grounded;
    }

    public void AirborneHorizontalMovement()
    {
        float desiredSpeed = PlayerInput.Instance.Horizontal.Value * moveSpeed;

        float acceleration;

        if (!PlayerInput.Instance.Horizontal.ReceivingInput)
            acceleration = 100f * 1f;
        else
            acceleration = 100f * 0.5f;

        m_MoveVector.x = Mathf.MoveTowards(m_MoveVector.x, desiredSpeed, acceleration * Time.deltaTime);


        //if (m_CharacterController2D.collisionFlags.CheckForHorizontal())
        //{
        //    m_MoveVector.x = (m_CharacterController2D.sideRaycastDistance + m_Box.size.x * 0.5f) * PlayerInput.Instance.Horizontal.Value;
        //}
    }

    public void AirborneVerticalMovement()
    {
        if(m_CharacterController2D.collisionFlags.IsCeilinged && m_MoveVector.y > 0)
        {
            m_MoveVector.y = 0;
        }

        m_MoveVector.y += m_VerticalAcceleration * Time.deltaTime;
    }

    public bool CheckForDashInput()
    {
        bool inputDash = PlayerInput.Instance.Dash.Down;
        bool inputDashUp = PlayerInput.Instance.Dash.Up;

        if (inputDash)
        {
            dashable.UpdateDashable();
        }

        return inputDash;
    }

    public void OnDash(Dashable dashable)
    {
        dashable.EnableDashability(timeToDashPoint, m_CharacterController2D.collisionFlags.IsGrounded);
        CalculateDashVelocity(out m_MoveVector);
        UpdateFacing();
        m_Animator.SetBool(m_HashDashingPara, true);
    }

    void CalculateDashVelocity(out Vector2 movement)
    {
        bool inputNone = PlayerInput.Instance.CheckAxisInputsNone();
        Vector2 dashDirection = PlayerInput.Instance.AxisInputsValue();
        dashDirection.Normalize();
        m_DashDirection = dashDirection;

        if (inputNone)
        {
            movement = m_DashVelocity * GetFacing() * Vector2.right;
        }
        else
        {
            movement = m_DashVelocity * dashDirection;
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

        //법선방향으로 계속 대쉬할것
        if (m_CharacterController2D.collisionFlags.CheckForHorizontal())
        {
            //m_CharacterController2D.ZeroMovement();
            return DashingEnd();
        }

        if(m_CharacterController2D.collisionFlags.inContactJumppad)
        {
            return DashingEnd();
        }

        bool airborneDashing = dashable.CurrentDashState == Dashable.DashState.AirborneDashing;
        bool grounded = m_CharacterController2D.collisionFlags.IsGrounded;

        if (airborneDashing && grounded && m_DashDirection.y < 0)
        {
            return DashingEnd();
        }

        if(m_CharacterController2D.collisionFlags.IsCeilinged)
        {
            return DashingEnd();
        }

        m_Animator.SetBool(m_HashDashingPara, true);
        return true;
    }

    bool DashingEnd()
    {
        //m_CharacterController2D.ZeroMove();
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
        bool faceLeft = PlayerInput.Instance.Horizontal.Value < 0f;
        bool faceRight = PlayerInput.Instance.Horizontal.Value > 0f;

        if (faceLeft)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            
            if(spriteOriginallyFacesRight)
            {
                m_CharacterController2D.UpdateBoxOffset(-m_BoxOriginallyOffsetSign);
            }
            else
            {
                m_CharacterController2D.UpdateBoxOffset(m_BoxOriginallyOffsetSign);
            }
        }
        else if (faceRight)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);

            if (spriteOriginallyFacesRight)
            {
                m_CharacterController2D.UpdateBoxOffset(m_BoxOriginallyOffsetSign);
            }
            else
            {
                m_CharacterController2D.UpdateBoxOffset(-m_BoxOriginallyOffsetSign);
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
                m_CharacterController2D.UpdateBoxOffset(-m_BoxOriginallyOffsetSign);
            }
            else
            {
                m_CharacterController2D.UpdateBoxOffset(m_BoxOriginallyOffsetSign);
            }
        }
        else
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);

            if (spriteOriginallyFacesRight)
            {
                m_CharacterController2D.UpdateBoxOffset(m_BoxOriginallyOffsetSign);
            }
            else
            {
                m_CharacterController2D.UpdateBoxOffset(-m_BoxOriginallyOffsetSign);
            }
        }
    }

    public float GetFacing()
    {
        return transform.localScale.x > 0 ? -1f : 1f;
    }

    public void CheckForGrabbingWall()
    {
        //airborne에서 grounded체크해버림
        if (m_CharacterController2D.collisionFlags.IsGrounded)
        {
            GrabbingEnd();
            return;
        }
        if (!m_CharacterController2D.collisionFlags.CheckForHorizontal())
        {
            GrabbingEnd();
            return;
        }
        if(m_CharacterController2D.collisionFlags.inContactJumppad)
        {
            GrabbingEnd();
            return;
        }
        if (m_MoveVector.y > 0)
        {
            GrabbingEnd();
            return;
        }

        if (!m_Slidingable)
        {
            m_MoveVector.y = 0;
        }

        if (m_WallTimerCoroutine == null)
        {
            Wait(m_WallSlidingByWait);
            m_IsLeaping = false;
        }

        m_Animator.SetBool(m_HashGrabbingPara, true);
    }

    void GrabbingEnd()
    {
        if (!m_IsLeaping && m_WallTimerCoroutine != null)
        {
            StopCoroutine(m_WallTimerCoroutine);
            m_WallTimerCoroutine = null;
        }

        m_Animator.SetBool(m_HashGrabbingPara, false);
    }

    public void WallLeapStateEnd()
    {
        if(m_IsLeaping)
        {
            StopCoroutine(m_WallTimerCoroutine);
            m_WallTimerCoroutine = null;

            m_Animator.SetBool(m_HashLeapingPara, false);
        }
    }

    void Wait(WaitForSeconds waitForSeconds)
    {
        if (m_WallTimerCoroutine == null)
        {
            m_WallTimerCoroutine = StartCoroutine(WallTimer(waitForSeconds));
        }
        else
        {
            StopCoroutine(m_WallTimerCoroutine);
            m_WallTimerCoroutine = StartCoroutine(WallTimer(waitForSeconds));
        }
    }

    IEnumerator WallTimer(WaitForSeconds forSeconds)
    {
        if (forSeconds.Equals(m_WallSlidingByWait))
        {
            m_Slidingable = false;
        }
        else
        {
            m_IsLeaping = true;
        }

        yield return forSeconds;

        if (forSeconds.Equals(m_WallSlidingByWait))
        {
            m_Slidingable = true;
            //0초일때 바로 true넘어가기 때문에 movevector가 초기화되지 않음
            m_MoveVector.y = 0;
        }
        else
        {
            m_IsLeaping = false;
        }
    }

    public void GrabbingWallHorizontalMovement()
    {
        m_MoveVector.x = PlayerInput.Instance.Horizontal.Value;
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
        int wallDirection = m_CharacterController2D.collisionFlags.IsLeftSide ? -1 : 1;

        if (PlayerInput.Instance.Horizontal.Value == wallDirection)
        {
            m_MoveVector.x = -wallDirection * wallLeapVelocity.x;
            m_MoveVector.y = wallLeapVelocity.y;
            UpdateFacing(-wallDirection < 0 ? true : false);

            m_Animator.SetBool(m_HashLeapingPara, true);

            Wait(m_WallLeapingEndWait);
        }
    }

    public bool CheckForWallLeaping()
    {
        m_Animator.SetBool(m_HashLeapingPara, m_IsLeaping);

        return m_IsLeaping;
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
        if (!PlayerInput.Instance.HaveControl)
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

    IEnumerator DieRespawnCoroutine(bool resetHealth)
    {
        PlayerInput.Instance.ReleaseControl();
        yield return ScreenFader.FadeSceneOut();
        Respawn(resetHealth);
        yield return ScreenFader.FadeSceneIn();
        PlayerInput.Instance.GainControl();
    }

    public bool CheckForJumpPadCollisionEnter()
    {
        if(m_CharacterController2D.collisionFlags.inContactJumppad && m_CharacterController2D.ContactCollider != null)
        {
            JumpPad jumpPad;

            if (PhysicsHelper.TryGetJumpPad(m_CharacterController2D.ContactCollider, out jumpPad))
            {
                if (!jumpPad.EventFired)
                {
                    StartStraightMoving(jumpPad.targetPosition, jumpPad.timeToPoint);
                    jumpPad.OnLaunch();
                    m_Animator.SetBool(m_HashLaunchingPara, true);

                    return true;
                }
            }
        }

        return false;
    }

    //피직스헬퍼로 플레이어가 충돌안했는데 점프대에서 캐처했다면 무시
    public void StartStraightMoving(Vector2 target, float time)
    {
        bool horizontally = !Mathf.Approximately(target.x, 0);
        float displacementY = target.y - m_CharacterController2D.Rigidbody2D.position.y;
        float acceleration = -(2 * displacementY) / Mathf.Pow(time, 2);
        Vector2 velocity = Vector2.zero;

        velocity.y = -(acceleration * time);

        if (horizontally)
        {
            float displacementX = target.x - m_CharacterController2D.Rigidbody2D.position.x;
            velocity.x = (displacementX / time);

            float launchDirection = Mathf.Sign(velocity.x);
            UpdateFacing(launchDirection < 0);
        }

        m_MoveVector = velocity;
        m_VerticalAcceleration = acceleration;

        if(horizontally)
        {
            ResetLaunchCoroutine(false, time);
        }
        else
        {
            ResetLaunchCoroutine(true, time);
        }
    }

    void ResetLaunchCoroutine(bool useableXAxis, float waitTime)
    {
        if (m_LaunchingTimerCoroutine != null)
        {
            StopCoroutine(m_LaunchingTimerCoroutine);
            m_LaunchingTimerCoroutine = null;
        }

        m_LaunchingTimerCoroutine = StartCoroutine(StartLaunchTimer(useableXAxis, waitTime));
    }

    public bool CheckForUseableXAxis()
    {
        return m_UseableXAxis;
    }

    public void LaunchEnd()
    {
        if(m_LaunchingTimerCoroutine != null)
        {
            StopCoroutine(m_LaunchingTimerCoroutine);
            m_LaunchingTimerCoroutine = null;
            m_VerticalAcceleration = m_OriginalGravity;
            m_UseableXAxis = true;
            m_Animator.SetBool(m_HashLaunchingPara, false);
        }
    }

    IEnumerator StartLaunchTimer(bool useableXAxis, float waitTime)
    {
        if(!useableXAxis)
        {
            m_UseableXAxis = false;
        }
        else
        {
            m_UseableXAxis = true;  //초기화
        }

        yield return new WaitForSeconds(waitTime);

        //감속조절

        m_VerticalAcceleration = m_OriginalGravity;
        m_UseableXAxis = true;
        m_Animator.SetBool(m_HashLaunchingPara, false);
    }

    public void StartParabolaMoving(Vector2 target, float height)
    {
        float gravity = m_VerticalAcceleration;
        float displacementY = target.y - m_CharacterController2D.Rigidbody2D.position.y;
        float displacementX = target.x - m_CharacterController2D.Rigidbody2D.position.x;

        float timeUp = Mathf.Sqrt(-2 * height / gravity);
        float timeDown = Mathf.Sqrt(2 * (displacementY - height) / gravity);
        float totalTime = timeUp + timeDown;

        float velocityX = (displacementX / totalTime);
        float velocityY = Mathf.Sqrt(-2 * gravity * height);
        Vector2 totalVelocity = new Vector2(velocityX, velocityY);

        float launchDirection = Mathf.Sign(velocityX);
        UpdateFacing(launchDirection < 0);

        m_MoveVector = totalVelocity;

        ResetLaunchCoroutine(false, totalTime);
    }
}
