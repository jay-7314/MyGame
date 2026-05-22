using UnityEngine;

public class VillageManager : MonoBehaviour
{
    [SerializeField] GameObject settingPage;
    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    public void CilcktoQuit()
    {
        Application.Quit();
    }

    public void ClicktoCancel()
    {
        settingPage.SetActive(false);
    }
}
