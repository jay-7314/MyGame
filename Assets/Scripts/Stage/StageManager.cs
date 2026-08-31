using UnityEngine;

public class StageManager : MonoBehaviour
{
    Character_Select select;
    void Start()
    {
        select = FindFirstObjectByType<Character_Select>();
        if (select != null)
        {
            select.enabled = false;
        }
    }
}
