using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildMenu : MonoBehaviour
{
    private TowerSlot currentSlot;
    private RectTransform rectTransform;
    private Canvas canvas;

    [SerializeField] private List<Button> buttons;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        
        // Ініціалізуємо кнопки при старті
        for (int i = 0; i < buttons.Count; i++)
        {
            int idx = i;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => BuildSelectedTower(idx));
        }
    }

    public void OpenMenu(TowerSlot slot)
    {
        currentSlot = slot;
        gameObject.SetActive(true);
        
        // Коректне позиціонування для Full HD
        PositionMenuNearSlot(slot);
        
        Debug.Log($"[BuildMenu] OpenMenu on {name}, slot = {slot.name}");
    }

    private void PositionMenuNearSlot(TowerSlot slot)
    {
        if (canvas == null || rectTransform == null) return;

        // Отримуємо екранні координати слота
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(slot.transform.position);
        
        // Для Screen Space Canvas
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay || 
            canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            // Перетворюємо екранні координати в локальні координати Canvas
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                screenPoint,
                canvas.worldCamera,
                out localPoint
            );
            
            // Встановлюємо позицію з урахуванням розмірів меню
            rectTransform.localPosition = localPoint + new Vector2(0, rectTransform.rect.height / 2 + 20);
            
            // Гарантуємо, що меню не виходить за межі екрану
            EnsureMenuIsOnScreen();
        }
        // Для World Space Canvas
        else if (canvas.renderMode == RenderMode.WorldSpace)
        {
            // Позиціонуємо меню в світових координатах зі зміщенням
            rectTransform.position = slot.transform.position + new Vector3(0, 1.5f, 0);
            
            // Орієнтуємо меню до камери
            rectTransform.rotation = Quaternion.LookRotation(
                rectTransform.position - Camera.main.transform.position);
        }
    }

    private void EnsureMenuIsOnScreen()
    {
        // Отримуємо розміри Canvas
        Vector2 canvasSize = canvas.GetComponent<RectTransform>().rect.size;
        
        // Отримуємо розміри меню
        Vector2 menuSize = rectTransform.rect.size;
        
        // Отримуємо поточну позицію меню
        Vector2 menuPos = rectTransform.anchoredPosition;
        
        // Перевіряємо, чи меню не виходить за межі екрану
        float rightEdge = menuPos.x + menuSize.x / 2;
        float leftEdge = menuPos.x - menuSize.x / 2;
        float topEdge = menuPos.y + menuSize.y / 2;
        float bottomEdge = menuPos.y - menuSize.y / 2;
        
        // Корегуємо позицію, якщо меню виходить за межі
        if (rightEdge > canvasSize.x / 2)
            menuPos.x = canvasSize.x / 2 - menuSize.x / 2;
        
        if (leftEdge < -canvasSize.x / 2)
            menuPos.x = -canvasSize.x / 2 + menuSize.x / 2;
        
        if (topEdge > canvasSize.y / 2)
            menuPos.y = canvasSize.y / 2 - menuSize.y / 2;
        
        if (bottomEdge < -canvasSize.y / 2)
            menuPos.y = -canvasSize.y / 2 + menuSize.y / 2;
        
        rectTransform.anchoredPosition = menuPos;
    }

    // Решта коду залишається незмінною
    public void BuildSelectedTower(int towerIndex)
    {
        if (currentSlot != null)
        {
            Debug.Log($"[BuildMenu] BuildSelectedTower {towerIndex} on slot {currentSlot.name}");
            currentSlot.BuildTower(towerIndex);
            CloseMenu();
        }
        else
        {
            Debug.LogWarning("[BuildMenu] currentSlot не вибраний!");
        }
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
        currentSlot = null;
        Debug.Log($"[BuildMenu] Closed {name}");
    }

    private void OnDisable()
    {
        currentSlot = null;
    }
}