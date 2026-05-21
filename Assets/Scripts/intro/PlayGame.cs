using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayGame : MonoBehaviour
{
    [SerializeField] GameObject titleNbtn, newGame;
    [SerializeField] GameObject[] enemys;
    [SerializeField] GameObject[] player;
    public void GameStart()
    {
        titleNbtn.SetActive(false);
        for(int i = 0; i<enemys.Length; i++)
        {
            enemys[i].SetActive(false);
        }
        newGame.SetActive(true);
    }

    public void SelectBtn()
    {
        SceneManager.LoadScene("Village");
    }

    public void ReturnToTitle()
    {
        newGame.SetActive(false);
        titleNbtn.SetActive(true);
        for (int i = 0; i < enemys.Length; i++)
        {
            enemys[i].SetActive(true);
        }
        Character_Select.ResetAll();

    }
}
