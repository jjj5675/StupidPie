using UnityEngine;
using UnityEngine.Events;

[SelectionBase]
[RequireComponent(typeof(Rigidbody2D))]
public class LiftPlatform : Platform
{
    public enum LiftPlatformType
    { 
        ONCE, BACK_FORTH/*, LOOP*/
    }

    public PlatformCatcher platformCatcher;
    public float speed = 1.0f;
    public LiftPlatformType liftType;
    public UnityEvent OnEnabled;
    public UnityEvent OnDisabled;

    public Vector3[] localNodes = new Vector3[2];
    public float[] waitTimes = new float[2];

    public Vector3[] worldNode { get { return m_WorldNode; } }
    protected Vector3[] m_WorldNode;

    protected int m_Current = 0;
    protected int m_Next = 0;
    protected int m_Dir = 1;    //노드 진행 방향 

    protected float m_WaitTime = -1.0f;

    protected Rigidbody2D m_Rigidbody2D;
    protected Vector2 m_Velocity;
    protected bool m_EventFired = false;

    public Vector2 Velocity { get { return m_Velocity; } }

    protected const int m_DelayedFrameCount = 2;
    protected int m_ActivationFrameCount = 0;
    protected bool m_PreviousWasPressed = false;

    protected override void Initialise()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Rigidbody2D.isKinematic = true;

        if (platformCatcher == null)
        {
            platformCatcher = GetComponent<PlatformCatcher>();
        }

        m_WorldNode = new Vector3[localNodes.Length];
        for (int i = 0; i < m_WorldNode.Length; i++)
        {
            m_WorldNode[i] = transform.TransformPoint(localNodes[i]);
        }

        m_WorldNode[0] = transform.position;

        m_Current = 0;
        m_Dir = 1;
        m_Next = localNodes.Length > 1 ? 1 : 0;

        m_WaitTime = waitTimes[0];

        if(isMovingAtStart)
        {
            m_Started = true;
        }
        else
        {
            m_Started = false;
        }
    }

    public override void ResetPlatform()
    {
        OnDisabled.Invoke();
        transform.position = m_WorldNode[0];
        m_WaitTime = waitTimes[0];
        m_Current = 0;
        m_Dir = 1;
        m_Next = localNodes.Length > 1 ? 1 : 0;
        m_EventFired = false;
        m_PreviousWasPressed = false;
        if (GetComponent<AudioSource>().isPlaying)
            GetComponent<AudioSource>().Stop();
        m_ActivationFrameCount = 0;
    }

    void FixedUpdate()
    {
        if (!m_Started && !platformCatcher.CaughtInteractionAbility)
        {
            if (m_EventFired)
            {
                if(!m_PreviousWasPressed)
                {
                    m_PreviousWasPressed = true;
                    m_ActivationFrameCount = 1;
                }
                else
                {
                    m_ActivationFrameCount += 1;
                }

                if (m_ActivationFrameCount > m_DelayedFrameCount)
                {
                    OnDisabled.Invoke();
                    m_EventFired = false;
                }
            }

            return;
        }

        //single node라면 이동할 곳이 없으므로
        if(m_Current == m_Next)
        {
            return;
        }

        if(m_WaitTime > 0)
        {
            m_WaitTime -= Time.deltaTime;
            return;
        }

        if(!m_EventFired)
        {
            OnEnabled.Invoke();
            m_EventFired = true;
        }


        float distanceToGo = speed * Time.deltaTime;

        while(distanceToGo > 0)
        {
            Vector2 direction = m_WorldNode[m_Next] - transform.position;
            float dist = distanceToGo;

            //목표점에 도달했을 때
            if(direction.sqrMagnitude < dist * dist)
            {
                //속도를 줄이고 현재 노드와 wait시간을 다음 노드로 설정합니다.
                dist = direction.magnitude;
                m_Current = m_Next;
                m_WaitTime = waitTimes[m_Current];

                if(m_Dir > 0)
                {
                    m_Next += 1;

                    //마지막 노드까지 도달했다면
                    if(m_Next >= m_WorldNode.Length)
                    {
                        switch(liftType)
                        {
                            case LiftPlatformType.BACK_FORTH:
                                //도달한 노드의 이전 노드설정, 노드 방향 설정
                                m_Next -= 2;
                                m_Dir = -1;
                                break;
                            case LiftPlatformType.ONCE:
                                m_Next -= 1;
                                StopMoving();
                                break;
                        }
                    }
                }
                else
                {
                    m_Next -= 1;

                    //처음 위치로 도달 했다면
                    if(m_Next < 0)
                    {
                        switch(liftType)
                        {
                            case LiftPlatformType.BACK_FORTH:
                                m_Next = 1;
                                m_Dir = 1;
                                break;
                            case LiftPlatformType.ONCE:
                                m_Next += 1;
                                StopMoving();
                                break;
                        }
                    }
                }
            }

            m_Velocity = direction.normalized * dist;

            m_Rigidbody2D.MovePosition(m_Rigidbody2D.position + m_Velocity);

            if (0 < m_Velocity.y)
            {
                Vector2 velocity = new Vector2(m_Velocity.x, m_Velocity.y * 0.2f);
                platformCatcher.MoveCaughtObjects(velocity);
            }
            else
            {
                platformCatcher.MoveCaughtObjects(m_Velocity);
            }

            //현재 프레임의 속도 제거
            distanceToGo -= dist;

            //
            if(m_WaitTime > 0.001f)
            {
                break;
            }
        }
    }
}
