using System;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    public GameObject Shooter { get; set; }

    protected Vector2 force;

    [Header("VFX")]
    public ParticleSystem trailEffect;      // 飞行/下落过程持续播放的尾焰/拖尾
    public ParticleSystem explosionEffect;  // 命中时爆炸
    [SerializeField] protected GameObject onHitVfx; // 命中目标时的VFX（通用）

    [Header("Life")]
    public float lifeTime = 6.0f;

    [Header("Collision Destroy")]
    public LayerMask groundLayers;          // 只撞到地面才销毁（推荐）
    public float destroyDelay = 0.0f;       // 命中后延迟销毁（给爆炸留时间）

    [Header("Hit Detect")]
    [Tooltip("可被命中的对象层（玩家/敌人都可以放进来）")]
    [SerializeField] protected LayerMask whatIsEnemy;
    [Tooltip("命中检测的圆心；不填默认用自身transform")]
    [SerializeField] protected Transform hitCheck;
    [Tooltip("OverlapCircle 半径")]
    [SerializeField] protected float hitRadius = 0.75f;
    [Tooltip("二次确认距离阈值（<=0 表示不做二次确认）")]
    [SerializeField] protected float hitDistanceThreshold = 0.75f;
    [Tooltip("是否只造成一次伤害（避免每帧掉血）")]
    [SerializeField] protected bool damageOnlyOnce = true;

    [Header("Damage Source")]
    [Tooltip("伤害来源（例如Boss的Entity_Stats）")]
    [SerializeField] protected Entity_Stats sourceStats;
    [SerializeField] protected DamageScaleData damageScaleData;

    public event Action<Skill> OnProjectileDestroyed;

    protected bool hasDamaged = false;   // 通用：是否已造成过伤害
    protected ElementType usedElement;

    public abstract void SetForce(Vector2 force);

    /// <summary>
    /// 让外部在生成后设置伤害来源（Boss/玩家等）
    /// </summary>
    public virtual void SetStats(GameObject source)
    {
        if (source == null) return;
        sourceStats = source.GetComponent<Entity_Stats>();
    }

    protected virtual void Awake()
    {
        // 默认命中检测点
        if (hitCheck == null) hitCheck = transform;

        // 默认缩放数据（避免空引用）
        if (damageScaleData == null)
            damageScaleData = new DamageScaleData();

        // 确保过程粒子播放
        if (trailEffect != null)
        {
            trailEffect.Clear(true);
            trailEffect.Play(true);
        }
    }

    protected virtual void Start()
    {
        // 通用生命周期销毁
        if (lifeTime > 0f)
            Invoke(nameof(DestroyProjectile), lifeTime);
    }

    protected virtual void Update()
    {
        // 通用：持续检测命中并造成伤害
        TryDamageTargets();
    }

    /// <summary>
    /// 通用命中检测 + 伤害结算
    /// </summary>
    protected virtual void TryDamageTargets()
    {
        if (damageOnlyOnce && hasDamaged) return;

        // 没有伤害来源就不结算（避免 GetAttackData 空引用）
        if (sourceStats == null) return;

        var cols = Physics2D.OverlapCircleAll(hitCheck.position, hitRadius, whatIsEnemy);
        if (cols == null || cols.Length == 0) return;

        foreach (var c in cols)
        {
            if (c == null) continue;
            if (Shooter != null && c.gameObject == Shooter) continue;

            // 二次确认距离（你之前的 0.75 逻辑）
            if (hitDistanceThreshold > 0f)
            {
                float dist = Vector2.Distance(transform.position, c.transform.position);
                if (dist > hitDistanceThreshold) continue;
            }

            // 伤害
            bool gotHit = DealDamageTo(c.transform);

            if (gotHit)
            {
                hasDamaged = true;

                // 命中即消失/爆炸：默认走爆炸销毁（也可改为 DestroyProjectile）
                ExplodeAndDestroy();
                break;
            }
        }
    }

    /// <summary>
    /// 通用伤害结算：TakeDamage + 状态 + onHitVfx
    /// </summary>
    protected virtual bool DealDamageTo(Transform target)
    {
        if (target == null) return false;

        var damageable = target.GetComponent<IDamageable>();
        if (damageable == null) return false;

        AttackData attackData = sourceStats.GetAttackData(damageScaleData);

        float physDamage = attackData.phyiscalDamage;
        float elemDamage = attackData.elementalDamage;
        ElementType element = attackData.element;

        bool gotHit = damageable.TakeDamage(physDamage, elemDamage, element, transform);

        if (element != ElementType.None)
        {
            var statusHandler = target.GetComponent<Entity_StatusHandler>();
            statusHandler?.ApplyStatusEffect(element, attackData.effectData);
        }

        if (gotHit && onHitVfx != null)
            Instantiate(onHitVfx, target.position, Quaternion.identity);

        usedElement = element;
        return gotHit;
    }

    protected void DestroyProjectile()
    {
        OnProjectileDestroyed?.Invoke(this);
        Destroy(gameObject);
    }

    protected void ExplodeAndDestroy()
    {
        // 播放爆炸（把爆炸粒子和弹体解耦）
        if (explosionEffect != null)
        {
            var vfx = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            vfx.Play(true);
            Destroy(vfx.gameObject, vfx.main.duration + vfx.main.startLifetime.constantMax);
        }

        // 停止拖尾（可选）
        if (trailEffect != null)
            trailEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        if (destroyDelay <= 0f) DestroyProjectile();
        else Invoke(nameof(DestroyProjectile), destroyDelay);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 不能打到自己
        if (collision.gameObject == Shooter) return;

        // 只在碰到地面层时销毁
        if (((1 << collision.gameObject.layer) & groundLayers) != 0)
            ExplodeAndDestroy();
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Transform p = hitCheck != null ? hitCheck : transform;

        Gizmos.DrawWireSphere(p.position, hitRadius);
        if (hitDistanceThreshold > 0f)
            Gizmos.DrawWireSphere(transform.position, hitDistanceThreshold);
    }
}
