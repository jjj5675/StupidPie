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
        publisher.SetObservers(false, false, 0, cellController.LastEnteringDestination.playerLocations);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private IEnumerator Transition(CellTransitionDestination.DestinationTag destinationTag)
    {
        m_IsTransitioning = true;
        publisher.GainOrReleaseControl(false);
        yield return ScreenFader.FadeSceneOut();

        CellTransitionDestination entrance;
        cellController.CurrentCell.GetCellDestination(destinationTag, out entrance);
        publisher.SetObservers(false, false, 0, entrance.playerLocations);

        yield return ScreenFader.FadeSceneIn();
        publisher.GainOrReleaseControl(true);
        m_IsTransitioning = false;
    }
}
