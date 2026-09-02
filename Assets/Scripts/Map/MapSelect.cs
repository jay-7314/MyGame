using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelect : MonoBehaviour
{
    [SerializeField] string stageName;

    public void GotoNextStage()
    {
        SceneManager.LoadScene(stageName);
    }
}