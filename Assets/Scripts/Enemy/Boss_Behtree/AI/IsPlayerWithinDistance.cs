using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class IsPlayerWithinDistance : BossConditional
{
    public SharedFloat distance = 5.0f;

    public override TaskStatus OnUpdate()
    {
        if (player == null) return TaskStatus.Failure;
        float d = Mathf.Abs(player.transform.position.x - transform.position.x);
        return d <= distance.Value ? TaskStatus.Success : TaskStatus.Failure;
    }
}
