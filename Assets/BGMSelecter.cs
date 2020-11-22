using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMSelecter : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource[] sources;
    
    void Awake()
    {
        
    }
    public void Switch(bool num)
    {
        if(num)
          Debug.Log("!!");
        sources[0].volume = num ? 1.0f : 0.1f;
        sources[1].volume = num ? 0.1f : 1.0f;
    }
}
