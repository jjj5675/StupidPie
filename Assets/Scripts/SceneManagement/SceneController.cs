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

    public CellController cellController;
    public Cell rootCell;
    public CellTransitionDestination.DestinationTag initalCellTransitionDestinationTag;
    public Publisher publisher;

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
        cellController.SetCells(rootCell, initalCellTransitionDestinationTag);
        publisher.SetObservers(false, false, true, cellController.LastEnteringDestination.playerLocations);
    }

    public static void Regame()
    {
        Instance.rootCell.GetCellDestination(CellTransitionDestination.DestinationTag.A, out CellTransitionDestination entrance);
        Instance.StartCoroutine(Instance.Transition(entrance, true, true, true, true));
        Instance.cellController.SetCells(Instance.rootCell, CellTransitionDestination.DestinationTag.A);
    }

    public static void Restage()
    {
        Instance.StartCoroutine(Instance.Transition(Instance.cellController.LastEnteringDestination, true, true, true, true));

    }

    public IEnumerator UnpauseCoroutine()
    {
        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync("UIMenus");
        publisher.GainOrReleaseControl(true);
        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator Transition(CellTransitionDestination entrance, bool fade, bool resetCell, bool dead, bool resetHealth)
    {
        m_IsTransitioning = true;
        publisher.GainOrReleaseControl(false);

        if (fade)
        {
            yield return ScreenFader.FadeSceneOut();
        }

        publisher.SetObservers(resetHealth, dead, true, entrance.playerLocations);

        if (fade)
        {
            yield return ScreenFader.FadeSceneIn();
        }

        publisher.GainOrReleaseControl(true);
        m_IsTransitioning = false;
    }
}
