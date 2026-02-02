using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveScript : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int enemyCount = 3;

    public float spawnRadius = 15f;
    public float spawnInterval = 30f;

    public Transform spawnCenter;

    private void Start()
    {
        if (spawnCenter == null)
            spawnCenter = transform;

        StartCoroutine(SpawnGraves());
    }

    private IEnumerator SpawnGraves()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnGrave();
        }
    }

    private void SpawnGrave()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector2 position = spawnCenter.position;
            Vector2 offset = Random.insideUnitCircle * spawnRadius;
            Instantiate(enemyPrefab, position + offset, Quaternion.identity);
        }
    }

    // Визуализация радиуса в редакторе
    private void OnDrawGizmosSelected()
    {
        if (spawnCenter == null)
            spawnCenter = transform;

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawWireSphere(spawnCenter.position, spawnRadius);
    }
}