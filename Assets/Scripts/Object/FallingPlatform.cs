using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FallingPlatform : Platform
{
    public PlatformCatcher platformCatcher;
    public float speed = 1.0f;
    public float maxSpeed = 0;
    public float waitFallingTime;

    protected Rigidbody2D m_Rigidbody2D;
    protected bool m_CanFall = true;
    protected Vector2 m_Velocity;
    protected Collider2D m_Collider;
    protected float m_CurrentDuration = 0;
    protected Vector3 m_StartingPosition;

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
        m_StartingPosition = transform.position;
    }

    public override void ResetPlatform()
    {
        m_CanFall = true;
        m_Rigidbody2D.bodyType = RigidbodyType2D.Static;
        m_Velocity = Vector2.zero;
        m_CurrentDuration = 0;
        transform.position = m_StartingPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!m_CanFall)
        {
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

                if (0 < platformCatcher.CaughtObjectCount)
                {
                    platformCatcher.MoveCaughtObjects(m_Velocity);
                }
            }

            float confinerBoundsMinY = CellController.Instance.CurrentCell.ConfinerBounds.min.y ;

            if(transform.position.y < confinerBoundsMinY)
            {
                m_CanFall = false;
                m_Velocity = Vector2.zero;
            }
        }
    }
}
