using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : CharacterController2D
{
    public float sideRaycastDistance;

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

        bool faceRight = m_Velocity.x > 0;
        bool faceLeft = m_Velocity.x < 0;

        Vector2 raycastDirection = Vector2.zero;
        Vector2 raycastStart;
        float raycastDistance;

        if (m_Box != null)
        {
            raycastStart = m_Rigidbody2D.position + m_Box.offset;
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

            UpdateRaycasting(m_RaycastPositions, raycastDirection, raycastDistance, m_RaycastPositions.Length, true);
            Vector2 hitNormal = Vector2.zero;

            collisionFlags.ResetWidth();
            collisionFlags.inContactJumppad = false;

            if (m_FoundHitList.Count != 0)
            {
                //if(faceLeft)
                //{
                //    collisionFlags.IsLeftSide = true;
                //}
                //else
                //{
                //    collisionFlags.IsRightSide = true;
                //}

                //if (collisionFlags.CheckForWidth() && PhysicsHelper.ColliderHasJumpPad(m_FoundHitList[m_FirstHitIndex].collider))
                //{
                //    collisionFlags.inContactJumppad = true;
                //    m_ContactCollider = m_FoundHitList[m_FirstHitIndex].collider;
                //}

                hitNormal = m_FoundHitList[m_FirstHitIndex].normal;
            }

            if (Mathf.Approximately(hitNormal.x, 0) && Mathf.Approximately(hitNormal.y, 0))
            {
                //리셋
                collisionFlags.ResetWidth();
            }
            else
            {
                if (m_Box != null)
                {
                    if (m_FoundHitList[m_FirstHitIndex].collider != null)
                    {
                        float boxWidth = m_Rigidbody2D.position.x + m_Box.offset.x + (m_Box.size.x * 0.5f * raycastDirection.x);
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
