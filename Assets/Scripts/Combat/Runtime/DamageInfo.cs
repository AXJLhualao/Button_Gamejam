using UnityEngine;

public enum DamageType
{
    Physical
}

public struct DamageInfo
{
    public float Amount { get; }
    public DamageType DamageType { get; }
    public Entity Source { get; }

    public DamageInfo(float amount, DamageType damageType, Entity source)
    {
        Amount = amount;
        DamageType = damageType;
        Source = source;
    }
}
