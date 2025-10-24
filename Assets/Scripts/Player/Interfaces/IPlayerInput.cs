using System;

/// <summary>
/// Interface used to dictate what information the player input script can share.
/// </summary>
public interface IPlayerInput
{
    event EventHandler<PlayerInputValueChangeEventArgs> ValueChangeEvent;
}
