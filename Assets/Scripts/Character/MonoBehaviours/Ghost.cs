using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    private SpriteRenderer m_SpriteRenderer;
    private float m_EffectDuration;

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetGhost(float duration, SpriteRenderer referenceRenderer, Color effectColor, bool useColor, Transform referenceTransform)
    {
        m_SpriteRenderer.sprite = referenceRenderer.sprite;
        m_SpriteRenderer.sortingLayerID = referenceRenderer.sortingLayerID;
        m_SpriteRenderer.sortingOrder = referenceRenderer.sortingOrder - 1;
        m_SpriteRenderer.flipX = referenceRenderer.flipX;

        m_EffectDuration = duration;
        transform.position = referenceTransform.position;
        transform.rotation = referenceTransform.rotation;

        if (useColor)
        {
            m_SpriteRenderer.material.SetColor("_Color", effectColor);
            m_SpriteRenderer.color = effectColor;
            StartCoroutine(DissapearSprite(useColor, m_SpriteRenderer.color));
        }
        else
        {
            StartCoroutine(DissapearSprite(useColor, m_SpriteRenderer.material.color, effectColor.a));
        }
    }

    IEnumerator DissapearSprite(bool useColor, Color startColor, float startAlpha = 0f)
    {
        bool finishedLerping = false;
        float startLerpTime = Time.time;

        Color transparencyColor = new Color(0, 0, 0, 0);

        while(!finishedLerping)
        {
            float timeSinceLerpStart = Time.time - startLerpTime;
            float percentComplete = timeSinceLerpStart / m_EffectDuration;

            if(1f <= percentComplete)
            {
                finishedLerping = true;

                if (useColor)
                {
                    m_SpriteRenderer.material.SetColor("_Color", transparencyColor);
                }
                else
                {
                    startColor.a = 0;
                    m_SpriteRenderer.color = startColor;
                }
            }

            if (useColor)
            {
                Color newColor = Color.Lerp(startColor, transparencyColor, percentComplete);
                m_SpriteRenderer.material.SetColor("_Color", newColor);
            }
            else
            {
                float newAlpha = Mathf.Lerp(startAlpha, 0, percentComplete);
                startColor.a = newAlpha;
                m_SpriteRenderer.color = startColor;
            }

            yield return null;
        }

        gameObject.SetActive(false);
    }
}
