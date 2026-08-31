using UnityEngine;

public class IntroCleanup : MonoBehaviour
{
    void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && player.scene.name == "DontDestroyOnLoad")
        {
            Destroy(player);
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            if (enemy.scene.name == "DontDestroyOnLoad")
            {
                Destroy(enemy);
            }
        }
    }
}