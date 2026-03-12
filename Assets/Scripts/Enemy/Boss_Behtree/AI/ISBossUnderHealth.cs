using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class ISBossUnderHealth : BossConditional
{
    public float healththref = 0.5f;
    public Enemy_Health bossHealth;
    public Entity_Stats bossStats;
    public override void OnAwake()
    {
        bossHealth = GetComponent<Enemy_Health>();
        bossStats=GetComponent<Entity_Stats>();
    }

    public override TaskStatus OnUpdate()
    {
        
        if (bossHealth && bossStats)
        {
            if (bossHealth.GetCurrentHealth() / bossStats.GetMaxHealth() <= healththref) return TaskStatus.Success;
            else return TaskStatus.Failure;
        }
        Debug.Log("null");
        return TaskStatus.Failure;
    }
}
