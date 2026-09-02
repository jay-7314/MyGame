using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] GameObject playerPrefabs;

    [Header("테스트용 - 폰의 느린 초기 로딩 흉내")]
    [SerializeField] float debugSpawnDelay = 0f; // 테스트 시에만 0.3~0.5 정도로 설정

    private void Awake()
    {
        if (debugSpawnDelay > 0f)
        {
            StartCoroutine(DelayedSpawn());
        }
        else
        {
            SpawnLogic();
        }
    }

    IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(debugSpawnDelay);
        SpawnLogic();
    }

    void SpawnLogic()
    {
        if (GameObject.FindWithTag("Player") == null)
        {
            var player = Instantiate(playerPrefabs, transform.position, transform.rotation);
            DontDestroyOnLoad(player);
        }

        if (SceneManager.GetActiveScene().name == "Map")
        {
            GameObject existingPlayer = GameObject.FindWithTag("Player");
            if (existingPlayer != null)
            {
                Destroy(existingPlayer);
            }
        }
    }
}