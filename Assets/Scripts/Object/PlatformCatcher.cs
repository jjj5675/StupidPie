using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCatcher : MonoBehaviour
{
    [Serializable]
    public class CaughtObject
    {
        public Rigidbody2D rigidbody;
        public Collider2D collider;
        public CharacterController2D character;
        public PlayerDataBase dataBase;
        public bool inContact;
        public bool checkedThisFrame;

        public void Move(Vector2 movement)
        {
            if (!inContact)
                return;

            if (character != null)
            {
                character.Move(movement);
            }
            else
            {
                rigidbody.MovePosition(rigidbody.position + movement);
            }
        }
    }

    public Rigidbody2D platformRigidbody;
    public Vector2[] normalDirection = new Vector2[4];
    public ContactFilter2D contactFilter;

    protected List<CaughtObject> m_CaughtObjects = new List<CaughtObject>(128);
    protected ContactPoint2D[] m_ContactPoints = new ContactPoint2D[20];
    protected PlatformCatcher m_ParentCatcher;
    protected Collider2D m_Collider;

    protected Action<Vector2> m_MoveDelegate = null;

    private const float epsilon = 0.01f;

    public ContactPoint2D[] ContactPoints { get { return m_ContactPoints; } }

    public int CaughtObjectCount
    {
        get
        {
            int count = 0;

            for (int i = 0; i < m_CaughtObjects.Count; i++)
            {
                if (m_CaughtObjects[i].inContact)
                {
                    count++;
                }
            }

            return count;
        }
    }

    public float CaughtObjcetMass
    {
        get
        {
            float mass = 0f;

            for (int i = 0; i < m_CaughtObjects.Count; i++)
            {
                if (m_CaughtObjects[i].inContact)
                {
                    mass += m_CaughtObjects[i].rigidbody.mass;
                }
            }

            return mass;
        }
    }

    public bool CaughtInteractionAbility
    {
        get
        {
            for (int i = 0; i < m_CaughtObjects.Count; i++)
            {
                if (m_CaughtObjects[i].inContact && m_CaughtObjects[i].dataBase.abilityTypes.Contains(PlayerDataBase.AbilityType.INTERACTION))
                {
                    return true;
                }
            }

            return false;
        }
    }

    void Awake()
    {
        if (platformRigidbody == null)
        {
            platformRigidbody = GetComponent<Rigidbody2D>();
        }

        if (m_Collider == null)
        {
            m_Collider = GetComponent<Collider2D>();

            if(m_Collider == null)
            {
                m_Collider = GetComponentInChildren<Collider2D>();
            }
        }

        for (int i = 0; i < normalDirection.Length; i++)
        {
            normalDirection[i].Normalize();
        }

        m_ParentCatcher = null;
        Transform currentParent = transform.parent;

        while(currentParent != null)
        {
            PlatformCatcher catcher = currentParent.GetComponent<PlatformCatcher>();

            if(catcher!=null)
            {
                m_ParentCatcher = catcher;
            }

            currentParent = currentParent.parent;
        }

        //자신과 부모에게 platformCatcher가 있을 때(ex 무빙플랫폼위의 스위치) 
        //MoveCaughtObjects 함수로 찾은 오브젝트를 움직일때 실제 움직이는건 부모에서 합니다
        //때문에 만약 자신보다 상위객체의 캐처가 있다면 그 부모객체의 대리자에 자식의 값을 설정합니다
        //그렇게 되면 부모의 대리자-> 자식의 함수 호출
        if(m_ParentCatcher != null)
        {
            m_ParentCatcher.m_MoveDelegate += MoveCaughtObjects;
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < m_CaughtObjects.Count; i++)
        {
            CaughtObject caughtObject = m_CaughtObjects[i];
            caughtObject.inContact = false;
            caughtObject.checkedThisFrame = false;
        }

        CheckRigidbodyContacts(platformRigidbody);

        bool checkAgain;

        do
        {
            for (int i = 0, count = m_CaughtObjects.Count; i < count; i++)
            {
                CaughtObject caughtObject = m_CaughtObjects[i];

                if (caughtObject.inContact)
                {
                    if (!caughtObject.checkedThisFrame)
                    {
                        CheckRigidbodyContacts(caughtObject.rigidbody);
                        caughtObject.checkedThisFrame = true;
                    }
                }

                if (!caughtObject.inContact)
                {
                    Collider2D caughtObjectCollider = m_CaughtObjects[i].collider;

                    bool verticalAlignement = (caughtObjectCollider.bounds.max.x > m_Collider.bounds.min.x) && (caughtObjectCollider.bounds.min.x < m_Collider.bounds.max.x);

                    if (verticalAlignement)
                    {
                        float yDiff = m_CaughtObjects[i].collider.bounds.min.y - m_Collider.bounds.max.y;

                        if (yDiff > 0 && yDiff < 0.05f)
                        {
                            caughtObject.inContact = true;
                            caughtObject.checkedThisFrame = true;
                        }
                    }
                }
            }

            checkAgain = false;

            for (int i = 0, count = m_CaughtObjects.Count; i < count; i++)
            {
                CaughtObject caughtObject = m_CaughtObjects[i];

                if (caughtObject.inContact && !caughtObject.checkedThisFrame)
                {
                    checkAgain = true;
                    break;
                }
            }
        }
        while (checkAgain);
    }


    void CheckRigidbodyContacts(Rigidbody2D rb)
    {
        int contactCount = rb.GetContacts(contactFilter, m_ContactPoints);

        for (int j = 0; j < contactCount; j++)
        {
            ContactPoint2D contactPoint2D = m_ContactPoints[j];

            //Collider2D contactCollider = contactPoint2D.collider.gameObject == gameObject ? contactPoint2D.otherCollider : contactPoint2D.collider;

            //if (!OnPlayerCollisionEnter(contactCollider, contactPoint2D))
            //{
            //    return;
            //}

            Rigidbody2D contactRigidbody = contactPoint2D.rigidbody == rb ? contactPoint2D.otherRigidbody : contactPoint2D.rigidbody;
            int listIndex = -1;

            for (int k = 0; k < m_CaughtObjects.Count; k++)
            {
                if (contactRigidbody == m_CaughtObjects[k].rigidbody)
                {
                    listIndex = k;
                    break;
                }
            }

            if (listIndex == -1)
            {
                if (contactRigidbody != null)
                {
                    if (contactRigidbody.bodyType != RigidbodyType2D.Static && contactRigidbody != platformRigidbody)
                    {
                        float dot = 0;

                        for (int i = 0; i < normalDirection.Length; i++)
                        {
                            dot = Vector2.Dot(contactPoint2D.normal, normalDirection[i]);
                            if (dot > 0.8f) break;

                        }

                        if (dot > 0.8f)
                        {
                            CaughtObject newCaughtObject = new CaughtObject
                            {
                                rigidbody = contactRigidbody,
                                character = contactRigidbody.GetComponent<CharacterController2D>(),
                                collider = contactRigidbody.GetComponent<Collider2D>(),
                                dataBase = contactRigidbody.GetComponent<PlayerBehaviour>().dataBase,
                                inContact = true,
                                checkedThisFrame = false
                            };

                            m_CaughtObjects.Add(newCaughtObject);
                        }
                    }
                }
            }
            else
            {
                m_CaughtObjects[listIndex].inContact = true;
            }
        }
    }

    public void MoveCaughtObjects(Vector2 velocity)
    {
        if(m_MoveDelegate != null)
        {
            m_MoveDelegate(velocity);
        }

        for (int i = 0, count = m_CaughtObjects.Count; i < count; i++)
        {

            CaughtObject caughtObject = m_CaughtObjects[i];
            if(m_ParentCatcher != null && m_ParentCatcher.m_CaughtObjects.Find((CaughtObject A) => { return A.rigidbody == caughtObject.rigidbody; }) != null)
            {
                continue;
            }

            m_CaughtObjects[i].Move(velocity);
        }

    }

    public bool IsPlatformCeiling()
    {
        foreach(CaughtObject obj in m_CaughtObjects)
        {
            if (obj.character.collisionFlags.IsCeilinged)
                return true;
        }
        return false;
    }

    //public bool CheckForCornerCollsionEnter2D(Vector2 point, Collider2D other, ContactPoint2D contactPoint2D)
    //{
    //    Collider2D platformCollider = contactPoint2D.collider.gameObject == other.gameObject ? contactPoint2D.otherCollider : contactPoint2D.collider;

    //    Vector2 topRight = platformCollider.bounds.max;
    //    Vector2 topLeft = new Vector2(platformCollider.bounds.min.x, topRight.y);

    //    Vector2 bottomLeft = other.bounds.min;
    //    Vector2 bottomRight = new Vector2(other.bounds.max.x, bottomLeft.y);

    //    Vector2 topRightDifference = new Vector2(point.x - topRight.x, point.y - topRight.y);
    //    Vector2 topLeftDifference = new Vector2(point.x - topLeft.x, point.y - topLeft.y);

    //    Vector2 bottomLeftDifference = new Vector2(point.x - bottomLeft.x, point.y - bottomLeft.y);
    //    Vector2 bottomRightDifference = new Vector2(point.x - bottomRight.x, point.y - bottomRight.y);

    //    bool platformCornerEnter = false;
    //    bool playerCornerEnter = false;

    //    if (epsilon >= Mathf.Abs(topRightDifference.x) && epsilon >= Mathf.Abs(topRightDifference.y))
    //    {
    //        platformCornerEnter = true;
    //    }

    //    if (epsilon >= Mathf.Abs(topLeftDifference.x) && epsilon >= Mathf.Abs(topLeftDifference.y))
    //    {
    //        platformCornerEnter = true;
    //    }

    //    if (epsilon >= Mathf.Abs(bottomLeftDifference.x) && epsilon >= Mathf.Abs(bottomLeftDifference.y))
    //    {
    //        playerCornerEnter = true;
    //    }

    //    if (epsilon >= Mathf.Abs(bottomRightDifference.x) && epsilon >= Mathf.Abs(bottomRightDifference.y))
    //    {
    //        playerCornerEnter = true;
    //    }

    //    if (platformCornerEnter || playerCornerEnter)
    //    {
    //        StartCoroutine(RealQuickIgnoreCollision(other, platformCollider));
    //        return true;
    //    }

    //    return false;
    //}

    //IEnumerator RealQuickIgnoreCollision(Collider2D collider1, Collider2D collider2)
    //{
    //    Debug.Log("잠깐");
    //    Physics2D.IgnoreCollision(collider1, collider2, true);
    //    yield return new WaitForSeconds(0.1f);
    //    Physics2D.IgnoreCollision(collider1, collider2, false);
    //}

    //bool OnPlayerCollisionEnter(Collider2D other, ContactPoint2D contactPoint)
    //{
    //    if (other.gameObject == PlayerCharacter.PlayerInstance.gameObject)
    //    {
    //        var controller = PlayerCharacter.PlayerInstance.gameObject.GetComponent<CharacterController2D>();

    //        if (!controller.collisionFlags.CheckForAllCollisionFlag())
    //        {
    //            if (CheckForCornerCollsionEnter2D(contactPoint.point, other, contactPoint))
    //            {
    //                Debug.Log("코너");
    //            }

    //            return false;
    //        }   //else 중간에 플레이어가 grounded체크하면 발동가능함
    //    }

    //    return true;
    //}
}
