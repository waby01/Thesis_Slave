using UnityEngine;

[CreateAssetMenu(fileName = "NewCardData", menuName = "ThesisSlave/Card Data")]
public class CardData : ScriptableObject
{
    [Header("General Info")]
    public string cardName;

    [TextArea(2, 3)]
    public string description;

    [Header("Card Mechanics")]
    public int staminaCost;

    public enum CardType { Damage, Heal, Draw, StaminaBuff }
    public CardType cardType;

    public int effectValue;

    [Header("Visuals (Optional)")]
    public Sprite cardArt;
}