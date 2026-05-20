using UnityEngine;

public class Quit : MonoBehaviour
{
    [SerializeField] GameObject QuitPage;

    private void OnEnable()
    {
        QuitPage.SetActive(false);

    }
    public void ClickedQuit()
    {
        QuitPage.SetActive(true);
    }

    public void ClickGameOver()
    {
        Application.Quit();
    }

    public void ClickQuitCencel()
    {
        QuitPage.SetActive(false);
    }
}
