using UnityEngine;

public class MoneyTargetManager : MonoBehaviour
{
    [Header("Налаштування переходу")]
    public int targetMoneyAmount = 50000;

    void Update()
    {
        if (Wallet.Instance != null && Wallet.Instance.Money >= targetMoneyAmount)
        {
            GameManager.Instance.OnGameWon();
        }
    }
}