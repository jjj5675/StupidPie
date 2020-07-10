using UnityEngine;

public class Singleton<T> : MonoBehaviour
    where T : MonoBehaviour
{
    private static T s_Instance;

    private static object m_lock = new object();

    private static bool m_ApplicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if(m_ApplicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' 는 이미 파괴되었습니다.");
                return null;
            }

            lock(m_lock)
            {
                if (s_Instance == null)
                {
                    s_Instance = (T)FindObjectOfType(typeof(T));

                    if(FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                            "' 는 하나만 존재할 수 있습니다.");
                        return s_Instance;
                    }

                    if(s_Instance == null)
                    {
                        GameObject singleton = new GameObject();
                        s_Instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();

                        Debug.Log("[Singleton] Instance '" + typeof(T) +
                            "' 가 생성되었습니다.");
                    }
                }

                return s_Instance;
            }
        }
    }

    private static bool IsDontDestroyOnLoad()
    {
        if(s_Instance == null)
        {
            return false;
        }

        if((s_Instance.gameObject.hideFlags & HideFlags.DontSave) == HideFlags.DontSave)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 스크립트가 삭제 된 후 인스턴스를 호출하면
    /// 에디터 씬에 그대로 오브젝트가 생성되어 존재하는 것을 방지
    /// </summary>

    public void OnDestroy()
    {
        if(IsDontDestroyOnLoad())
        {
            m_ApplicationIsQuitting = true;
        }
    }
}
