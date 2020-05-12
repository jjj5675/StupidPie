using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FallingPlatform : Platform
{
    public PlatformCatcher platformCatcher;
    public float speed = 1.0f;
    public float maxSpeed = 0;
    public float waitFallingTime;
    public float raycastDistance = 0.1f;

    protected Rigidbody2D m_Rigidbody2D;
    protected bool m_CanFall = true;
    protected Vector2 m_Velocity;
    protected bool m_IsStaticObject = false;
    protected RaycastHit2D[] m_GroundHit = new RaycastHit2D[3];
    protected Vector2[] m_RaycastPositions = new Vector2[3];
    protected Collider2D m_Collider;
    protected float m_CurrentDuration = 0;

    protected override void Initialise()
    {
        m_Collider = GetComponent<Collider2D>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Rigidbody2D.bodyType = RigidbodyType2D.Static;

        if (platformCatcher == null)
        {
            platformCatcher = GetComponent<PlatformCatcher>();
        }

        m_PlatformType = PlatformType.FALLING;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!m_CanFall)
        {
            return;
        }

        if (m_IsStaticObject)
        {
            m_Rigidbody2D.bodyType = RigidbodyType2D.Static;
            m_CurrentDuration = 0;
            m_CanFall = false;
            return;
        }

        if (0 < platformCatcher.CaughtObjectCount)
        {
            m_Rigidbody2D.isKinematic = true;
        }

        if (m_Rigidbody2D.isKinematic)
        {
            if (m_CurrentDuration < waitFallingTime)
            {
                m_CurrentDuration += Time.deltaTime;
            }

            if (waitFallingTime <= m_CurrentDuration)
            {
                float distanceToGo = speed * Time.deltaTime;
                m_Velocity += Vector2.down * distanceToGo * Time.deltaTime;

                m_Rigidbody2D.MovePosition(m_Rigidbody2D.position + m_Velocity);
                platformCatcher.MoveCaughtObjects(m_Velocity);

                ColliderBottomEndCheck();
            }
        }
    }

    public void ColliderBottomEndCheck()
    {
        Vector2 raycastStart = m_Rigidbody2D.position + m_Collider.offset;

        if(m_Collider != null)
        {
            Vector2 raycastStartBottomCenter = raycastStart + Vector2.down * (m_Collider.bounds.size.y * 0.5f);

            m_RaycastPositions[0] = raycastStartBottomCenter + (Vector2.left * m_Collider.bounds.size.x * 0.5f);
            m_RaycastPositions[1] = raycastStartBottomCenter;
            m_RaycastPositions[2] = raycastStartBottomCenter + (Vector2.right * m_Collider.bounds.size.x * 0.5f);

            for (int i = 0; i < m_RaycastPositions.Length; i++)
            {
                int count = Physics2D.Raycast(m_RaycastPositions[i], Vector2.down, platformCatcher.contactFilter, m_GroundHit, raycastDistance);

                if(count != 0)
                {
                    float middleHitHeight = m_GroundHit[0].point.y;
                    float colliderHeight = m_Rigidbody2D.position.y + m_Collider.offset.y - m_Collider.bounds.size.y * 0.5f;

                    m_IsStaticObject = m_Velocity.y <= 0;
                    m_IsStaticObject &= middleHitHeight < colliderHeight + raycastDistance;

                    if(m_IsStaticObject)
                    {
                        break;
                    }
                }
            }
        }
    }
}
