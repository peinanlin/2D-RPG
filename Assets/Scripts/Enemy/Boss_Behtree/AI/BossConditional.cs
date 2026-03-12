using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class BossConditional:Conditional
{   
    protected Rigidbody2D rb;
    protected Animator anim;
    protected Player player;

    public override void OnAwake()
    {
        rb=GetComponent<Rigidbody2D>();
        anim=gameObject.GetComponentInChildren<Animator>();
        player = Player.instance;
    }
}
