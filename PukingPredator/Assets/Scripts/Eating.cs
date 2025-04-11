using UnityEngine;

[RequireComponent(typeof(Player))]
public class Eating : InputBehaviour
{
    /// <summary>
    /// The player's playerAnim component
    /// </summary>
    private PlayerAnimation anim;

    /// <summary>
    /// The players inventory.
    /// </summary>
    private Inventory inventory => player.inventory;

    /// <summary>
    /// The player component.
    /// </summary>
    private Player player;

    

    void Start()
    {
        player = GetComponent<Player>();
        anim = player.GetComponent<PlayerAnimation>();

        Subscribe(InputEvent.onEat, GameInput_Eat);
    }



    private void GameInput_Eat()
    {
        if (inventory.isFull)
        {
            AudioManager.Instance.PlaySFX(AudioID.CannotEat);
            return;
        }

        var targetInteractable = player.targetInteractable;
        if (targetInteractable == null) { return; }

        //TODO: should this be shifted down to the consume object script so that
        //      it only triggers if you actually eat stuff? getting a collectable
        //      isnt the same as eating
        AudioManager.Instance.PlaySFX(AudioID.Eat, wait: true);
        anim.StartEatAnim();

        var targetObject = targetInteractable.gameObject;
        ConsumeObject(targetObject);
    }

    /// <summary>
    /// Consumes the object and updates the inventory
    /// </summary>
    private void ConsumeObject(GameObject obj)
    {
        var consumableData = obj.GetComponent<Consumable>();
        if (consumableData == null || !consumableData.isConsumable) { return; }

        inventory.PushItem(consumableData);
        consumableData.SetState(ItemState.beingConsumed);
    }

}
