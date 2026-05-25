using UnityEngine;

[CreateAssetMenu(fileName ="Speaker", menuName = "Dialogue/Speaker")]

public class SpeakerData : ScriptableObject
{
    public enum SpeakerType
    {
        Player,
        Npc
    }

    public string speakerId;
    public string speakerName;
    public Sprite speakerImg;
    public SpeakerType speakerType;
}
