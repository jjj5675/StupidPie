using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public enum FadeType
    {
        Black
    }

    private static ScreenFader s_Instance;

    public static ScreenFader Instance
    {
        get
        {
            if (s_Instance != null)
            {
                return s_Instance;
            }

            s_Instance = FindObjectOfType<ScreenFader>();

            if (s_Instance != null)
            {
                return s_Instance;
            }

            Create();

            return s_Instance;
        }
    }

    public static void Create()
    {
        GameObject gameObject = new GameObject("ScreenFader");
        s_Instance = gameObject.AddComponent<ScreenFader>();
    }

    public CanvasGroup blackCanvasGroup;
    public float fadeDuration;

    private bool m_IsFading;

    public static bool IsFading { get { return Instance.m_IsFading; } }

    void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    IEnumerator Fade(float finalAlpha, CanvasGroup canvasGroup)
    {
        Debug.Log("Fade");
        m_IsFading = true;
        canvasGroup.blocksRaycasts = true;
        float fadeSpeed = Mathf.Abs(canvasGroup.alpha - finalAlpha) / fadeDuration;

        while (!Mathf.Approximately(canvasGroup.alpha, finalAlpha))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        m_IsFading = false;
        canvasGroup.alpha = finalAlpha;
        canvasGroup.blocksRaycasts = false;
        Debug.Log(canvasGroup.alpha);
    }

    public static IEnumerator FadeSceneIn()
    {
        
        CanvasGroup canvasGroup = Instance.blackCanvasGroup;
        canvasGroup.gameObject.SetActive(true);
        yield return Instance.StartCoroutine(Instance.Fade(0f, canvasGroup));
        canvasGroup.gameObject.SetActive(false);
    }

    public static IEnumerator FadeSceneOut(FadeType fadeType = FadeType.Black)
    {
        CanvasGroup canvasGroup = Instance.blackCanvasGroup;
        canvasGroup.gameObject.SetActive(true);
        yield return Instance.StartCoroutine(Instance.Fade(1f, canvasGroup));
    }
}
