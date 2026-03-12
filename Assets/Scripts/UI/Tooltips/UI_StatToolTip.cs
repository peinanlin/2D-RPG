using TMPro;
using UnityEngine;

public class UI_StatToolTip : UI_ToolTip
{
    private Player_Stats playerStats;
    private TextMeshProUGUI statToolTipText;

    protected override void Awake()
    {
        base.Awake();
        playerStats = FindFirstObjectByType<Player_Stats>();
        statToolTipText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ShowToolTip(bool show, RectTransform targetRect,StatType statType)
    {
        base.ShowToolTip(show, targetRect);
        statToolTipText.text = GetStatTextByType(statType);
    }

    public string GetStatTextByType(StatType type)
    {
        switch (type)
        {
            // 主要属性
            case StatType.Strength:
                return "每点力量增加 1 点物理伤害" +
                       "\n每点力量增加 0.5% 暴击伤害";

            case StatType.Agility:
                return "每点敏捷增加 0.3% 暴击率" +
                       "\n每点敏捷增加 0.5% 闪避率";

            case StatType.Intelligence:
                return "每点智力增加 0.5% 元素抗性" +
                       "\n每点智力增加 1 点元素伤害" +
                       "\n如果所有元素伤害为 0, 则不会获得该加成";

            case StatType.Vitality:
                return "每点体质增加 5 点最大生命值" +
                       "\n每点体质增加 1 点护甲";

            // 物理伤害
            case StatType.Damage:
                return "决定你的攻击造成的物理伤害";

            case StatType.CritChance:
                return "你的攻击造成暴击的概率";

            case StatType.CritPower:
                return "提高暴击时造成的伤害倍率";

            case StatType.ArmorReduction:
                return "你的攻击可以无视敌人一定百分比的护甲";

            case StatType.AttackSpeed:
                return "决定你的攻击速度";

            // 防御
            case StatType.MaxHealth:
                return "决定你的最大生命值上限";

            case StatType.HealthRegen:
                return "每秒恢复的生命值";

            case StatType.Armor:
                return "减少受到的物理伤害" +
                       "\n护甲减伤最高为 85%" +
                       "\n当前减伤：" + playerStats.GetArmorMitigation(0) * 100 + "%";

            case StatType.Evasion:
                return "完全躲避攻击的概率" +
                       "\n最高为 85%";

            // 元素伤害
            case StatType.IceDamage:
                return "决定你的攻击造成的冰霜伤害";

            case StatType.FireDamage:
                return "决定你的攻击造成的火焰伤害";

            case StatType.LightningDamage:
                return "决定你的攻击造成的闪电伤害";

            case StatType.ElementalDamage:
                return "元素伤害是三种元素伤害的组合" +
                       "\n数值最高的元素会触发对应的元素状态效果并造成全部伤害" +
                       "\n另外两个元素会额外提供 50% 的伤害加成";

            // 元素抗性
            case StatType.IceResistance:
                return "减少受到的冰霜伤害";

            case StatType.FireResistance:
                return "减少受到的火焰伤害";

            case StatType.LightningResistance:
                return "减少受到的闪电伤害";

            default:
                return "该属性暂无说明";
        }
    }
}
