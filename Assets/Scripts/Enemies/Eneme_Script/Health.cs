using UnityEngine;

namespace Ilumisoft.HealthSystem
{
    [AddComponentMenu("Health System/Health")]
    public class Health : HealthComponent
    {
        [SerializeField] float maxHealth = 100.0f;
        [SerializeField, Range(0, 1)] float initialRatio = 1.0f;

        public override float MaxHealth { get => maxHealth; set => maxHealth = value; }
        public override float CurrentHealth { get; set; }
        public override bool IsAlive => CurrentHealth > 0;

        void Awake() => SetHealth(maxHealth * initialRatio);

        public override void SetHealth(float health)
        {
            float newHealth = Mathf.Clamp(health, 0, MaxHealth);
            float difference = newHealth - CurrentHealth;
            CurrentHealth = newHealth;

            if (difference != 0)
                InvokeHealthChanged(difference); // Використовуємо метод з базового класу

            if (CurrentHealth <= 0) 
                InvokeHealthEmpty(); // Використовуємо метод з базового класу
        }

        public override void AddHealth(float amount)
        {
            if (!IsAlive) return;
            SetHealth(CurrentHealth + amount);
        }

        public override void ApplyDamage(float damage)
        {
            if (!IsAlive) return;
            SetHealth(CurrentHealth - damage);
        }
    }
}