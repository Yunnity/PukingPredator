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
        var targetInteractable = player.targetInteractable;

        if (inventory.isFull) { return; }
        if (targetInteractable == null) { return; }

        AudioManager.Instance.PlaySFX(AudioManager.ClipName.Eating);
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
