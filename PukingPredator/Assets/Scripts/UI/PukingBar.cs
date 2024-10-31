using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PukingBar : InputBehaviour
{
    [SerializeField] private Slider slider;
    private bool isCharging;
    private const float MAX_PUKE_DURATION = 2f;

    // Start is called before the first frame update
    void Start()
    {
        Subscribe(InputEvent.onPukeStart, StartChargingSlider);
        Subscribe(InputEvent.onPuke, ResetSlider);
    }

    void Update()
    {
        if (isCharging)
        {
            slider.value += (Time.deltaTime / MAX_PUKE_DURATION) * slider.maxValue;
        }
    }

    public void StartChargingSlider()
    {
        isCharging = true;
    }

    public void ResetSlider()
    {
        isCharging = false;
        slider.value = 0;
    }
}
