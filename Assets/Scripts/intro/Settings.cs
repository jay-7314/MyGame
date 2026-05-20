using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] GameObject settingsPage;

    private void OnEnable()
    {
        settingsPage.SetActive(false);
    }

    public void ClickedSettings()
    {
        settingsPage.SetActive(true);
    }

    public void SettingSaveBtn()
    {
        settingsPage.SetActive(false);
    }

    public void SettingCancelBtn()
    {
        settingsPage.SetActive(false);
    }
}
