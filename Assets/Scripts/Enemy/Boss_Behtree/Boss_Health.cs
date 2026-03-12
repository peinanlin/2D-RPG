using UnityEngine;

public class Boss_Health : Enemy_Health
{
    private Boss_DeathHandler deathHandler;

    protected override void Awake()
    {
        base.Awake();
        deathHandler = GetComponent<Boss_DeathHandler>();
    }

    protected override void Die()
    {
        if (isDead)
            return;

        base.Die();
        deathHandler?.HandleDeath();
    }
}