using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FallingPlatform : MonoBehaviour
{
    public PlatformCatcher platformCatcher;
    public float speed = 1.0f;
    public float waitFallingTime;

    protected Rigidbody2D m_Rigidbody2D;
    protected bool m_Falling;
    protected Vector2 m_Velocity;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Rigidbody2D.isKinematic = true;

        if (platformCatcher == null)
        {
            platformCatcher = GetComponent<PlatformCatcher>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(0 < platformCatcher.CaughtObjectCount)
        {
            m_Falling = true;
        }

        if(m_Falling)
        {
            float distanceToGo = speed * Time.deltaTime;
            m_Velocity = Vector2.down * distanceToGo;

            m_Rigidbody2D.MovePosition(m_Rigidbody2D.position + m_Velocity);
            platformCatcher.MoveCaughtObjects(m_Velocity);
        }
    }
}
