using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController2D))]
[RequireComponent(typeof(Animator))]
public class PlayerBehaviour : MonoBehaviour
{
    public PlayerDataBase dataBase;
    public CellController cellController;
    public SpriteRenderer spriteRenderer;
    public Dashable dashable;
    public GameObject debugMenu;

    public float moveSpeed;
    public float groundAcceleration;
    public float groundDeceleration;

    public float dashDistance;
    public float timeToDashPoint;

    public float jumpHeight;
    public float timeToJumpApex;
    public float maxAirborneSpeed;
    [Range(0f, 1f)] public float airborneAccelProportion;
    [Range(0f, 1f)] public float airborneDecelProportion;

    public Vector2 wallLeapVelocity;
    public float slidingSpeed;
    public float slidingByWaitTime;
    public float maxSlidingSpeed;
    [Range(0f, 1f)] public float slidingSpeedDecelProportion;

    public bool spriteOriginallyFacesRight;
    public float maxIdleDuration;

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
    private float m_IdleElapsedTime;
    private Action m_HackController;
    private Vector2 m_FlashPoint;

    private readonly int m_HashGroundedPara = Animator.StringToHash("Grounded");
    private readonly int m_HashDashingPara = Animator.StringToHash("Dashing");
    private readonly int m_HashGrabbingPara = Animator.StringToHash("Grabbing");
    private readonly int m_HashCrouchingPara = Animator.StringToHash("Crouching");
    private readonly int m_HashRespawnPara = Animator.StringToHash("Respawn");
    private readonly int m_HashDeadPara = Animator.StringToHash("Dead");
    private readonly int m_HashHorizontalPara = Animator.StringToHash("Horizontal");
    private readonly int m_HashVerticalPara = Animator.StringToHash("Vertical");
    private readonly int m_HashBoringPara = Animator.StringToHash("Boring");
    private readonly int m_HashInteractPara = Animator.StringToHash("Interact");
    private readonly int m_HashElectricContactPara = Animator.StringToHash("ElectricContact");

    private const float m_GroundedStickingVelocityModifier = 10f;

    public float JumpVelocity { get { return m_JumpVelocity; } }
    public Vector2 MoveVector { get { return m_MoveVector; } }

    void Awake()
    {
        dataBase.SetDate(transform, GetComponents<PlayerInput>(), GetComponent<Damageable>(), GetComponent<Animator>(), GetComponent<BoxCollider2D>(), GetComponent<CharacterController2D>(), GetComponent<Scoreable>());
        //컴파일 시작시 초기화 구문. 컨트롤러 설정, 데미지 상호작용여부, 애니메이션, 컬라이더, 스코어링, 캐릭터컨트롤러(물리부) 초기화. 
        //함수 자체가 초기화 함수임. f12 참조바람
        m_Observer = new Observer(dataBase);
        var publisher = FindObjectOfType<Publisher>();
        //옵저버 클래스를 하나 생성해서 초기화 해 주고, 현 씬 내의 퍼블리셔를 넣어줌.
        m_Observer.Subscribe(publisher);
    }

    // Start is called before the first frame update
    void Start()
    {
        //애니메이션 링크를 초기화.
        SceneLinkedSMB<PlayerBehaviour>.Initialise(dataBase.animator, this);

        //스프라이트의 좌우를 초기화.
        spriteRenderer.flipX = spriteOriginallyFacesRight;

        //물리부 초기화.
        m_CurrentGravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        m_OriginallyGravity = m_CurrentGravity;
        m_JumpVelocity = Mathf.Abs(m_CurrentGravity) * timeToJumpApex;

        m_TimeToLeapHeight = -wallLeapVelocity.y / m_CurrentGravity;
        m_OriginallyAirborneAccelProp = airborneAccelProportion;


        m_DashDeceleration = (2 * dashDistance) / Mathf.Pow(timeToDashPoint, 2);
        m_DashVelocity = m_DashDeceleration * timeToDashPoint;

        m_WallLeapingEndWait = new WaitForSeconds(m_TimeToLeapHeight);
    }

