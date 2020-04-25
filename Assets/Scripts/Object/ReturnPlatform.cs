using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ReturnPlatform : MonoBehaviour
{
    public PlatformCatcher platformCatcher;
    public float speed = 1.0f;

    public bool isMovingAtStart = true;

    public Vector3 localNode;

    protected Vector3 m_OriginNode;
    protected Vector3 m_WorldNodeDirection;
    protected Vector3 m_OriginallyWorldNodeDirection;

    protected Rigidbody2D m_Rigidbody2D;
    protected Vector2 m_Velocity;

    protected bool m_Started = false;

    public Vector2 Velocity {  get { return m_Velocity; } }



    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Rigidbody2D.isKinematic = true;

        if(platformCatcher == null)
        {
            platformCatcher = GetComponent<PlatformCatcher>();
        }

        m_WorldNodeDirection = transform.TransformPoint(localNode) - transform.position;
        m_OriginallyWorldNodeDirection = m_WorldNodeDirection;

        m_OriginNode = transform.position;
    }

    protected void Initialise()
    {
        if (isMovingAtStart)
        {
            m_Started = true;
        }
        else
        {
            m_Started = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(platformCatcher.CaughtIresCharacter)
        {
            m_Started = true;
        }

        if(!m_Started)
        {
            return;
        }

        float distanceToGo = speed * Time.deltaTime;

        while(distanceToGo > 0)
        {
            Vector2 direction;
            float dist = distanceToGo;

            if (platformCatcher.CaughtIresCharacter)
            {
                direction = m_WorldNodeDirection;
            }
            else
            {
                direction = m_OriginNode - transform.position;

                if(direction.sqrMagnitude < dist * dist)
                {
                    dist = direction.magnitude;
                    StopMoving();
                }
            }

            m_Velocity = direction.normalized * dist;

            m_Rigidbody2D.MovePosition(m_Rigidbody2D.position + m_Velocity);
            platformCatcher.MoveCaughtObjects(m_Velocity);

            distanceToGo -= dist;
        }
    }

    public void StopMoving()
    {
        m_Started = false;
    }
}
