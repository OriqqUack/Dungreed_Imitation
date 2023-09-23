using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : CreatureController
{
    /*void Start()
    {
        부모 start가 막힘
    }*/

    public override bool Init()
    {
        if (base.Init())
            return false;

        //TODO
        ObjectType = Define.ObjectType.Monster;

        return true;
    }

    void FixedUpdate() //물리 움직임은 Fixed
    {
        PlayerController pc = Managers.Object.Player;
        if (pc == null)
            return;
        
        Vector3 dir = pc.transform.position - transform.position;
        Vector3 newPos  = transform.position + dir.normalized * Time.deltaTime * _speed;
        GetComponent<Rigidbody2D>().MovePosition(newPos);

        GetComponent<SpriteRenderer>().flipX = dir.x > 0;   
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController target = collision.gameObject.GetComponent<PlayerController>();
        if (target.IsValid() == false)
            return;
        if (this.IsValid() == false) // pooling에 들어갔는데 물리적 처리가 될때 사용.
            return;

        if (_coDotDamage != null)
            StopCoroutine(_coDotDamage);

        _coDotDamage = StartCoroutine(CoStartDotDamage(target));
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        PlayerController target = collision.gameObject.GetComponent<PlayerController>();
        if (target.IsValid() == false)
            return;
        if (this.IsValid() == false) // pooling에 들어갔는데 물리적 처리가 될때 사용.
            return;

        if (_coDotDamage != null)
            StopCoroutine(_coDotDamage); // 예외 처리

        _coDotDamage = null;
    }

    Coroutine _coDotDamage;
    public IEnumerator CoStartDotDamage(PlayerController target)
    {
        while (true)
        {
            //피해를 입는건 피해를 입는 쪽에서 처리하는게 좋음
            target.OnDamaged(this, 2);
            //target.
            yield return new WaitForSeconds(0.1f);
        }
    }

    protected override void OnDead()
    {
        base.OnDead();

        if(_coDotDamage != null )
            StopCoroutine(_coDotDamage);
        _coDotDamage = null;

        //죽을 때 보석 스폰
        GemController gc = Managers.Object.Spawn<GemController>(transform.position);

        Managers.Object.Despawn(this);
    }
}
