using UnityEngine;
using Ilumisoft.HealthSystem;

public class BaseHealth : HealthComponent
{
    [SerializeField] private BaseHealthConfig config;
    [SerializeField] private BaseHealthbar healthbar;
    
    private float currentHealth;
    
    public override float MaxHealth 
    { 
        get => config.maxHealth;
        set => config.maxHealth = Mathf.RoundToInt(value); // Додано сеттер
    }
    
    public override float CurrentHealth 
    { 
        get => currentHealth;
        set => SetHealth(value); // Додано сеттер
    }
    
    public override bool IsAlive => currentHealth > 0;

    private void Start()
    {
        currentHealth = config.maxHealth;
        
        if (healthbar != null)
        {
            healthbar.SetHealth(this);
        }
    }

    private void Update()
    {
        if (config.healthRegenRate > 0 && currentHealth < MaxHealth)
        {
            AddHealth(config.healthRegenRate * Time.deltaTime);
        }
    }

    public override void SetHealth(float health)
    {
        float newHealth = Mathf.Clamp(health, 0, MaxHealth);
        float difference = newHealth - currentHealth;
        currentHealth = newHealth;

        if (difference != 0)
            InvokeHealthChanged(difference);

        if (currentHealth <= 0)
            InvokeHealthEmpty();
    }

    public override void AddHealth(float amount)
    {
        if (!IsAlive) return;
        SetHealth(currentHealth + amount);
    }

    public override void ApplyDamage(float damage)
    {
        if (!IsAlive) return;
        SetHealth(currentHealth - damage);
    }
}