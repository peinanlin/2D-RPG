using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class IsPlayerOutDistance : BossConditional
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public SharedFloat distance = 5.0f;

    public override TaskStatus OnUpdate()
    {
        if (player == null) return TaskStatus.Failure;
        float d = Mathf.Abs(player.transform.position.x - transform.position.x);
        return d > distance.Value ? TaskStatus.Success : TaskStatus.Failure;
    }
}
