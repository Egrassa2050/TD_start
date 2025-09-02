using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }

    [Header("Список веж")]
    [SerializeField] private List<GameObject> towerPrefabs = new List<GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public GameObject GetTowerPrefab(int index)
    {
        if (index >= 0 && index < towerPrefabs.Count) return towerPrefabs[index];
        return null;
    }

    public IReadOnlyList<GameObject> TowerPrefabs => towerPrefabs; // <- саме так називай
}