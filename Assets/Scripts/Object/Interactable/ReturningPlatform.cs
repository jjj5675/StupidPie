using UnityEngine;

public class ReturningPlatform : Platform
{
    public PlatformCatcher platformCatcher;
    public float speed = 1.0f;

    public Vector3 localNode;

    protected Vector3 m_OriginNode;
    protected Vector3 m_WorldNodeDirection;
    protected Vector3 m_OriginallyWorldNodeDirection;

    protected Rigidbody2D m_Rigidbody2D;
    protected Vector2 m_Velocity;

    protected bool m_Returnable;

    public Vector2 Velocity { get { return m_Velocity; } }

    protected override void Initialise()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Rigidbody2D.isKinematic = true;

        if (platformCatcher == null)
        {
            platformCatcher = GetComponent<PlatformCatcher>();
        }

        m_WorldNodeDirection = transform.TransformPoint(localNode) - transform.position;
        m_OriginallyWorldNodeDirection = m_WorldNodeDirection;

        m_OriginNode = transform.position;

        if (isMovingAtStart)
        {
            m_Started = true;
        }
        else
        {
            m_Started = false;
        }

        m_Returnable = false;
    }

    public override void ResetPlatform()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_Returnable && !m_Started && !platformCatcher.CaughtInteractionAbility)
        {
            float distanceToGo = speed * Time.deltaTime;

            while (distanceToGo > 0)
            {
                Vector2 direction = m_OriginNode - transform.position;
                float squaredDirection = direction.sqrMagnitude;
                float dist = distanceToGo;

                if (squaredDirection < dist * dist)
                {
                    m_Returnable = false;

                    if(Mathf.Approximately(squaredDirection,0))
                    {
                        return;
                    }

                    dist = direction.magnitude;
                }

                m_Velocity = direction.normalized * dist;
                m_Rigidbody2D.MovePosition(m_Rigidbody2D.position + m_Velocity);
                distanceToGo -= dist;
            }

            return;
        }
        else if (m_Started || platformCatcher.CaughtInteractionAbility)
        {
            m_Returnable = true;
            m_Velocity = m_WorldNodeDirection.normalized * speed * Time.deltaTime;

            m_Rigidbody2D.MovePosition(m_Rigidbody2D.position + m_Velocity);
            platformCatcher.MoveCaughtObjects(m_Velocity);
        }
    }
}
