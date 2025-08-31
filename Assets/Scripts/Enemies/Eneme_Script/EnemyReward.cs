using UnityEngine;
using Ilumisoft.HealthSystem; // Додаємо потрібний простір імен

public class EnemyReward : MonoBehaviour
{
    [SerializeField] private int rewardAmount = 25;
    
    private Health health;

    void Start()
    {
        health = GetComponent<Health>();
        if (health != null)
        {
            health.OnHealthEmpty += GiveReward;
        }
    }

    private void GiveReward()
    {
        if (Wallet.Instance != null)
        {
            Wallet.Instance.AddMoney(rewardAmount);
            Debug.Log($"Отримано {rewardAmount} монет за знищення ворога");
        }
    }

    void OnDestroy()
    {
        if (health != null)
        {
            health.OnHealthEmpty -= GiveReward;
        }
    }
}