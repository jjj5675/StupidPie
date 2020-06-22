using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ElectricTrap : MonoBehaviour
{
    /*
    1. 아이레스가 해당 함정에 닿는 순간 조작이 안되는 상태로 바꾸고 
    애니메이션으로 넘어가는 연출을 보여준 다음, 아이레스가 해당 함정 반대편으로 넘어가지도록 한다.

    2. 아이레스가 해당 함정에 닿는 순간 아이레스의 모습에 변화를 주거나, 
    아이레스에게 이펙트 이미지를 넣어준다.
    */

    public float transitionDistance;

    int m_PlayerLayerIndex;
    Dictionary<Collider2D, PlayerBehaviour> m_DataBaseCache = new Dictionary<Collider2D, PlayerBehaviour>(2);
    Tilemap m_Tilemap;

    Vector2Int[] tileWay;

    private void Awake()
    {
        m_PlayerLayerIndex = LayerMask.NameToLayer("Player");
        GetComponent<CompositeCollider2D>().isTrigger = true;
        m_Tilemap = GetComponent<Tilemap>();

        tileWay = new Vector2Int[]
        {
            new Vector2Int(-1,0),
            new Vector2Int(1,0),
            new Vector2Int(0,-1),
            new Vector2Int(0,1),
            new Vector2Int(-1,1),
            new Vector2Int(-1,-1),
            new Vector2Int(1,1),
            new Vector2Int(1,-1),
        };
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (m_PlayerLayerIndex == collision.gameObject.layer)
        {
            if (!m_DataBaseCache.ContainsKey(collision))
            {
                m_DataBaseCache.Add(collision, collision.gameObject.GetComponent<PlayerBehaviour>());
            }

            if (m_DataBaseCache.TryGetValue(collision, out PlayerBehaviour  behaviour))
            {
                if (!behaviour)
                {
                    return;
                }

                if (behaviour.dataBase.abilityTypes.Contains(PlayerDataBase.AbilityType.INTERACTION))
                {
                    if(behaviour.GetFlash())
                    {
                        return;
                    }

                    Vector3Int localPosition = m_Tilemap.WorldToCell(collision.transform.position);

                    for (int i = 0; i < tileWay.Length; i++)
                    {
                        int nx = localPosition.x + tileWay[i].x;
                        int ny = localPosition.y + tileWay[i].y;

                        Vector3Int tilePosition = new Vector3Int(nx, ny, localPosition.z);

                        if (m_Tilemap.HasTile(tilePosition))
                        {
                            Vector3 worldPosition = m_Tilemap.CellToWorld(tilePosition);
                            Vector2 moveVector;

                            if (worldPosition.x < collision.transform.position.x)
                            {
                                moveVector = Vector2.left * transitionDistance;
                            }
                            else
                            {
                                moveVector = Vector2.right * transitionDistance;
                            }

                            behaviour.OnFlash(moveVector);
                            break;
                        }

                    }
                }
                else
                {
                    behaviour.dataBase.damageable.TakeDamage(null);
                }
            }
        }
    }
}
