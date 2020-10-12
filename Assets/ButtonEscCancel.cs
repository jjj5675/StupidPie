using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

public class ButtonEscCancel : MonoBehaviour
{
    public UnityEvent excOn;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            excOn.Invoke();
    }
}
