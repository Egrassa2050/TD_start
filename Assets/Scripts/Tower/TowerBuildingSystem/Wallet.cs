using System;
using UnityEngine;
using TMPro;

public class Wallet : MonoBehaviour
{
    public static Wallet Instance { get; private set; }

    [SerializeField, Tooltip("Початкова сума")]
    private int money = 100;

    [SerializeField, Tooltip("Перетягни свій TMP_Text сюди")]
    private TMP_Text moneyText;

    // Подія: (новий баланс, дельта) дельта додатня при додаванні, від'ємна при витраті
    public event Action<int,int> OnMoneyChanged;

    public int Money => money;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        UpdateText();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void UpdateText()
    {
        if (moneyText != null)
            moneyText.text = money.ToString();
    }

    // Повертає true — якщо вистачило грошей і списання пройшло
    public bool SpendMoney(int amount)
    {
        if (amount <= 0) return true;
        if (money >= amount)
        {
            money -= amount;
            UpdateText();
            OnMoneyChanged?.Invoke(money, -amount);
            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        if (amount == 0) return;
        money += amount;
        UpdateText();
        OnMoneyChanged?.Invoke(money, amount);
    }
}