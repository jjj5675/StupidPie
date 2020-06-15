using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class AutoCameraSetup : MonoBehaviour
{
    public GameObject subvcam;
    public GameObject mainCamera;
    public CinemachineVirtualCamera m_MainVCam;
    public CinemachineVirtualCamera m_SubVCam;
    public UnityEvent OnDisabledPreviousCell;

    CinemachineConfiner m_SubCinemachineConfiner;
    CinemachineConfiner m_MainCinemachineConfiner;
    CinemachineBrain m_MainCinemachineBrain;
    bool m_IsCellChanging = false;

    const int m_DelayFrameCount = 1;
    int m_ActivationFrameCount = 0;


    void Awake()
    {
        m_MainCinemachineConfiner = GetComponent<CinemachineConfiner>();
        m_MainCinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();

        if (subvcam != null)
        {
            m_SubCinemachineConfiner = subvcam.GetComponent<CinemachineConfiner>();
            subvcam.SetActive(false);
        }
        else
        {
            Debug.LogWarning("교체할 카메라가 없습니다.");
        }
    }

    void LateUpdate()
    {
        if (!m_IsCellChanging)
        {
            return;
        }

        if (m_ActivationFrameCount < m_DelayFrameCount)
        {
            m_ActivationFrameCount += 1;
            return;
        }

        if (!m_MainCinemachineBrain.IsBlending)
        {
            m_MainCinemachineConfiner.m_BoundingShape2D = m_SubCinemachineConfiner.m_BoundingShape2D;
            m_MainCinemachineConfiner.InvalidatePathCache();
            subvcam.SetActive(false);
            OnDisabledPreviousCell.Invoke();
            m_ActivationFrameCount = 0;
            m_IsCellChanging = false;
        }
    }

    public void SetVCamFollow(Transform transform)
    {
        m_MainVCam.Follow = transform;
        m_SubVCam.Follow = transform;
    }

    public void DisabledScreenEdges()
    {
        m_MainCinemachineConfiner.m_ConfineScreenEdges = false;
    }
    public void EnabledScreenEdges()
    {
        m_MainCinemachineConfiner.m_ConfineScreenEdges = true;
    }

    public void SetVCam(Collider2D newBound, bool swapVCam = false)
    {
        if (swapVCam)
        {
            subvcam.SetActive(true);
            m_IsCellChanging = true;
            m_SubCinemachineConfiner.m_BoundingShape2D = newBound;
            m_SubCinemachineConfiner.InvalidatePathCache();
        }
        else
        {
            m_MainCinemachineConfiner.m_BoundingShape2D = newBound;
            m_MainCinemachineConfiner.InvalidatePathCache();
        }
    }
}
