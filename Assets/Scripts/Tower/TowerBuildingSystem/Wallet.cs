using UnityEngine;
using TMPro;

public class Wallet : MonoBehaviour
{
    public static Wallet Instance;
    public int money = 10000;
    public Transform moneyTextTransform;
    private TMP_Text moneyText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (moneyTextTransform != null)
            moneyText = moneyTextTransform.GetComponent<TMP_Text>();

        UpdateMoneyText();
        // Видалено DontDestroyOnLoad - тепер об'єкт не зберігається між сценами
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            UpdateMoneyText();
            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyText();
    }

    private void UpdateMoneyText()
    {
        if (moneyText != null)
            moneyText.text = money.ToString();
    }
}