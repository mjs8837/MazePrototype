using UnityEngine;

/// <summary>
/// Main class responsible for player related functionality.
/// </summary>
public class Player : MonoBehaviour
{
    // Component Fields
    private Camera _playerCamera;
    private CharacterController _characterController;
    private IPlayerInput _playerInput;

    // Input Fields
    private Vector2 _moveDirection;
    private Vector2 _cameraMoveDirection;

    // Movement Fields
    [SerializeField, Header("Movement"), Tooltip("Modify to influence player movement speed.")]
    private float _movementSpeedMultiplier;

    private Vector3 _movementVector = Vector3.zero;
    private Vector3 _gravityMovement = Vector3.zero;
    private float _gravity = -9.8f;

    // Camera Fields
    [SerializeField, Header("Camera"), Tooltip("Modify to influence player camera rotation speed.")]
    private float _cameraMovementSpeedMultiplier;

    [SerializeField, Tooltip("Modify to change how far the player can look up/down.")]
    private Vector2 _cameraXAngleClamps = new(-30.0f, 45.0f);

    private Vector2 _cameraAngles = Vector2.zero;

    // Point Fields
    private int _points;
    private int _collectiblesGrabbed;

    private void Start()
    {
        _playerCamera = GetComponentInChildren<Camera>();
        _characterController = GetComponent<CharacterController>();
        _playerInput = GetComponentInChildren<IPlayerInput>();

        // Binding to the player input value change function to detect when input has been registered.
        if (_playerInput != null)
        {
            _playerInput.ValueChangeEvent += _playerInput_ValueChangeEvent;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        rotateCamera();
        movePlayer();
    }

    private void OnDestroy()
    {
        if (_playerInput != null)
        {
            _playerInput.ValueChangeEvent -= _playerInput_ValueChangeEvent;
        }
    }

    /// <summary>
    /// Function used to add points to the player and signal to update the points text.
    /// </summary>
    /// <param name="collectible">The collectible that was grabbed.</param>
    public void GrabCollectible(Collectible collectible)
    {
        _points += collectible.Points;
        _collectiblesGrabbed++;

        UIManager.Instance.SetPointsText(_points);
    }

    /// <summary>
    /// Helper function that contains functionality to rotate the player camera based on input.
    /// </summary>
    private void rotateCamera()
    {
        _cameraAngles.y += _cameraMoveDirection.x * _cameraMovementSpeedMultiplier;
        _cameraAngles.x = Mathf.Clamp(_cameraAngles.x + -_cameraMoveDirection.y * _cameraMovementSpeedMultiplier, _cameraXAngleClamps.x, _cameraXAngleClamps.y);

        _playerCamera.transform.localRotation = Quaternion.Euler(_cameraAngles.x, _cameraAngles.y, 0.0f);
    }

    /// <summary>
    /// Helper function that contains functionality to move the player and implement gravity based on input.
    /// </summary>
    private void movePlayer()
    {
        _gravityMovement.y = checkGrounded() && _gravityMovement.y < 0.0f ? _gravity : _gravityMovement.y;

        _movementVector = (_moveDirection.x * _playerCamera.transform.right) + (_playerCamera.transform.forward * _moveDirection.y);
        _characterController.Move(_movementSpeedMultiplier * Time.deltaTime * _movementVector);

        // Adding gravity if it should be added
        _gravityMovement.y += _gravity * Time.deltaTime;

        _characterController.Move(_gravityMovement * Time.deltaTime);
    }

    /// <summary>
    /// Helper function used to check if the player is currently on the ground.
    /// </summary>
    /// <returns>Returns the result of `Physics.CheckSphere` based on if the player is currently colliding with an object on the floor layer.</returns>
    private bool checkGrounded()
    {
        // Check for a sphere collision at the base of the player controller.
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _characterController.height / 2.0f, transform.position.z);
        return Physics.CheckSphere(spherePosition, _characterController.radius, LayerMask.GetMask("Floor"));
    }

    /// <summary>
    /// Event function responsible for pulling in data from <see cref="IPlayerInput"/> and storing it.
    /// </summary>
    /// <param name="sender">The object that sent the event args.</param>
    /// <param name="e">The event args for this event; in this case the type of input and the new input value.</param>
    private void _playerInput_ValueChangeEvent(object sender, PlayerInputValueChangeEventArgs e)
    {
        switch (e.PlayerInputType)
        {
            case PlayerInputTypes.Move:

                _moveDirection = (Vector2)e.Value;
                break;

            case PlayerInputTypes.Camera:

                _cameraMoveDirection = (Vector2)e.Value;
                break;

            default:

                break;
        }
    }
}