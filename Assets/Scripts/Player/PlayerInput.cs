using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private enum PlayerInputTypes
    {
        Move,
        Camera
    }

    [SerializeField]
    private InputActionAsset _inputActionAsset;

    private Vector2 _moveInput;
    private Vector2 _cameraInput;

    public Vector2 MoveInput
    {
        get { return _moveInput; }
    }

    public Vector2 CameraInput
    {
        get { return _cameraInput; }
    }

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

                        _moveInput = callbackContext.ReadValue<Vector2>();
                        break;

                    case PlayerInputTypes.Camera:

                        _cameraInput = callbackContext.ReadValue<Vector2>().normalized;
                        break;

                    default:

                        Debug.LogWarning("An unhandled player input type was parsed.");
                        break;
                }
            }
        }
    }
}
