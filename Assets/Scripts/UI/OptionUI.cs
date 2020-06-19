using UnityEngine;
using System.Collections;
using UnityEditor;

public class OptionUI : MonoBehaviour
{
    public RectTransform[] pauseCanvases;

    public Vector2 anchorMin;
    public Vector2 anchorMax;

    protected int m_CurrentCanvas = 0;
    protected int m_NextCanvas = 0;
    protected Vector4 m_CurrentAnchor;


    void Start()
    {
        m_CurrentAnchor = new Vector4(
     pauseCanvases[m_CurrentCanvas].anchorMin.x, pauseCanvases[m_CurrentCanvas].anchorMin.y,
     pauseCanvases[m_CurrentCanvas].anchorMax.x, pauseCanvases[m_CurrentCanvas].anchorMax.y);

        pauseCanvases[m_NextCanvas].anchorMin -= anchorMin;
        pauseCanvases[m_NextCanvas].anchorMax += anchorMax;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (m_CurrentCanvas != 0)
            {
                if (m_CurrentCanvas == 4)
                {
                    m_NextCanvas -= 2;
                }
                else
                {
                    m_NextCanvas--;
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (m_CurrentCanvas < pauseCanvases.Length - 1)
            {
                if (m_CurrentCanvas == 2)
                {
                    m_NextCanvas += 2;
                }
                else
                {
                    m_NextCanvas++;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            switch (m_CurrentCanvas)
            {
                case 0:
                    Resume();
                    break;
                case 1:
                    Restage();
                    break;
                case 2:
                    Regame();
                    break;
                case 3:
                    Setting();
                    break;
                default:
                    Exit();
                    break;
            }
        }

        if (m_CurrentCanvas != m_NextCanvas)
        {
            SetAnchor();
        }
    }

    void SetAnchor()
    {
        //Reset Canvas 
        pauseCanvases[m_CurrentCanvas].anchorMin = new Vector2(m_CurrentAnchor.x, m_CurrentAnchor.y);
        pauseCanvases[m_CurrentCanvas].anchorMax = new Vector2(m_CurrentAnchor.z, m_CurrentAnchor.w);

        //Set Canvas
        m_CurrentCanvas = m_NextCanvas;

        m_CurrentAnchor = new Vector4(
        pauseCanvases[m_NextCanvas].anchorMin.x, pauseCanvases[m_NextCanvas].anchorMin.y,
        pauseCanvases[m_NextCanvas].anchorMax.x, pauseCanvases[m_NextCanvas].anchorMax.y);

        pauseCanvases[m_NextCanvas].anchorMin -= anchorMin;
        pauseCanvases[m_NextCanvas].anchorMax += anchorMax;
    }

    void Resume()
    {
        SceneController.Instance.UnPause(true);
    }

    void Restage()
    {
        SceneController.Instance.UnPause(false);
        SceneController.Instance.Restage();
    }

    void Regame()
    {
        SceneController.Instance.UnPause(false);
        SceneController.Instance.Regame();
    }

    void Setting()
    {
        Debug.Log("Setting");
    }

    void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Qait();
#endif
    }

}
