using UnityEngine;

public class TurnToTargetWithTag : MonoBehaviour
{
    public string targetTag = "Enemy";  // тег ворога
    public float turnSpeed = 360f;      // градусів за секунду
    public float searchInterval = 0.5f; // як часто шукати нову ціль

    private Transform currentTarget;
    private float searchTimer = 0f;

    void Update()
    {
        // Шукаємо ціль через інтервал
        searchTimer -= Time.deltaTime;
        if (searchTimer <= 0f)
        {
            FindClosestTarget();
            searchTimer = searchInterval;
        }

        // Поворот до цілі
        if (currentTarget != null)
        {
            Vector3 direction = currentTarget.position - transform.position;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);
            }
        }
    }

    void FindClosestTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(targetTag);
        float minDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = enemy.transform;
            }
        }

        currentTarget = closest;
    }
}