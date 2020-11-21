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
        sources[0].volume = num ? 0.6f : 0.1f;
        sources[1].volume = num ? 0.1f : 0.6f;
    }
}
