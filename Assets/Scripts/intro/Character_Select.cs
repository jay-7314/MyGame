using UnityEngine;
using UnityEngine.UI;

public class Character_Select : MonoBehaviour
{
    Animator anim;
    public bool isSelect = false;
    public static Character_Select currentSelected = null;

    [SerializeField] RawImage characterImgs;
    [SerializeField] Text characterStory, characterFeature;

    [SerializeField] Texture2D myImgs;
    [TextArea(2,10)]
    [SerializeField] string mystory;
    [TextArea(2,10)]
    [SerializeField] string myfeature;

    [SerializeField] GameObject newGame;

    private void OnEnable()
    {
        anim = GetComponent<Animator>();
        isSelect = true;
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
        anim.SetBool("isRun", isSelect);
    }

    private void OnMouseDown()
    {
        CharacterSelect();
        CharacterChange();
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

   public void CharacterChange()
    {
        characterImgs.texture = myImgs;
        characterStory.text = mystory;
        characterFeature.text = myfeature;
    }

    public static void ResetAll()
    {

        Character_Select[] all = FindObjectsByType<Character_Select>(FindObjectsSortMode.None);
        for(int i = 0; i< all.Length; i++)
        {
            all[i].isSelect = false;
        }
        currentSelected = null;
    }

}
