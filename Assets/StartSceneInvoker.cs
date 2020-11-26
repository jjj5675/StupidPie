using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneInvoker : MonoBehaviour
{
    public TransitionPoint point;
    public void Invoker()
    {
        SceneController.Instance.TransitionToScene(point);
    }
}
