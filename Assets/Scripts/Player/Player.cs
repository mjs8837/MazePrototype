using UnityEngine;

public class Player : MonoBehaviour
{
    private IPlayerInput _playerInput;

    private void Start()
    {
        _playerInput = GetComponentInChildren<IPlayerInput>();

        if (_playerInput != null)
        {
            _playerInput.ValueChangeEvent += _playerInput_ValueChangeEvent;
        }
    }

    private void _playerInput_ValueChangeEvent(object sender, ValueChangeEventArgs e)
    {
        switch (e.PlayerInputType)
        {
            case PlayerInputTypes.Move:

                Vector2 moveDirection = (Vector2)e.Value;
                transform.position += new Vector3(moveDirection.x, 0.0f, moveDirection.y);
                break;

            case PlayerInputTypes.Camera:

                Vector2 cameraMoveDirection = (Vector2)e.Value;
                Debug.Log(cameraMoveDirection);
                break;

            default:

                break;
        }
    }
}
