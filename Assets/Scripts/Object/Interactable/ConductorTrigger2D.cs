using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConductorTrigger2D : Platform
{
    public GameObject wireTilemap;
    public UnityEvent OnEnabled;
    public UnityEvent OnDisabled;

    Collider2D m_WireCollider;
    int m_PlayerLayerIndex;
    bool m_TriggerEnabled = false;
    Collider2D[] m_OverlapBuffer;
    ContactFilter2D m_ContactFilter = new ContactFilter2D();

    Dictionary<Collider2D, PlayerDataBase> m_DataBaseCache;
    Dictionary<Collider2D, Platform> m_PlatformCache;

    private void Awake()
    {
        m_WireCollider = wireTilemap.GetComponent<CompositeCollider2D>();
        m_PlayerLayerIndex = LayerMask.NameToLayer("Player");
        m_ContactFilter.useLayerMask = true;
        m_ContactFilter.useTriggers = true;
        m_ContactFilter.layerMask = 1 << LayerMask.NameToLayer("Platform");
        Physics2D.queriesStartInColliders = false;
        GetComponent<Collider2D>().isTrigger = true;
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
            platform.Value.StopMoving();
        }
    }

    private void OnEnable()
    {
        m_DataBaseCache = new Dictionary<Collider2D, PlayerDataBase>(2);
        m_PlatformCache = new Dictionary<Collider2D, Platform>(16);
        m_OverlapBuffer = new Collider2D[16];
    }

    private void OnDisable()
    {
        if(m_DataBaseCache.Count != 0)
        {
            m_DataBaseCache.Clear();
        }
        if(m_PlatformCache.Count!=0)
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

        for(int i=0; i<count; i++)
        {
            if(!m_PlatformCache.ContainsKey(m_OverlapBuffer[i]))
            {
                m_PlatformCache.Add(m_OverlapBuffer[i], m_OverlapBuffer[i].GetComponent<Platform>());
            }

            if(m_PlatformCache.TryGetValue(m_OverlapBuffer[i], out Platform platform))
            {
                if(platform)
                {
                    platform.StartMoving();
                }
            }
        }
    }

    //오브젝트 위로 홀로그램 띄우기
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_PlayerLayerIndex == collision.gameObject.layer)
        {
            if (!m_DataBaseCache.ContainsKey(collision))
            {
                m_DataBaseCache.Add(collision, collision.GetComponent<PlayerBehaviour>().dataBase);
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (m_PlayerLayerIndex == collision.gameObject.layer)
        {
            if (m_DataBaseCache.TryGetValue(collision, out PlayerDataBase dataBase))
            {
                if (!dataBase)
                {
                    return;
                }

                if (dataBase.playerInput.Interact.Down)
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

                        foreach(var platform in m_PlatformCache)
                        {
                            platform.Value.StopMoving();
                        }
                    }

                    //상호작용 애니메이션추가
                }
            }
        }
    }

    //홀로그램 끄기
    void OnTriggerExit2D(Collider2D collision)
    {
        if (m_PlayerLayerIndex == collision.gameObject.layer)
        {

        }
    }
}
