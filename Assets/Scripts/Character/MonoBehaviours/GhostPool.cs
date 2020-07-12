using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPool : MonoBehaviour
{
    public float effectDuration;
    public float spawnRate;
    //public int maxGhosts;
    public GameObject prefab;
    public bool useColor;
    public Color effectColor;

    private SpriteRenderer m_SpriteRenderer;
    private int m_SortingLayer;
    [SerializeField]
    private bool m_Started = false;
    private float m_CurrentSpawnTime;

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_SortingLayer = m_SpriteRenderer.sortingLayerID;
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
            GameObject ghost = Instantiate(prefab);
            ghost.GetComponent<Ghost>().SetGhost(effectDuration, m_SpriteRenderer, effectColor, useColor, transform);
            m_CurrentSpawnTime = 0f;
        }
    }
}
