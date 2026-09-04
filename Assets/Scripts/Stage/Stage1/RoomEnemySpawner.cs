using UnityEngine;

public class RoomEnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;         // 생성할 적 프리팹 (Skeleton 등)
    [SerializeField] Transform[] spawnPoints;         // 이 구역에서 적이 나올 위치들
    [SerializeField] RoomManager roomManager;         // 이 구역의 RoomManager (씬에 있는 것)

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject enemyObj = Instantiate(enemyPrefab, spawnPoints[i].position, Quaternion.identity);

            Skeleton skeleton = enemyObj.GetComponent<Skeleton>();
            if (skeleton != null)
            {
                skeleton.SetRoomManager(roomManager);   // 코드로 직접 연결해줌
            }
        }

        if (roomManager != null)
        {
            roomManager.SetTotalEnemyCount(spawnPoints.Length);
        }
    }
}