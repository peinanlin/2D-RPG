
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
[TaskCategory("Boss/Common")]
[TaskDescription("Returns Success only when cooldown is ready; otherwise Failure. Consumes cooldown on success.")]
public class Cooldown : BossConditional
{
    public SharedFloat cooldown = 1f;

    // 不想每个实例都写一个 lastTime 变量的话，可以把它做成 SharedFloat 并在 Variables 里建
    public SharedFloat lastTime;

    public override TaskStatus OnUpdate()
    {
        float now = Time.time;

        // 第一次运行时 lastTime 默认 0，会立刻通过一次（一般是你想要的）
        if (now - lastTime.Value >= cooldown.Value)
        {
            lastTime.Value = now; // 消耗冷却
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}
