using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EndMove : MonoBehaviour
{
    
    bool canMove = false;

    void Update()
    {
        if(canMove && Input.anyKeyDown)
        {
            SceneController.Instance.TransitionToUIScenes("Credit");
        }
    }
    public void Ended()
    {
        canMove = true;
    }
    
}
