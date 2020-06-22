using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : CharacterController2D
{
    public float sideRaycastDistance;

    //public bool wallTest;

    //protected Vector2 m_BoxOffset;


    //public Vector2 BoxOffset { get { return m_BoxOffset; } }

    //public void SetBoxOffset()
    //{
    //    m_BoxOffset = m_Box.offset;
    //}

    //public void UpdateBoxOffset(float sign)
    //{
    //    if (!Mathf.Approximately(sign, Mathf.Sign(m_BoxOffset.x)))
    //    {
    //        m_BoxOffset.x = Mathf.Abs(m_Box.offset.x) * sign;
    //    }

    //    m_BoxOffset.y = m_Box.offset.y;
    //}

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

    //public void UpdateRaycasting2(Vector2[] positions, Vector2 direction, float distance, int raycastCount)
    //{
    //    m_FoundHitList.Clear();

    //    for(int i=0; i<m_HitBuffer.Length; i++)
    //    {
    //        m_HitBuffer[i] = new RaycastHit2D();
    //    }

    //    float minHitDistance = float.MaxValue;

    //    for (int i = 0; i < raycastCount; i++)
    //    {
    //        int count = Physics2D.Raycast(positions[i], direction, m_ContactFilter2D, m_HitBuffer, distance);
    //        Debug.DrawRay(positions[i], direction * distance, Color.red);

    //        if(wallTest && count == 0)
    //        {
    //            Debug.Log("");
    //        }

    //        if (count > 0)
    //        {
    //            m_FoundHitList.Add(m_HitBuffer[0]);

    //            if (m_HitBuffer[0].distance < minHitDistance)
    //            {
    //                m_FirstHitIndex = m_FoundHitList.Count - 1;
    //                minHitDistance = m_HitBuffer[0].distance;
    //            }
    //        }
    //    }
    //}

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
                if(faceLeft)
                {
                    collisionFlags.IsLeftSide = true;
                }
                else
                {
                    collisionFlags.IsRightSide = true;
                }

                if (collisionFlags.CheckForWidth() && PhysicsHelper.ColliderHasJumpPad(m_FoundHitList[m_FirstHitIndex].collider))
                {
                    collisionFlags.inContactJumppad = true;
                    m_ContactCollider = m_FoundHitList[m_FirstHitIndex].collider;
                }

                //hitNormal = m_FoundHitList[m_FirstHitIndex].normal;
            }

            //if (Mathf.Approximately(hitNormal.x, 0) && Mathf.Approximately(hitNormal.y, 0))
            //{
            //    //리셋
            //    collisionFlags.ResetWidth();
            //}
            //else
            //{
            //    if (m_Box != null)
            //    {
            //        if (m_FoundHitList[m_FirstHitIndex].collider != null)
            //        {
            //            float boxWidth = m_Rigidbody2D.position.x + m_Box.offset.x + (m_Box.size.x * 0.5f * raycastDirection.x);
            //            float middleHitWidth = m_FoundHitList[m_FirstHitIndex].point.x;

            //            if (faceRight)
            //            {
            //                bool stuck = middleHitWidth < boxWidth + sideRaycastDistance;
            //                collisionFlags.IsRightSide = stuck;
            //            }
            //            else if (faceLeft)
            //            {
            //                bool stuck = boxWidth - sideRaycastDistance < middleHitWidth;
            //                collisionFlags.IsLeftSide = stuck;
            //            }

            //            if (collisionFlags.CheckForWidth() && PhysicsHelper.ColliderHasJumpPad(m_FoundHitList[m_FirstHitIndex].collider))
            //            {
            //                collisionFlags.inContactJumppad = true;
            //                m_ContactCollider = m_FoundHitList[m_FirstHitIndex].collider;
            //            }
            //        }
            //    }
            //}
        }

    }
}
