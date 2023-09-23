using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : BaseController
{
    //나한테 걸어준 버프 목록

    protected float _speed = 1.0f;

    public int Hp { get; set; } = 100;
    public int MaxHp { get; set; } = 100;


    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnDamaged(BaseController attacker, int damage)
    {
        Hp -= damage;
        if(Hp <= 0)
        {
            Hp = 0;
            OnDead();
        }
    }

    protected virtual void OnDead()
    {

    }
}
