using UnityEngine;

[CreateAssetMenu(fileName = "SpawnData", menuName = "Spawn/SpawnData")]
public class SpawnData : ScriptableObject
{
    public PlayerSpawnPoint[] whereToSpawn;
    public Vector3[] spawnPoint;
}
