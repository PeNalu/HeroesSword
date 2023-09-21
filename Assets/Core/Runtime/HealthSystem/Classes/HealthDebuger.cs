using UnityEngine;

public class HealthDebuger : MonoBehaviour
{
    [SerializeField]
    private StackHealth stackHealth;

    [SerializeField]
    private float damage = 150f;

    private void Awake()
    {
        //stackHealth.OnDeath += OnDead;
        stackHealth.OnStackDeath += OnDead;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            TakeDamage();
        }
    }

    private void OnDead()
    {
        Debug.Log("Dead");
    }

    private void OnDead(int count)
    {
        Debug.Log($"Dead {count}");
    }

    private void TakeDamage()
    {
        Debug.Log($"Take Damage: {damage}");
        stackHealth.TakeDamage(damage);
    }
}
