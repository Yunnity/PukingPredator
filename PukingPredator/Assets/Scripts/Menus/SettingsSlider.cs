using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SettingsSlider : MonoBehaviour
{
    [SerializeField]
    private GameSettingFloat setting;

    private Slider slider;



    private void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = GameSettings.GetFloatSetting(setting);
    }



    public void OnChange()
    {
        GameSettings.SetFloatSetting(setting, slider.value);
    }
}

