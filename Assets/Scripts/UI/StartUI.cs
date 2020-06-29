using UnityEngine;

public class StartUI : MonoBehaviour
{
    public CutsceneDirector cutsceneDirector;
    public TransitionPoint transitionPoint;

    protected bool m_IsTransitioning = false;

    // Start is called before the first frame update
    void Start()
    {
        cutsceneDirector.Play();
    }

    //// Update is called once per frame
    void Update()
    {
        if (cutsceneDirector.gameObject.activeSelf)
        {
            if (!m_IsTransitioning)
            {
                // 로그 종료
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    cutsceneDirector.Stop();
                    SceneController.Instance.TransitionToScene(transitionPoint);
                    m_IsTransitioning = true;
                }
                else if (Input.anyKeyDown)  //다음 로그 이동
                {
                    cutsceneDirector.Next();
                    if (cutsceneDirector.End)
                    {
                        SceneController.Instance.TransitionToScene(transitionPoint);
                        m_IsTransitioning = true;
                    }
                }
            }
        }
    }
}
