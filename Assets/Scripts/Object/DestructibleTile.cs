using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructibleTile : MonoBehaviour
{
    public PlatformCatcher platformCatcher;
    public GameObject destroyed;
    public float waitBreakDuration;

    private Tilemap m_Tilemap;
    private List<Vector3Int> m_TileArray = new List<Vector3Int>(10);

    // Start is called before the first frame update
    void Start()
    {
        m_Tilemap = GetComponent<Tilemap>();

        if (platformCatcher == null)
        {
            platformCatcher = GetComponent<PlatformCatcher>();
        }
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
                        StartCoroutine(DestroyTile(cell));
                    }
                }

            }
        }
    }

    IEnumerator DestroyTile(Vector3Int tilePos)
    {
        yield return new WaitForSeconds(waitBreakDuration);
        m_Tilemap.SetColor(tilePos, new Color(1.0f, 1.0f, 1.0f, 0.0f));
        GameObject.Instantiate(destroyed, m_Tilemap.CellToLocalInterpolated(tilePos + new Vector3(0.5f, 0.5f)), Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        m_TileArray.RemoveAt(0);
        m_Tilemap.SetTile(tilePos, null);
    }

}
