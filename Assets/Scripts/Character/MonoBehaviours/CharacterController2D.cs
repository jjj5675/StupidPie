using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class CharacterController2D : MonoBehaviour
{
    public LayerMask groundedLayerMask;
    public float groundedRaycastDistance;
    public float sideRaycastDistance;
    public CollisionFlags collisionFlags;

    Vector2 m_PreviousPosition;
    Vector2 m_CurrentPostion;
    Vector2 m_NextMovement;
    Vector2 m_Velocity;
    Rigidbody2D m_Rigidbody2D;
    BoxCollider2D m_Box;
    Vector2 m_BoxOffset;
    Vector2 m_SideNormal;
    Vector2 m_GroundNormal;
    Collider2D m_ContactCollider;
    ContactFilter2D m_ContactFilter2D;
    RaycastHit2D[] m_HitBuffer = new RaycastHit2D[3];
    int m_ProximateHitIndex;
    Vector2[] m_RaycastPositions = new Vector2[3];
    List<RaycastHit2D> m_FoundHitList = new List<RaycastHit2D>(3);

    public Rigidbody2D Rigidbody2D { get { return m_Rigidbody2D; } }
    public Vector2 BoxOffset { get { return m_BoxOffset; } }
    public Vector2 Velocity { get { return m_Velocity; } }
    public Vector2 GroundNormal { get { return m_GroundNormal; } }
    public Vector2 SideNormal { get { return m_SideNormal; } }
    public Collider2D ContactCollider { get { return m_ContactCollider; } }

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
        ResetJumpPadObject();

        m_PreviousPosition = m_Rigidbody2D.position;
        m_CurrentPostion = m_PreviousPosition + m_NextMovement;
        m_Velocity = (m_CurrentPostion - m_PreviousPosition) / Time.deltaTime;

        m_Rigidbody2D.MovePosition(m_CurrentPostion);

        m_NextMovement = Vector2.zero;

        CheckHorizontalCollisions();

        CheckVerticalCollisions();
        CheckVerticalCollisions(false);
    }

    void ResetJumpPadObject()
    {
        m_ContactCollider = null;
        collisionFlags.inContactJumppad = false;
    }

    public void SetBoxOffset()
    {
        m_BoxOffset = m_Box.offset;
    }

    public void UpdateBoxOffset(float sign)
    {
        if (!Mathf.Approximately(sign, Mathf.Sign(m_BoxOffset.x)))
        {
            m_BoxOffset.x = Mathf.Abs(m_Box.offset.x) * sign;
        }

        m_BoxOffset.y = m_Box.offset.y;
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

    public void ZeroMovement()
    {
        m_NextMovement = Vector2.zero;
    }

    public void CheckHorizontalCollisions()
    {
        bool faceRight = m_Velocity.x > 0 ? true : false;
        bool faceLeft = m_Velocity.x < 0 ? true : false;

        Vector2 raycastDirection = Vector2.zero;
        Vector2 raycastStart;
        float raycastDistance = 0;

        if (m_Box != null)
        {
            raycastStart = m_Rigidbody2D.position + m_BoxOffset;
            raycastDistance = m_Box.size.x * 0.5f + sideRaycastDistance * 2f;

            if (faceRight)
            {
                raycastDirection = Vector2.right;
                Vector2 raycastStartRightCenter = raycastStart + Vector2.right * (m_Box.size.x * 0.5f);

                m_RaycastPositions[0] = raycastStartRightCenter + Vector2.up * (m_Box.size.y * 0.5f);
                m_RaycastPositions[1] = raycastStartRightCenter;
                m_RaycastPositions[2] = raycastStartRightCenter + Vector2.down * (m_Box.size.y * 0.5f);
            }
            else if (faceLeft)
            {
                raycastDirection = Vector2.left;
                Vector2 raycastStartLeftCenter = raycastStart + Vector2.left * (m_Box.size.x * 0.5f);

                m_RaycastPositions[0] = raycastStartLeftCenter + Vector2.up * (m_Box.size.y * 0.5f);
                m_RaycastPositions[1] = raycastStartLeftCenter;
                m_RaycastPositions[2] = raycastStartLeftCenter + Vector2.down * (m_Box.size.y * 0.5f);
            }

            UpdateRaycast(m_RaycastPositions, raycastDirection, raycastDistance, m_RaycastPositions.Length);

            Vector2 hitNormal;
            CalculateNormal(out hitNormal);


            if (Mathf.Approximately(hitNormal.x, 0) && Mathf.Approximately(hitNormal.y, 0))
            {
                collisionFlags.ResetHorizontalFlag();
            }
            else
            {
                if (m_Box != null)
                {
                    if (m_FoundHitList[m_ProximateHitIndex].collider != null)
                    {
                        float capsuleSideWidth = m_Rigidbody2D.position.x + m_BoxOffset.x + (m_Box.size.x * 0.5f * raycastDirection.x);
                        float middleHitWidth = m_FoundHitList[m_ProximateHitIndex].point.x;

                        if (faceRight)
                        {
                            bool stuck = middleHitWidth < capsuleSideWidth + sideRaycastDistance;
                            collisionFlags.IsRightSide = stuck;
                        }
                        else if (faceLeft)
                        {
                            bool stuck = capsuleSideWidth - sideRaycastDistance < middleHitWidth;
                            collisionFlags.IsLeftSide = stuck;
                        }

                        if (collisionFlags.CheckForHorizontal() && PhysicsHelper.ColliderHasJumpPad(m_FoundHitList[m_ProximateHitIndex].collider))
                        {
                            collisionFlags.inContactJumppad = true;
                            m_ContactCollider = m_FoundHitList[m_ProximateHitIndex].collider;
                        }
                    }
                }
            }
        }
    }

    public void CheckVerticalCollisions(bool bottom = true)
    {
        Vector2 raycastDirection = Vector2.zero;
        Vector2 raycastStart;
        float raycastDistance = 0;

        if (m_Box != null)
        {
            raycastStart = m_Rigidbody2D.position + m_BoxOffset;
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

        UpdateRaycast(m_RaycastPositions, raycastDirection, raycastDistance, m_RaycastPositions.Length);

        Vector2 groundNormal;
        CalculateNormal(out groundNormal);

        if (Mathf.Approximately(groundNormal.x, 0) && Mathf.Approximately(groundNormal.y, 0))
        {
            if (bottom)
            {
                collisionFlags.IsGrounded = false;
            }
            else
            {
                collisionFlags.IsCeilinged = false;
            }
        }
        else
        {
            if (m_Box != null)
            {
                if (m_FoundHitList[m_ProximateHitIndex].collider != null)
                {
                    float middleHitHeight = m_FoundHitList[m_ProximateHitIndex].point.y;

                    if (bottom)
                    {
                        float capsuleBottomHeight = m_Rigidbody2D.position.y + m_BoxOffset.y - m_Box.size.y * 0.5f;
                        collisionFlags.IsGrounded = m_Velocity.y <= 0;
                        collisionFlags.IsGrounded &= middleHitHeight < capsuleBottomHeight + groundedRaycastDistance;
                    }
                    else
                    {
                        float capsuleTopHeight = m_Rigidbody2D.position.y + m_BoxOffset.y + m_Box.size.y * 0.5f;
                        collisionFlags.IsCeilinged = capsuleTopHeight + groundedRaycastDistance < middleHitHeight;
                    }

                    if (collisionFlags.CheckForVertical() && !collisionFlags.inContactJumppad && PhysicsHelper.ColliderHasJumpPad(m_FoundHitList[m_ProximateHitIndex].collider))
                    {
                        collisionFlags.inContactJumppad = true;
                        m_ContactCollider = m_FoundHitList[m_ProximateHitIndex].collider;
                    }
                }
            }
        }
    }

    public void UpdateRaycast(Vector2[] positions, Vector2 direction, float distance, int raycastCount)
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
                    m_ProximateHitIndex = m_FoundHitList.Count - 1;
                    minHitDistance = m_HitBuffer[0].distance;
                }
            }
        }
    }

    public void CalculateNormal(out Vector2 normal)
    {
        normal = Vector2.zero;
        int hitCount = m_FoundHitList.Count;

        for (int i = 0; i < hitCount; i++)
        {
            if (m_FoundHitList[i].collider != null)
            {
                normal += m_FoundHitList[i].normal;
            }
        }

        if (hitCount != 0)
        {
            normal.Normalize();
        }
    }

    public struct CollisionFlags
    {
        public bool IsLeftSide, IsRightSide;
        public bool IsGrounded, IsCeilinged;
        public bool inContactJumppad;

        public void ResetHorizontalFlag()
        {
            IsLeftSide = IsRightSide = false;
        }

        public bool CheckForHorizontal() { return IsLeftSide || IsRightSide ? true : false; }
        public bool CheckForVertical() { return IsGrounded || IsCeilinged ? true : false; }
        public bool CheckForAllCollisionFlag()
        {
            return CheckForHorizontal() || IsGrounded || IsCeilinged;
        }
    }
}
