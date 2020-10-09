using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MovingContainer : MonoBehaviour
{

    public bool canMove;
    public Vector3 moveType;
    public PlatformCatcher catcher;

    private Vector3 startPos;
    private bool canFixVector;
    private bool playerEnter;
    private float canFixVectorTemp=0;
    private void Start()
    {
        startPos = transform.position;
        
    }

    public void Reset()
    {
        transform.position = startPos;
    }
    public void SetVector(Vector3 input)
    {
       
        if (!canFixVector)
            return;

        if(!playerEnter)
        {
            playerEnter = true;
            return;
        }

        canFixVector = false;
        canMove = true;
        moveType = input;
    }

    public void StopMove()
    {
        canMove = false;
        moveType = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if(canFixVectorTemp<=1.0f && !canFixVector)
        {
            canFixVectorTemp += Time.deltaTime;

        }
        else
        {
            canFixVectorTemp = 0;
            canFixVector = true;
        }

        if(canMove)
        {


            transform.position += moveType * Time.deltaTime;
            
            catcher.MoveCaughtObjects(Time.deltaTime * moveType);
            
        }
    }

}
