using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomImage : MonoBehaviour
{
    // Start is called before the first frame update
    private SpriteRenderer[] randomImages ;

    void Awake()
    {
        randomImages = new SpriteRenderer[transform.childCount];
        for (int i=0; i<randomImages.Length; i++)
        {
            randomImages[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();
        }
    }

    void Start()
    {
        int randSeed = Random.Range(0, 100);
        GetComponent<Image>().sprite = randomImages[randSeed % randomImages.Length].sprite;
        
    }

    // Update is called once per frame
    
}
