using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualUI : MonoBehaviour
{
   

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            Publisher.Instance.GainOrReleaseControl(true);
            this.gameObject.SetActive(false);
        }
    }
}
