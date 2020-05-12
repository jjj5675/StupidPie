using UnityEngine;
using Cinemachine;

public class ScreenSystemChange : MonoBehaviour
{
    public enum ScreenType
    {
        SINGLE, SPLIT
    }

    public GameObject singleScreen;
    public GameObject splitScreen;

    public Transform seriCharacter;
    public Transform iresCharacter;

    public float maximumOrthoSize;

    float characterMaximumDistance = 1f;
    ScreenType m_CurrentScreenType;

    CinemachineConfiner[] m_SplitScreenConfiners; 

    void Awake()
    {
        characterMaximumDistance = maximumOrthoSize * 2f * Camera.main.aspect;

        m_SplitScreenConfiners = splitScreen.GetComponentsInChildren<CinemachineConfiner>();
    }

    void OnEnable()
    {
        Vector2 direction = seriCharacter.position - iresCharacter.position;

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
            m_SplitScreenConfiners[i].m_BoundingShape2D = CellController.Instance.CurrentCell.confinerCollider;
        }
    }

    void FixedUpdate()
    {
        Vector2 direction = seriCharacter.position - iresCharacter.position;

        if(direction.sqrMagnitude < characterMaximumDistance * characterMaximumDistance)
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
                m_SplitScreenConfiners[i].m_BoundingShape2D = CellController.Instance.CurrentCell.confinerCollider;
            }

            singleScreen.SetActive(false);
            splitScreen.SetActive(true);
            m_CurrentScreenType = ScreenType.SPLIT;
        }
    }
}
