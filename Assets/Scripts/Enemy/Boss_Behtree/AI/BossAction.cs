using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class BossAction : Action
{
    protected Rigidbody2D rb;
    protected Animator anim;
    protected Player player;

    public override void OnAwake()
    {
        player = Player.instance;
        rb=GetComponent<Rigidbody2D>();
        anim=gameObject.GetComponentInChildren<Animator>();
    }
}
