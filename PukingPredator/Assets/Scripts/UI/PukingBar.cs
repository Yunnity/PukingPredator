using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PukingBar : InputBehaviour
{
    [SerializeField] private GameObject sliderObject;
    private Slider slider;
    private bool isCharging;
    private const float MAX_PUKE_DURATION = 2f;
    private Player player;
    private Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        Subscribe(InputEvent.onPukeStart, StartChargingSlider);
        Subscribe(InputEvent.onPuke, ResetSlider);
        player = GameObject.Find("Player").GetComponent<Player>();
        inventory = player.inventory;
        slider = sliderObject.GetComponent<Slider>();
    }

    void Update()
    {
        if (inventory.itemCount == 0) sliderObject.SetActive(false);
        else sliderObject.SetActive(true);

        if (isCharging)
        {
            slider.value += (Time.deltaTime / MAX_PUKE_DURATION) * slider.maxValue;
        }
    }

    public void StartChargingSlider()
    {
        if (inventory.itemCount > 0)
        {
            isCharging = true;
        }
    }

    public void ResetSlider()
    {
        isCharging = false;
        slider.value = 0;
    }
}
