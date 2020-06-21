using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

public class OptionUI : MonoBehaviour
{

    [Serializable]
    public class CanvasTable
    {
        public RectTransform[] buttonCanvases;
        public Dictionary<Image, Image[]> imageDict;
        public bool isChangeSize; 
    }

    public CanvasTable[] canvasTable;

    public Vector2 anchorMin;
    public Vector2 anchorMax;

    protected List<List<Image>> m_ButtonImages; 
    protected int m_CurrentTable = 0;
    protected int m_CurrentCanvas = 0;
    protected int m_NextCanvas = 0;
    protected Vector4 m_CurrentAnchor;

    private void Awake()
    {
        //이미지 메모리 할당
        m_ButtonImages = new List<List<Image>>(canvasTable.Length);

        for (int i=0; i<canvasTable.Length; i++)
        {
            //메모리 할당
            canvasTable[i].imageDict = new Dictionary<Image, Image[]>();


           for (int k=0; k<canvasTable[i].buttonCanvases.Length; k++)
            {
                Image selfImage = canvasTable[i].buttonCanvases[k].GetComponentInChildren<Image>(true);

                Image[] childImages = selfImage.GetComponentsInChildren<Image>(true);

                ExcludeSelf<Image>(selfImage, ref childImages);


                canvasTable[i].imageDict.Add(canvasTable[i].buttonCanvases[k].GetComponentInChildren<Image>(true),
                    canvasTable[i].buttonCanvases[k].GetComponentInChildren<Image>(true).GetComponentsInChildren<Image>(true));
            }

            m_ButtonImages.Add(new List<Image>(canvasTable[i].imageDict.Keys));
        }
    }

    void ExcludeSelf<T>(T self, ref T[] childs)
        where T : Component
    {
        for(int i=0; i<childs.Length; i++)
        {
            if(self == childs[i])
            {
                Array.IndexOf(childs, i);
            }
        }
    }

    void Start()
    {
        m_CurrentAnchor = new Vector4(
     canvasTable[m_CurrentTable].buttonCanvases[m_CurrentCanvas].anchorMin.x, canvasTable[m_CurrentTable].buttonCanvases[m_CurrentCanvas].anchorMin.y,
     canvasTable[m_CurrentTable].buttonCanvases[m_CurrentCanvas].anchorMax.x, canvasTable[m_CurrentTable].buttonCanvases[m_CurrentCanvas].anchorMax.y);

        canvasTable[m_CurrentTable].buttonCanvases[m_NextCanvas].anchorMin -= anchorMin;
       canvasTable[m_CurrentTable].buttonCanvases[m_NextCanvas].anchorMax += anchorMax;

        canvasTable[m_CurrentTable].imageDict.TryGetValue(m_ButtonImages[m_CurrentTable][m_CurrentCanvas], out Image[] values);
        m_ButtonImages[m_CurrentTable][m_CurrentCanvas].color = values[2].color;
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.UpArrow))
    //    {
    //        if (m_CurrentCanvas != 0)
    //        {
    //            if (m_CurrentCanvas == 4)
    //            {
    //                m_NextCanvas -= 2;
    //            }
    //            else
    //            {
    //                m_NextCanvas--;
    //            }
    //        }
    //    }
    //    else if (Input.GetKeyUp(KeyCode.DownArrow))
    //    {
    //        if (m_CurrentCanvas < pauseCanvases.Length - 1)
    //        {
    //            if (m_CurrentCanvas == 2)
    //            {
    //                m_NextCanvas += 2;
    //            }
    //            else
    //            {
    //                m_NextCanvas++;
    //            }
    //        }
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Return))
    //    {
    //        switch (m_CurrentCanvas)
    //        {
    //            case 0:
    //                Resume();
    //                break;
    //            case 1:
    //                Restage();
    //                break;
    //            case 2:
    //                Regame();
    //                break;
    //            case 3:
    //                Setting();
    //                break;
    //            default:
    //                Exit();
    //                break;
    //        }
    //    }

    //    if (m_CurrentCanvas != m_NextCanvas)
    //    {
    //        SetAnchor();
    //    }
    //}

    //void SetAnchor()
    //{
    //    //Reset Canvas 
    //    pauseCanvases[m_CurrentCanvas].anchorMin = new Vector2(m_CurrentAnchor.x, m_CurrentAnchor.y);
    //    pauseCanvases[m_CurrentCanvas].anchorMax = new Vector2(m_CurrentAnchor.z, m_CurrentAnchor.w);

    //    //Set Canvas
    //    m_CurrentCanvas = m_NextCanvas;

    //    m_CurrentAnchor = new Vector4(
    //    pauseCanvases[m_NextCanvas].anchorMin.x, pauseCanvases[m_NextCanvas].anchorMin.y,
    //    pauseCanvases[m_NextCanvas].anchorMax.x, pauseCanvases[m_NextCanvas].anchorMax.y);

    //    pauseCanvases[m_NextCanvas].anchorMin -= anchorMin;
    //    pauseCanvases[m_NextCanvas].anchorMax += anchorMax;
    //}

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
