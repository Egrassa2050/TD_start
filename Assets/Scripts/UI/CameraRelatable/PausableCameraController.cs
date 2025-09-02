using UnityEngine;

public class PausableCameraController : MonoBehaviour, IPausable
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float shiftSpeed = 20f;
    [SerializeField] private Transform target;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 50f;

    private bool isPaused;
    private float currentDistance;
    private Vector3 direction; // напрямок від цілі до камери

    private void Start()
    {
        PauseManager.Instance.RegisterPausable(this);

        if (target != null)
        {
            transform.LookAt(target);
            direction = (transform.position - target.position).normalized; // фіксуємо напрямок
            currentDistance = Vector3.Distance(transform.position, target.position);
        }
    }

    private void OnDestroy() => PauseManager.Instance?.UnregisterPausable(this);

    void Update()
    {
        if (isPaused) return;

        HandleMovement();
        HandleZoom();

        if (target != null)
        {
            transform.LookAt(target);
        }
    }

    private void HandleMovement()
    {
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? shiftSpeed : speed;

        Vector3 move = new Vector3(
            Input.GetAxis("Horizontal"),
            0f,
            Input.GetAxis("Vertical")
        ) * currentSpeed * Time.deltaTime;

        transform.position += move;

        if (target != null)
        {
            direction = (transform.position - target.position).normalized; // оновлюємо напрямок після руху
            currentDistance = Vector3.Distance(transform.position, target.position);
        }
    }

    private void HandleZoom()
    {
        if (target == null) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            currentDistance -= scroll * zoomSpeed;
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

            transform.position = target.position + direction * currentDistance;
        }
    }

    public void OnPause() => isPaused = true;
    public void OnResume() => isPaused = false;
}
