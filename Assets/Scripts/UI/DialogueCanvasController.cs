using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Playables;

public class DialogueCanvasController : MonoBehaviour
{
    [Serializable]
    public class PortraitAnimator
    {
        public string name;
        public RuntimeAnimatorController runtimeAnimator;
    }

    public Animator canvasAnimator;
    public Animator faceAnimator;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI mainText;

    public List<PortraitAnimator> portraitAnimators;

    private readonly int m_HashActivePara = Animator.StringToHash("Active");

    private Coroutine m_DeactiveCoroutine;
    private Action Resume;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Resume != null)
            {
                Resume.Invoke();
            }
        }
    }

    public void ActivateCanvasWithTranslatedText()
    {
        if (m_DeactiveCoroutine != null)
        {
            StopCoroutine(m_DeactiveCoroutine);
            m_DeactiveCoroutine = null;
        }

        gameObject.SetActive(true);
    }

    IEnumerator SetAnimatorParameterWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canvasAnimator.SetBool(m_HashActivePara, false);
    }

    public void DeactivateCanvasWithDelay(float delay)
    {
        m_DeactiveCoroutine = StartCoroutine(SetAnimatorParameterWithDelay(delay));
    }

    public void Next(string phraseKey, int index)
    {
        if(!DialogueManager.Instance.CheckForTextRead())
        {
            Debug.LogError("Translator의 Read 버튼을 눌러 텍스트를 읽어와야 합니다.");
            return;
        }

        Phrase phrase = DialogueManager.Instance[phraseKey][index];

        nameText.text = phrase.name;
        mainText.text = phrase.value;

        foreach (var portraitAnimator in portraitAnimators)
        {
            if (portraitAnimator.name == nameText.text)
            {
                faceAnimator.runtimeAnimatorController = portraitAnimator.runtimeAnimator;
                faceAnimator.SetTrigger(phrase.portrait);
            }
        }

        //canvasAnimator.SetBool(m_HashActivePara, true);
    }

    public void SendResumeAction(Action action)
    {
        Resume = action;
    }
}
