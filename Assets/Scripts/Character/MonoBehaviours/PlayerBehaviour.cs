using System.Collections;
using System.Data;
using UnityEngine;

[RequireComponent(typeof(PlayerController2D))]
[RequireComponent(typeof(Animator))]
public class PlayerBehaviour : MonoBehaviour
{
    public PlayerDataBase dataBase;
    public CellController cellController;
    public Publisher publisher;
    public SpriteRenderer spriteRenderer;
    public Dashable dashable;

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

    public RandomAudioPlayer footstepAudioPlayer;
    public RandomAudioPlayer jumpAudioPlayer;
    public RandomAudioPlayer wallSlidingAudioPlayer;

    private bool m_InPause = false;

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

    private Observer m_Observer;
    private Vector2 m_MoveVector;
    private Vector2 m_DashDirection;
    private bool m_IsParabolaDash = false;
    private WaitForSeconds m_WallLeapingEndWait;
    private Coroutine m_WallLeapCoroutine;
    private Coroutine m_JumpPadCoroutine;

    private readonly int m_HashGroundedPara = Animator.StringToHash("Grounded");
    private readonly int m_HashDashingPara = Animator.StringToHash("Dashing");
    private readonly int m_HashGrabbingPara = Animator.StringToHash("Grabbing");
    private readonly int m_HashCrouchingPara = Animator.StringToHash("Crouching");
    private readonly int m_HashRespawnPara = Animator.StringToHash("Respawn");
    private readonly int m_HashDeadPara = Animator.StringToHash("Dead");
    private readonly int m_HashHorizontalPara = Animator.StringToHash("Horizontal");
    private readonly int m_HashVerticalPara = Animator.StringToHash("Vertical");

    private const float m_GroundedStickingVelocityModifier = 10f;

    public float JumpVelocity { get { return m_JumpVelocity; } }
    public Vector2 MoveVector { get { return m_MoveVector; } }

    void Awake()
    {
        dataBase.SetDate(transform, GetComponent<PlayerInput>(), GetComponent<Damageable>(), GetComponent<Animator>(), GetComponent<BoxCollider2D>(), GetComponent<CharacterController2D>());
        m_Observer = new Observer(dataBase);
        m_Observer.Subscribe(publisher);
    }

    // Start is called before the first frame update
    void Start()
    {
        SceneLinkedSMB<PlayerBehaviour>.Initialise(dataBase.animator, this);

        spriteRenderer.flipX = spriteOriginallyFacesRight;

        m_CurrentGravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        m_OriginallyGravity = m_CurrentGravity;
        m_JumpVelocity = Mathf.Abs(m_CurrentGravity) * timeToJumpApex;

        m_OriginallyAirborneAccelProp = airborneAccelProportion;

        m_TimeToLeapHeight = -wallLeapVelocity.y / m_CurrentGravity;

        m_DashDeceleration = (2 * dashDistance) / Mathf.Pow(timeToDashPoint, 2);
        m_DashVelocity = m_DashDeceleration * timeToDashPoint;

        m_WallLeapingEndWait = new WaitForSeconds(m_TimeToLeapHeight);
    }

    void Update()
    {
        if(dataBase.playerInput.Pause.Down)
        {
            if(!m_InPause)
            {
                if(ScreenFader.IsFading)
                {
                    return;
                }

                publisher.GainOrReleaseControl(false);
                publisher.GainPause();
                m_InPause = true;
                Time.timeScale = 0;
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("UIMenus", UnityEngine.SceneManagement.LoadSceneMode.Additive);
            }
        }

        if(m_InPause && Time.timeScale == 1)
        {
            m_InPause = false;
        }
    }

    void FixedUpdate()
    {
        dataBase.character.Move(m_MoveVector * Time.deltaTime);
        dataBase.animator.SetFloat(m_HashHorizontalPara, m_MoveVector.x);
        dataBase.animator.SetFloat(m_HashVerticalPara, m_MoveVector.y);
    }


    public void TeleportToColliderBottom()
    {
        Vector2 colliderBottom = dataBase.character.Rigidbody2D.position + dataBase.collider.offset
            + Vector2.down * (dataBase.collider.bounds.size.y * 0.5f);
        dataBase.character.Teleport(colliderBottom);
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
        float desiredSpeed = dataBase.playerInput.Horizontal.Value * moveSpeed;
        float acceleration = dataBase.playerInput.Horizontal.ReceivingInput ? groundAcceleration : groundDeceleration;
        m_MoveVector.x = Mathf.MoveTowards(m_MoveVector.x, desiredSpeed, acceleration * Time.deltaTime);

        //if (m_CharacterController2D.collisionFlags.CheckForHorizontal())
        //{
        //    m_MoveVector.x = (m_CharacterController2D.sideRaycastDistance + m_Box.size.x * 0.5f) * PlayerInput.Instance.Horizontal.Value;
        //}
    }

