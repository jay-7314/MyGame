using UnityEngine;

public class StageManager : MonoBehaviour
{
    Character_Select select;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        select = FindFirstObjectByType<Character_Select>();
        if (select != null)
        {
            select.enabled = false;
        }
    }
}
