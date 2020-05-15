using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructibleTile : Platform
{
    public PlatformCatcher platformCatcher;
    public GameObject destroyed;
    public float waitBreakDuration;
    public TileBase refreshTile;

    private Tilemap m_Tilemap;
    private Tilemap m_StartingTilemap;
    private List<Vector3Int> m_TileArray = new List<Vector3Int>(10);
    private List<Vector3Int> m_DestroyedTilePositions = new List<Vector3Int>(50);
    private Coroutine m_DestroyTileCoroutine;
    private bool m_IsRefreshing = false;

    protected override void Initialise()
    {
        m_Tilemap = GetComponent<Tilemap>();

        if (platformCatcher == null)
        {
            platformCatcher = GetComponent<PlatformCatcher>();
        }
    }

    public override void ResetPlatform()
    {
        m_IsRefreshing = true;

        if (m_DestroyTileCoroutine != null)
        {
            StopCoroutine(m_DestroyTileCoroutine);
            m_DestroyTileCoroutine = null;
        }

        for(int i=0; i<m_DestroyedTilePositions.Count; i++)
        {
            m_Tilemap.SetColor(m_DestroyedTilePositions[i], new Color(1.0f, 1.0f, 1.0f, 1.0f));
            m_Tilemap.SetTile(m_DestroyedTilePositions[i], refreshTile);
        }

        m_TileArray.Clear();
        m_DestroyedTilePositions.Clear();

        Invoke("DisableRefreshTile", 0.5f);
    }

    void FixedUpdate()
    {
        if (0 < platformCatcher.CaughtObjectCount)
        {
            if (platformCatcher.ContactPoints[0].collider)
            {
                Vector3 hitPosition = Vector3.zero;
                Vector2 normal = Vector2.up;
                hitPosition.x = platformCatcher.ContactPoints[0].point.x - 0.01f * normal.x;
                hitPosition.y = platformCatcher.ContactPoints[0].point.y - 0.01f * normal.y;

                Vector3Int cell = m_Tilemap.WorldToCell(hitPosition);
                Color tileColor = m_Tilemap.GetColor(cell);

                if (Mathf.Approximately(tileColor.a, 0))
                {
                    return;
                }


                TileBase tileBase = m_Tilemap.GetTile(cell);

                if (tileBase != null)
                {
                    if (!m_TileArray.Contains(cell))
                    {
                        m_TileArray.Add(cell);
                        m_DestroyedTilePositions.Add(cell);
                        m_DestroyTileCoroutine = StartCoroutine(DestroyTile(cell));
                    }
                }

            }
        }
    }

    void DisableRefreshTile()
    {
        m_IsRefreshing = false;
    }

    IEnumerator DestroyTile(Vector3Int tilePos)
    {
        yield return new WaitForSeconds(waitBreakDuration);

        if (!m_IsRefreshing)
        {
            m_Tilemap.SetColor(tilePos, new Color(1.0f, 1.0f, 1.0f, 0.0f));
            GameObject.Instantiate(destroyed, m_Tilemap.CellToLocalInterpolated(tilePos + new Vector3(0.5f, 0.5f)), Quaternion.identity);
        }
        else
        {
            yield break;
        }

        yield return new WaitForSeconds(0.5f);

        if (!m_IsRefreshing)
        {
            m_TileArray.RemoveAt(0);
            m_Tilemap.SetTile(tilePos, null);
        }
    }

}