    public void PlayFootstep()
    {
        footstepAudioPlayer.PlayRandomSound();
    }

    public void PlayWallSliding()
    {
        wallSlidingAudioPlayer.PlayRandomSound();
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
        return dataBase.playerInput.Jump.Down;
    }

    public bool CheckForUseableXAxis()
    {
        return m_UseableXAxis;
    }

    public bool CheckForGrounded()
    {
        bool grounded = dataBase.character.collisionFlags.IsGrounded;

        dataBase.animator.SetBool(m_HashGroundedPara, grounded);

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
        return dataBase.character.collisionFlags.CheckForWidth();
    }

    public void SetJumpingMovement()
    {
        m_MoveVector.y = m_JumpVelocity;
        jumpAudioPlayer.PlayRandomSound();
    }

    public void AirborneHorizontalMovement()
    {
        float desiredSpeed = dataBase.playerInput.Horizontal.Value * moveSpeed;

        float acceleration;

        if (!dataBase.playerInput.Horizontal.ReceivingInput)
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
        if (dataBase.character.collisionFlags.IsCeilinged && !dataBase.character.collisionFlags.inContactJumppad && m_MoveVector.y > 0)
        {
            m_MoveVector.y = 0;
        }

        m_MoveVector.y += m_CurrentGravity * Time.deltaTime;
    }

    public bool CheckForDashInput()
    {
        bool inputDash = dataBase.playerInput.Dash.Down;

        if (inputDash)
        {
            dashable.UpdateDashable();
        }

        return inputDash;
    }

    public void OnDash(Dashable dashable)
    {
        dashable.EnableDashability(timeToDashPoint, dataBase.character.collisionFlags.IsGrounded);
        CalculateDashVelocity(out m_MoveVector);
        UpdateFacing();
        dataBase.animator.SetBool(m_HashDashingPara, true);
    }

