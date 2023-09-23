using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : CreatureController
{
    /*void Start()
    {
        �θ� start�� ����
    }*/

    public override bool Init()
    {
        if (base.Init())
            return false;

        //TODO
        ObjectType = Define.ObjectType.Monster;

        return true;
    }

    void FixedUpdate() //���� �������� Fixed
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
        if (this.IsValid() == false) // pooling�� ���µ� ������ ó���� �ɶ� ���.
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
        if (this.IsValid() == false) // pooling�� ���µ� ������ ó���� �ɶ� ���.
            return;

        if (_coDotDamage != null)
            StopCoroutine(_coDotDamage); // ���� ó��

        _coDotDamage = null;
    }

    Coroutine _coDotDamage;
    public IEnumerator CoStartDotDamage(PlayerController target)
    {
        while (true)
        {
            //���ظ� �Դ°� ���ظ� �Դ� �ʿ��� ó���ϴ°� ����
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

        //���� �� ���� ����
        GemController gc = Managers.Object.Spawn<GemController>(transform.position);

        Managers.Object.Despawn(this);
    }
}
