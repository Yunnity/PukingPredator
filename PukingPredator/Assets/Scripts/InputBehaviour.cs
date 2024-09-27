using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputBehaviour : MonoBehaviour
{
    /// <summary>
    /// Used to track the state of all the subscriptions.
    /// </summary>
    private bool areSubscriptionsActive = false;

    /// <summary>
    /// The handler for user input.
    /// </summary>
    [HideInInspector]
    public GameInput gameInput;

    /// <summary>
    /// A list of events and the actions subscribed to them.
    /// </summary>
    private List<(EventType eventType, Action action)> subscriptions = new();

    /// <summary>
    /// The coroutine used to fetch the gameInput and apply the subscriptions.
    /// </summary>
    private Coroutine getGameInputCoroutine = null;



    protected virtual void Awake()
    {
        //start a coroutine that runs once per frame till it successfully finds GameInput object
        getGameInputCoroutine = StartCoroutine(GetGameInput());
    }

    private void OnDestroy()
    {
        if (getGameInputCoroutine != null) { StopCoroutine(getGameInputCoroutine); }

        SetSubscribedAll(false);
    }



    /// <summary>
    /// Gets the GameInput instance and then activates all subscriptions.
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetGameInput()
    {
        while (gameInput == null)
        {
            gameInput = GameInput.Instance;
            yield return new WaitForEndOfFrame();
        }

        //subscribe to all of the correct events now that gameInput is found
        SetSubscribedAll(true);
    }

    /// <summary>
    /// Used to subscribe or unsubscribe from all events at once.
    /// </summary>
    /// <param name="isSubscribed"></param>
    private void SetSubscribedAll(bool isSubscribed)
    {
        if (areSubscriptionsActive == isSubscribed) { return; }
        areSubscriptionsActive = isSubscribed;

        foreach (var subscription in subscriptions)
        {
            if (isSubscribed)
            {
                gameInput.Subscribe(subscription.eventType, subscription.action);
            }
            else
            {
                gameInput.Unsubscribe(subscription.eventType, subscription.action);
            }
        }
    }

    /// <summary>
    /// Used to add an event binding.
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="action"></param>
    public void Subscribe(EventType eventType, Action action)
    {
        subscriptions.Add((eventType, action));
        if (areSubscriptionsActive)
        {
            gameInput.Subscribe(eventType, action);
        }
    }

    /// <summary>
    /// Used to remove an event binding.
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="action"></param>
    public void Unsubscribe(EventType eventType, Action action)
    {
        subscriptions.RemoveAll(s => s.eventType == eventType && s.action == action);
        if (areSubscriptionsActive)
        {
            gameInput.Unsubscribe(eventType, action);
        }
    }
}
