using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Playables;
using System.Text.RegularExpressions;

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
    private Action EndAct;
    private bool canSkip=false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Resume != null)
            {
                Resume.Invoke();
                
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) && canSkip)
        {
            if (EndAct != null)
            {
                EndAct.Invoke();
                DirectorTrigger.instance.OnDirectorFinish.Invoke();
                canSkip = false;
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

        var devide = Regex.Split(mainText.text, "<>");
        string added;
        if (devide.Length > 1)
        {
            added = devide[0] + "\n" + devide[1];
            mainText.text = added;
        }
        

        

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
    IEnumerator OnOffText()
    {
        yield return new WaitForFixedUpdate();
        mainText.fontStyle = FontStyles.Bold;
        
        yield return new WaitForFixedUpdate();
        mainText.enabled = true; ;

        yield break;
    }
    public void SetLine()
    {
        StartCoroutine(OnOffText());
    }

    public void SendResumeAction(Action action)
    {
        Resume = action;
        canSkip = true;
    }
    public void SendEndAction(Action action)
    {
        EndAct = action;
    }
}
