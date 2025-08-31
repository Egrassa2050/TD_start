using UnityEngine;

public class MoneyTargetManager : MonoBehaviour
{
    [Header("Налаштування переходу")]
    public int targetMoneyAmount = 50000;
    
    void Update()
    {
        // Постійно перевіряємо стан гаманця
        if (Wallet.Instance != null && Wallet.Instance.money >= targetMoneyAmount)
        {
            GameManager.Instance.OnGameWon();
        }
    }
}