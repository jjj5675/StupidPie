using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CutsceneDirector : MonoBehaviour
{
    [Serializable]
    public class ClipTable
    {
        [Tooltip("재생할 이미지입니다")]
        public Sprite[] clips;
        [Tooltip("읽어올 텍스트입니다")]
        public TextMeshProUGUI[] readTexts;
        [Tooltip("재생할 클립의 텍스트 수입니다")]
        [Range(0, 10)]
        public int[] textRangeOfClip;
    }

    public ClipTable clipTable;
    [Tooltip("Read Texts의 Text를 Write Text에 작성합니다")]
    public TextMeshProUGUI writeText;

    [Tooltip("시작할 때 Fade 효과를 줍니다")]
    public bool fadeAtStart;
    [Tooltip("Write Text에서 Text를 작성하는 데 걸리는 지연시간입니다")]
    [Range(0f, 1f)]
    public float writeDelay = 0.1f;

    public Image cutSceneImage;

    protected int m_CurrentClip = 0;
    protected int m_CharacterCount;
    protected int m_CurrentTextCount = 0;
    protected bool m_End = false;
    protected bool m_ImageChange = true;

    protected int m_TextRangeIter = 0;
    protected Coroutine m_WriteCoroutine;

    protected bool m_IsTurning = false;
    protected int m_DelayTextCount = 5;

    public bool End { get { return m_End; } }

    public void Play()
    {
        if (!cutSceneImage.enabled)
        {
            cutSceneImage.enabled = true;
        }

        m_WriteCoroutine = StartCoroutine(Write(fadeAtStart, m_CurrentTextCount));
    }
    public void Next()
    {
        if(ScreenFader.IsFading || m_End)
        {
            return;
        }

        if(m_IsTurning)
        {
            return;
        }

        //다음 Text를 가져온다.
        m_CurrentTextCount++;

        //모든 Text를 출력했다면 
        if (clipTable.readTexts.Length <= m_CurrentTextCount)
        {
            m_End = true;
            return;
        }

        m_TextRangeIter++;

        //현재 클립 범위에 해당하는 텍스트 오브젝트를 순회했다면
        if (clipTable.textRangeOfClip[m_CurrentClip] == m_TextRangeIter)
        {
            //다음 클립으로 이동한다.
            m_CurrentClip++;
            m_TextRangeIter = 0;
            m_ImageChange = true;
        }

        //실행중인 코루틴은 종료하고 다시시작한다.
        if (m_WriteCoroutine != null)
        {
            StopCoroutine(m_WriteCoroutine);
        }

        //다음 페이지로 넘긴다.
        m_WriteCoroutine = StartCoroutine(Write(true, m_CurrentTextCount));
    }

    public void Stop()
    {
        if (m_WriteCoroutine != null)
        {
            StopCoroutine(m_WriteCoroutine);
            m_WriteCoroutine = null;
        }
    }

    IEnumerator Write(bool fade, int textIndex)
    {
        m_IsTurning = true;

        if (m_ImageChange && fade)
        {
            yield return ScreenFader.FadeSceneOut(ScreenFader.FadeType.Black);

            // 현재 클립이미지 저장
            cutSceneImage.sprite = clipTable.clips[m_CurrentClip];
            writeText.text = "";
            m_ImageChange = false;

            yield return ScreenFader.FadeSceneIn();
        }

        //Output Text를 클리어하고 Text의 길이를 저장한다.
        writeText.text = "";
        m_CharacterCount = clipTable.readTexts[textIndex].text.Length;

        if (m_CharacterCount == 0)
        {
            m_CharacterCount = clipTable.readTexts[textIndex].text.Length;
        }

        //딜레이 카운트
        for (int i = 0; i < m_DelayTextCount; i++)
        {
            writeText.text += clipTable.readTexts[textIndex].text.Substring(i, 1);
            yield return new WaitForSeconds(writeDelay);
        }

        m_IsTurning = false;

        //텍스트 개수만큼 반복해서 지연시켜가며 한글자씩 출력한다.
        for (int i = m_DelayTextCount; i < m_CharacterCount; i++)
        {
            writeText.text += clipTable.readTexts[textIndex].text.Substring(i, 1);
            yield return new WaitForSeconds(writeDelay);
        }

    }
}
