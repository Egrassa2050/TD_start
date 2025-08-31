using UnityEngine;
using UnityEngine.UI;
using Ilumisoft.HealthSystem;

public class BaseHealthbar : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private BaseHealthConfig config;
    
    [Header("UI References")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image fillImage;
    [SerializeField] private Text healthText;
    
    [Header("Settings")]
    [SerializeField] private bool alignWithCamera = true;
    [SerializeField, Min(0.1f)] private float changeSpeed = 100f;
    [SerializeField] private bool showHealthText = true;
    [SerializeField] private bool hideEmpty = false;
    
    private HealthComponent healthComponent;
    private float currentValue;
    private Camera mainCamera;

    public void SetHealth(HealthComponent health)
    {
        if (healthComponent != null)
        {
            healthComponent.OnHealthChanged -= OnHealthChanged;
            healthComponent.OnHealthEmpty -= OnHealthEmpty;
        }

        healthComponent = health;
        
        if (healthComponent != null)
        {
            currentValue = healthComponent.CurrentHealth;
            healthComponent.OnHealthChanged += OnHealthChanged;
            healthComponent.OnHealthEmpty += OnHealthEmpty;
            
            UpdateHealthDisplay();
        }
    }

    private void Start()
    {
        // Знаходимо компоненти, якщо вони не встановлені в інспекторі
        if (canvas == null) 
            canvas = GetComponentInParent<Canvas>();
    
        if (canvas == null)
            canvas = GetComponent<Canvas>();
    
        if (fillImage == null)
        {
            Image[] images = GetComponentsInChildren<Image>();
            foreach (Image img in images)
            {
                if (img.type == Image.Type.Filled)
                    fillImage = img;
            }
        
            // Якщо не знайшли filled image, беремо першу знайдену
            if (fillImage == null && images.Length > 0)
                fillImage = images[0];
        }
    
        if (healthText == null)
            healthText = GetComponentInChildren<Text>();
    
        if (healthText != null)
            healthText.gameObject.SetActive(showHealthText);
    
        // Знаходимо HealthComponent бази
        if (healthComponent == null)
        {
            HealthComponent baseHealth = GetComponentInParent<HealthComponent>();
            if (baseHealth != null)
                SetHealth(baseHealth);
        }
    
        UpdateHealthDisplay();
    }

    private void Update()
    {
        if (healthComponent == null) return;
        
        currentValue = Mathf.MoveTowards(currentValue, healthComponent.CurrentHealth, Time.deltaTime * changeSpeed);
        
        UpdateFillbar();
        
        if (alignWithCamera && mainCamera != null)
        {
            transform.forward = mainCamera.transform.forward;
        }
        
        UpdateVisibility();
    }

    private void OnHealthChanged(float difference)
    {
        // Додаткові ефекти при зміні здоров'я
    }

    private void OnHealthEmpty()
    {
        Debug.Log("Base health is empty!");
        // Викликаємо подію програшу
        GameManager.Instance.OnGameLost();
    }

    private void UpdateFillbar()
    {
        if (fillImage != null)
        {
            float fillAmount = Mathf.InverseLerp(0, healthComponent.MaxHealth, currentValue);
            fillImage.fillAmount = fillAmount;
            // Видалено зміну кольору - використовується тільки текстура
        }
        
        if (healthText != null && showHealthText)
        {
            healthText.text = $"{Mathf.RoundToInt(currentValue)}/{healthComponent.MaxHealth}";
        }
    }

    private void UpdateVisibility()
    {
        if (canvas == null || healthComponent == null) return;

        bool shouldShow = config.showHealthbarAlways || 
                         (healthComponent.CurrentHealth < healthComponent.MaxHealth && 
                          healthComponent.CurrentHealth > 0);

        if (hideEmpty && Mathf.Approximately(healthComponent.CurrentHealth, 0))
        {
            shouldShow = false;
        }

        canvas.gameObject.SetActive(shouldShow);
    }

    private void UpdateHealthDisplay()
    {
        currentValue = healthComponent.CurrentHealth;
        UpdateFillbar();
        UpdateVisibility();
    }

    private void OnDestroy()
    {
        if (healthComponent != null)
        {
            healthComponent.OnHealthChanged -= OnHealthChanged;
            healthComponent.OnHealthEmpty -= OnHealthEmpty;
        }
    }
}