using UnityEngine;

public class SettingsMenu : MenuRoot
{
    [SerializeField]
    private GameObject sensitivitySliderMenu;

    [SerializeField]
    private GameObject keyBoardControls;

    [SerializeField]
    private GameObject controllerControls;

    public void OnNextButtonClicked()
    {
        if (sensitivitySliderMenu.activeInHierarchy)
        {
            sensitivitySliderMenu.SetActive(false);
            keyBoardControls.SetActive(true);
            controllerControls.SetActive(false);
        }
        else if (keyBoardControls.activeInHierarchy)
        {
            sensitivitySliderMenu.SetActive(false);
            keyBoardControls.SetActive(false);
            controllerControls.SetActive(true);
        }
        else
        {
            sensitivitySliderMenu.SetActive(true);
            keyBoardControls.SetActive(false);
            controllerControls.SetActive(false);
        }
    }
}
