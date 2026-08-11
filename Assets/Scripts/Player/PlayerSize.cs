using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSize : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoadSize;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoadSize;
    }

    void OnSceneLoadSize(Scene scene, LoadSceneMode mode)
    {
        if(gameObject.name == "Aren")
        {
            if (scene.name == "Intro")
            {
                transform.localScale = new Vector3(2, 2, 1);
            }
            else if (scene.name == "Village")
            {
                transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            }
        }
        else
        {
            if (scene.name == "Intro")
            {
                transform.localScale = new Vector3(3, 3, 1);
            }
            else if (scene.name == "Village")
            {
                transform.localScale = new Vector3(2.2f, 2.2f, 2.2f);
            }
        }
        
    }
}
