using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostGenerator : MonoBehaviour
{
    public Color effectColor;
    public float effectDuration;
    public float spawnRate;
    public int maxGhosts;
    public bool useColor;

    private SpriteRenderer m_SpriteRenderer;
    private int m_SortingLayer;
    private bool m_Started = false;

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



    }
}
