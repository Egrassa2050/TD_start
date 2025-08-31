// BuildMenu.cs
using UnityEngine;

public class BuildMenu : MonoBehaviour
{
    public static BuildMenu Instance;

    private TowerSlot currentSlot;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void OpenMenu(TowerSlot slot)
    {
        currentSlot = slot;
        gameObject.SetActive(true);
    }

    public void BuildSelectedTower(int towerIndex)
    {
        if (currentSlot != null)
        {
            currentSlot.BuildTower(towerIndex);
        }
        else
        {
            Debug.LogWarning("currentSlot не вибраний!");
        }
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }
}