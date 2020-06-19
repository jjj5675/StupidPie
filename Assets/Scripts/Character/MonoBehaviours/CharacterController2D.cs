using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class CharacterController2D : MonoBehaviour
{
    public LayerMask groundedLayerMask;
    public float groundedRaycastDistance;
    public CollisionFlags collisionFlags;

    protected Vector2 m_PreviousPosition;
    protected Vector2 m_CurrentPostion;
    protected Vector2 m_NextMovement;
    protected Vector2 m_Velocity;
    protected Rigidbody2D m_Rigidbody2D;
    protected BoxCollider2D m_Box;
    protected ContactFilter2D m_ContactFilter2D;
    protected RaycastHit2D[] m_HitBuffer = new RaycastHit2D[3];
    protected int m_FirstHitIndex;
    protected Vector2[] m_RaycastPositions = new Vector2[3];
    protected List<RaycastHit2D> m_FoundHitList = new List<RaycastHit2D>(3);
    protected Collider2D m_ContactCollider;
    public Collider2D ContactCollider { get { return m_ContactCollider; } }
    public Rigidbody2D Rigidbody2D { get { return m_Rigidbody2D; } }
    public Vector2 Velocity { get { return m_Velocity; } }

    void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Box = GetComponent<BoxCollider2D>();

        m_CurrentPostion = m_Rigidbody2D.position;
        m_PreviousPosition = m_Rigidbody2D.position;

        m_ContactFilter2D.layerMask = groundedLayerMask;
        m_ContactFilter2D.useLayerMask = true;
        m_ContactFilter2D.useTriggers = false;

        Physics2D.queriesStartInColliders = false;
    }

    void FixedUpdate()
    {
        m_PreviousPosition = m_Rigidbody2D.position;
        m_CurrentPostion = m_PreviousPosition + m_NextMovement;
        m_Velocity = (m_CurrentPostion - m_PreviousPosition) / Time.deltaTime;

        m_Rigidbody2D.MovePosition(m_CurrentPostion);

        m_NextMovement = Vector2.zero;

        CheckBoxWidthCollisions();
        CheckBoxHeightCollisions();
        CheckBoxHeightCollisions(false);
    }

    public void Teleport(Vector2 position)
    {
        Vector2 delta = position - m_CurrentPostion;
        m_PreviousPosition += delta;
        m_CurrentPostion = position;
        m_Rigidbody2D.MovePosition(position);
    }

    public void Move(Vector2 movement)
    {
        m_NextMovement += movement;
    }


    public void UpdateRaycasting(Vector2[] positions, Vector2 direction, float distance, int raycastCount)
    {
        m_FoundHitList.Clear();
        float minHitDistance = float.MaxValue;

        for (int i = 0; i < raycastCount; i++)
        {
            int count = Physics2D.Raycast(positions[i], direction, m_ContactFilter2D, m_HitBuffer, distance);
            Debug.DrawRay(positions[i], direction * distance, Color.red);

            if (count > 0)
            {
                m_FoundHitList.Add(m_HitBuffer[0]);

                if (m_HitBuffer[0].distance < minHitDistance)
                {
                    m_FirstHitIndex = m_FoundHitList.Count - 1;
                    minHitDistance = m_HitBuffer[0].distance;
                }
            }
        }
    }

    public virtual void CheckBoxHeightCollisions(bool bottom = true)
    {
        Vector2 raycastDirection = Vector2.zero;
        Vector2 raycastStart;
        float raycastDistance = 0;

        if (m_Box != null)
        {
            raycastStart = m_Rigidbody2D.position + m_Box.offset;
            raycastDistance = m_Box.size.x * 0.5f + groundedRaycastDistance * 2f;

            if (bottom)
            {
                raycastDirection = Vector2.down;
                Vector2 raycastStartBottomCenter = raycastStart + Vector2.down * (m_Box.size.y * 0.5f);

                m_RaycastPositions[0] = raycastStartBottomCenter + (Vector2.left * m_Box.size.x * 0.5f);
                m_RaycastPositions[1] = raycastStartBottomCenter;
                m_RaycastPositions[2] = raycastStartBottomCenter + (Vector2.right * m_Box.size.x * 0.5f);
            }
            else
            {
                raycastDirection = Vector2.up;
                Vector2 raycastStartBottomCenter = raycastStart + Vector2.up * (m_Box.size.y * 0.5f);

                m_RaycastPositions[0] = raycastStartBottomCenter + (Vector2.left * m_Box.size.x * 0.5f);
                m_RaycastPositions[1] = raycastStartBottomCenter;
                m_RaycastPositions[2] = raycastStartBottomCenter + (Vector2.right * m_Box.size.x * 0.5f);
            }
        }

        UpdateRaycasting(m_RaycastPositions, raycastDirection, raycastDistance, m_RaycastPositions.Length);
        Vector2 hitNormal = Vector2.zero;

        if (m_FoundHitList.Count != 0)
            hitNormal = m_FoundHitList[m_FirstHitIndex].normal;

        if (Mathf.Approximately(hitNormal.x, 0) && Mathf.Approximately(hitNormal.y, 0))
        {
            collisionFlags.ResetHeight(bottom);
        }
        else
        {
            if (m_Box != null)
            {
                if (m_FoundHitList[m_FirstHitIndex].collider != null)
                {
                    float middleHitHeight = m_FoundHitList[m_FirstHitIndex].point.y;
                    float colliderHeight = m_Rigidbody2D.position.y + m_Box.offset.y;
                    colliderHeight = bottom ? colliderHeight - m_Box.size.y * 0.5f : colliderHeight + m_Box.size.y * 0.5f;

                    if (bottom)
                    {
                        collisionFlags.IsGrounded = m_Velocity.y <= 0;
                        collisionFlags.IsGrounded &= middleHitHeight < colliderHeight + groundedRaycastDistance;
                    }
                    else
                    {
                        collisionFlags.IsCeilinged = colliderHeight + groundedRaycastDistance < middleHitHeight;
                    }
                }
            }
        }
    }

    abstract public void CheckBoxWidthCollisions();

    public struct CollisionFlags
    {
        public bool IsLeftSide, IsRightSide;
        public bool IsGrounded, IsCeilinged;
        public bool inContactJumppad;

        public void ResetWidth()
        {
            IsLeftSide = IsRightSide = false;
        }

        public void ResetHeight(bool bottom)
        {
            if (bottom)
                IsGrounded = false;
            else
                IsCeilinged = false;
        }

        public bool CheckForWidth() { return IsLeftSide || IsRightSide; }
        public bool CheckForHeight() { return IsGrounded || IsCeilinged; }
    }
}
