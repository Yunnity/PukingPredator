using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensitvityControlsMenu : MenuRoot
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    public override void OnEnable()
    {
        base.OnEnable();
        resumeButton.interactable = false;
        restartButton.interactable = false;
        settingsButton.interactable = false;
        quitButton.interactable = false;
    }

    private void OnDisable()
    {
        resumeButton.interactable = true;
        restartButton.interactable = true;
        settingsButton.interactable = true;
        quitButton.interactable = true;
    }
}
