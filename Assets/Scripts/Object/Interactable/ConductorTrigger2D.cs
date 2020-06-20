using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConductorTrigger2D : Platform
{
    public GameObject wireTilemap;
    public bool spriteOriginallyFacesRight;
    public Transform facingLeftHackingPoint;
    public Transform facingRightHackingPoint;

    public UnityEvent OnEnabled;
    public UnityEvent OnDisabled;

    Animator m_Animator;
    Collider2D m_WireCollider;
    int m_PlayerLayerIndex;
    bool m_TriggerEnabled = false;
    Collider2D[] m_OverlapBuffer;
    ContactFilter2D m_ContactFilter = new ContactFilter2D();
    Dictionary<Collider2D, PlayerBehaviour> m_DataBaseCache;
    Dictionary<Collider2D, Platform> m_PlatformCache;
    Animator m_PlayerAnim;

    readonly int m_HashOperatePara = Animator.StringToHash("Operate");

    private void Awake()
    {
        m_WireCollider = wireTilemap.GetComponent<CompositeCollider2D>();
        m_PlayerLayerIndex = LayerMask.NameToLayer("Player");
        m_ContactFilter.useLayerMask = true;
        m_ContactFilter.useTriggers = true;
        m_ContactFilter.layerMask = 1 << LayerMask.NameToLayer("Platform");
        Physics2D.queriesStartInColliders = false;
        GetComponent<Collider2D>().isTrigger = true;
        m_Animator = GetComponentInChildren<Animator>();
    }

    protected override void Initialise()
    {
    }

    public override void ResetPlatform()
    {
        m_TriggerEnabled = false;
        OnDisabled.Invoke();

        foreach (var platform in m_PlatformCache)
        {
            if (platform.Value)
            {
                platform.Value.StopMoving();
            }
        }
    }

    private void OnEnable()
    {
        m_DataBaseCache = new Dictionary<Collider2D, PlayerBehaviour>(16);
        m_PlatformCache = new Dictionary<Collider2D, Platform>(16);
        m_OverlapBuffer = new Collider2D[16];
    }

    private void OnDisable()
    {
        if (m_DataBaseCache.Count != 0)
        {
            m_DataBaseCache.Clear();
        }
        if (m_PlatformCache.Count != 0)
        {
            m_PlatformCache.Clear();
        }

        System.Array.Clear(m_OverlapBuffer, 0, m_OverlapBuffer.Length);
    }

    public void ConductorOperate()
    {
        if (!m_TriggerEnabled)
        {
            m_TriggerEnabled = true;
            OnEnabled.Invoke();
        }
        else
        {
            m_TriggerEnabled = false;
            OnDisabled.Invoke();

            foreach (var platform in m_PlatformCache)
            {
                if (platform.Value)
                {
                    platform.Value.StopMoving();
                }
            }
        }
    }

    private void Update()
    {
        if (!m_TriggerEnabled)
        {
            return;
        }

        int count = Physics2D.OverlapCollider(m_WireCollider, m_ContactFilter, m_OverlapBuffer);

        for (int i = 0; i < count; i++)
        {
            if (!m_PlatformCache.ContainsKey(m_OverlapBuffer[i]))
            {
                m_PlatformCache.Add(m_OverlapBuffer[i], m_OverlapBuffer[i].GetComponent<Platform>());
            }

            if (m_PlatformCache.TryGetValue(m_OverlapBuffer[i], out Platform platform))
            {
                if (platform)
                {
                    platform.StartMoving();
                }
            }
        }
    }

    //오브젝트 위로 UI 띄우기
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_PlayerLayerIndex == collision.gameObject.layer)
        {
            if (!m_DataBaseCache.ContainsKey(collision))
            {
                m_DataBaseCache.Add(collision, collision.GetComponent<PlayerBehaviour>());
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (m_PlayerLayerIndex == collision.gameObject.layer)
        {
            if (m_DataBaseCache.TryGetValue(collision, out PlayerBehaviour playerBehaviour))
            {
                if (!playerBehaviour)
                {
                    return;
                }

                if (playerBehaviour.dataBase.playerInput.Interact.Down)
                {
                    //언제나 Gounded상태일때만 실행한다.
                    if (!playerBehaviour.dataBase.character.collisionFlags.IsGrounded)
                    {
                        return;
                    }

                    Vector2 dx;
                    bool facingLeft;

                    if (spriteOriginallyFacesRight)
                    {
                        float diffX = facingRightHackingPoint.position.x - playerBehaviour.dataBase.character.Rigidbody2D.position.x;
                        dx = Vector2.right * diffX;
                        facingLeft = true;
                    }
                    else
                    {
                        float diffX = facingLeftHackingPoint.position.x - playerBehaviour.dataBase.character.Rigidbody2D.position.x;
                        dx = Vector2.right * diffX;
                        facingLeft = false;
                    }

                    Vector2 velocity = dx / Time.deltaTime;
                    playerBehaviour.dataBase.character.Move(velocity * Time.deltaTime);
                    ///////////////////////////////////////////////////////

                    //상호작용시작
                    playerBehaviour.OnHack(facingLeft, ConductorOperate);
                    m_Animator.SetTrigger(m_HashOperatePara);
                }
            }
        }

    }

    //UI말풍선 끄기
    void OnTriggerExit2D(Collider2D collision)
    {
        if (m_PlayerLayerIndex == collision.gameObject.layer)
        {

        }
    }
}



