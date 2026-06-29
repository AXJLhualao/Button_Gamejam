using System;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private int gold;

    public event Action<int> OnGoldChanged;

    public int Gold => gold;

    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        gold += amount;
        OnGoldChanged?.Invoke(gold);
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0 || gold < amount) return false;
        gold -= amount;
        OnGoldChanged?.Invoke(gold);
        return true;
    }

    public void SetGold(int amount)
    {
        gold = Mathf.Max(0, amount);
        OnGoldChanged?.Invoke(gold);
    }
}
