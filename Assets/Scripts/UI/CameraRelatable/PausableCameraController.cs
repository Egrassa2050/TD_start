using UnityEngine;

public class PausableCameraController : MonoBehaviour, IPausable
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float shiftSpeed = 20f;
    [SerializeField] private float mouseSensitivity = 2f;

    private float pitch;
    private float yaw;
    private bool isPaused;

    private void Start() => PauseManager.Instance.RegisterPausable(this);
    private void OnDestroy() => PauseManager.Instance?.UnregisterPausable(this);

    void Update()
    {
        if (isPaused) return;
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? shiftSpeed : speed;
        Vector3 move = transform.forward * Input.GetAxis("Vertical") + 
                       transform.right * Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.Space)) move += Vector3.up;
        if (Input.GetKey(KeyCode.LeftControl)) move += Vector3.down;

        transform.position += move * currentSpeed * Time.deltaTime;
    }

    private void HandleRotation()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.eulerAngles = new Vector3(pitch, yaw, 0f);
    }

    public void OnPause() => isPaused = true;
    public void OnResume() => isPaused = false;
}