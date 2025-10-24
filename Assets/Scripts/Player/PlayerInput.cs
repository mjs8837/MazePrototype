using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour, IPlayerInput
{
    [SerializeField]
    private InputActionAsset _inputActionAsset;

    public event EventHandler<ValueChangeEventArgs> ValueChangeEvent;

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
                    case PlayerInputTypes.Move:

                        ValueChangeEvent?.Invoke(this, new ValueChangeEventArgs(playerInputType, callbackContext.ReadValue<Vector2>()));
                        break;

                    case PlayerInputTypes.Camera:

                        ValueChangeEvent?.Invoke(this, new ValueChangeEventArgs(playerInputType, callbackContext.ReadValue<Vector2>().normalized));
                        break;

                    default:

                        Debug.LogWarning("An unhandled player input type was parsed.");
                        break;
                }
            }
        }
    }
}
