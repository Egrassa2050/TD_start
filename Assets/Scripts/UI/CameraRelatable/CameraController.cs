using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Movement")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float shiftSpeed = 20f;
    [SerializeField] private float mouseSensitivity = 2f;

    private float pitch = 0f;
    private float yaw = 0f;

    // Додаємо подію для сповіщення про зміну стану заморозки
    public System.Action<bool> OnFreezeStateChanged;

    // Заміняємо поле на property для виклику події
    private bool freezeCamera;
    public bool FreezeCamera
    {
        get => freezeCamera;
        set
        {
            if (freezeCamera != value)
            {
                freezeCamera = value;
                OnFreezeStateChanged?.Invoke(freezeCamera);
            }
        }
    }

    void Update()
    {
        if (FreezeCamera) return;

        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? shiftSpeed : speed;
        Vector3 move = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.Space)) move += Vector3.up;
        if (Input.GetKey(KeyCode.LeftControl)) move += Vector3.down;

        transform.position += move * currentSpeed * Time.deltaTime;
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.eulerAngles = new Vector3(pitch, yaw, 0f);
    }

    // Метод для зовнішнього управління заморозкою камери
    public void SetFreeze(bool freeze)
    {
        FreezeCamera = freeze;
    }
}