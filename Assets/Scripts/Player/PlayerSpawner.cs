using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] GameObject playerPrefabs;

    private void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            player = Instantiate(playerPrefabs, transform.position, transform.rotation);
            DontDestroyOnLoad(player);
        }
        else
        {
            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;
        }
    }
}