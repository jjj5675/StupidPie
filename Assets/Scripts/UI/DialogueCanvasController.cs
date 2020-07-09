using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueCanvasController : MonoBehaviour
{
    public Animator animator;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI mainText;
    
    private List<Phrase> m_GetPhrases = new List<Phrase>();
    private readonly int m_HashActivePara = Animator.StringToHash("Active");
    private Coroutine m_DeactiveCoroutine;

    IEnumerator SetAnimatorParameterWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool(m_HashActivePara, false);
    }

    public void ActivateCanvasWithTranslatedText(string phraseKey)
    {
        m_GetPhrases = DialogueManager.Instance[phraseKey];

        if(m_GetPhrases == null)
        {
            Debug.LogError("Key not found.");
            return;
        }

        if(m_DeactiveCoroutine != null)
        {
            StopCoroutine(m_DeactiveCoroutine);
            m_DeactiveCoroutine = null;
        }

        gameObject.SetActive(true);
        animator.SetBool(m_HashActivePara, true);

        foreach(var phrase in m_GetPhrases)
        {
            print(phrase.name + " : " + phrase.value);
        }
    }

    public void DeactivateCanvasWithDelay(float delay)
    {
        m_DeactiveCoroutine = StartCoroutine(SetAnimatorParameterWithDelay(delay));
    }
}
