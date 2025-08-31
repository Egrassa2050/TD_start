using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SoftwareCursor : MonoBehaviour
{
    [Header("References")]
    public Camera targetCamera;
    public Canvas parentCanvas;
    public float raycastDistance = 100f;
    public LayerMask raycastLayerMask = -1;

    private RectTransform cursorRect;
    private Image cursorImage;
    private GameObject currentHoveredObject;
    private readonly PointerEventData pointerEventData = new PointerEventData(EventSystem.current);

    void Start()
    {
        InitializeCursor();
        InitializeEventSystem();
    }

    void Update()
    {
        UpdateCursorPosition();
        UpdateCursorRotation();
        HandleRaycasting();
        HandleInput();
    }

    /// <summary>
    /// Ініціалізує компоненти курсора
    /// </summary>
    private void InitializeCursor()
    {
        cursorRect = GetComponent<RectTransform>();
        cursorImage = GetComponent<Image>();
        
        if (parentCanvas == null)
            parentCanvas = GetComponentInParent<Canvas>();
        
        if (targetCamera == null)
            targetCamera = Camera.main;

        // Встановлюємо курсор в центр екрану
        cursorRect.anchorMin = Vector2.one * 0.5f;
        cursorRect.anchorMax = Vector2.one * 0.5f;
        cursorRect.pivot = Vector2.one * 0.5f;
        cursorRect.anchoredPosition = Vector2.zero;
    }

    /// <summary>
    /// Переконується, що в сцені є EventSystem
    /// </summary>
    private void InitializeEventSystem()
    {
        if (EventSystem.current == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
    }

    /// <summary>
    /// Оновлює позицію курсора в центрі екрану
    /// </summary>
    private void UpdateCursorPosition()
    {
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceCamera && targetCamera != null)
        {
            // Для режиму ScreenSpaceCamera
            cursorRect.position = targetCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
        }
        else
        {
            // Для режимів ScreenSpaceOverlay і WorldSpace
            cursorRect.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        }
    }

    /// <summary>
    /// Оновлює обертання курсора відповідно до камери
    /// </summary>
    private void UpdateCursorRotation()
    {
        if (targetCamera != null)
        {
            cursorRect.rotation = targetCamera.transform.rotation;
        }
    }

    /// <summary>
    /// Обробляє raycast для виявлення об'єктів
    /// </summary>
    private void HandleRaycasting()
    {
        // Raycast для UI елементів
        HandleUIRaycast();
        
        // Raycast для 3D об'єктів
        Handle3DRaycast();
    }

    /// <summary>
    /// Виконує raycast для UI елементів
    /// </summary>
    private void HandleUIRaycast()
    {
        pointerEventData.position = cursorRect.position;
        
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        GameObject newHoveredObject = results.Count > 0 ? results[0].gameObject : null;

        if (currentHoveredObject != newHoveredObject)
        {
            ExecuteEvents.Execute(currentHoveredObject, pointerEventData, ExecuteEvents.pointerExitHandler);
            ExecuteEvents.Execute(newHoveredObject, pointerEventData, ExecuteEvents.pointerEnterHandler);
            currentHoveredObject = newHoveredObject;
        }
    }

    /// <summary>
    /// Виконує raycast для 3D об'єктів
    /// </summary>
    private void Handle3DRaycast()
    {
        if (targetCamera == null) return;

        Ray ray = targetCamera.ScreenPointToRay(cursorRect.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, raycastLayerMask))
        {
            // Обробка 3D об'єктів
            ExecuteEvents.Execute(hit.collider.gameObject, pointerEventData, ExecuteEvents.pointerEnterHandler);
            currentHoveredObject = hit.collider.gameObject;
        }
    }

    /// <summary>
    /// Обробляє введення користувача
    /// </summary>
    private void HandleInput()
    {
        // Обробка лівої кнопки миші
        if (Input.GetMouseButtonDown(0))
        {
            ExecuteEvents.Execute(currentHoveredObject, pointerEventData, ExecuteEvents.pointerClickHandler);
        }

        // Обробка правої кнопки миші
        if (Input.GetMouseButtonDown(1))
        {
            ExecuteEvents.Execute(currentHoveredObject, pointerEventData, ExecuteEvents.pointerDownHandler);
        }

        // Додаткова обробка інших типів введення за потреби
    }
}