using System;
using UnityEngine;

public class StackHealth : MonoBehaviour, IHealth
{
    [SerializeField]
    private int stackCount;

    [SerializeField]
    private float health;

    //Stored reuired properties.
    private float currentHealth;
    private int currentStackCount;

    protected virtual void Awake()
    {
        currentHealth = health;
        currentStackCount = stackCount;
    }

    public int GetCurrentStack()
    {
        return currentStackCount;
    }

    #region [IHealth Implementation]
    public float GetHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(float amount)
    {
        amount = Mathf.Abs(amount);
        float fullDamage = amount;
        int deathCount = 0;
        
        while (true)
        {
            float previousHealth = currentHealth;
            float result = Mathf.Max(0, currentHealth - amount);
            if(result == 0)
            {
                deathCount++;
                currentStackCount--;
                if(currentStackCount == 0)
                {
                    OnTakeDamage?.Invoke(fullDamage);
                    OnDeath?.Invoke(this);
                    return;
                }

                amount -= previousHealth;
                currentHealth = health;
                if (amount == 0) break;
            }
            else
            {
                currentHealth = result;
                break;
            }
        }

        if(deathCount > 0)
        {
            OnStackDeath?.Invoke(deathCount);
        }
        OnTakeDamage?.Invoke(fullDamage);
    }
    #endregion

    #region [Event Callback Functions]
    public event Action<StackHealth> OnDeath;
    public event Action<int> OnStackDeath;
    public event Action<float> OnTakeDamage;
    #endregion
}
