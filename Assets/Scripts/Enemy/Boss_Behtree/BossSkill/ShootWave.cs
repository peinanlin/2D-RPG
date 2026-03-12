using UnityEngine;

public class ShootWave : Skill
{
    public float speed = 5.0f;

    private Vector3 direction;
    private Vector3 velocity;

    public override void SetForce(Vector2 force)
    {
        this.force = force;
        direction = ((Vector3)force).normalized;
    }

    protected override void Update()
    {
        base.Update(); // 关键：保留 Skill 的通用命中检测/伤害结算

        velocity += direction * speed * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
    }
}