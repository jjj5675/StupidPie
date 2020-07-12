using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<TPool, TObject> : MonoBehaviour
    where TPool : ObjectPool<TPool, TObject>
    where TObject : PoolObject<TPool, TObject>, new()
{
    public GameObject prefab;
    public int initalPoolCount = 10;
    [HideInInspector]
    public List<TObject> pool = new List<TObject>();

    // Start is called before the first frame update
    void Start()
    {
        for(int i=0; i<initalPoolCount; i++)
        {
            TObject newPoolObject = CreateNewPoolObject();
            pool.Add(newPoolObject);
        }
    }

    protected TObject CreateNewPoolObject()
    {
        TObject newPoolObject = new TObject();
        newPoolObject.instance = Instantiate(prefab);
        //newPoolObject.instance.transform.SetParent(transform);
        newPoolObject.inPool = true;
        newPoolObject.SetReferences(this as TPool);
        newPoolObject.Sleep();
        return newPoolObject;
    }

    public virtual TObject Pop()
    {
        for(int i=0; i<pool.Count; i++)
        {
            if(pool[i].inPool)
            {
                pool[i].inPool = false;
                pool[i].WakeUp();
                return pool[i];
            }
        }

        TObject newPoolObject = CreateNewPoolObject();
        pool.Add(newPoolObject);
        newPoolObject.inPool = false;
        newPoolObject.WakeUp();
        return newPoolObject;
    }

    public virtual void Push(TObject poolObject)
    {
        poolObject.inPool = true;
        poolObject.Sleep();
    }
}

[Serializable]
public abstract class PoolObject<TPool, TObject>
    where TPool : ObjectPool<TPool, TObject>
    where TObject : PoolObject<TPool, TObject>, new()
{
    public bool inPool;
    public GameObject instance;
    public TPool objectPool;

    public void SetReferences(TPool pool)
    {
        objectPool = pool;
        SetReferences();
    }

    protected virtual void SetReferences()
    { }

    public virtual void WakeUp()
    { }

    public virtual void Sleep()
    { }

    public virtual void ReturnToPool()
    {
        TObject thisObject = this as TObject;
        objectPool.Push(thisObject);
    }
}