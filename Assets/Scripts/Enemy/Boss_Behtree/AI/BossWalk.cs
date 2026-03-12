using System;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class BossWalk:BossAction
{
    public SharedFloat speed = 5f;
    public SharedFloat stopDistance = 5f;
    public string walkBoolName = "walk";

    public override TaskStatus OnUpdate()
    {
        if (player == null || rb == null) return TaskStatus.Failure;

        float dx = player.transform.position.x - transform.position.x;
        float abs = Mathf.Abs(dx);

        if (abs <= stopDistance.Value)
        {
            // 到达攻击距离：停下并结束（让上层进入攻击分支）
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (anim) anim.SetBool(walkBoolName, false);
            return TaskStatus.Success;
        }
        float dir = Mathf.Sign(dx);
        rb.linearVelocity = new Vector2(dir * speed.Value, rb.linearVelocity.y);

        if (anim) anim.SetBool(walkBoolName, true);
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        if (rb) rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        if (anim) anim.SetBool(walkBoolName, false);
    }
}
