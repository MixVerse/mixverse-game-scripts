using UnityEngine;

// Ѫ�����ֵ����ӿ�
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

    //Ĭ��level1 �ȼ� ����Ѫ�� 
    public LinearInt baseHealth = new LinearInt { baseValue = 100 };

    //�����ظ���
    public int baseRecoveryRate = 1;

    //Ѫ�����淽ʽ���飬����Ѫ������ķ���
    IHealthBonus[] _bonusComponents;
    IHealthBonus[] bonusComponents =>
        _bonusComponents ?? (_bonusComponents = GetComponents<IHealthBonus>());

    //����Ѫ�����ֵ
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

    //����Ѫ���ظ�����
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
