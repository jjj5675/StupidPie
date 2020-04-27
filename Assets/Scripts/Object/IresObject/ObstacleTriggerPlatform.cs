using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityScript.Scripting.Pipeline;

public class ObstacleTriggerPlatform : MonoBehaviour
{
    public PlatformCatcher platformCatcher;

    protected bool m_Started = false;
    protected Damager[] m_Damagers;
    protected bool m_DamagerOnEabled = false;

    protected Collider2D m_Collider;
    protected List<ObstacleTriggerPlatform> m_Platforms = new List<ObstacleTriggerPlatform>(128);
    public Damager[] Damagers { get { return m_Damagers; } }

    // Start is called before the first frame update
    void Start()
    {
        if (platformCatcher == null)
        {
            platformCatcher = GetComponent<PlatformCatcher>();
        }

        if (m_Collider == null)
        {
            m_Collider = GetComponent<Collider2D>();
        }

        m_Damagers = GetComponentsInChildren<Damager>();

        if (m_Damagers != null)
        {
            for (int i = 0, count = m_Damagers.Length; i < count; i++)
            {
                m_Damagers[i].enabled = false;
            }
        }

        m_DamagerOnEabled = false;

        Initialise();
        ObstaclePlatformSearch();
    }

    protected void Initialise()
    {
        m_Started = false;
    }

    protected void ObstaclePlatformSearch()
    {
        //루트 오브젝트
        Queue<Collider2D> queue = new Queue<Collider2D>(30);
        Collider2D[] overlapColliders = new Collider2D[30];
        ContactFilter2D contactFilter = new ContactFilter2D();

        queue.Enqueue(m_Collider);

        while (queue.Count != 0)
        {
            Collider2D collider = queue.Dequeue();
            m_Platforms.Add(collider.GetComponent<ObstacleTriggerPlatform>());

            int count = collider.OverlapCollider(contactFilter, overlapColliders);

            for (int i = 0; i < count; i++)
            {
                ObstacleTriggerPlatform obstacleTrigger = overlapColliders[i].GetComponent<ObstacleTriggerPlatform>();

                if (obstacleTrigger != null)
                {
                    if (m_Platforms.Contains(obstacleTrigger))
                    {
                        continue;
                    }
                    else
                    {
                        queue.Enqueue(overlapColliders[i]);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!m_Started && !platformCatcher.CaughtIresCharacter)
        {
            if (m_DamagerOnEabled)
            {
                OffEnabledDamagers();
                m_DamagerOnEabled = false;
            }

            return;
        }

        if (!m_DamagerOnEabled)
        {
            OnEnabledDamagers();
            m_DamagerOnEabled = true;
        }
    }

    void OnEnabledDamagers()
    {
        for (int i = 0, count = m_Platforms.Count; i < count; i++)
        {
            int damagerCount = m_Platforms[i].Damagers.Length;

            for (int k = 0; k < damagerCount; k++)
            {
                m_Platforms[i].Damagers[k].enabled = true;
            }
        }
    }

    void OffEnabledDamagers()
    {
        for (int i = 0, count = m_Platforms.Count; i < count; i++)
        {
            int damagerCount = m_Platforms[i].Damagers.Length;

            for (int k = 0; k < damagerCount; k++)
            {
                m_Platforms[i].Damagers[k].enabled = false;
            }
        }
    }

    public void StartMoving()
    {
        m_Started = true;
    }

    public void StopMoving()
    {
        m_Started = false;
    }
}
