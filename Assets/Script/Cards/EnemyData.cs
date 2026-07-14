using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ThesisSlave/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Info")]
    public string enemyName;

    [Header("Enemy Stats")]
    public int maxRevisiBar;
    public int baseDamage;

    [Header("Visuals (Optional)")]
    public Sprite enemySprite;
}