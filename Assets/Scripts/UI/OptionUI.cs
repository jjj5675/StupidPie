using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class OptionUI : MonoBehaviour
{

    [Serializable]
    public class ImageTable
    {
        public RectTransform[] buttonImages;
        public Dictionary<Image, SpriteRenderer[]> switchImagesDict;
        public bool isChangeSize;
    }

    public ImageTable[] imageTable;

    public Vector2 anchorMin;
    public Vector2 anchorMax;

    //테이블 개수, 테이블의 이미지 갯수 (딕셔너리 키값)
    protected List<List<Image>> m_ButtonImages;

    //테이블 인덱스
    protected int m_CurrentTable = 0;

    //이미지 인덱스
    protected int m_CurrentImages = 0;
    protected int m_NextImages = 0;

    protected Vector4 m_CurrentAnchor;

    //off
    private readonly int m_OffImageIndex = 0;
    private readonly int m_ONImageIndex = 1;


    private void Awake()
    {
        m_ButtonImages = new List<List<Image>>(imageTable.Length);

        //이미지 메모리 할당
        for (int i = 0; i < imageTable.Length; i++)
        {
            //메모리 할당
            imageTable[i].switchImagesDict = new Dictionary<Image, SpriteRenderer[]>();


            for (int k = 0; k < imageTable[i].buttonImages.Length; k++)
            {
                imageTable[i].switchImagesDict.Add(imageTable[i].buttonImages[k].GetComponent<Image>(),
                    imageTable[i].buttonImages[k].GetComponentsInChildren<SpriteRenderer>(true));
            }

            m_ButtonImages.Add(new List<Image>(imageTable[i].switchImagesDict.Keys));
        }
    }

    void Start()
    {
        SetButtonImage();
    }

    void SetButtonImage()
    {
        m_CurrentAnchor = new Vector4(
imageTable[m_CurrentTable].buttonImages[m_CurrentImages].anchorMin.x, imageTable[m_CurrentTable].buttonImages[m_CurrentImages].anchorMin.y,
imageTable[m_CurrentTable].buttonImages[m_CurrentImages].anchorMax.x, imageTable[m_CurrentTable].buttonImages[m_CurrentImages].anchorMax.y);

        imageTable[m_CurrentTable].buttonImages[m_NextImages].anchorMin -= anchorMin;
        imageTable[m_CurrentTable].buttonImages[m_NextImages].anchorMax += anchorMax;

        imageTable[m_CurrentTable].switchImagesDict.TryGetValue(m_ButtonImages[m_CurrentTable][m_CurrentImages], out SpriteRenderer[] valuse);
        m_ButtonImages[m_CurrentTable][m_CurrentImages].color = valuse[m_ONImageIndex].color;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (m_CurrentImages != 0)
            {
                m_NextImages--;

                //if (m_CurrentCanvas == 4)
                //{
                //    m_NextCanvas -= 2;
                //}
                //else
                //{
                //    m_NextCanvas--;
                //}
            }
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            //현재 테이블 버튼갯수만큼
            if (m_CurrentImages < imageTable[m_CurrentTable].buttonImages.Length - 1)
            {
                m_NextImages++;

                //if (m_CurrentCanvas == 2)
                //{
                //    m_NextCanvas += 2;
                //}
                //else
                //{
                //    m_NextCanvas++;
                //}
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            // pause Canvas
            if (m_CurrentTable == 0)
            {
                switch (m_CurrentImages)
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
            else
            {
                // regame Canvas
            }
        }


        //이미지 변경하겠다면
        if (imageTable[m_CurrentTable].isChangeSize)
        {
            if (m_CurrentImages != m_NextImages)
            {
                //SetAnchor();
            }
        }
    }

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
//#if UNITY_EDITOR
//        EditorApplication.isPlaying = false;
//#else
//        Application.Qait();
//#endif
    }

}