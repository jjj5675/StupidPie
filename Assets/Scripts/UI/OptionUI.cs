using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class OptionUI : MonoBehaviour
{

    [Serializable]
    public class ImageTable
    {
        public RectTransform[] buttonImages;
        [HideInInspector]
        public Dictionary<Image, SpriteRenderer[]> switchImagesDict;
        public bool isChangeSize;
    }

    public ImageTable[] imageTable;
    //캔버스 개수
    public GameObject[] canvases;

    public Vector2 sizeDelta;

    //테이블 개수, 테이블의 이미지 갯수 (딕셔너리 키값)
    protected List<List<Image>> m_ButtonImages;

    //테이블 인덱스
    protected int m_CurrentTable = 0;

    //이미지 인덱스
    protected int m_CurrentImage = 0;
    protected int m_NextImage = 0;

    protected int m_ReturnImage = 0;

    protected Vector2 m_CurrentSize;
    protected Vector2 m_CurrentPosition;

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
        m_CurrentImage = 0;
        m_NextImage = 0;

        if (imageTable[m_CurrentTable].isChangeSize)
        {
            //되돌릴때를 위해 처음 이미지의 Anchor값 저장 
            m_CurrentSize = imageTable[m_CurrentTable].buttonImages[m_CurrentImage].sizeDelta;
            m_CurrentPosition = imageTable[m_CurrentTable].buttonImages[m_CurrentImage].anchoredPosition;

            //현재 이미지의 Anchor 값 변경
            imageTable[m_CurrentTable].buttonImages[m_NextImage].sizeDelta += sizeDelta;
            imageTable[m_CurrentTable].buttonImages[m_NextImage].anchoredPosition += new Vector2(-(sizeDelta.x * 0.5f), 0);
        }

        //이미지 체인지 (활성화 ON)
        imageTable[m_CurrentTable].switchImagesDict.TryGetValue(m_ButtonImages[m_CurrentTable][m_CurrentImage], out SpriteRenderer[] valuse);
        m_ButtonImages[m_CurrentTable][m_CurrentImage].color = valuse[m_ONImageIndex].color;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (m_CurrentImage != 0)
            {
                if (m_CurrentTable == 0 && m_CurrentImage == 4)
                {
                    m_NextImage -= 2;
                }
                else
                {
                    m_NextImage--;
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            //현재 테이블 버튼갯수만큼
            if (m_CurrentImage < imageTable[m_CurrentTable].buttonImages.Length - 1)
            {
                if (m_CurrentTable == 0 && m_CurrentImage == 2)
                {
                    m_NextImage += 2;
                }
                else
                {
                    m_NextImage++;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            // pause Canvas
            if (m_CurrentTable == 0)
            {
                switch (m_CurrentImage)
                {
                    case 0:
                        Resume();
                        break;
                    case 1:
                        Restage();
                        break;
                    case 2:
                        NextUIActivation(true);
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
                switch (m_CurrentImage)
                {
                    case 0:
                        Regame();
                        break;
                    default:
                        NextUIActivation(false);
                        break;
                }

            }
        }


        if (m_CurrentImage != m_NextImage)
        {
            SetAnchor();
        }
    }

    void SetAnchor()
    {
        //현재 이미지 변경 (비활성화 Off)
        imageTable[m_CurrentTable].switchImagesDict.TryGetValue(m_ButtonImages[m_CurrentTable][m_CurrentImage], out SpriteRenderer[] valuse);
        m_ButtonImages[m_CurrentTable][m_CurrentImage].color = valuse[m_OffImageIndex].color;

        //현재 이미지 변경 (활성화 On)
        imageTable[m_CurrentTable].switchImagesDict.TryGetValue(m_ButtonImages[m_CurrentTable][m_NextImage], out valuse);
        m_ButtonImages[m_CurrentTable][m_NextImage].color = valuse[m_ONImageIndex].color;

        //이미지 크기 변경을 하겠다면 
        if (imageTable[m_CurrentTable].isChangeSize)
        {
            //현재 테이블중 현재이미지의 Anchor 값 초기화(축소)
            imageTable[m_CurrentTable].buttonImages[m_CurrentImage].sizeDelta = m_CurrentSize;
            imageTable[m_CurrentTable].buttonImages[m_CurrentImage].anchoredPosition = m_CurrentPosition;

            //현재 테이블중 다음 이미지의 Anchor 값 저장
            m_CurrentSize = imageTable[m_CurrentTable].buttonImages[m_NextImage].sizeDelta;
            m_CurrentPosition = imageTable[m_CurrentTable].buttonImages[m_NextImage].anchoredPosition;

            //현재 테이블중 다음이미지의 Anchor값 변경 (확대)
            imageTable[m_CurrentTable].buttonImages[m_NextImage].sizeDelta += sizeDelta;
            imageTable[m_CurrentTable].buttonImages[m_NextImage].anchoredPosition += new Vector2(-(sizeDelta.x * 0.5f), 0);
        }

        //다음 이미지로 변경
        m_CurrentImage = m_NextImage;
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

    void NextUIActivation(bool nextActivate)
    {
        if (nextActivate)
        {
            if (m_CurrentTable + 1 < canvases.Length)
            {
                m_ReturnImage = m_CurrentImage;

                //현재 이미지 off
                imageTable[m_CurrentTable].switchImagesDict.TryGetValue(m_ButtonImages[m_CurrentTable][m_CurrentImage],
                    out SpriteRenderer[] valuese);
                m_ButtonImages[m_CurrentTable][m_CurrentImage].color = valuese[m_OffImageIndex].color;

                //현재 테이블 비활성
                canvases[m_CurrentTable].SetActive(false);

                m_CurrentTable++;

                //다음 테이블 활성
                canvases[m_CurrentTable].SetActive(true);

                m_CurrentImage = 0;
                m_NextImage = 0;

                //바뀐 테이블 이미지 활성화
                imageTable[m_CurrentTable].switchImagesDict.TryGetValue(m_ButtonImages[m_CurrentTable][m_CurrentImage],
    out valuese);
                m_ButtonImages[m_CurrentTable][m_CurrentImage].color = valuese[m_ONImageIndex].color;

            }
        }
        else
        {
            if (0 <= m_CurrentTable - 1)
            {
                //바뀐 테이블 이미지 활성화
                imageTable[m_CurrentTable].switchImagesDict.TryGetValue(m_ButtonImages[m_CurrentTable][m_CurrentImage],
    out SpriteRenderer[] valuese);
                m_ButtonImages[m_CurrentTable][m_CurrentImage].color = valuese[m_OffImageIndex].color;

                //현재 테이블 활성
                canvases[m_CurrentTable].SetActive(false);

                m_CurrentTable--;

                //다음 테이블 비활성
                canvases[m_CurrentTable].SetActive(true);

                m_CurrentImage = m_ReturnImage;
                m_NextImage = m_ReturnImage;

                //돌아갈 이미지 On
                imageTable[m_CurrentTable].switchImagesDict.TryGetValue(m_ButtonImages[m_CurrentTable][m_ReturnImage],
                    out valuese);
                m_ButtonImages[m_CurrentTable][m_ReturnImage].color = valuese[m_ONImageIndex].color;
            }
        }
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
        Application.Quit();
#endif
    }

}