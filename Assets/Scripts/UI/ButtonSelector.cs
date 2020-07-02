using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonSelector : MonoBehaviour
{
    [Serializable]
    public class ImageTable
    {
        public RectTransform[] buttonImages;

        public UnityEvent[] OnPressed;

        [HideInInspector]
        public Dictionary<Image, SpriteRenderer[]> switchImagesDict;
        public Vector2 sizeDelta;
        public bool isChangeSize;
    }

    public readonly struct SelectedButton
    {
        public SelectedButton(int canvasIndex, int imageIndex)
        {
            this.canvasIndex = canvasIndex;
            this.imageIndex = imageIndex;
        }

        public int canvasIndex { get; }
        public int imageIndex { get; }
    }

    public ImageTable[] imageTable;
    //캔버스 개수
    public List<Canvas> canvases;

    //테이블 개수, 테이블의 이미지 갯수 (딕셔너리 키값)
    protected List<List<Image>> m_ButtonImages;

    //테이블 인덱스
    protected int m_CurrentCanvas = 0;

    //이미지 인덱스
    protected int m_CurrentImage = 0;
    protected int m_NextImage = 0;

    protected Stack<SelectedButton> m_StackOfButton = new Stack<SelectedButton>();

    protected Vector2 m_CurrentDeltaSize;
    protected Vector2 m_CurrentAnchoredPosition;

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
        SetImageSizeAndPosition(false);
        ChangeImage(m_ONImageIndex);
    }


    private void Update()
    {
        //입력을 하거나 못하는 조건 추가

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (m_CurrentImage != 0)
            {
                if (m_CurrentCanvas == 0 && m_CurrentImage == 4)
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
            if (m_CurrentImage < imageTable[m_CurrentCanvas].buttonImages.Length - 1)
            {
                if (m_CurrentCanvas == 0 && m_CurrentImage == 2)
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
            imageTable[m_CurrentCanvas].OnPressed[m_CurrentImage].Invoke();
        }

        if (m_CurrentImage != m_NextImage)
        {
            ChangeImage(m_OffImageIndex);
            SetImageSizeAndPosition(true);

            //다음 이미지로 변경
            m_CurrentImage = m_NextImage;

            SetImageSizeAndPosition(false);
            ChangeImage(m_ONImageIndex);
        }
    }

    void SetImageSizeAndPosition(bool resetImage)
    {
        if (imageTable[m_CurrentCanvas].isChangeSize)
        {
            if (resetImage)
            {
                //현재 테이블중 현재이미지의 Anchor 값 초기화(축소)
                imageTable[m_CurrentCanvas].buttonImages[m_CurrentImage].sizeDelta = m_CurrentDeltaSize;
                imageTable[m_CurrentCanvas].buttonImages[m_CurrentImage].anchoredPosition = m_CurrentAnchoredPosition;
            }
            else
            {
                //되돌릴때를 위해 처음 이미지의 Anchor값 저장 
                m_CurrentDeltaSize = imageTable[m_CurrentCanvas].buttonImages[m_CurrentImage].sizeDelta;
                m_CurrentAnchoredPosition = imageTable[m_CurrentCanvas].buttonImages[m_CurrentImage].anchoredPosition;

                //현재 이미지의 Anchor 값 변경
                imageTable[m_CurrentCanvas].buttonImages[m_CurrentImage].sizeDelta += imageTable[m_CurrentCanvas].sizeDelta;
                imageTable[m_CurrentCanvas].buttonImages[m_CurrentImage].anchoredPosition += new Vector2(-(imageTable[m_CurrentCanvas].sizeDelta.x * 0.5f), 0);
            }
        }
    }

    void ChangeImage(int index)
    {
        imageTable[m_CurrentCanvas].switchImagesDict.TryGetValue(m_ButtonImages[m_CurrentCanvas][m_CurrentImage], out SpriteRenderer[] valuese);
        m_ButtonImages[m_CurrentCanvas][m_CurrentImage].sprite = valuese[index].sprite;
    }


    public void Move(Canvas canvas)
    {
        if (canvases.Contains(canvas))
        {
            //현재 이미지 off
            ChangeImage(m_OffImageIndex);

            //이미지 사이즈 변경
            SetImageSizeAndPosition(true);

            //현재 캔버스 비활성화
            canvases[m_CurrentCanvas].gameObject.SetActive(false);

            //이동할 캔버스의 인덱스를 구한다.
            int index = canvases.IndexOf(canvas);

            // 이전 캔버스와 이동할 캔버스가 같으면 이전 캔버스로 돌아간다.
            if(m_StackOfButton.Count != 0 && m_StackOfButton.Peek().canvasIndex == index)
            {
                var prevButton = m_StackOfButton.Pop();

                m_CurrentCanvas = prevButton.canvasIndex;

                m_CurrentImage = prevButton.imageIndex;
                m_NextImage = m_CurrentImage;
            }
            else
            {
                SelectedButton selectedButton = new SelectedButton(m_CurrentCanvas, m_CurrentImage);

                m_StackOfButton.Push(selectedButton);
                m_CurrentCanvas = index;

                m_CurrentImage = 0;
                m_NextImage = 0;
            }

            //이동한 캔버스 활성화
            canvases[m_CurrentCanvas].gameObject.SetActive(true);

            SetImageSizeAndPosition(false);

            //처음 이미지 활성화
            ChangeImage(m_ONImageIndex);
        }
    }
}
