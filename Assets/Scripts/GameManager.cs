using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
    }
}
