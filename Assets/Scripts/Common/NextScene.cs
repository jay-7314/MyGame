using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
        public void NextSceneBtn()
    {
        SceneManager.LoadScene("Stage1");
    }
}
