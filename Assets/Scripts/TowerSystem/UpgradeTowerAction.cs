using UnityEngine;

[CreateAssetMenu(menuName = "TowerDefense/Actions/Upgrade Tower Action")]
public class UpgradeTowerAction : ScriptableObject
{
    public int damageModifier;
    public float rangeModifier;
    public float weightModifier;
    public int costModifier;
}