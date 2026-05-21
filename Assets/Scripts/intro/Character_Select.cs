using UnityEngine;

public class Character_Select : MonoBehaviour
{
    Animator anim;
    bool isSelect = false;
    static Character_Select currentSelected = null;

    [SerializeField] GameObject newGame;

    private void OnEnable()
    {
        anim = GetComponent<Animator>();
    }

    private void OnDisable()
    {
        if(currentSelected == this)
        {
            currentSelected = null;
            isSelect = false;
        }
    }

    public void Update()
    {
        anim.SetBool("isSelect", isSelect);
    }

    private void OnMouseDown()
    {
        CharacterSelect();
    }

    public void CharacterSelect()
    {
        if (!newGame.activeSelf) return;
        
        if(currentSelected != null && currentSelected != this)
        {
            currentSelected.isSelect = false;
        }
        isSelect = true;
        currentSelected = this;
    }

}
