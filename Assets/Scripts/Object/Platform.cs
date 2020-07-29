using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.Tilemaps;

public abstract class Platform : MonoBehaviour
{
    public enum PlatformType
    {
        NONE, LIGHTING, MOVING, RETURN, OBSTACLE_TRIGGER, FALLING, JUMPING, SWITCH, SPIKE_TRIGGER
    }

    protected enum TriggerState
    {
        ENTER, EXIT
    }

    public bool isMovingAtStart = true;

    protected PlatformType m_PlatformType;
    protected TriggerState m_CurrentTriggerState;
    protected bool m_Started = false;

    // Start is called before the first frame update
    void Start()
    {
        Initialise();
    }

    public abstract void ResetPlatform();
    protected abstract void Initialise();

    protected void SearchOverlapPlatforms<TComponent>(Collider2D rootCollider, out TComponent[] copyArray, int resultCount, bool addRootCollider = true)
        where TComponent : Platform
    {
        Queue<Collider2D> queue = new Queue<Collider2D>(resultCount);
        List<TComponent> searchList = new List<TComponent>(resultCount);
        Collider2D[] colliderResults = new Collider2D[resultCount];
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = true;

        queue.Enqueue(rootCollider);

        if (addRootCollider)
        {
            searchList.Add(rootCollider.GetComponent<TComponent>());
        }

        while (queue.Count != 0)
        {
            Collider2D startCollider = queue.Dequeue();
            int count = startCollider.OverlapCollider(contactFilter, colliderResults);

            for (int i = 0; i < count; i++)
            {
                TComponent component = colliderResults[i].GetComponent<TComponent>();

                if (component != null)
                {
                    if (!searchList.Contains(component))
                    {
                        queue.Enqueue(colliderResults[i]);
                        searchList.Add(component);
                    }
                }
            }
        }

        copyArray = searchList.ToArray();
    }

    protected void SearchOverlapCharacter(Collider2D collider, ContactFilter2D contactFilter2D, int ressultCount)
    {
        collider.enabled = true;
        Collider2D[] colliderResults = new Collider2D[ressultCount];

        int count = collider.OverlapCollider(contactFilter2D, colliderResults);

        for (int i = 0; i < count; i++)
        {
            Damageable damageable = colliderResults[i].GetComponent<PlayerBehaviour>().dataBase.damageable;

            if (damageable != null)
            {
                damageable.TakeDamage(null);
                break;
            }
        }

        collider.enabled = false;
    }

    protected void ChangeAllTiles(Tilemap tilemap, Sprite sprite)
    {
        if (tilemap == null)
        {
            Debug.LogWarning("타일맵이 설정되지 않았습니다.");
            return;
        }

        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = sprite;

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);

