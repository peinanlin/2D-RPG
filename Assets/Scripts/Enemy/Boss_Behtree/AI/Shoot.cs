using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Core.Character;

public class Shoot : BossAction
{

    public Transform weaponTransform;
    public ShootWave wirePrefab;
    public float horizontalForce;
    public float verticalForce;

    public bool shakeCamera;

    public override TaskStatus OnUpdate()
    {
        //foreach (var weapon in weapons)
        //{
        //    var projectile = UnityEngine.Object.Instantiate(weapon.projectilePrefab, weapon.weaponTransform.position, Quaternion.identity);
        //    projectile.Shooter = gameObject;

        //    var force = new Vector2(weapon.horizontalForce * transform.localScale.x, weapon.verticalForce);
        //    projectile.SetForce(force);

        //    if (shakeCamera)
        //        CameraController.Instance.ShakeCamera(0.5f);
        //}
        var wire = Object.Instantiate(wirePrefab, weaponTransform.position, Quaternion.identity);
        wire.Shooter = gameObject;
        var force = new Vector2(horizontalForce * -transform.localScale.x, verticalForce);
        wire.SetForce(force);
        wire.SetStats(this.gameObject);
        if (shakeCamera)
            CameraController.Instance.ShakeCamera(0.5f);

        return TaskStatus.Success;
    }
}
