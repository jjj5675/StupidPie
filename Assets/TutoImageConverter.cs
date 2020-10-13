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
        if (isIres)
        {
            int ires = (int)SceneController.Instance.gameObject.GetComponent<ControllerSets>().iresKeySet;
            if (SceneController.Instance.gameObject.GetComponent<ControllerSets>().iresKeySet != ControllerSets.KeySetTypes.Controller
                && SceneController.Instance.gameObject.GetComponent<ControllerSets>().seriKeySet == ControllerSets.KeySetTypes.Controller)
                ires = 2;

            GetComponent<SpriteRenderer>().sprite
                = transform.GetChild(ires).GetComponent<SpriteRenderer>().sprite;

        }
        else
        {
            int seri = (int)SceneController.Instance.gameObject.GetComponent<ControllerSets>().seriKeySet;
            if (SceneController.Instance.gameObject.GetComponent<ControllerSets>().seriKeySet != ControllerSets.KeySetTypes.Controller
                && SceneController.Instance.gameObject.GetComponent<ControllerSets>().iresKeySet == ControllerSets.KeySetTypes.Controller)
                seri = 4;
            GetComponent<SpriteRenderer>().sprite
                = transform.GetChild(seri).GetComponent<SpriteRenderer>().sprite;
        }
    }

    // Update is called once per frame
    
}
