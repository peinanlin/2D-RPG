using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FireBall : Skill
{
    [Header("Physics")]
    public float gravityScale = 1.0f;

    private Rigidbody2D rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
    }

    public override void SetForce(Vector2 force)
    {
        this.force = force;
        rb.linearVelocity = force;
    }
}