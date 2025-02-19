using UnityEngine;
using UnityEngine.UI;

public class NoteControl : Note
{
    /// <summary>
    /// The image that shows the control.
    /// </summary>
    private Image controlImage;

    [SerializeField]
    private Sprite controlSpriteKeyboardAndMouse;
    [SerializeField]
    private Sprite controlSpriteXBOX;
    [SerializeField]
    private Sprite controlSpritePS4;

    private GameInput gameInput;

    /// <summary>
    /// The text shown by the note before the control.
    /// </summary>
    [SerializeField]
    private string text1 = "";

    /// <summary>
    /// The text shown by the note after the control.
    /// </summary>
    [SerializeField]
    private string text2 = "";



    private void Start()
    {
        gameInput = GameInput.Instance;

        if (text1 != "") { _ = AddText(text1); }

        var controlObj = AddImage(controlSpriteKeyboardAndMouse, new Vector2(36f, 36f));
        controlImage = controlObj.GetComponent<Image>();
        gameInput.Subscribe(InputEvent.onDeviceSwapAny, UpdateControlImage);

        if (text2 != "") { _ = AddText(text2); }
    }

    private void OnDestroy()
    {
        if (gameInput == null) { return; }
        gameInput.Unsubscribe(InputEvent.onDeviceSwapAny, UpdateControlImage);
    }



    private void UpdateControlImage()
    {
        switch(gameInput.inputDeviceType)
        {
            case InputDeviceType.gamepad:
                switch (gameInput.gamepadType)
                {
                    case GamepadType.playStation:
                        controlImage.sprite = controlSpritePS4;
                        break;

                    default:
                        controlImage.sprite = controlSpriteXBOX;
                        break;
                }
                
                break;

            default:
                controlImage.sprite = controlSpriteKeyboardAndMouse;
                break;
        }
    }
}
