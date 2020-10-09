using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCollider2D : Platform
{
    private MovingContainer container;
    private Rigidbody2D m_Rigidbody2D;
    private CompositeCollider2D composite;
    private PlatformCatcher catcher;
    public Vector3 moveType;
    protected override void Initialise()
    {
        container = GetComponentInParent<MovingContainer>();
        composite = GetComponent<CompositeCollider2D>();
        catcher = GetComponent<PlatformCatcher>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }
    public override void ResetPlatform()
    {
        container.Reset();
    }

    private void FixedUpdate()
    {
        if (catcher.IsPlatformCeiling())
        {
            Debug.Log("!!");
            container.SetVector(new Vector3(0,1,0));
        }
        CheckBottomEndCollider();
    }
    void CheckBottomEndCollider()
    {

        Vector2 raycastStart = m_Rigidbody2D.position;
        RaycastHit2D m_FoundHits;
        m_FoundHits = Physics2D.BoxCast(raycastStart, composite.bounds.extents, 0f, moveType, moveType.magnitude);

        if (m_FoundHits && m_FoundHits.collider.gameObject.GetComponent<CompositeCollider2D>())
        {
            container.StopMove();
        }

        //    Vector2 raycastStart = m_Rigidbody2D.position  + new Vector2(moveType.x,moveType.y)  * 0.5f;
        //RaycastHit2D[] m_FoundHits = new RaycastHit2D[10];

        //int count = Physics2D.BoxCast(raycastStart, composite.bounds.extents.x, 0f, Vector2.down, catcher.contactFilter, m_FoundHits, moveType.magnitude);

        //    for (int i = 0; i < count; i++)
        //    {
        //        if (m_FoundHits[i].collider != null)
        //        {
        //            float middleHitHeight = m_FoundHits[i].point.y;
        //            float colliderHeight = m_Rigidbody2D.position.y + m_Box.offset.y - m_Box.size.y * 0.5f;

        //            if (m_Velocity.y <= 0f && middleHitHeight < colliderHeight + m_GroundRaycastDistance)
        //            {
        //                if (Publisher.Instance.TryGetObserver(m_FoundHits[i].collider, out Observer observer))
        //                {
        //                    observer.PlayerInfo.damageable.TakeDamage(null);
        //                    m_CanFall = false;
        //                    return;
        //                }

        //                float distance = m_FoundHits[i].distance;

        //                if (distance * 0.6f < speed * Time.deltaTime)
        //                {
        //                    float diffY = raycastStart.y - m_FoundHits[i].point.y;

        //                    m_Rigidbody2D.MovePosition(m_Rigidbody2D.position + (Vector2.down * diffY));

        //                    groundHitAudioPlayer.PlayRandomSound();

        //                    m_CanFall = false;
        //                    return;
        //                }
        //            }
        //        }
        //    }




    }

    private void OnCollisionEnter2D(Collision2D colid)
    {

        if (Publisher.Instance.TryGetObserver(colid.collider, out Observer observer))
        {
            if (observer.PlayerInfo.animator.GetBool("Dashing") || observer.PlayerInfo.animator.GetBool("Grounded"))
            {
                Debug.Log("!!");
                container.SetVector(moveType);
            }
        }
        else
        {
            container.StopMove();
        }
        
        
    }
    
    
}
