using System.Collections;
using UnityEngine;


public class JumpPad : MonoBehaviour
{
    public Transform target;
    public float timeToPoint;
    public bool useOnlyVerticalMovement;

    private PlatformEffector2D m_PlatformEffector;
    private Vector2 m_TargetPosition;
    private Animator m_Animator;
    private bool m_EventFired;

    public Vector2 targetPosition { get { return m_TargetPosition; } }
    public bool EventFired { get { return m_EventFired; } }

    private readonly int m_HashJumpPadState = Animator.StringToHash("Base Layer.JumpPad");


    void Awake()
    {
        m_PlatformEffector = GetComponent<PlatformEffector2D>();
        m_Animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        m_TargetPosition = target.position;

        if (useOnlyVerticalMovement)
        {
            m_TargetPosition.x = 0;
        }
    }

    public void OnLaunch()
    {
        m_EventFired = true;
        m_Animator.Play(m_HashJumpPadState, -1, 0);

        //if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        //{
        //    m_Animator.SetBool(m_HashPushingPara, false);
        //}

        StartCoroutine(StartLaunch());
    }

    IEnumerator StartLaunch()
    {
        int playerLayerMask = 1 << LayerMask.NameToLayer("Player");
        m_PlatformEffector.colliderMask &= ~playerLayerMask;

        gameObject.layer = LayerMask.NameToLayer("Default");

        yield return new WaitForSeconds(0.5f);

        m_EventFired = false;
        m_PlatformEffector.colliderMask |= playerLayerMask;
        gameObject.layer = LayerMask.NameToLayer("Platform");

    }
}
