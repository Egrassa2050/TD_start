using UnityEngine;
using UnityEngine.UI;

namespace Ilumisoft.HealthSystem.UI
{
    [AddComponentMenu("Health System/UI/Healthbar")]
    public class Healthbar : MonoBehaviour
    {
        [field: SerializeField]
        public HealthComponent Health { get; set; }

        [SerializeField] Canvas canvas;
        [SerializeField] Image fillImage;
        [SerializeField] bool hideEmpty = false;
        [SerializeField] bool alignWithCamera = false;
        [SerializeField, Min(0.1f)] float changeSpeed = 100;

        protected float currentValue; // Змінено на protected
        Camera mainCamera;

        protected virtual void Start() // Додано virtual
        {
            mainCamera = Camera.main;
            if (Health != null) 
            {
                currentValue = Health.CurrentHealth;
            }
            UpdateVisibility();
        }

        void Update()
        {
            if (Health == null) return;

            if (alignWithCamera) AlignWithCamera();
            
            currentValue = Mathf.MoveTowards(currentValue, Health.CurrentHealth, Time.deltaTime * changeSpeed);
            UpdateFillbar();
            UpdateVisibility();
        }

        void AlignWithCamera()
        {
            if (mainCamera != null)
                transform.forward = mainCamera.transform.forward;
        }

        protected void UpdateFillbar() // Змінено на protected
        {
            if (fillImage != null)
                fillImage.fillAmount = Mathf.InverseLerp(0, Health.MaxHealth, currentValue);
        }

        void UpdateVisibility()
        {
            if (canvas == null) return;

            bool shouldShow = !(hideEmpty && Mathf.Approximately(Health.CurrentHealth, 0));
            canvas.gameObject.SetActive(shouldShow);
        }
    }
}