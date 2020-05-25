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
    protected BoxCollider2D m_Box;
    protected float m_CurrentDuration = 0;
    protected Vector3 m_StartingPosition;
    protected RaycastHit2D[] m_FoundHits = new RaycastHit2D[3];
    protected Vector2 m_RaycastSize;
    protected float m_GroundRaycastDistance;
    protected float m_RaycastDistance;
    protected bool m_IsGrounded;

    protected override void Initialise()
    {
        m_Box = GetComponent<BoxCollider2D>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Rigidbody2D.bodyType = RigidbodyType2D.Static;

        m_GroundRaycastDistance = 0.00001f;
        m_RaycastDistance = m_Box.size.y * 0.5f + m_GroundRaycastDistance * 2f;
        m_RaycastSize = new Vector2(m_Box.size.x * 0.5f, m_RaycastDistance);

        if (platformCatcher == null)
        {
            platformCatcher = GetComponent<PlatformCatcher>();
        }

        m_PlatformType = PlatformType.FALLING;
        m_StartingPosition = transform.position;
    }

    public override void ResetPlatform()
    {
        transform.position = m_StartingPosition;
        m_Rigidbody2D.position = m_StartingPosition;
        m_Rigidbody2D.bodyType = RigidbodyType2D.Static;
        m_Velocity = Vector2.zero;
        m_CurrentDuration = 0;
        m_CanFall = true;
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

            CheckBottomEndCollider();

            float confinerBoundsMinY = CellController.Instance.CurrentCell.ConfinerBounds.min.y;

            if (transform.position.y < confinerBoundsMinY)
            {
                m_CanFall = false;
                m_Velocity = Vector2.zero;
            }
        }
    }

    void CheckBottomEndCollider()
    {
        if (m_Box != null)
        {
            PlayerBehaviour playerBehaviour = null;

            Vector2 raycastStart = m_Rigidbody2D.position + m_Box.offset + Vector2.down * m_Box.size.y * 0.5f;

            int count = Physics2D.BoxCast(raycastStart, m_RaycastSize, 0f, Vector2.down, platformCatcher.contactFilter, m_FoundHits, m_RaycastDistance);

            for (int i = 0; i < count; i++)
            {
                if (m_FoundHits[i].collider != null)
                {
                    float middleHitHeight = m_FoundHits[i].point.y;
                    float colliderHeight = m_Rigidbody2D.position.y + m_Box.offset.y - m_Box.size.y * 0.5f;

                    if (m_Velocity.y <= 0f && middleHitHeight < colliderHeight + m_GroundRaycastDistance)
                    {
                        if (PlayableCharacterFactory.TryGetCollider(PlayerBehaviour.PlayableCharacter.IRES) == m_FoundHits[i].collider)
                        {
                            playerBehaviour = PlayableCharacterFactory.TryGetBehaviour(PlayerBehaviour.PlayableCharacter.IRES);
                        }
                        else if (PlayableCharacterFactory.TryGetCollider(PlayerBehaviour.PlayableCharacter.SERI) == m_FoundHits[i].collider)
                        {
                            playerBehaviour = PlayableCharacterFactory.TryGetBehaviour(PlayerBehaviour.PlayableCharacter.SERI);
                        }

                        if(playerBehaviour != null)
                        {
                            playerBehaviour.damageable.TakeDamage(null, true);
                            m_CanFall = false;
                            return;
                        }

                        float distance = m_FoundHits[i].distance;

                        if (distance < 0.2f)
                        {
                            m_CanFall = false;
                            return;
                        }
                    }
                }
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;

    //    if (m_Rigidbody2D != null)
    //    {
    //        Vector2 raycastStart = m_Rigidbody2D.position + m_Box.offset + Vector2.down * m_Box.size.y * 0.5f;

    //        Gizmos.DrawCube(raycastStart, m_RaycastSize);
    //    }
    //}
}
