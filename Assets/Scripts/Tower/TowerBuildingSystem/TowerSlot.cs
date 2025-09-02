using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class TowerSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Transform towerSpawnPoint;
    private GameObject currentTower;

    public bool HasTower => currentTower != null;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (HasTower) return;

        if (!CanAffordAnyTower())
        {
            Debug.Log("Недостатньо грошей для будівництва будь-якої вежі!");
            return;
        }

        if (BuildMenu.Instance != null) BuildMenu.Instance.OpenMenu(this);
    }

    private bool CanAffordAnyTower()
    {
        var prefabs = BuildManager.Instance.TowerPrefabs;
        for (int i = 0; i < prefabs.Count; i++)
        {
            var prefab = prefabs[i];
            if (prefab == null) continue;
            var t = prefab.GetComponent<Tower>();
            if (t != null && Wallet.Instance.Money >= t.Cost) return true;
        }
        return false;
    }

    public void BuildTower(int towerIndex)
    {
        var prefab = BuildManager.Instance.GetTowerPrefab(towerIndex);
        if (prefab == null) return;

        var towerComp = prefab.GetComponent<Tower>();
        if (towerComp == null) return;

        if (!Wallet.Instance.SpendMoney(towerComp.Cost))
        {
            Debug.Log("Недостатньо грошей для будівництва!");
            return;
        }

        currentTower = Instantiate(prefab, towerSpawnPoint.position, Quaternion.identity);
        BuildMenu.Instance.CloseMenu();
    }

    public void SellTower()
    {
        if (!HasTower) return;
        var t = currentTower.GetComponent<Tower>();
        if (t != null) Wallet.Instance.AddMoney(t.Cost / 2);
        Destroy(currentTower);
    }
}