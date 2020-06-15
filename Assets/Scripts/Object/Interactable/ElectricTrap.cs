using System.Collections.Generic;
using UnityEngine;

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
    Dictionary<Collider2D, PlayerDataBase> m_DataBaseCache;

    private void Awake()
    {
        m_PlayerLayerIndex = LayerMask.NameToLayer("Player");
    }

    private void OnEnable()
    {
        m_DataBaseCache = new Dictionary<Collider2D, PlayerDataBase>(2);
    }

    private void OnDisable()
    {
        if (m_DataBaseCache.Count != 0)
        {
            m_DataBaseCache.Clear();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_PlayerLayerIndex == collision.gameObject.layer)
        {
            if (!m_DataBaseCache.ContainsKey(collision.collider))
            {
                m_DataBaseCache.Add(collision.collider, collision.gameObject.GetComponent<PlayerBehaviour>().dataBase);
            }

            if (m_DataBaseCache.TryGetValue(collision.collider, out PlayerDataBase dataBase))
            {
                if (!dataBase)
                {
                    return;
                }

                if (dataBase.abilityTypes.Contains(PlayerDataBase.AbilityType.INTERACTION))
                {
                    if (collision.contacts[0].point.x < collision.transform.position.x)
                    {
                        GameObjectTeleporter.Teleport(collision.gameObject, collision.transform.position + Vector3.left * transitionDistance);
                    }
                    else
                    {
                        GameObjectTeleporter.Teleport(collision.gameObject, collision.transform.position + Vector3.right * transitionDistance);
                    }
                }
                else
                {
                    dataBase.damageable.TakeDamage(null);
                }
            }
        }
    }
}
