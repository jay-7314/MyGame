using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] GameObject playerPrefabs;

    private void Awake()
    {
        if(GameObject.FindWithTag("Player") == null)
        {
            Vector3 spawnPos = Vector3.zero;
            Quaternion spawnRot = Quaternion.identity;

            GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
            if(spawnPoint != null)
            {
                spawnPos = spawnPoint.transform.position;
                spawnRot = spawnPoint.transform.rotation;
            }

            var player = Instantiate(playerPrefabs, spawnPos, spawnRot);
            DontDestroyOnLoad(player);
        }
    }
}
