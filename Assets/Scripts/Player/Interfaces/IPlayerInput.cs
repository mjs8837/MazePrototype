using System;

public interface IPlayerInput
{
    event EventHandler<ValueChangeEventArgs> ValueChangeEvent;
}
