using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Data/Skill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public Sprite icon;
    public float damage;
    public float cooldown;
    public float manaCost;
    public string description;
}
