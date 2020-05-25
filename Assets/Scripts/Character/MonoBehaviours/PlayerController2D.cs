using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : CharacterController2D
{
    public float sideRaycastDistance;

    protected Vector2 m_BoxOffset;
    protected Collider2D m_ContactCollider;

    public Vector2 BoxOffset { get { return m_BoxOffset; } }
    public Collider2D ContactCollider { get { return m_ContactCollider; } }

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

    public override void CheckBoxHeightCollisions(bool bottom = true)
    {
        base.CheckBoxHeightCollisions(bottom);

        if (m_FoundHitList.Count != 0)
        {
            if (collisionFlags.CheckForHeight() && !collisionFlags.inContactJumppad && PhysicsHelper.ColliderHasJumpPad(m_FoundHitList[m_FirstHitIndex].collider))
            {
                collisionFlags.inContactJumppad = true;
                m_ContactCollider = m_FoundHitList[m_FirstHitIndex].collider;
            }
        }
    }

    public override void CheckBoxWidthCollisions()
    {
        m_ContactCollider = null;
        collisionFlags.inContactJumppad = false;

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

            UpdateRaycasting(m_RaycastPositions, raycastDirection, raycastDistance, m_RaycastPositions.Length);
            Vector2 hitNormal = Vector2.zero;

            if (m_FoundHitList.Count != 0)
            {
                hitNormal = m_FoundHitList[m_FirstHitIndex].normal;
            }

            if (Mathf.Approximately(hitNormal.x, 0) && Mathf.Approximately(hitNormal.y, 0))
            {
                collisionFlags.ResetWidth();
            }
            else
            {
                if (m_Box != null)
                {
                    if (m_FoundHitList[m_FirstHitIndex].collider != null)
                    {
                        float boxWidth = m_Rigidbody2D.position.x + m_BoxOffset.x + (m_Box.size.x * 0.5f * raycastDirection.x);
                        float middleHitWidth = m_FoundHitList[m_FirstHitIndex].point.x;

                        if (faceRight)
                        {
                            bool stuck = middleHitWidth < boxWidth + sideRaycastDistance;
                            collisionFlags.IsRightSide = stuck;
                        }
                        else if (faceLeft)
                        {
                            bool stuck = boxWidth - sideRaycastDistance < middleHitWidth;
                            collisionFlags.IsLeftSide = stuck;
                        }

                        if(collisionFlags.CheckForWidth())
                        {
                            Debug.Log("충돌");
                        }

                        if (collisionFlags.CheckForWidth() && PhysicsHelper.ColliderHasJumpPad(m_FoundHitList[m_FirstHitIndex].collider))
                        {
                            collisionFlags.inContactJumppad = true;
                            m_ContactCollider = m_FoundHitList[m_FirstHitIndex].collider;
                        }
                    }
                }
            }
        }

    }
}
