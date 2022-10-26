using UnityEngine;

// 血量最大值增益接口
public interface IHealthBonus
{
    int GetHealthBonus(int baseHealth);
    int GetHealthRecoveryBonus();
}

[RequireComponent(typeof(Level))]
[DisallowMultipleComponent]
public class Health : Energy
{
    public Level level;

    //默认level1 等级 基础血量 
    public LinearInt baseHealth = new LinearInt { baseValue = 100 };

    //基础回复率
    public int baseRecoveryRate = 1;

    //血量增益方式数组，多种血量增益的方法
    IHealthBonus[] _bonusComponents;
    IHealthBonus[] bonusComponents =>
        _bonusComponents ?? (_bonusComponents = GetComponents<IHealthBonus>());

    //计算血量最大值
    public override int max
    {
        get
        {
            int bonus = 0;
            int baseThisLevel = baseHealth.Get(level.current);
            foreach (IHealthBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetHealthBonus(baseThisLevel);
            return baseThisLevel + bonus;
        }
    }

    //计算血量回复速率
    public override int recoveryRate
    {
        get
        {
            
            int bonus = 0;
            foreach (IHealthBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetHealthRecoveryBonus();
            return baseRecoveryRate + bonus;
        }
    }
}
