using UnityEngine;

//参考Health,基本一样
public interface IManaBonus
{
    int GetManaBonus(int baseMana);
    int GetManaRecoveryBonus();
}

[RequireComponent(typeof(Level))]
[DisallowMultipleComponent]
public class Mana : Energy
{
    public Level level;
    public LinearInt baseMana = new LinearInt { baseValue = 100 };
    public int baseRecoveryRate = 1;

    IManaBonus[] _bonusComponents;
    IManaBonus[] bonusComponents =>
        _bonusComponents ?? (_bonusComponents = GetComponents<IManaBonus>());


    public override int max
    {
        get
        {
            
            int bonus = 0;
            int baseThisLevel = baseMana.Get(level.current);
            foreach (IManaBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetManaBonus(baseThisLevel);
            return baseThisLevel + bonus;
        }
    }

    public override int recoveryRate
    {
        get
        {
            
            int bonus = 0;
            foreach (IManaBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetManaRecoveryBonus();
            return baseRecoveryRate + bonus;
        }
    }
}
