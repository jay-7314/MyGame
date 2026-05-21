using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    void Awake()
    {
        if(null == instance)
        {
            instance = this;
            Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        
    }
}
