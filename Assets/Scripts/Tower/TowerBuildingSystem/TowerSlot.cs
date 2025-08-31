// TowerSlot.cs - з доданою перевіркою грошей
using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    private GameObject currentTower;
    public GameObject buildMenuUI;
    public Transform towerSpawnPoint;

    void OnMouseDown()
    {
        if (currentTower != null)
        {
            Debug.Log("На цьому слоті вже є башня!");
            return;
        }

        // Перевіряємо, чи вистачає грошей на будь-яку вежу
        if (!CanAffordAnyTower())
        {
            Debug.Log("Недостатньо грошей для будівництва будь-якої вежі!");
            return;
        }

        if (buildMenuUI != null)
        {
            buildMenuUI.SetActive(true);
            BuildMenu.Instance.OpenMenu(this);
        }
    }

    private bool CanAffordAnyTower()
    {
        foreach (GameObject towerPrefab in BuildManager.Instance.towerPrefabs)
        {
            Tower towerComponent = towerPrefab.GetComponent<Tower>();
            if (towerComponent != null && Wallet.Instance.money >= towerComponent.cost)
            {
                return true;
            }
        }
        return false;
    }

    public void BuildTower(int towerIndex)
    {
        GameObject towerPrefab = BuildManager.Instance.GetTowerPrefab(towerIndex);
        if (towerPrefab == null) return;

        Tower towerComponent = towerPrefab.GetComponent<Tower>();
        if (towerComponent == null) return;

        int cost = towerComponent.cost;

        if (Wallet.Instance.SpendMoney(cost))
        {
            currentTower = Instantiate(towerPrefab, towerSpawnPoint.position, Quaternion.identity);
            BuildMenu.Instance.CloseMenu();
        }
        else
        {
            Debug.Log("Недостатньо грошей для будівництва!");
        }
    }
}