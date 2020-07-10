﻿using System.Collections;
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

    public enum Portraits
    {
        Portrait1, Portrait2, Portrait3, Portrait4
    }

    public Animator canvasAnimator;
    public Animator faceAnimator;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI mainText;

    public List<PortraitAnimator> portraitAnimators;

    private readonly int m_HashActivePara = Animator.StringToHash("Active");

    private Dictionary<Portraits, int> m_FaceAnimHashParameters = new Dictionary<Portraits, int>();
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

    public void SetHashParameter()
    {
        if (m_FaceAnimHashParameters.Count != 0)
        {
            return;
        }

        m_FaceAnimHashParameters.Add(Portraits.Portrait1, Animator.StringToHash("Portrait1"));
        m_FaceAnimHashParameters.Add(Portraits.Portrait2, Animator.StringToHash("Portrait2"));
        m_FaceAnimHashParameters.Add(Portraits.Portrait3, Animator.StringToHash("Portrait3"));
        m_FaceAnimHashParameters.Add(Portraits.Portrait4, Animator.StringToHash("Portrait4"));
    }

    public void Next(string phraseKey, int index, Portraits portrait)
    {
        if(!DialogueManager.Instance.CheckForTextRead())
        {
            Debug.LogError("Translator의 Read 버튼을 눌러 텍스트를 읽어와야 합니다.");
            return;
        }

        gameObject.SetActive(true);

        Phrase phrase = DialogueManager.Instance[phraseKey][index];

        nameText.text = phrase.name;
        mainText.text = phrase.value;

        foreach (var portraitAnimator in portraitAnimators)
        {
            if (portraitAnimator.name == nameText.text)
            {
                faceAnimator.runtimeAnimatorController = portraitAnimator.runtimeAnimator;
                m_FaceAnimHashParameters.TryGetValue(portrait, out int hashPara);
                faceAnimator.SetTrigger(hashPara);
            }
        }


        canvasAnimator.SetBool(m_HashActivePara, true);
    }

    public void SendResumeAction(Action action)
    {
        Resume = action;
    }
}
