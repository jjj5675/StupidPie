using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConductorTrigger2D : Platform
{
    public GameObject wireTilemap;
    public Transform facingLeftHackingPoint;
    public Transform facingRightHackingPoint;
    public bool m_SpriteOriginallyFacesRight;

    public UnityEvent OnEnabled;
    public UnityEvent OnDisabled;
    public GameObject hologram;

    public AudioSource OnSound;
    public AudioSource OffSound;

    Animator m_Animator;
    Collider2D m_WireCollider;
    bool m_TriggerEnabled = false;
    Collider2D[] m_OverlapBuffer;
    ContactFilter2D m_ContactFilter = new ContactFilter2D();
    Dictionary<Collider2D, PlayerBehaviour> m_DataBaseCache;
    Dictionary<Collider2D, Platform> m_PlatformCache;

    readonly int m_HashOperatePara = Animator.StringToHash("Operate");

    private void Awake()
    {
        if(wireTilemap!=null)
          m_WireCollider = wireTilemap.GetComponent<CompositeCollider2D>();
        m_ContactFilter.useLayerMask = true;
        m_ContactFilter.useTriggers = true;
        m_ContactFilter.layerMask = 1 << LayerMask.NameToLayer("Platform");
        Physics2D.queriesStartInColliders = false;
        GetComponent<Collider2D>().isTrigger = true;
        m_Animator = GetComponent<Animator>();

        if (m_SpriteOriginallyFacesRight)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
            Vector3 temp = facingLeftHackingPoint.position;
            facingLeftHackingPoint.position = facingRightHackingPoint.position;
            facingRightHackingPoint.position = temp;
        }
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
        if (Publisher.Instance.ColliderHasObserver(collision))
        {
            if (!m_DataBaseCache.ContainsKey(collision))
            {
                m_DataBaseCache.Add(collision, collision.GetComponent<PlayerBehaviour>());
            }
            //if(collision.GetComponent<PlayerBehaviour>().dataBase.abilityTypes.Contains(PlayerDataBase.AbilityType.INTERACTION))
            //{
            //    if (!m_TriggerEnabled)
            //        OnSound.Play();
            //    else
            //        OffSound.Play();
            //}
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (Publisher.Instance.ColliderHasObserver(collision))
        {
            if (m_DataBaseCache.TryGetValue(collision, out PlayerBehaviour playerBehaviour))
            {
                if (!playerBehaviour)
                {
                    return;
                }

                bool iterating = false;
                foreach (PlayerInput inp in playerBehaviour.dataBase.playerInput)
                {
                    if (inp.Interact.Down)
                    {
                        iterating = true;
                        if (!m_TriggerEnabled && !OnSound.isPlaying && !OffSound.isPlaying)
                            OnSound.Play();
                        else if (m_TriggerEnabled && !OnSound.isPlaying && !OffSound.isPlaying)
                            OffSound.Play();
                        break;
                    }
                }
                if (iterating)
                {
                    if (!playerBehaviour.dataBase.character.collisionFlags.IsGrounded)
                    {
                        return;
                    }

                    if (playerBehaviour.dataBase.animator.GetBool("Interact"))
                    {
                        return;
                    }

                    Vector2 displecement;
                    bool facingLeft;

                    if (m_SpriteOriginallyFacesRight)
                    {
                        float diffX = facingRightHackingPoint.position.x - playerBehaviour.dataBase.transform.position.x;
                        displecement = Vector2.right * diffX;
                        facingLeft = true;
                    }
                    else
                    {
                        float diffX = facingLeftHackingPoint.position.x - playerBehaviour.dataBase.transform.position.x;
                        displecement = Vector2.right * diffX;
                        facingLeft = false;
                    }

                    Vector2 velocity = displecement / Time.deltaTime;
                    playerBehaviour.dataBase.character.Move(velocity * Time.deltaTime);

                    playerBehaviour.OnHack(facingLeft, ConductorController);
                    m_Animator.enabled = true;
                    m_Animator.SetTrigger(m_HashOperatePara);
                }
            }
        }
    }

    public void OnHologram()
    {
        hologram.SetActive(true);
    }

    void ConductorController()
    {
        if (!m_TriggerEnabled)
        {
            m_TriggerEnabled = true;
            //OnSound.Play();
            OnEnabled.Invoke();
        }
        else
        {
            m_TriggerEnabled = false;
            //OffSound.Play();
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

    //UI말풍선 끄기
    //void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (m_PlayerLayerIndex == collision.gameObject.layer)
    //    {

    //    }
    //}
}



