using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualUI : MonoBehaviour
{
   

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab) || Input.GetButtonDown("Manual1"))
        {
            Publisher.Instance.GainOrReleaseControl(true);
            UIManager.instance.ToggleHUDCanvas(true);
            this.gameObject.SetActive(false);
        }
    }
}
