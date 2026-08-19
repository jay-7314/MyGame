using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefabs;

    private void Awake()
    {
        if (GameObject.FindWithTag("Enemy") == null)
        {
            var enemy = Instantiate(enemyPrefabs, transform.position, transform.rotation);
            DontDestroyOnLoad(enemy);
        }

        if (SceneManager.GetActiveScene().name == "Map")
        {
            GameObject existingEnemy = GameObject.FindWithTag("Enemy");
            if (existingEnemy != null)
            {
                Destroy(existingEnemy);
            }
        }
    }
}