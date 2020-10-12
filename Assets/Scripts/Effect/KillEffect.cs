using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillEffect : MonoBehaviour
{
    public float killTime;


    void FixedUpdate()
    {
        killTime -= Time.deltaTime;
        if (killTime <= 0)
            Destroy(this.gameObject);
    }
}