    //업데이트 부에서는 픽스드 타임과 무관한 시스템 업데이트를 담당함.
    void Update()
    {
        foreach (PlayerInput inp in dataBase.playerInput)
        {
            if (!inp.Pause.Down && !inp.DebugMenuOpen.Down)
                continue;
            //f12 디버그 메뉴 활성화 코드. 
            if (inp.DebugMenuOpen.Down)
            {
                if (!debugMenu.activeSelf)
                {
                    Publisher.Instance.GainOrReleaseControl(false);
                    inp.DebugMenuOpen.GainControl();
                    debugMenu.SetActive(true);
                }
                else
                {
                    Publisher.Instance.GainOrReleaseControl(true);
                    debugMenu.SetActive(false);
                }
            }

            //정지버튼 활성화. 
            if (inp.Pause.Down)
            {
                if (!m_InPause)
                {
                    if (ScreenFader.IsFading)
                    {
                        return;
                    }

                    Publisher.Instance.GainOrReleaseControl(false);
                    Publisher.Instance.GainPause();
                    m_InPause = true;
                    Time.timeScale = 0;
                    //메뉴 씬을 로드하되, 기존 씬을 닫지 않는다.
                    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("UIMenus", UnityEngine.SceneManagement.LoadSceneMode.Additive);
                    UIManager.Instance.ToggleHUDCanvas(false);
                }
            }
            break;
        }
        //퍼즈를 끝냈을 때 다시 움직이도록 함.
        if (m_InPause && Time.timeScale == 1)
        {
            m_InPause = false;
        }
    }

    //픽스드 업데이트. 인풋에 의한 움직임과, 속도값을 파라미터로 하는 애니메이션 컨트롤
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

    public void TeleportToColliderSide()
    {
        Vector2 directon = dataBase.character.Velocity.x < 0 ? Vector2.left : Vector2.right;
        Vector2 colliderSide = dataBase.character.Rigidbody2D.position + dataBase.collider.offset
            + directon * (dataBase.collider.bounds.size.x * 0.5f);

        dataBase.character.Teleport(colliderSide);
    }

    //이동 벡터를 파라미터로 이동벡터를 저장합니다. 실시간 저장.
    public void SetMoveVector(Vector2 newMovement)
    {
        m_MoveVector = newMovement;
    }

    //수직 이동에 사용하는 특수 함수
    public void SetVerticalMovement(float newVerticalMovement)
    {
        m_MoveVector.y = newVerticalMovement;
    }

    //수평 이동에 사용하는 특수 함수.
    public void SetHorizontalMovement(float newHorizontalMovement)
    {
        m_MoveVector.x = newHorizontalMovement;
    }


    public void GroundedHorizontalMovement()
    {
        float desiredSpeed = 0; ;
        float acceleration=0;
        foreach (PlayerInput inp in dataBase.playerInput)
        {
            
            desiredSpeed = inp.Horizontal.Value * moveSpeed;
            acceleration = inp.Horizontal.ReceivingInput ? groundAcceleration : groundDeceleration;
            
            if (inp.Horizontal.ReceivingInput)
                break;
        }
        m_MoveVector.x = Mathf.MoveTowards(m_MoveVector.x, desiredSpeed, acceleration * Time.deltaTime);
        //if (m_CharacterController2D.collisionFlags.CheckForHorizontal())
        //{
        //    m_MoveVector.x = (m_CharacterController2D.sideRaycastDistance + m_Box.size.x * 0.5f) * PlayerInput.Instance.Horizontal.Value;
        //}
    }


    public void OnFlash(Vector2 position)
    {
        dataBase.animator.SetBool(m_HashElectricContactPara, true);
        m_FlashPoint = position;
        foreach (PlayerInput inp in dataBase.playerInput)
        {
            
            inp.ReleaseControl(true);
        };
    }

    //animtion Clip
    public void OnFlashMove()
    {
        dataBase.animator.SetBool(m_HashElectricContactPara, false);
        dataBase.character.Move(m_FlashPoint);
        foreach (PlayerInput inp in dataBase.playerInput)
        {
            
            inp.GainControl();
        }
    }

    public bool GetFlash()
    {
        return dataBase.animator.GetBool(m_HashElectricContactPara);
    }
    public void OnHack(bool facing, Action action)
    {
        dataBase.animator.SetBool(m_HashInteractPara, true);
        UpdateFacing(facing);

        m_HackController = action;
    }

    //실제 Exit가 끝났는지 여부는 판단 x 진입하면 바로 실행
    //animation Clip
    public void MachineOperate()
    {
        dataBase.animator.SetBool(m_HashInteractPara, false);
        m_HackController.Invoke();
    }

    public void CheckForIdleElapsed()
    {
        m_IdleElapsedTime += Time.deltaTime;
        foreach (PlayerInput inp in dataBase.playerInput)
        {
            Vector2 v = inp.AxisInputsValue();

            if (v.x != 0 || v.y != 0 || inp.Jump.Down)
            {
                m_IdleElapsedTime = 0;
                return;
            }
           
        }

        if (m_IdleElapsedTime > maxIdleDuration)
        {
            dataBase.animator.SetBool(m_HashBoringPara, true);
        }
    }

    public void CheckForBoring()
    {
        foreach (PlayerInput inp in dataBase.playerInput)
        {
            Vector2 v = inp.AxisInputsValue();

            if (v.x != 0 || v.y != 0 || inp.Jump.Down)
            {
                m_IdleElapsedTime = 0;
                dataBase.animator.SetBool(m_HashBoringPara, false);
            }
        }
    }

