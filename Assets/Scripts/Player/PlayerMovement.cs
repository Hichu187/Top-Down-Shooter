using UnityEngine;
using UnityEngine.InputSystem;

public enum ControlScheme { Auto, Desktop, Mobile }

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private Joystick moveJoystick;
    [SerializeField] private Joystick aimJoystick;

    [Header("Control")]
    [SerializeField] private ControlScheme controlScheme = ControlScheme.Auto;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -20f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 720f;

    [Header("Aim")]
    [SerializeField] private float maxAimDistance = 10f;

    // Consumed by PlayerCamera
    public Vector3 AimDirection { get; private set; }
    public float AimStrength { get; private set; }

    private CharacterController _controller;
    private Camera _camera;
    private float _verticalVelocity;
    private bool _isDesktop;
    private InputAction _moveAction;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _camera = Camera.main;
        _isDesktop = controlScheme switch
        {
            ControlScheme.Desktop => true,
            ControlScheme.Mobile  => false,
            _                     => !Application.isMobilePlatform,
        };

        var playerMap = inputActions.FindActionMap("Player", throwIfNotFound: true);
        _moveAction = playerMap.FindAction("Move", throwIfNotFound: true);
        playerMap.Enable();
    }

    private void OnDestroy()
    {
        inputActions.FindActionMap("Player")?.Disable();
    }

    private void Update()
    {
        ApplyGravity();
        Move();
        Rotate();
    }

    private void ApplyGravity()
    {
        _verticalVelocity = _controller.isGrounded
            ? -1f
            : _verticalVelocity + gravity * Time.deltaTime;
    }

    private void Move()
    {
        Vector2 input = _isDesktop ? _moveAction.ReadValue<Vector2>() : moveJoystick.Direction;

        Vector3 horizontal = new Vector3(input.x, 0f, input.y) * moveSpeed;
        Vector3 motion = horizontal + Vector3.up * _verticalVelocity;

        _controller.Move(motion * Time.deltaTime);
    }

    private void Rotate()
    {
        Vector3 dir;

        if (_isDesktop)
        {
            bool lmbHeld = Mouse.current != null && Mouse.current.leftButton.isPressed;
            if (lmbHeld)
            {
                Vector3 raw = GetMouseWorldOffset();
                float dist = raw.magnitude;
                AimStrength = Mathf.Clamp01(dist / maxAimDistance);
                AimDirection = dist > 0.01f ? raw / dist : Vector3.zero;
                dir = raw;
            }
            else
            {
                AimStrength = 0f;
                AimDirection = Vector3.zero;
                dir = Vector3.zero;
            }
        }
        else
        {
            Vector2 aimInput = aimJoystick.Direction;
            Vector2 raw = aimInput.magnitude > 0.1f ? aimInput : moveJoystick.Direction;
            AimStrength = raw.magnitude;
            AimDirection = AimStrength > 0.01f ? new Vector3(raw.x, 0f, raw.y) / AimStrength : Vector3.zero;
            dir = AimDirection;
        }

        if (dir.sqrMagnitude < 0.01f)
            return;

        Quaternion target = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, rotationSpeed * Time.deltaTime);
    }

    // Returns raw XZ world-space vector from player to mouse hit point
    private Vector3 GetMouseWorldOffset()
    {
        if (_camera == null || Mouse.current == null)
            return Vector3.zero;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Ray ray = _camera.ScreenPointToRay(mouseScreenPos);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * transform.position.y);

        if (!groundPlane.Raycast(ray, out float distance))
            return Vector3.zero;

        Vector3 worldPoint = ray.GetPoint(distance);
        Vector3 offset = worldPoint - transform.position;
        offset.y = 0f;
        return offset;
    }
}
