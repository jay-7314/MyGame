using System.Collections.Generic;
using UnityEngine;

public enum GameFlagKey
{
    None,
    Skip,
}

public class GameFlags : MonoBehaviour
{
    public static GameFlags instance;

    HashSet<GameFlagKey> flags = new HashSet<GameFlagKey>();

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void setFlag(GameFlagKey key) =>flags.Add(key);
    public bool hasFlag(GameFlagKey key) => flags.Contains(key);
}