            if (tilemap.HasTile(localPlace))
            {
                tilemap.SetTile(localPlace, tile);
            }
        }

        tilemap.RefreshAllTiles();
    }

    public virtual void StartMoving()
    {
        m_Started = true;
    }

    public virtual void StopMoving()
    {
        m_Started = false;
    }

    /// <summary>
    /// ///////
    /// </summary>

    public Vector2 liftSpeed;

    protected List<StaticMover> m_StaticMovers = new List<StaticMover>();

    private Vector2 m_MovementCounter;

    public virtual void MoveVExact(int move) { }

    public void MoveStaticMovers(Vector2 amount)
    {
        foreach (StaticMover staticMover in m_StaticMovers)
        {
            staticMover.Move(amount);
        }
    }

    public bool Intersects(Bounds a, Tilemap b)
    {
        return (a.min.x < b.transform.position.x + b.size.x) && (b.transform.position.x < a.max.x)
            && (a.min.y < b.transform.position.y + b.size.y) && (b.transform.position.y < a.max.y);
    }

    public bool Collide(Collider2D self, Collider2D other)
    {
        //if(other is BoxCollider2D)
        //{
        //    return Collide(other as BoxCollider2D);
        //}
        if (other is CompositeCollider2D)
        {
            return Collide(self, other.GetComponent<TilemapCollider2D>());
        }
        //if(other is CircleCollider2D)
        //{
        //    return Collide(other as CircleCollider2D);
        //}
        throw new System.Exception("해당 충돌체 타입에 맞는 충돌은 구현되지 않습니다.");
    }

    //bool Collide(BoxCollider2D box)
    //{
    //    return true;
    //}
    bool Collide(Collider2D a, TilemapCollider2D b)
    {
        Tilemap tilemap = b.GetComponent<Tilemap>();

        if (Intersects(a.bounds, tilemap))
        {
            int num = (int)((a.bounds.min.x - tilemap.transform.position.x) / tilemap.size.x);
            int num2 = (int)((a.bounds.max.y - tilemap.transform.position.y) / tilemap.size.y);
            int width = (int)((a.bounds.max.x - tilemap.transform.position.x - 1f) / tilemap.size.x) - num + 1;
            int height = (int)((a.bounds.min.y - tilemap.transform.position.y - 1f) / tilemap.size.y) - num2 + 1;
            return CheckRect(num, num2, width, height, tilemap);
        }

        return false;
    }

    bool CheckRect(int x, int y, int width, int height, Tilemap tilemap)
    {
        if (x < 0)
        {
            width += x;
            x = 0;
        }
        if (y < 0)
        {
            height += y;
            y = 0;
        }
        if (x + width > tilemap.size.x)
        {
            width = tilemap.size.x - x;
        }
        if (y + height > tilemap.size.y)
        {
            height = tilemap.size.y - y;
        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tilemap.HasTile(new Vector3Int(x + i, y + j, 0)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    //bool Collide(CircleCollider2D circle)
    //{
    //    return true;
    //}

    public Collider2D First(Collider2D a, IEnumerable<Collider2D> b)
    {
        foreach (var entity in b)
        {
            if (a != null && entity != null && (a != entity && entity.enabled) && Collide(a, entity))
            {
                return entity;
            }
        }
        return null;
    }

    public Collider2D CollideFirst(Collider2D a, IEnumerable<Collider2D> b, Vector2 at)
    {
        Vector2 position = a.transform.position;
        a.transform.position = at;
        Collider2D result = First(a, b);
        a.transform.position = position;
        return result;
    }


    public bool MoveVCollideSolids(float moveV, Collider2D self, ContactFilter2D contactFilter)
    {
        if (Time.deltaTime == 0f)
        {
            liftSpeed.y = 0f;
        }
        else
        {
            liftSpeed.y = moveV / Time.deltaTime;
        }

        m_MovementCounter.y = m_MovementCounter.y + moveV;
        int num = Mathf.RoundToInt(m_MovementCounter.y);

        if (num != 0)
        {
            m_MovementCounter.y = m_MovementCounter.y - num;
            return MoveVExactCollideSolids(num, self, contactFilter);
        }

        return false;
    }

    public bool MoveVExactCollideSolids(int moveV, Collider2D self, ContactFilter2D contactFilter)
    {
        float y = self.offset.y;
        //float y = transform.position.y;
        int num = (int)Mathf.Sign(moveV);
        int num2 = 0;
        List<Collider2D> results = new List<Collider2D>();
        Collider2D solid = null;

        while (moveV != 0)
        {
            self.offset += Vector2.up * num;
            self.OverlapCollider(contactFilter, results);

            if (results.Count != 0)
            {
                solid = CollideFirst(self, results, transform.position + Vector3.up * (float)num);

                if(solid != null)
                {
                    break;
                }
            }

            num2 += num;
            moveV -= num;
            //transform.position = new Vector3(transform.position.x, transform.position.y + (float)num, transform.position.z);
            self.offset = new Vector2(self.offset.x, self.offset.y + num);
        }

        self.offset = new Vector2(self.offset.x, y);
        //transform.position = new Vector3(transform.position.x, y, transform.position.z);
        MoveVExact(num2);

        return solid != null;
    }
}
