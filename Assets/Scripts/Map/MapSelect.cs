using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelect : MonoBehaviour
{
    [SerializeField] Object stageName;

    public void OnMouseDown()
    {
        SceneManager.LoadScene(stageName.name);
        Debug.Log("Å¬¸¯µÊ");
    }



    //public void ()
    //{
    //    SceneManager.LoadScene(stageName.name);
    //    Debug.Log("Å¬¸¯µÊ");
    //}
}
