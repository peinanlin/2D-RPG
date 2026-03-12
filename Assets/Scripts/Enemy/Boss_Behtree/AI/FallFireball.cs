using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using DG.Tweening;
using Unity.Mathematics;

public class FallFireball : BossAction
{
    public Collider2D spawnAreaCollider;
    public FireBall fireballPrefab;
    public int spawnCount = 4;
    public float spawnInterval = 0.3f;

    public override TaskStatus OnUpdate()
    {
        var sequence = DOTween.Sequence();
        for (int i = 0; i < spawnCount; i++)
        {
            sequence.AppendCallback(SpawnRocks);
            sequence.AppendInterval(spawnInterval);
        }
        return TaskStatus.Success;
    }

    private void SpawnRocks()
    {
        var randomX = UnityEngine.Random.Range(spawnAreaCollider.bounds.min.x, spawnAreaCollider.bounds.max.x);
        var fireball = Object.Instantiate(fireballPrefab, new Vector2(randomX, spawnAreaCollider.bounds.min.y), quaternion.identity);
        fireball.SetStats(this.gameObject);
        fireball.SetForce(Vector2.zero);
    }
}
