using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    private GameObject currentTower;

    [Header("Налаштування слота")]
    public GameObject buildMenuUI;
    public Transform towerSpawnPoint;

    void OnMouseDown()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;
            
        if (currentTower != null)
        {
            Debug.Log($"[TowerSlot] На слоті {name} вже є вежа!");
            return;
        }

        if (buildMenuUI == null)
        {
            Debug.LogWarning($"[TowerSlot] buildMenuUI не підключена у {name}!");
            return;
        }

        BuildMenu menu = buildMenuUI.GetComponent<BuildMenu>();
        if (menu != null)
        {
            menu.OpenMenu(this);
            Debug.Log($"[TowerSlot] Відкрито панель {buildMenuUI.name} для слоту {name}");
        }
        else
        {
            Debug.LogWarning($"[TowerSlot] На {buildMenuUI.name} немає компоненту BuildMenu!");
        }
    }

    public bool BuildTower(int towerIndex)
    {
        if (currentTower != null)
        {
            Debug.Log($"[TowerSlot] На слоті {name} вже стоїть вежа!");
            return false;
        }

        var prefab = BuildManager.Instance.GetTowerPrefab(towerIndex);
        if (prefab == null)
        {
            Debug.LogWarning($"[TowerSlot] Немає префабу для індексу {towerIndex}");
            return false;
        }

        // Забираємо вартість із компонента Tower на префабі
        Tower towerData = prefab.GetComponent<Tower>();
        if (towerData == null)
        {
            Debug.LogWarning($"[TowerSlot] Префаб {prefab.name} не містить компонент Tower!");
            return false;
        }

        int cost = towerData.Cost;

        if (Wallet.Instance == null)
        {
            Debug.LogWarning("[TowerSlot] Wallet.Instance == null");
            return false;
        }

        // Списуємо гроші перед створенням — якщо не вистачає, не будуємо
        if (!Wallet.Instance.SpendMoney(cost))
        {
            Debug.Log($"[TowerSlot] Недостатньо грошей для побудови ({cost})");
            return false;
        }

        // Інстанціюємо вежу після успішного списання
        currentTower = Instantiate(prefab, towerSpawnPoint.position, towerSpawnPoint.rotation);
        Debug.Log($"[TowerSlot] Побудовано {prefab.name} у слоті {name} за {cost}");
        return true;
    }

    public void ClearTower()
    {
        currentTower = null;
        Debug.Log($"[TowerSlot] Слот {name} очищено");
    }
}
