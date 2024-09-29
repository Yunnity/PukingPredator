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
    private List<(InputEvent inputEvent, Action action)> subscriptions = new();

    /// <summary>
    /// The coroutine used to fetch the gameInput and apply the subscriptions.
    /// </summary>
    private Coroutine getGameInputCoroutine = null;



    protected virtual void Awake()
    {
        //start a coroutine that runs once per frame till it successfully finds GameInput object
        getGameInputCoroutine = StartCoroutine(GetGameInput());
    }

    protected virtual void OnDestroy()
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
                gameInput.Subscribe(subscription.inputEvent, subscription.action);
            }
            else
            {
                gameInput.Unsubscribe(subscription.inputEvent, subscription.action);
            }
        }
    }

    /// <summary>
    /// Used to add an event binding.
    /// </summary>
    /// <param name="inputEvent"></param>
    /// <param name="action"></param>
    public void Subscribe(InputEvent inputEvent, Action action)
    {
        subscriptions.Add((inputEvent, action));
        if (areSubscriptionsActive)
        {
            gameInput.Subscribe(inputEvent, action);
        }
    }

    /// <summary>
    /// Used to remove an event binding.
    /// </summary>
    /// <param name="inputEvent"></param>
    /// <param name="action"></param>
    public void Unsubscribe(InputEvent inputEvent, Action action)
    {
        subscriptions.RemoveAll(s => s.inputEvent == inputEvent && s.action == action);
        if (areSubscriptionsActive)
        {
            gameInput.Unsubscribe(inputEvent, action);
        }
    }
}
