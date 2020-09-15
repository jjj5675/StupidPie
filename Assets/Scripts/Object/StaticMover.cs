using System;
using UnityEngine;

public class StaticMover : MonoBehaviour
{
    public Action<Vector2> OnMove;

    public void Move(Vector2 amount)
    {
        if(OnMove!=null)
        {
            OnMove(amount);
            return;
        }
        transform.position += new Vector3(amount.x, amount.y, 0.0f);
    }
}
