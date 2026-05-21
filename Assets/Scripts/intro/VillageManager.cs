using UnityEngine;

public class VillageManager : MonoBehaviour
{
    [SerializeField] GameObject settingPage;

    public void CilcktoQuit()
    {
        Application.Quit();
    }

    public void ClicktoCancel()
    {
        settingPage.SetActive(false);
    }
}
