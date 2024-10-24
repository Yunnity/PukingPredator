using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Player))]
public class Eating : InputBehaviour
{
    /// <summary>
    /// The mass of the player before having eaten anything.
    /// </summary>
    private float baseMass;

    /// <summary>
    /// Default size of the character.
    /// </summary>
    private Vector3 baseScale;

    /// <summary>
    /// The players inventory.
    /// </summary>
    private Inventory inventory => player.inventory;

    /// <summary>
    /// Multiplier for the mass of items in the players inventory. 0.05 means 5%
    /// of the weight of items is added to the players mass when consumed.
    /// </summary>
    private const float MASS_FACTOR = 0.05f;

    /// <summary>
    /// The player component.
    /// </summary>
    private Player player;

    /// <summary>
    /// The player's playerAnim component
    /// </summary>
    private PlayerAnimation anim;

    /// <summary>
    /// Force applied to object when puked. Depends on how long the puke button
    /// was held down for.
    /// </summary>
    private float pukeForce
    {
        get {
            float holdPercent = Mathf.Clamp(gameInput.pukeHoldDuration / MAX_PUKE_DURATION, 0, 1);
            return Mathf.Lerp(MIN_PUKE_FORCE, MAX_PUKE_FORCE, holdPercent);
        }
    }
    private const float MIN_PUKE_FORCE = 1f;
    private const float MAX_PUKE_FORCE = 20f;
    private const float MAX_PUKE_DURATION = 2f;

    /// <summary>
    /// The rigidbody of the player.
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// How much the inventory size impacts the size of the player
    /// </summary>
    private const float SCALE_FACTOR = 0.2f;

    /// <summary>
    /// How quickly the players size grows.
    /// </summary>
    private const float SCALE_RATE = 8f;

    private Vector3 targetScale;

    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<Player>();
        anim = player.GetComponent<PlayerAnimation>();

        baseMass = rb.mass;

        inventory.onChange += UpdateMass;
        
        baseScale = gameObject.transform.localScale;
        targetScale = baseScale;

        Subscribe(InputEvent.onEat, GameInput_Eat);
        Subscribe(InputEvent.onPuke, GameInput_Puke);
    }

    private void Update()
    {
        var rate = SCALE_RATE * Time.deltaTime;
        player.transform.localScale = Vector3.Lerp(player.transform.localScale, targetScale, rate);
    }



    private void GameInput_Eat()
    {
        var targetInteractable = player.targetInteractable;

        if (inventory.isFull) { return; }
        if (targetInteractable == null) { return; }

        AudioManager.Instance.PlaySFX("Eating", 2.0f);
        anim.StartEatAnim();

        var targetObject = targetInteractable.gameObject;
        ConsumeObject(targetObject);
    }
    
    private void GameInput_Puke()
    {
        if (inventory.isEmpty) { return; }

        Consumable itemToPuke = inventory.PopItem();
        if (itemToPuke == null) return;

        itemToPuke.SetState(ItemState.beingPuked);

        anim.StartPukeAnim();

        //puke forward and with a little force upwards
        var pukeDir = transform.forward + Vector3.up * 0.1f;

        Rigidbody itemRb = itemToPuke.GetComponent<Rigidbody>();
        if (itemRb != null)
        {
            //TODO: make this code work if there is no rigid body. perhaps just set
            //...the velocity and if there was a rigid body then we can reduce the velocity
            //...while calculating it based on the mass?
            itemRb.AddForce(pukeDir * pukeForce * itemRb.mass, ForceMode.Impulse);
        }
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

    private void UpdateMass()
    {
        var totalItemMass = inventory.totalMass;
        int currInventoryCount = inventory.itemCount;

        //TODO: change this to use totalItemMass instead of the count once masses are fine tuned
        rb.mass = baseMass + currInventoryCount * MASS_FACTOR;
        targetScale = baseScale * (1 + SCALE_FACTOR * currInventoryCount);
    }

}
