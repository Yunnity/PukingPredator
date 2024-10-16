using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    private Consumable _item;
    /// <summary>
    /// The item the UI represents.
    /// </summary>
    public Consumable item
    {
        get { return _item; }
        set
        {
            _item = value;
            labelUI.text = _item.gameObject.name;
            labelUI.fontSize = 20;
        }
    }

    /// <summary>
    /// The label for the item name.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI labelUI;

    /// <summary>
    /// The visual timer for the item.
    /// </summary>
    [SerializeField]
    private Image timerUI;

    private void Start()
    {
        if (!item.canDecay) { timerUI.enabled = false; }
    }

    // Update is called once per frame
    void Update()
    {
        if (item.canDecay && item.decayTimer != null)
        {
            timerUI.fillAmount = item.decayTimer.percentRemaining;
        }
    }
}
