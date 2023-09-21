using System;
using UnityEngine;

public class ObjectHealth : MonoBehaviour, IHealth
{
    [SerializeField]
    private float maxHealth;

    [SerializeField]
    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    #region [IHealth Implementation]
    public float GetHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(float amount)
    {
        amount = Mathf.Abs(amount);
        currentHealth = Mathf.Max(0, currentHealth - amount);

        if(currentHealth == 0)
        {
            OnDeath?.Invoke();
        }
    }
    #endregion

    #region [Event Callback Functions]
    public event Action OnDeath;
    #endregion
}
