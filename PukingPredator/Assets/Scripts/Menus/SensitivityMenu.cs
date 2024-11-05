using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SensitivityMenu : MonoBehaviour
{
    [SerializeField] private GameObject sensitivitySlider;
    [SerializeField] private float sensitivity = 0.3f;
    [SerializeField] private GameObject firstGameObjectSelected;
    private Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        slider = sensitivitySlider.GetComponent<Slider>();
    }

    void Update()
    {
        // Get trigger values from the controller
        float rightTrigger = Input.GetAxis("Horizontal");
        //float leftTrigger = Input.GetAxis("LeftTrigger");

        float sliderChange = rightTrigger * sensitivity;

        // Update the slider value while clamping within the slider's min/max range
        slider.value = Mathf.Clamp(slider.value + sliderChange, slider.minValue, slider.maxValue);
    }

    public void ChangeSensitivity()
    {
        GameManager.sensitivity = slider.value;
    }

    private void OnEnable()
    {
        if (slider != null)
        {
            slider.value = GameManager.sensitivity;
        }
        EventSystem.current.SetSelectedGameObject(firstGameObjectSelected);
    }
}
