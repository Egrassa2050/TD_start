using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Target : MonoBehaviour
{
    public int index;

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        string[] parts = gameObject.name.Split('_');
        if (parts.Length > 1 && int.TryParse(parts[1], out int parsedIndex))
        {
            index = parsedIndex;
            FullPathway.AddTarget(gameObject, index);
        }
        else
        {
            Debug.LogWarning($"Target name '{gameObject.name}' is not in the format Target_number");
        }
    }
}