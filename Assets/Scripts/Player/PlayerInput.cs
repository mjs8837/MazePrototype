using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class used to handle reading input from the Unity Input System and pass it to <see cref="Player"/>.
/// </summary>
public class PlayerInput : MonoBehaviour, IPlayerInput
{
    [SerializeField]
    private InputActionAsset _inputActionAsset;

    public event EventHandler<PlayerInputValueChangeEventArgs> ValueChangeEvent;

    void Start()
    {
        InputActionMap gameActionMap = _inputActionAsset.FindActionMap("Game");
        
        foreach (InputAction action in gameActionMap?.actions)
        {
            action.started += gameAction_HandleAction;
            action.performed += gameAction_HandleAction;
            action.canceled += gameAction_HandleAction;
        }
    }

    private void gameAction_HandleAction(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed || callbackContext.canceled)
        {
            if (Enum.TryParse(callbackContext.action.name, out PlayerInputTypes playerInputType))
            {
                switch (playerInputType)
                {
                    case PlayerInputTypes.Move or
                        PlayerInputTypes.Camera:

                        ValueChangeEvent?.Invoke(this, new PlayerInputValueChangeEventArgs(playerInputType, callbackContext.ReadValue<Vector2>()));
                        break;

                    case PlayerInputTypes.Sprint:

                        ValueChangeEvent?.Invoke(this, new PlayerInputValueChangeEventArgs(playerInputType, callbackContext.ReadValue<float>()));
                        break;

                    default:

                        Debug.LogWarning("An unhandled player input type was parsed.");
                        break;
                }
            }
        }
    }
}
