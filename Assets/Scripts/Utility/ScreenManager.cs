using UnityEngine;
using Cinemachine;

public class ScreenManager : MonoBehaviour
{
    public enum ScreenType
    {
        SINGLE, SPLIT
    }

    public GameObject singleScreen;
    public GameObject splitScreen;

    public AutoCameraSetup autoCameraSetup;
    public GameObject targetGroup;

    public Transform character1;
    public Transform character2;
    public CellController cellController;

    public float maximumOrthoSize;

    float characterMaximumDistance = 1f;
    ScreenType m_CurrentScreenType;
    bool m_CanChange = true;
    CinemachineConfiner[] m_SplitScreenConfiners;

    void Awake()
    {
        characterMaximumDistance = maximumOrthoSize * 2f * Camera.main.aspect;
        m_SplitScreenConfiners = splitScreen.GetComponentsInChildren<CinemachineConfiner>();
        
    }

    void OnEnable()
    {
        int characterCount = 0;
        var rootObj = gameObject.scene.GetRootGameObjects();
        Transform follow = null;
        foreach(var go in rootObj)
        {
            if(go.GetComponent<PlayerBehaviour>() != null && go.activeSelf)
            {
                follow = go.transform;
                characterCount++;
            }
        }

        if(characterCount < 2)
        {
            m_CanChange = false;
            autoCameraSetup.SetVCamFollow(follow);
        }
        else
        {
            m_CanChange = true;
            autoCameraSetup.SetVCamFollow(targetGroup.transform);
        }

        Vector2 direction = character1.position - character2.position;

        if (direction.sqrMagnitude < characterMaximumDistance * characterMaximumDistance)
        {
            m_CurrentScreenType = ScreenType.SINGLE;
            singleScreen.SetActive(true);
        }
        else
        {
            m_CurrentScreenType = ScreenType.SPLIT;
            splitScreen.SetActive(true);
        }
    }

    void Start()
    {
        for (int i = 0; i < m_SplitScreenConfiners.Length; i++)
        {
            m_SplitScreenConfiners[i].m_BoundingShape2D = cellController.CurrentCell.confinerCollider;
        }
    }

    void FixedUpdate()
    {
        if(!m_CanChange)
        {
            return;
        }

        Vector2 direction = character1.position - character2.position;

        if (direction.sqrMagnitude < characterMaximumDistance * characterMaximumDistance)
        {
            if(m_CurrentScreenType == ScreenType.SINGLE)
            {
                return;
            }

            splitScreen.SetActive(false);
            singleScreen.SetActive(true);
            m_CurrentScreenType = ScreenType.SINGLE;
        }
        else
        {
            if (m_CurrentScreenType == ScreenType.SPLIT)
            {
                return;
            }

            for(int i=0; i<m_SplitScreenConfiners.Length; i++)
            {
                m_SplitScreenConfiners[i].m_BoundingShape2D = cellController.CurrentCell.confinerCollider;
            }

            singleScreen.SetActive(false);
            splitScreen.SetActive(true);
            m_CurrentScreenType = ScreenType.SPLIT;
        }
    }
}
