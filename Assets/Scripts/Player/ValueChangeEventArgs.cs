using System;

public class ValueChangeEventArgs : EventArgs
{
    public PlayerInputTypes PlayerInputType;
    public object Value;

    public ValueChangeEventArgs(PlayerInputTypes playerInputType, object value)
    {
        PlayerInputType = playerInputType;
        Value = value;
    }
}
