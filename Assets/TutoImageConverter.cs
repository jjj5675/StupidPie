using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutoImageConverter : MonoBehaviour
{
    public bool isIres;
    
    // Start is called before the first frame update
    void Start()
    {
        if(isIres)
        {

            GetComponent<SpriteRenderer>().sprite = transform.GetChild((int)SceneController.Instance.gameObject.GetComponent<ControllerSets>().iresKeySet).GetComponent<SpriteRenderer>().sprite;
        }
        else
            GetComponent<SpriteRenderer>().sprite = transform.GetChild((int)SceneController.Instance.gameObject.GetComponent<ControllerSets>().seriKeySet).GetComponent<SpriteRenderer>().sprite;
    }

    // Update is called once per frame
    
}
