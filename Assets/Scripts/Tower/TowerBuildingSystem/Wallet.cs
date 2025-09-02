using UnityEngine;

public class Wallet : MonoBehaviour
{
    public static Wallet Instance { get; private set; }

    [SerializeField, Tooltip("Поточні гроші")]
    private int money = 100;

    public int Money => money;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public bool SpendMoney(int amount)
    {
        if (amount <= 0) return true;
        if (money >= amount) { money -= amount; return true; }
        return false;
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }
}