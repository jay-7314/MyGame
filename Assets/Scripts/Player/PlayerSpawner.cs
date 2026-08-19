using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] GameObject playerPrefabs;

    private void Awake()
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