using System;
using UnityEngine;

[Serializable]
public class DialogueChoice
{
    public string choiceText;
    public DialogueData nextDialogue;
    public GameFlagKey setFlagOnSelect;
    public bool isNextScene = false;
}