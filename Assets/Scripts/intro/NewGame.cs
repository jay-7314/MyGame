using UnityEngine;

public class NewGame : MonoBehaviour
{
    private void OnEnable()
    {
        GameObject player = GameObject.Find("Player0");
        if(player != null)
        {
            player.GetComponent<Character_Select>().CharacterSelect();
        }
    }
}
