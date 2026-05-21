using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPage : MonoBehaviour
{
    [SerializeField] GameObject[] charactors;
    [SerializeField] GameObject titleNbtn, loadPage;

    public void ClicktoLoad()
    {
        for(int i = 0; i<charactors.Length; i++)
        {
            charactors[i].SetActive(false);
        }
        titleNbtn.SetActive(false);
        loadPage.SetActive(true);
    }

    public void LoadtoGame()
    {
        SceneManager.LoadScene("Village");    
    }

    public void ReturnToTitle()
    {
        for (int i = 0; i < charactors.Length; i++)
        {
            charactors[i].SetActive(true);
        }
        titleNbtn.SetActive(true);
        loadPage.SetActive(false);
    }
}
