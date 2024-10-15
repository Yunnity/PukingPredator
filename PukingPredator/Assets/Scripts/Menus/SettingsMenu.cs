using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject keyBoardControls;
    [SerializeField] private GameObject controllerControls;

    public void OnNextButtonClicked()
    {
        if (keyBoardControls.activeInHierarchy)
        {
            keyBoardControls.SetActive(false);
            controllerControls.SetActive(true);
        }
        else
        {
            keyBoardControls.SetActive(true);
            controllerControls.SetActive(false);
        }
    }
}
