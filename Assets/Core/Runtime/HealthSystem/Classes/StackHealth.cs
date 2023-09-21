using System;
using System.Collections.Generic;
using UnityEngine;

public class StackHealth : MonoBehaviour, IHealth
{
    [SerializeField]
    private int stackCount;

    [SerializeField]
    private float health;

    [SerializeField]
    private List<DamageFilter> damageFilters;

    [SerializeField]
    private List<GroupDeadEvent> groupDeadEvents;

    [SerializeField]
    private List<DeadEvent> deadEvents;

    //Stored reuired properties.
    private float currentHealth;
    private int currentStackCount;
    private List<DamageFilter> currentfilters;
    private List<GroupDeadEvent> currentGroupDeadEvents;
    private List<DeadEvent> currentDeadEvents;

    protected virtual void Awake()
    {
        currentHealth = health;
        currentStackCount = stackCount;

        UnitController unitController = GetComponent<UnitController>();

        currentfilters = new List<DamageFilter>();
        for (int i = 0; i < damageFilters.Count; i++)
        {
            DamageFilter filter = Instantiate(this.damageFilters[i]);
            currentfilters.Add(filter);
            filter.OnEndFilter += OnEndFilter;
        }

        currentGroupDeadEvents = new List<GroupDeadEvent>();
        for (int i = 0;i < groupDeadEvents.Count; i++)
        {
            GroupDeadEvent deadEvent = Instantiate(groupDeadEvents[i]);
            deadEvent.Initialize(unitController);
            currentGroupDeadEvents.Add(deadEvent);
            deadEvent.OnEnd += OnEndDeadEvent;
            OnStackDeath += deadEvent.OnGroupDead;
        }

        currentDeadEvents = new List<DeadEvent>();
        for (int i = 0; i < deadEvents.Count; i++)
        {
            DeadEvent deadEvent = Instantiate(deadEvents[i]);
            deadEvent.Initialize(unitController);
            OnDeath += deadEvent.OnDead;
            currentDeadEvents.Add(deadEvent);
        }

        OnTakeDamage += OnDamaged;
    }

    private void OnDamaged(float damage)
    {
        Logger.GetRuntimeInstance().Log($"{this.gameObject.name} take <color=red>{damage}</color> damage!");
    }

    public int GetCurrentStack()
    {
        return currentStackCount;
    }

    public void AddDamageFilter(DamageFilter damageFilter)
    {
        DamageFilter filter = Instantiate(damageFilter);
        currentfilters.Add(filter);
        filter.OnEndFilter += OnEndFilter;
    }

    public void AddGroupDadeEvent(GroupDeadEvent deadEvent)
    {
        GroupDeadEvent dEvent = Instantiate(deadEvent);
        currentGroupDeadEvents.Add(dEvent);
        dEvent.OnEnd += OnEndDeadEvent;
    }

    public void OnEndFilter(DamageFilter damageFilter)
    {
        damageFilter.OnEndFilter -= OnEndFilter;
        currentfilters.Remove(damageFilter);
    }

    public void OnEndDeadEvent(GroupDeadEvent deadEvent)
    {
        deadEvent.OnEnd -= OnEndDeadEvent;
        currentGroupDeadEvents.Remove(deadEvent);
    }

    public void SetHealth(float value)
    {
        health = value;
        currentHealth = health; 
    }

    #region [IHealth Implementation]
    public float GetHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(float amount)
    {
        amount = (int)Mathf.Abs(amount);

        for (int i = 0; i < currentfilters.Count; i++)
        {
            amount = currentfilters[i].Filter(amount);
        }
        amount = (int)amount;

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
                    OnStackDeath?.Invoke(deathCount);
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
