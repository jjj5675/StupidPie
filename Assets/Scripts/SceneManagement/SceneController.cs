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
    public ScreenManager screenManager;
    public MenuActivityController menuActivityController;

    private Scene m_CurrentZoneScene;
    private bool m_Transitioning;

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
        if (cellController)
        {
            cellController.SetCell(rootCell, initalCellTransitionDestinationTag);
        }
        if (screenManager)
        {
            screenManager.autoCameraSetup.SetMainConfinerBound(rootCell.confinerCollider);
        }
        if (Publisher.Instance)
        {
            Publisher.Instance.SetObservers(false, true, cellController.LastEnteringDestination.locations);
        }
    }

    public void UnPause(bool inputControl)
    {
        if (Time.timeScale > 0)
        {
            return;
        }

        StartCoroutine(UnpauseCoroutine(inputControl));
        menuActivityController.SetActive(true);
    }

    IEnumerator UnpauseCoroutine(bool inputControl)
    {
        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync("UIMenus");
        if (inputControl)
        {
            Publisher.Instance.GainOrReleaseControl(true);
        }
        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame();
    }

    public void Restage()
    {
        menuActivityController.TimerUI.StopTimer();
        Publisher.Instance.SetAnimState(false, true);
        StartCoroutine(InTransition(true, false, cellController.LastEnteringDestination));
    }

    public void Regame()
    {
        menuActivityController.TimerUI.StopTimer();
        Publisher.Instance.SetAnimState(false, true);
        rootCell.GetCellDestination(initalCellTransitionDestinationTag, out CellTransitionDestination cellTransitionDestination);
        StartCoroutine(InTransition(true, true, cellTransitionDestination));
    }

    public void TransitionToScene(TransitionPoint transitionPoint)
    {
        StartCoroutine(Transition(transitionPoint.newSceneName, CellTransitionDestination.DestinationTag.A));
    }

    private IEnumerator Transition(string newSceneName, CellTransitionDestination.DestinationTag destinationTag, TransitionPoint.TransitionType transitionType = TransitionPoint.TransitionType.DifferentZone)
    {
        m_Transitioning = true;

        yield return StartCoroutine(ScreenFader.FadeSceneOut(ScreenFader.FadeType.Black));
        yield return SceneManager.LoadSceneAsync(newSceneName);

        cellController = FindObjectOfType<CellController>();
        screenManager = FindObjectOfType<ScreenManager>();
        menuActivityController = FindObjectOfType<MenuActivityController>();

        Publisher.Instance.GainOrReleaseControl(false);
        cellController.GetRootCell(out rootCell);
        cellController.SetCell(rootCell, destinationTag);
        Publisher.Instance.SetObservers(false, true, cellController.LastEnteringDestination.locations);
        screenManager.autoCameraSetup.SetMainConfinerBound(rootCell.confinerCollider);

        yield return StartCoroutine(ScreenFader.FadeSceneIn());

        Publisher.Instance.GainOrReleaseControl(true);
        m_Transitioning = false;
    }

    private IEnumerator InTransition(bool fade, bool cameraSetting, CellTransitionDestination entrance)
    {
        m_Transitioning = true;
        Publisher.Instance.GainOrReleaseControl(false);

        if (fade)
        {
            yield return ScreenFader.FadeSceneOut();
        }

        Publisher.Instance.SetAnimState(true, false);
        Publisher.Instance.SetObservers(true, true, entrance.locations);
        cellController.CurrentCell.ResetCell(false);

        if (cameraSetting)
        {
            cellController.SetCell(rootCell, initalCellTransitionDestinationTag);
            if (cellController.PreviousCell != rootCell)
            {
                cellController.DisablePreviousCell();
            }
            screenManager.autoCameraSetup.SetMainConfinerBound(rootCell.confinerCollider);
        }

        menuActivityController.TimerUI.ResetTimer();

        if (fade)
        {
            yield return ScreenFader.FadeSceneIn();
        }

        Publisher.Instance.GainOrReleaseControl(true);
        menuActivityController.TimerUI.StartTimer();
        m_Transitioning = false;
    }
}
