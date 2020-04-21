using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance
    {
        get
        {
            if (instance != null)
                return instance;

            instance = FindObjectOfType<SceneController>();

            if (instance != null)
                return instance;

            Create();

            return instance;
        }
    }

    private static SceneController instance;

    public static SceneController Create()
    {
        GameObject sceneControllerGameObject = new GameObject("SceneController");
        instance = sceneControllerGameObject.AddComponent<SceneController>();

        return instance;
    }

    public SceneTransitionDestination initalSceneTransitionDestination;

    private Scene m_CurrentZoneScene;
    private SceneTransitionDestination.DestinationTag m_ZoneRestartDestinationTag;
    private bool m_IsTransitioning;

    void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        if(initalSceneTransitionDestination != null)
        {
            SetEnteringGameObjectLocation(initalSceneTransitionDestination);
        }
        else
        {
            m_CurrentZoneScene = SceneManager.GetActiveScene();
            m_ZoneRestartDestinationTag = SceneTransitionDestination.DestinationTag.A;
        }
            
    }

    private void SetEnteringGameObjectLocation(SceneTransitionDestination entrance)
    {
        if(entrance == null)
        {
            Debug.LogWarning("이동시킬 위치 입력이 설정되지 않았습니다.");
            return;
        }

        Transform entranceLocation = entrance.transform;
        Transform enteringTransform = entrance.transitioningGameObject.transform;
        enteringTransform.position = entranceLocation.position;
        enteringTransform.rotation = entranceLocation.rotation;
    }

    public static void RestartZone(bool resetHealth = true)
    {
        if(resetHealth && PlayerCharacter.PlayerInstance != null)
        {
            //체력 회복
        }

        Instance.StartCoroutine(Instance.Transition(Instance.m_ZoneRestartDestinationTag));
    }

    private IEnumerator Transition(SceneTransitionDestination.DestinationTag destinationTag)
    {
        m_IsTransitioning = true;
        PlayerInput.Instance.ReleaseControl();
        yield return ScreenFader.FadeSceneOut();
        SceneTransitionDestination entrance = GetDestination(destinationTag);
        SetEnteringGameObjectLocation(entrance);
        yield return ScreenFader.FadeSceneIn();
        PlayerInput.Instance.GainControl();
        m_IsTransitioning = false;
    }

    private SceneTransitionDestination GetDestination(SceneTransitionDestination.DestinationTag destinationTag)
    {
        SceneTransitionDestination[] entrances = FindObjectsOfType<SceneTransitionDestination>();

        for(int i=0; i<entrances.Length; i++)
        {
            if (entrances[i].destinationTag == destinationTag)
                return entrances[i];
        }

        Debug.LogWarning("전이 시킬 " + destinationTag + " 태그가 없습니다.");
        return null;
    }
}
