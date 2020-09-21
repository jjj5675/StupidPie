using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingTrap : MonoBehaviour
{

    float rad = 0;
    Vector3 target;
    Vector3 startVec;
    
    // Start is called before the first frame update
    void Start()
    {
        target = transform.parent.position;
        startVec = transform.position - target;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rad += Mathf.PI * Time.deltaTime;
        Vector3 moveVec;
        moveVec
            = new Vector3(Mathf.Cos(rad) * startVec.x - Mathf.Sin(rad) * startVec.y,
            Mathf.Cos(rad) * startVec.y + Mathf.Sin(rad) * startVec.x,0);

        Debug.Log(moveVec);
        transform.position = target+ moveVec;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * rad);
    }
}
