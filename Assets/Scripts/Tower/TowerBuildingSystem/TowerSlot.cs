using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    private GameObject currentTower;

    [Header("Налаштування слота")]
    public GameObject buildMenuUI;
    public Transform towerSpawnPoint;

    void OnMouseDown()
    {
        // Перевіряємо, чи не клікнули через UI
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

        // Вмикаємо UI і передаємо слот у скрипт BuildMenu
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

    public void BuildTower(int towerIndex)
    {
        if (currentTower != null)
        {
            Debug.Log($"[TowerSlot] На слоті {name} вже стоїть вежа!");
            return;
        }

        var prefab = BuildManager.Instance.GetTowerPrefab(towerIndex);
        if (prefab == null)
        {
            Debug.LogWarning($"[TowerSlot] Немає префабу для індексу {towerIndex}");
            return;
        }

        currentTower = Instantiate(prefab, towerSpawnPoint.position, towerSpawnPoint.rotation);
        Debug.Log($"[TowerSlot] Побудовано {prefab.name} у слоті {name}");

        // Меню закривається через виклик CloseMenu в BuildMenu
    }

    public void ClearTower()
    {
        currentTower = null;
        Debug.Log($"[TowerSlot] Слот {name} очищено");
    }
}