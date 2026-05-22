using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] GameObject settingPage;
    [SerializeField] GameObject[] pages;

    int currentPageindex = 0;

    private void Start()
    {
        ShowPage(currentPageindex);
    }

    void ShowPage(int index)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == index);
        }
    }

    public void PrevPageArrow()
    {
        if (currentPageindex > 0)
        {
            currentPageindex--;
            ShowPage(currentPageindex);
        }
    }

    public void NextPageArrow()
    {
        if (currentPageindex < pages.Length - 1)
        {
            currentPageindex++;
            ShowPage(currentPageindex);
        }
    }

    public void OpenSettinsPage()
    {
        bool isOpen = !settingPage.activeSelf;
        settingPage.SetActive(isOpen);

        Time.timeScale = isOpen ? 0 : 1;
    }

    public void CilcktoQuit()
    {
        Application.Quit();
    }

    public void ClicktoCancel()
    {
        settingPage.SetActive(false);
        Time.timeScale = 1;
    }
}
