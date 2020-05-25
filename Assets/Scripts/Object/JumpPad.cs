using System.Collections;
using UnityEngine;

public class JumpPad : Platform
{
    public Transform target;
    public float timeToPoint;
    public bool useOnlyVertically;

    private Vector2 m_TargetPosition;
    private Animator m_Animator;
    private bool m_EventFired;
    private Collider2D padCollider;

    public Vector2 targetPosition { get { return m_TargetPosition; } }
    public bool EventFired { get { return m_EventFired; } }

    private readonly int m_HashJumpPadState = Animator.StringToHash("Base Layer.BouncePad");
    //private readonly int m_HashBouncingEndPara = Animator.StringToHash("BouncingEnd");


    void Awake()
    {
        padCollider = GetComponent<Collider2D>();
        m_Animator = GetComponentInChildren<Animator>();
    }

    protected override void Initialise()
    {
        m_TargetPosition = target.position;

        if (useOnlyVertically)
        {
            m_TargetPosition.x = 0;
        }

        m_PlatformType = PlatformType.JUMPING;
    }

    public override void ResetPlatform()
    {
    }

    public void OnLaunch(Collider2D collider)
    {
        m_EventFired = true;

        m_Animator.Play(m_HashJumpPadState, -1, 0);
        
        //if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        //{
        //    m_Animator.SetBool(m_HashBouncingEndPara, true);
        //}

        StartCoroutine(StartLaunch(collider));
    }

    IEnumerator StartLaunch(Collider2D collider)
    {
        Physics2D.IgnoreCollision(collider, padCollider, true);

        yield return new WaitForSeconds(0.5f);

        Physics2D.IgnoreCollision(collider, padCollider, false);
        m_EventFired = false;
    }
}
