using UnityEngine;

namespace Ilumisoft.HealthSystem
{
    public abstract class HealthComponent : MonoBehaviour
    {
        public abstract float MaxHealth { get; set; }
        public abstract float CurrentHealth { get; set; }
        public abstract bool IsAlive { get; }

        public event System.Action<float> OnHealthChanged;
        public event System.Action OnHealthEmpty;

        public abstract void SetHealth(float health);
        public abstract void AddHealth(float amount);
        public abstract void ApplyDamage(float damage);

        // Методи для виклику подій
        protected void InvokeHealthChanged(float change)
        {
            OnHealthChanged?.Invoke(change);
        }

        protected void InvokeHealthEmpty()
        {
            OnHealthEmpty?.Invoke();
        }
    }
}