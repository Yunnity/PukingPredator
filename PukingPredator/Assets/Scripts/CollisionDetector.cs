using System;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    private Action onCollision;



    private void OnCollisionEnter(Collision collision)
    {
        onCollision?.Invoke();
    }



    public void Subscribe(Action action)
    {
        onCollision += action;
    }

    public void Unsubscribe(Action action)
    {
        onCollision -= action;
    }
}

