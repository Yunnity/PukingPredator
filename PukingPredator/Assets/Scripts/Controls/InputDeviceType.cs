using System;

public enum InputDeviceType
{
    keyboard,
    mouse,
    gamepad,
    other,
}
public enum GamepadType
{
    xbox,
    playStation,
    other,
}

static class InputDeviceTypeMethods
{
    public static bool IsKeyboardOrMouse(this InputDeviceType idt)
    {
        switch (idt)
        {
            case InputDeviceType.keyboard:
                return true;
            case InputDeviceType.mouse:
                return true;
            default:
                return false;
        }
    }
}
