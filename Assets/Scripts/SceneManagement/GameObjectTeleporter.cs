using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectTeleporter : MonoBehaviour
{
    public static GameObjectTeleporter Instance
    {
        get
        {
            if (instance != null)
                return instance;

            instance = FindObjectOfType<GameObjectTeleporter>();

            if (instance != null)
                return instance;

            GameObject gameObjectTeleporter = new GameObject("GameObjectTeleporter");
            instance = gameObjectTeleporter.AddComponent<GameObjectTeleporter>();

            return instance;
        }
    }

    private static GameObjectTeleporter instance;
    private bool m_IsTransitioning;

    void Awake()
    {
        if(Instance == null)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public static void Teleport(GameObject transitioningGameObject, Vector3 destinationPosition)
    {
        Instance.StartCoroutine(Instance.Transition(transitioningGameObject, destinationPosition, false));
    }

    private IEnumerator Transition(GameObject transitioningGameObject, Vector3 destinationPosition, bool fade)
    {
        m_IsTransitioning = true;
        transitioningGameObject.transform.position = destinationPosition;
        m_IsTransitioning = false;
        yield return null; //임시
    }
}
