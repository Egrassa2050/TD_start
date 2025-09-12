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
        PositionMenuNearSlot(slot);
        Debug.Log($"[BuildMenu] OpenMenu on {name}, slot = {slot.name}");
    }

    private void PositionMenuNearSlot(TowerSlot slot)
    {
        if (canvas == null || rectTransform == null) return;

        Vector3 screenPoint = Camera.main.WorldToScreenPoint(slot.transform.position);
        
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay || 
            canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                screenPoint,
                canvas.worldCamera,
                out localPoint
            );
            rectTransform.localPosition = localPoint + new Vector2(0, rectTransform.rect.height / 2 + 20);
            EnsureMenuIsOnScreen();
        }
        else if (canvas.renderMode == RenderMode.WorldSpace)
        {
            rectTransform.position = slot.transform.position + new Vector3(0, 1.5f, 0);
            rectTransform.rotation = Quaternion.LookRotation(rectTransform.position - Camera.main.transform.position);
        }
    }

    private void EnsureMenuIsOnScreen()
    {
        Vector2 canvasSize = canvas.GetComponent<RectTransform>().rect.size;
        Vector2 menuSize = rectTransform.rect.size;
        Vector2 menuPos = rectTransform.anchoredPosition;
        float rightEdge = menuPos.x + menuSize.x / 2;
        float leftEdge = menuPos.x - menuSize.x / 2;
        float topEdge = menuPos.y + menuSize.y / 2;
        float bottomEdge = menuPos.y - menuSize.y / 2;
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

    public void BuildSelectedTower(int towerIndex)
    {
        if (currentSlot != null)
        {
            Debug.Log($"[BuildMenu] BuildSelectedTower {towerIndex} on slot {currentSlot.name}");
            bool built = currentSlot.BuildTower(towerIndex);
            if (built) CloseMenu();
            else Debug.Log("[BuildMenu] Будівництво не вдалося");
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