    public void StopBoring()
    {
        m_IdleElapsedTime = 0;
        dataBase.animator.SetBool(m_HashBoringPara, false);
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
        bool returnVal = false;
        foreach(PlayerInput inp in dataBase.playerInput)
        {
            if(inp.Jump.Down)
            {
                returnVal = true;
                break;
            }
        }
        return returnVal;
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
        foreach (PlayerInput inp in dataBase.playerInput)
        {
            if (!inp.Horizontal.ReceivingInput)
                continue;
            float desiredSpeed = inp.Horizontal.Value * moveSpeed;

            float acceleration;

            if (!inp.Horizontal.ReceivingInput)
                acceleration = groundAcceleration * airborneAccelProportion;
            else
                acceleration = groundDeceleration * airborneDecelProportion;

            m_MoveVector.x = Mathf.MoveTowards(m_MoveVector.x, desiredSpeed, acceleration * Time.deltaTime);

            break;
        }
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
        m_MoveVector.y = Mathf.Clamp(m_MoveVector.y, -maxAirborneSpeed, 50.0f);
    }

    public bool CheckForDashInput()
    {
        bool inputDash = false;
        foreach(PlayerInput inp in dataBase.playerInput)
        {
            if (inp.Dash.Down)
                inputDash = true;
        }

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
        movement = Vector2.zero;
        foreach(PlayerInput inp in dataBase.playerInput)
        {
            bool inputNone = inp.CheckAxisInputsNone();


            Vector2 dashDirection = inp.AxisInputsValue();
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

            if (inputNone)
                break;
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

            if (Mathf.Approximately(m_MoveVector.x, 0) && dataBase.character.collisionFlags.IsCeilinged)
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

    public void SignalFlipX()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    public void UpdateFacing()
    {
        foreach (PlayerInput inp in dataBase.playerInput)
        {
            bool faceLeft = inp.Horizontal.Value < 0f;
            bool faceRight = inp.Horizontal.Value > 0f;

            if (faceLeft)
            {
                spriteRenderer.flipX = !spriteOriginallyFacesRight;
            }
            else if (faceRight)
            {
                spriteRenderer.flipX = spriteOriginallyFacesRight;
            }
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
        if (dataBase.character.collisionFlags.hitCount != 3)
        {
            dataBase.animator.SetBool(m_HashGrabbingPara, false);
            return;
        }

        if (dataBase.animator.GetBool(m_HashDashingPara))
        {
            dataBase.animator.SetBool(m_HashGrabbingPara, false);
            return;
        }
        if (dataBase.character.collisionFlags.IsGrounded)
        {
            m_CurrentTimeToWaitSliding = 0f;
            dataBase.animator.SetBool(m_HashGrabbingPara, false);
            return;
        }
        if (!dataBase.character.collisionFlags.CheckForWidth())
        {
            dataBase.animator.SetBool(m_HashGrabbingPara, false);
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
        foreach (PlayerInput inp in dataBase.playerInput)
        {
            if(inp.Horizontal.ReceivingInput)
              m_MoveVector.x = inp.Horizontal.Value;
        }
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
        if (!dataBase.abilityTypes.Contains(PlayerDataBase.AbilityType.WALL_JUMP))
        {
            return;
        }

        int wallDirection = dataBase.character.collisionFlags.IsLeftSide ? -1 : 1;

        foreach (PlayerInput inp in dataBase.playerInput)
        {
            if (inp.Horizontal.Value == wallDirection)
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
                break;
            }
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
        Publisher.Instance.SetAnimState(true, false);
        Publisher.Instance.SetObservers(true, true, cellController.LastEnteringDestination.locations);
        cellController.CurrentCell.ResetCell(false);
    }

    public void OnDie()
    {
        foreach (PlayerInput inp in dataBase.playerInput)
        {
            if (!inp.HaveControl)
            {
                return;
            }

            dataBase.animator.SetTrigger(m_HashDeadPara);
            StartCoroutine(DieRespawnCoroutine());
            break;
        }
    }

    //Flash때도 체크
    public void CheckForHack()
    {
        if(dataBase.animator.GetBool(m_HashInteractPara))
        {
            dataBase.animator.SetBool(m_HashInteractPara, false);
        }
    }

    // fadeduration * 2 < invulnerabilityDuration
    IEnumerator DieRespawnCoroutine()
    {
        Publisher.Instance.GainOrReleaseControl(false);
        yield return new WaitForSeconds(1.0f);
        yield return StartCoroutine(ScreenFader.FadeSceneOut());
        Respawn();
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(ScreenFader.FadeSceneIn());
        Publisher.Instance.GainOrReleaseControl(true);
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
