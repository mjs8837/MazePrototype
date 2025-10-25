using System;

/// <summary>
/// Class used to represent the event arguments to pass around when a player input value changes.
/// Inherits from <see cref="EventArgs"/>
/// </summary>
public class PlayerInputValueChangeEventArgs : EventArgs
{
    public PlayerInputTypes PlayerInputType;
    public object Value;

    public PlayerInputValueChangeEventArgs(PlayerInputTypes playerInputType, object value)
    {
        PlayerInputType = playerInputType;
        Value = value;
    }
}
