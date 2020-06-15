using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class ConductorTrigger2D : MonoBehaviour
{
    public GameObject wireTilemap;
    public UnityEvent OnEnable;
    public UnityEvent OnDisable;

    Collider2D m_WireCollider;
    int m_PlayerLayerIndex;
    bool m_TriggerEnabled = false;
    Collider2D[] m_OverlapBuffer = new Collider2D[10];
    ContactFilter2D m_ContactFilter = new ContactFilter2D();

    Dictionary<Collider2D, PlayerDataBase> m_DataBaseCache = new Dictionary<Collider2D, PlayerDataBase>(2);
    Dictionary<Collider2D, Platform> m_PlatformCache = new Dictionary<Collider2D, Platform>(10);

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
                Debug.Log(m_OverlapBuffer[i].name);
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

                if (dataBase.playerInput.Interaction.Down)
                {
                    if (!m_TriggerEnabled)
                    {
                        m_TriggerEnabled = true;
                        OnEnable.Invoke();
                    }
                    else
                    {
                        m_TriggerEnabled = false;
                        OnDisable.Invoke();

                        foreach(var item in m_PlatformCache)
                        {
                            item.Value.StopMoving();
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
