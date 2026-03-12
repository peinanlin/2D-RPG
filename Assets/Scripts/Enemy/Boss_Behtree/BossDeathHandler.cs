using System.Collections;
using BehaviorDesigner.Runtime;
using UnityEngine;

public class Boss_DeathHandler : MonoBehaviour
{
    [SerializeField] private string deathTriggerName = "dead";
    [SerializeField] private float destroyDelay = 2f;
    [SerializeField] private bool disableColliderOnDeath = true;
    [SerializeField] private bool stopRigidbodyOnDeath = true;
    [SerializeField] private bool destroyAfterDeath = false;

    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D[] colliders;
    private BehaviorTree behaviorTree;
    private Entity_Combat combat;
    private Entity_Health health;

    private bool deathHandled;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<Collider2D>();
        behaviorTree = GetComponent<BehaviorTree>();
        combat = GetComponent<Entity_Combat>();
        health = GetComponent<Entity_Health>();
    }

    public void HandleDeath()
    {
        if (deathHandled)
            return;

        deathHandled = true;

        // 1. 停行为树，避免继续走攻击/移动逻辑
        if (behaviorTree != null)
            behaviorTree.enabled = false;

        // 2. 停止攻击逻辑
        if (combat != null)
            combat.enabled = false;

        // 3. 停止移动
        if (rb != null && stopRigidbodyOnDeath)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
        }

        // 4. 先禁用受击/碰撞，避免死后还被打或推
        if (disableColliderOnDeath)
        {
            foreach (var col in colliders)
            {
                if (col != null)
                    col.enabled = false;
            }
        }

        // 5. 播放死亡动画
        if (anim != null)
        {
            anim.ResetTrigger("attack");
            anim.ResetTrigger("fireball");
            anim.ResetTrigger("fireball_end");
            anim.ResetTrigger("wire");
            anim.SetBool("walk", false);
            anim.SetTrigger(deathTriggerName);
        }

        // 6. 如果你现在还没做动画事件，就先用延迟收尾
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(destroyDelay);

        if (destroyAfterDeath)
            Destroy(gameObject);
    }

    // 如果你后面在死亡动画最后一帧加 Animation Event，可以调用这个
    public void OnDeathAnimationFinished()
    {
        if (destroyAfterDeath)
            Destroy(gameObject);
    }
}