    void CalculateDashVelocity(out Vector2 movement)
    {
        bool inputNone = dataBase.playerInput.CheckAxisInputsNone();
        Vector2 dashDirection = dataBase.playerInput.AxisInputsValue();
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

    public void ResetAirborneDashState()
    {
        dashable.SetState(Dashable.DashState.Ready);
    }

    public void ResetDashState()
    {
        if (dashable.CurrentDashState == Dashable.DashState.Cooldown)
        {
            dashable.SetState(Dashable.DashState.Ready);
        }
    }

    public bool CheckForDashing()
    {
        if (dashable.CurrentDashState == Dashable.DashState.Cooldown)
        {
            return DashingEnd();
        }

        if (!m_IsParabolaDash)
        {
            if (dataBase.character.collisionFlags.CheckForWidth())
            {
                return DashingEnd();
            }

            bool airborneDashing = dashable.CurrentDashState == Dashable.DashState.AirborneDashing;
            bool grounded = dataBase.character.collisionFlags.IsGrounded;

            if (airborneDashing && grounded && m_DashDirection.y < 0)
            {
                return DashingEnd();
            }

            if (dataBase.character.collisionFlags.IsCeilinged)
            {
                return DashingEnd();
            }
        }

        if (dataBase.character.collisionFlags.inContactJumppad)
        {
            return DashingEnd();
        }


        dataBase.animator.SetBool(m_HashDashingPara, true);
        return true;
    }

    bool DashingEnd()
    {
        dashable.DisableDashability();
        dataBase.animator.SetBool(m_HashDashingPara, false);
        return false;
    }

    public void DashDecelMovement()
    {
        m_MoveVector += m_DashDeceleration * (-m_DashDirection) * Time.deltaTime;
    }

    public void UpdateFacing()
    {
        bool faceLeft = dataBase.playerInput.Horizontal.Value < 0f;
        bool faceRight = dataBase.playerInput.Horizontal.Value > 0f;

        if (faceLeft)
        {
            spriteRenderer.flipX = !spriteOriginallyFacesRight;
        }
        else if (faceRight)
        {
            spriteRenderer.flipX = spriteOriginallyFacesRight;
        }
    }

    public void UpdateFacing(bool faceLeft)
    {
        if (faceLeft)
        {
            spriteRenderer.flipX = !spriteOriginallyFacesRight;
        }
        else
        {
            spriteRenderer.flipX = spriteOriginallyFacesRight;
        }
    }

    public float GetFacing()
    {
        return spriteRenderer.flipX != spriteOriginallyFacesRight ? -1f : 1f;
    }

    public void CheckForGrabbingWall()
    {
        if (dataBase.animator.GetBool(m_HashDashingPara))
        {
            dataBase.animator.SetBool(m_HashGrabbingPara, false);
            return;
        }
        if (dataBase.character.collisionFlags.IsGrounded)
        {
            m_CurrentTimeToWaitSliding = 0f;
            dataBase.animator.SetBool(m_HashGrabbingPara, false);
            //m_PlayerController2D.wallTest = false;

            return;
        }
        if (!dataBase.character.collisionFlags.CheckForWidth())
        {
            dataBase.animator.SetBool(m_HashGrabbingPara, false);
            //m_PlayerController2D.wallTest = false;

            return;
        }
        if (dataBase.character.collisionFlags.inContactJumppad)
        {
            dataBase.animator.SetBool(m_HashGrabbingPara, false);
            return;
        }
        if (m_MoveVector.y > 0)
        {
            dataBase.animator.SetBool(m_HashGrabbingPara, false);
            return;
        }

        if (!m_Slidingable)
        {
            m_MoveVector.y = 0;
        }

        WaitForSliding();

        dataBase.animator.SetBool(m_HashGrabbingPara, true);
        //m_PlayerController2D.wallTest = true;
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
        m_MoveVector.x = dataBase.playerInput.Horizontal.Value;
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
        int wallDirection = dataBase.character.collisionFlags.IsLeftSide ? -1 : 1;

        if (dataBase.playerInput.Horizontal.Value == wallDirection)
        {
            m_MoveVector.x = -wallDirection * wallLeapVelocity.x;
            m_MoveVector.y = wallLeapVelocity.y;
            UpdateFacing(-wallDirection < 0);

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

    public void Respawn()
    {
        publisher.SetAnimState(true, false);
        publisher.SetObservers(true, true, cellController.LastEnteringDestination.locations);
        cellController.CurrentCell.ResetCell(false);
    }

    public void OnDie()
    {
        if (!dataBase.playerInput.HaveControl)
        {
            return;
        }

        dataBase.animator.SetTrigger(m_HashDeadPara);
        StartCoroutine(DieRespawnCoroutine());
    }

    // fadeduration * 2 < invulnerabilityDuration
    IEnumerator DieRespawnCoroutine()
    {
        publisher.GainOrReleaseControl(false);
        yield return new WaitForSeconds(1.0f);
        yield return StartCoroutine(ScreenFader.FadeSceneOut());
        Respawn();
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(ScreenFader.FadeSceneIn());
        publisher.GainOrReleaseControl(true);
    }

    public bool CheckForJumpPadCollisionEnter()
    {
        if (dataBase.character.collisionFlags.inContactJumppad && dataBase.character.ContactCollider != null)
        {

            if (PhysicsHelper.TryGetJumpPad(dataBase.character.ContactCollider, out JumpPad jumpPad))
            {
                if (!jumpPad.EventFired)
                {
                    StartStraightMoving(jumpPad.targetPosition, jumpPad.timeToPoint, jumpPad.transform.position);
                    jumpPad.OnLaunch(dataBase.collider);

                    if (!jumpPad.useOnlyVertically)
                    {
                        m_UseableXAxis = false;
                    }

                    if (m_JumpPadCoroutine != null)
                    {
                        StopCoroutine(m_JumpPadCoroutine);
                    }

                    m_JumpPadCoroutine = StartCoroutine(WaitForJumpingPadEnd(jumpPad.timeToPoint, jumpPad.useOnlyVertically, jumpPad.airborneAccelProportion));

                    return true;
                }
            }
        }

        return false;
    }

    IEnumerator WaitForJumpingPadEnd(float waitForSec, bool useOnlyVertically, float accelProportion)
    {
        yield return new WaitForSeconds(waitForSec);
        m_CurrentGravity = m_OriginallyGravity;
        m_UseableXAxis = true;

        if (!useOnlyVertically)
        {
            airborneAccelProportion = accelProportion;
        }

        yield return new WaitForSeconds(0.05f);

        airborneAccelProportion = m_OriginallyAirborneAccelProp;
    }

    //피직스헬퍼로 플레이어가 충돌안했는데 점프대에서 캐처했다면 무시
    public void StartStraightMoving(Vector2 target, float time, Vector2 padPosition)
    {
        bool horizontally = !Mathf.Approximately(target.x, 0);
        float displacementY = target.y - dataBase.character.Rigidbody2D.position.y;
        float acceleration = -(2 * displacementY) / Mathf.Pow(time, 2);
        Vector2 velocity = Vector2.zero;

        velocity.y = -(acceleration * time);

        if (horizontally)
        {
            float displacementX = target.x - dataBase.character.Rigidbody2D.position.x;
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
