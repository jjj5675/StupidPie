using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuKeyInput : MonoBehaviour
{

    public UnityEvent tabOn;
    public UnityEvent tabOff;
    public UnityEvent ESCOn;
    public UnityEvent f5On;
    // Start is called before the first frame update

    bool istabOn = false;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab) || Input.GetButtonDown("Manual1"))
        {
            if (istabOn)
            {
                tabOff.Invoke();
                istabOn = false;
            }
            else
            {
                tabOn.Invoke();
                istabOn = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Fire3"))
            ESCOn.Invoke();

        if (Input.GetKeyDown(KeyCode.F5) || Input.GetButtonDown("Restart1"))
            f5On.Invoke();
    }
}
