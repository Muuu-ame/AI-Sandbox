
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Camera Settings")]
    [SerializeField] private float _mouseSensitivity = 0.15f;
    [SerializeField] private float _minPitch = -60f;
    [SerializeField] private float _maxPitch = 70f;
    [SerializeField] private float _cameraDistance = 3f;
    [SerializeField] private float _cameraHeight = 2f;

    private float yaw;
    private float pitch = 15f;

    private void Start()
    {
        LockCursor();

        yaw = player.eulerAngles.y;
    }

    private void Update()
    {
        // ESCでカーソル解除
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            UnlockCursor();
        }

        // 左クリックで再固定
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame &&
            Cursor.lockState != CursorLockMode.Locked)
        {
            LockCursor();
        }

        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        yaw += mouseDelta.x * _mouseSensitivity;
        pitch -= mouseDelta.y * _mouseSensitivity;

        pitch = Mathf.Clamp(pitch, _minPitch, _maxPitch);
    }

    private void LateUpdate()
    {
        if (player == null) return;

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 targetPosition =
            player.position + Vector3.up * _cameraHeight;

        transform.position =
            targetPosition - rotation * Vector3.forward * _cameraDistance;

        transform.rotation = rotation;

        // プレイヤーも左右の向きをカメラに合わせる
        player.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
