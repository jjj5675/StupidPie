using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPool : ObjectPool<GhostPool, GhostObject>
{
    public float effectDuration;
    public float spawnRate;
    public bool useColor;
    public Color effectColor;

    [HideInInspector]
    public SpriteRenderer spriteRenderer;
    private bool m_Started = false;
    private float m_CurrentSpawnTime;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void StartSpawnGhost()
    {
        m_Started = true;
    }

    public void StopSpawnGhost()
    {
        m_Started = false;
        m_CurrentSpawnTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_Started)
        {
            return;
        }

        m_CurrentSpawnTime += Time.deltaTime;

        if(spawnRate <= m_CurrentSpawnTime)
        {
            Pop();
            m_CurrentSpawnTime = 0f;
        }
    }
}

public class GhostObject : PoolObject<GhostPool, GhostObject>
{
    public Transform transform;
    public Ghost ghost;

    protected override void SetReferences()
    {
        transform = instance.transform;
        ghost = instance.GetComponent<Ghost>();
        ghost.ghostPoolObject = this;
    }

    public override void WakeUp()
    {
        instance.SetActive(true);
        ghost.SetGhost(objectPool.effectDuration, objectPool.transform, objectPool.spriteRenderer, objectPool.effectColor, objectPool.useColor);
    }

    public override void Sleep()
    {
        instance.SetActive(false);
    }
}
