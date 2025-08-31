// BuildManager.cs
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    [Header("Список веж")]
    public List<GameObject> towerPrefabs;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public GameObject GetTowerPrefab(int index)
    {
        if (index >= 0 && index < towerPrefabs.Count)
            return towerPrefabs[index];
        return null;
    }
}