using Cinemachine;
using UnityEngine;

public class AutoCameraSetup : MonoBehaviour
{
    static private AutoCameraSetup s_Instance;
    static public AutoCameraSetup Instance { get { return s_Instance; } }
    public GameObject targetGroup;
    public GameObject subCamera;
    public GameObject mainCamera;

    CinemachineVirtualCamera m_SubVirtualCam;
    CinemachineConfiner m_SubCinemachineConfiner;

    CinemachineVirtualCamera m_MainVirtualCam;
    CinemachineConfiner m_MainCinemachineConfiner;
    CinemachineBrain m_MainCinemachineBrain;
    bool m_IsCellChanging = false;

    static int DELAYDEFRAME_COUNT = 1;
    int m_ActivationFrameCount = 0;


    void Awake()
    {
        s_Instance = this;

        m_MainCinemachineConfiner = GetComponent<CinemachineConfiner>();
        m_MainVirtualCam = GetComponent<CinemachineVirtualCamera>();
        m_MainCinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();

        if (subCamera != null)
        {
            m_SubVirtualCam = subCamera.GetComponent<CinemachineVirtualCamera>();
            m_SubCinemachineConfiner = subCamera.GetComponent<CinemachineConfiner>();
            subCamera.SetActive(false);
        }
        else
        {
            Debug.LogWarning("교체할 카메라가 없습니다.");
        }
    }

    void Start()
    {
        m_MainCinemachineConfiner.m_BoundingShape2D = CellController.Instance.CurrentCell.confinerCollider;
    }

    void LateUpdate()
    {
        if(!m_IsCellChanging)
        {
            return;
        }

        if(m_ActivationFrameCount < DELAYDEFRAME_COUNT)
        {
            m_ActivationFrameCount += 1;
            return;
        }

        if (!m_MainCinemachineBrain.IsBlending)
        {
            m_MainCinemachineConfiner.m_BoundingShape2D = m_SubCinemachineConfiner.m_BoundingShape2D;
            subCamera.SetActive(false);
            CellController.Instance.OnDisabledPreviousCell();
            m_ActivationFrameCount = 0;
            m_IsCellChanging = false;
        }
    }

    public void DisabledScrrenEdges()
    {
        m_MainCinemachineConfiner.m_ConfineScreenEdges = false;
    }
    public void EnabledScrrenEdges()
    {
        m_MainCinemachineConfiner.m_ConfineScreenEdges = true;
    }

    public void SwapVirtualCamera(Collider2D newBound)
    {
        m_SubCinemachineConfiner.m_BoundingShape2D = newBound;
        subCamera.SetActive(true);
        m_IsCellChanging = true;
    }
}
