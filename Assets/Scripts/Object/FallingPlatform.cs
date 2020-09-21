using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FallingPlatform : Platform
{
    public PlatformCatcher platformCatcher;
    public float speed = 1.0f;
    public float maxSpeed = 0;
    public float waitFallingTime;
    public RandomAudioPlayer groundHitAudioPlayer;

    protected Rigidbody2D m_Rigidbody2D;
    protected bool m_CanFall = true;
    protected Vector2 m_Velocity;
    protected BoxCollider2D m_Box;
    protected float m_CurrentDuration = 0;
    protected Vector3 m_StartingPosition;
    protected RaycastHit2D[] m_FoundHits = new RaycastHit2D[10];
    protected Vector2 m_RaycastSize;
    protected float m_GroundRaycastDistance;
    protected float m_RaycastDistance;
    protected bool m_IsGrounded;
    protected float confinerBoundsMinY;
    protected RandomAudioPlayer PlayerOn;
    protected RandomAudioPlayer FallDown;

    protected override void Initialise()
    {
        m_Box = GetComponent<BoxCollider2D>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Rigidbody2D.bodyType = RigidbodyType2D.Static;

        m_GroundRaycastDistance = 0.00001f;
        m_RaycastDistance = m_Box.size.y * 0.5f + m_GroundRaycastDistance * 2f;
        m_RaycastSize = new Vector2(m_Box.size.x * 0.9f, m_RaycastDistance);

        PlayerOn = transform.GetChild(0).GetChild(1).GetComponent<RandomAudioPlayer>();
        FallDown = transform.GetChild(0).GetChild(0).GetComponent<RandomAudioPlayer>();

        if (platformCatcher == null)
        {
            platformCatcher = GetComponent<PlatformCatcher>();
        }

        m_PlatformType = PlatformType.FALLING;
        m_StartingPosition = transform.position;

        var rootObj = gameObject.scene.GetRootGameObjects();
        foreach (var go in rootObj)
        {
            if (go.name.Equals("CellController"))
            {
                confinerBoundsMinY = go.GetComponent<CellController>().CurrentCell.ConfinerBounds.min.y;
                break;
            }
        }

        if (confinerBoundsMinY == 0)
        {
            Debug.LogError("셀 바운즈 설정 에러");
        }
    }

    private void OnDisable()
    {
        System.Array.Clear(m_FoundHits, 0, m_FoundHits.Length);
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
            PlayerOn.PlayRandomSound();
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

            if (transform.position.y < confinerBoundsMinY)
            {
                m_CanFall = false;
                m_Velocity = Vector2.zero;
                FallDown.PlayRandomSound();
            }
        }
    }

    void CheckBottomEndCollider()
    {
        if (m_Box != null)
        {
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
                        if(Publisher.Instance.TryGetObserver(m_FoundHits[i].collider, out Observer observer))
                        {
                            observer.PlayerInfo.damageable.TakeDamage(null);
                            m_CanFall = false;
                            return;
                        }

                        float distance = m_FoundHits[i].distance;

                        if (distance * 0.6f < speed * Time.deltaTime)
                        {
                            float diffY = raycastStart.y - m_FoundHits[i].point.y;

                            m_Rigidbody2D.MovePosition(m_Rigidbody2D.position + (Vector2.down * diffY));

                            groundHitAudioPlayer.PlayRandomSound();

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
