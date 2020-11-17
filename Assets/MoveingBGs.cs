using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveingBGs : MonoBehaviour
{
    private float xBound;

    public float speed;

    void Awake()
    {
        xBound = (int)GetComponentInChildren<SpriteRenderer>().bounds.size.x;
        
        transform.GetChild(1).localPosition = new Vector3(xBound, 0, transform.GetChild(1).localPosition.z);
    }
    void Update()
    {
        for(int i=0; i<transform.childCount; i++)
        {
            transform.GetChild(i).localPosition -= speed * Time.deltaTime * new Vector3(1, 0, 0);
            if (transform.GetChild(i).localPosition.x < -xBound)
                transform.GetChild(i).localPosition = new Vector3(xBound, 0, transform.GetChild(i).localPosition.z);
        }
    }
}
