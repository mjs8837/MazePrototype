using System.Collections;
using Unity.VisualScripting;
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
    private float _sprint;

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

    // Sprint Fields
    [SerializeField, Tooltip("Modify to change how much sprinting changes movement speed.")]
    private float _sprintSpeedMultiplier;

    private Coroutine _sprintCoroutine;
    private Coroutine _enduraceReplenishCoroutine;
    private Coroutine _enduranceCooldownCoroutine;

    private bool _canSprint = true;
    private float _endurance = 1.0f;

    private const float NO_ENDURANCE_COOLDOWN = 1.0f;
    private const float SPRINT_INPUT_RELEASED_COOLDOWN = 0.3f;

    private const float ENDURANCE_FILL_RATE = 0.3f;
    private const float ENDURANCE_DEPLETION_RATE = 0.4f;

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
        float sprintSpeed = _endurance > 0 && _canSprint ? _sprint * _sprintSpeedMultiplier : 0.0f;

        _movementVector = (_moveDirection.x * _playerCamera.transform.right) + (_playerCamera.transform.forward * _moveDirection.y);
        _characterController.Move((_movementSpeedMultiplier + sprintSpeed) * Time.deltaTime * _movementVector);

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
    /// Helper coroutine used to handle when to remove/start replenishing endurance while sprinting.
    /// </summary>
    /// <returns>Delays between frames for each iteration of the while loop.</returns>
    private IEnumerator handleSprint()
    {
        // While the player still has endurance, and is still holding the sprint input
        while (_endurance > 0.0f)
        {
            _endurance -= Time.deltaTime * ENDURANCE_DEPLETION_RATE;
            UIManager.Instance.SetEnduranceBarFill(_endurance);
            yield return null;
        }

        _sprint = 0.0f;
        _sprintCoroutine = null;
        _enduranceCooldownCoroutine ??= StartCoroutine(sprintCooldown(NO_ENDURANCE_COOLDOWN));
        _enduraceReplenishCoroutine ??= StartCoroutine(replenishEndurance());
    }

    /// <summary>
    /// Helper coroutine used to enforce a cooldown to prevent the sprint input from being pressed when it shouldn't be.
    /// </summary>
    /// <param name="cooldownDuration">The duration of the cooldown.</param>
    /// <returns>Delays for the duration of the cooldown duration.</returns>
    private IEnumerator sprintCooldown(float cooldownDuration)
    {
        _canSprint = false;
        yield return new WaitForSeconds(cooldownDuration);
        _canSprint = true;

        _enduranceCooldownCoroutine = null;
    }

    /// <summary>
    /// Helper coroutine used to replenish endurance.
    /// </summary>
    /// <returns>Delays for a frame after each iteration of the while loop.</returns>
    private IEnumerator replenishEndurance()
    {
        while (_endurance < 1.0f)
        {
            _endurance += Time.deltaTime * ENDURANCE_FILL_RATE;
            UIManager.Instance.SetEnduranceBarFill(_endurance);
            yield return null;
        }

        _endurance = 1.0f;
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

            case PlayerInputTypes.Sprint:

                if (!_canSprint)
                {
                    return;
                }

                _sprint = (float)e.Value;

                // Sprint input detected.
                if (_sprint > 0.0f)
                {
                    _sprintCoroutine = StartCoroutine(handleSprint());

                    // Stop replenishing endurance if starting to sprint.
                    if (_enduraceReplenishCoroutine != null)
                    {
                        StopCoroutine(_enduraceReplenishCoroutine);
                        _enduraceReplenishCoroutine = null;
                    }
                }
                else
                {
                    // If stopping sprint by releasing the input, start the cooldown with the input cooldown.
                    if (_sprintCoroutine != null)
                    {
                        StopCoroutine(_sprintCoroutine);
                        _sprintCoroutine = null;
                        _enduranceCooldownCoroutine ??= StartCoroutine(sprintCooldown(SPRINT_INPUT_RELEASED_COOLDOWN));
                    }

                    _enduraceReplenishCoroutine ??= StartCoroutine(replenishEndurance());
                }
                
                break;

            default:

                break;
        }
    }
}