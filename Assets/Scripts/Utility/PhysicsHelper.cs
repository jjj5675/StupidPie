using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsHelper : MonoBehaviour
{
    private static PhysicsHelper instance;

    public static PhysicsHelper Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            instance = FindObjectOfType<PhysicsHelper>();

            if (instance != null)
            {
                return instance;
            }

            Create();

            return instance;
        }
        set
        { instance = value; }
    }

    static void Create()
    {
        GameObject gameObject = new GameObject("PhysicsHelper");
        instance = gameObject.AddComponent<PhysicsHelper>();
    }

    Dictionary<Collider2D, JumpPad> m_JumpPadCache = new Dictionary<Collider2D, JumpPad>();

    void Awake()
    {
        if(Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance.PopulateColliderDictionary(m_JumpPadCache);
    }

    private void PopulateColliderDictionary<TComponent>(Dictionary<Collider2D, TComponent> dict)
        where TComponent : Component
    {
        TComponent[] component = FindObjectsOfType<TComponent>();

        for(int i=0; i<component.Length; i++)
        {
            Collider2D[] colliders = component[i].GetComponents<Collider2D>();

            for(int k = 0; k<colliders.Length; k++)
            {
                dict.Add(colliders[k], component[i]);
            }
        }
    }

    public static bool ColliderHasJumpPad(Collider2D collider)
    {
        return Instance.m_JumpPadCache.ContainsKey(collider);
    }

    public static bool TryGetJumpPad(Collider2D collider, out JumpPad jumpPad)
    {
        return Instance.m_JumpPadCache.TryGetValue(collider, out jumpPad);
    }
}
