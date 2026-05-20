using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayGame : MonoBehaviour
{
    public void GameStart()
    {
        SceneManager.LoadScene("Village");
    }
}
