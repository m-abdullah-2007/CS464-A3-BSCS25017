using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform[] spawnPoints;   // must NOT be children of this object
    [SerializeField] int firstWave = 2, extraPerWave = 1, maxPerWave = 5;
    [SerializeField] float spawnGap = 1f, restBetweenWaves = 4f;
    int wave;

    IEnumerator Start()
    {
        while (true)
        {
            wave++;
            int count = Mathf.Min(firstWave + (wave - 1) * extraPerWave, maxPerWave);
            for (int i = 0; i < count; i++)
            {
                Transform at = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(enemyPrefab, at.position, at.rotation, transform);
                yield return new WaitForSeconds(spawnGap);
            }
            yield return new WaitUntil(() => transform.childCount == 0);
            yield return new WaitForSeconds(restBetweenWaves);
        }
    }
}