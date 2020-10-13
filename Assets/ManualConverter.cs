using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManualConverter : MonoBehaviour
{
    public bool isIres;
    void Start()
    {
        int ImageNum;
        if(isIres)
        {
            ImageNum = (int)SceneController.Instance.gameObject.GetComponent<ControllerSets>().iresKeySet;
            if (SceneController.Instance.gameObject.GetComponent<ControllerSets>().iresKeySet != ControllerSets.KeySetTypes.Controller
                && SceneController.Instance.gameObject.GetComponent<ControllerSets>().seriKeySet == ControllerSets.KeySetTypes.Controller)
                ImageNum = 2;
               
        }
        else
        {
            ImageNum = (int)SceneController.Instance.gameObject.GetComponent<ControllerSets>().seriKeySet;
            if (SceneController.Instance.gameObject.GetComponent<ControllerSets>().seriKeySet != ControllerSets.KeySetTypes.Controller
                && SceneController.Instance.gameObject.GetComponent<ControllerSets>().iresKeySet == ControllerSets.KeySetTypes.Controller)
                ImageNum = 4;
        }

        GetComponent<Image>().sprite = transform.GetChild(ImageNum).GetComponent<SpriteRenderer>().sprite;
    }

    // Update is called once per frame
    
}
