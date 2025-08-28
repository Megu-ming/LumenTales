using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Setting")]
    public EnemyData enemyData;
    public GameObject enemyPrefab;
    public float respawnTime = 7f;

    [Header("State")]
    public GameObject enemyInstance;

    private Coroutine spawnLoop;

    private void OnEnable()
    {
        if(enemyInstance == null)
            SpawnEnemy();

        spawnLoop = StartCoroutine(RespawnEnemy());
    }

    private void OnDisable()
    {
        if (spawnLoop != null)
            StopCoroutine(spawnLoop);
    }

    private IEnumerator RespawnEnemy()
    {
        var waitRespawn = new WaitForSeconds(respawnTime);

        while(true)
        {
            if(!enemyInstance.activeSelf)
            {
                yield return waitRespawn;
                SpawnEnemy();
            }
            else
                yield return null;
        }
    }

    private void SpawnEnemy()
    {
        if(enemyPrefab == null)
        {
            Debug.LogWarning($"[{name}] Prefab이 비어있습니다");
            return;
        }

        if(enemyInstance == null)
        {
            enemyInstance = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            return;
        }

        if(!enemyInstance.activeSelf)
        {
            enemyInstance.transform.position = transform.position;
            Status status = enemyInstance.GetComponent<Status>();
            status.Respawn();
            enemyInstance.SetActive(true);

            return;
        }

    }
}
