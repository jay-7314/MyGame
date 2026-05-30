using UnityEngine;

public class NewGame : MonoBehaviour
{
    private void OnEnable()
    {
        GameObject player = GameObject.Find("Aren");
        if(player != null)
        {
            player.GetComponent<Character_Select>().CharacterSelect();
            player.GetComponent<Character_Select>().CharacterChange();
        }
    }
}
