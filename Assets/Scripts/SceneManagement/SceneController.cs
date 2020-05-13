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

    public CellTransitionDestination.DestinationTag initalCellTransitionDestinationTag;

    private Scene m_CurrentZoneScene;
    private bool m_IsTransitioning;

    void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        CellTransitionDestination entrance;
        CellController.Instance.CurrentCell.GetCellDestination(initalCellTransitionDestinationTag, out entrance);
        SetEnteringGameObjectLocation(entrance);
    }

    private void SetEnteringGameObjectLocation(CellTransitionDestination entrance)
    {
        Transform entranceSeriLocation = entrance.seriLocation.transform;
        Transform entranceIresLocation = entrance.iresLocation.transform;

        Transform enteringSeriTransform = CellController.Instance.transitioningSeri.transform;
        Transform enteringIresTransform = CellController.Instance.transitioningIres.transform;

        enteringSeriTransform.position = entranceSeriLocation.position;
        enteringIresTransform.position = entranceIresLocation.position;
        enteringSeriTransform.rotation = entranceSeriLocation.rotation;
        enteringIresTransform.rotation = entranceIresLocation.rotation;
    }

    private IEnumerator Transition(CellTransitionDestination.DestinationTag destinationTag)
    {
        m_IsTransitioning = true;
        //PlayerInput.Instance.ReleaseControl();
        yield return ScreenFader.FadeSceneOut();

        CellTransitionDestination entrance;
        CellController.Instance.CurrentCell.GetCellDestination(destinationTag, out entrance);
        SetEnteringGameObjectLocation(entrance);

        yield return ScreenFader.FadeSceneIn();
        //PlayerInput.Instance.GainControl();
        m_IsTransitioning = false;
    }
}
