using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("View")]
    [Range(1f, 30f)]     [SerializeField] private float distance = 12f;
    [Range(10f, 89f)]    [SerializeField] private float pitch = 60f;
    [Range(-180f, 180f)] [SerializeField] private float yaw = 0f;

    [Header("Follow")]
    [SerializeField] private float smoothSpeed = 10f;

    [Header("Aim Offset")]
    [SerializeField] private float maxHorizontalShift = 3f;
    [SerializeField] private float maxDistanceBonus = 5f;
    [SerializeField] private float aimOffsetSmooth = 6f;

    private Vector3 _currentAimOffset;
    private float _currentDistance;

    private void Awake()
    {
        _currentDistance = distance;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetAimOffset = Vector3.zero;
        float targetDistance = distance;

        if (playerMovement != null)
        {
            float strength = playerMovement.AimStrength;
            targetAimOffset = playerMovement.AimDirection * (maxHorizontalShift * strength);
            targetDistance = distance + maxDistanceBonus * strength;
        }

        _currentAimOffset = Vector3.Lerp(_currentAimOffset, targetAimOffset, aimOffsetSmooth * Time.deltaTime);
        _currentDistance  = Mathf.Lerp(_currentDistance,  targetDistance,  aimOffsetSmooth * Time.deltaTime);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desired = target.position + _currentAimOffset + rotation * (Vector3.back * _currentDistance);

        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
        transform.rotation = rotation;
    }
}
