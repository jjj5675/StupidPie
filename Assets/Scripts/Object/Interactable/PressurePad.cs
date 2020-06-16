using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlatformCatcher))]
public class PressurePad : MonoBehaviour
{
    public enum ActivationType
    {
        ItemCount, itemMass
    }

    public ActivationType activationType;
    public int requiredCount;
    public float requiredMass;
    public UnityEvent OnPressed;
    public UnityEvent OnReleased;

    protected bool m_EventFired;
    protected Animator m_Animator;
    protected PlatformCatcher m_PlatformCatcher;
    protected SpriteRenderer m_SpriteRenderer;
    protected float m_Speed = 5f;

    protected readonly int m_HashPressedPara = Animator.StringToHash("Pressed");
    protected readonly int m_HashReleasedPara = Animator.StringToHash("Released");

    protected const int m_DelayedFrameCount = 2;
    protected int m_ActivationFrameCount = 0;
    protected bool m_PreviousWasPressed = false;

    public bool EventFired { get { return m_EventFired; } }

    private void Awake()
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_PlatformCatcher = GetComponent<PlatformCatcher>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (activationType == ActivationType.ItemCount)
        {
            if (requiredCount <= m_PlatformCatcher.CaughtObjectCount)
            {
                if (!m_PreviousWasPressed)
                {
                    m_PreviousWasPressed = true;
                    m_ActivationFrameCount = 1;
                }
                else
                {
                    m_ActivationFrameCount += 1;
                }

                if (m_ActivationFrameCount > m_DelayedFrameCount && !m_EventFired)
                {
                    m_Animator.SetTrigger(m_HashPressedPara);
                    m_EventFired = true;
                    OnPressed.Invoke();
                }
            }
            else
            {
                if(m_PreviousWasPressed)
                {
                    m_PreviousWasPressed = false;
                    m_ActivationFrameCount = 1;
                }
                else
                {
                    m_ActivationFrameCount += 1;
                }

                if (m_ActivationFrameCount > m_DelayedFrameCount && m_EventFired)
                {
                    m_Animator.SetTrigger(m_HashReleasedPara);
                    m_EventFired = false;
                    OnReleased.Invoke();
                }
            }
        }
        else
        {
            if (requiredMass <= m_PlatformCatcher.CaughtObjcetMass)
            {
                if (requiredCount <= m_PlatformCatcher.CaughtObjectCount)
                {
                    if (!m_PreviousWasPressed)
                    {
                        m_PreviousWasPressed = true;
                        m_ActivationFrameCount = 1;
                    }
                    else
                    {
                        m_ActivationFrameCount += 1;
                    }

                    if (m_ActivationFrameCount > m_DelayedFrameCount && !m_EventFired)
                    {
                        m_Animator.SetTrigger(m_HashPressedPara);
                        m_EventFired = true;
                        OnPressed.Invoke();
                    }
                }
                else
                {
                    if (m_PreviousWasPressed)
                    {
                        m_PreviousWasPressed = false;
                        m_ActivationFrameCount = 1;
                    }
                    else
                    {
                        m_ActivationFrameCount += 1;
                    }

                    if (m_ActivationFrameCount > m_DelayedFrameCount && m_EventFired)
                    {
                        m_Animator.SetTrigger(m_HashReleasedPara);
                        m_EventFired = false;
                        OnReleased.Invoke();
                    }
                }
            }
        }

        if (m_EventFired && m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.99f)
        {
            m_PlatformCatcher.MoveCaughtObjects(Vector2.down * m_Speed * Time.deltaTime);
        }
    }
}
