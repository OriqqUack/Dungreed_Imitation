using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : CreatureController
{
    Vector2 _moveDir = Vector2.zero;
    float _speed = 5.0f;

    float EnvCollectDist { get; set; } = 1.0f;

    public Vector2 MoveDir
    {
        get { return _moveDir; }
        set { _moveDir = value.normalized; }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _speed = 5.0f;
        Managers.Game.OnMoveDirChanged += HandleOnMoveDirChanged;

        StartProjectile();

        return true;
    }

    private void OnDestroy()
    {
        if(Managers.Game != null)
        {
            Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChanged;
        }
    }

    void HandleOnMoveDirChanged(Vector2 dir)
    {
        _moveDir = dir;
    }

    void Update()
    {
        MovePlayer();
        CollectEnv();
    }

    void MovePlayer()
    {
        _moveDir = Managers.Game.MoveDir; // ������Ʈ ������ ��� ����� �ϴ� ���� �δ㽺������ �������� ���� ������ �ǽð� ������ ���Ѵٸ� delegate ���.

        Vector3 dir = _moveDir * _speed * Time.deltaTime;
        transform.position += dir;
    }

    void CollectEnv()
    {
        float sqrCollectDist = EnvCollectDist * EnvCollectDist; //��Ʈ�� ���ϰ� ���ϱ⿡ �̷��� �ι� ������.
        List<GemController> gems = Managers.Object.Gem.ToList();
        foreach(GemController gem in gems)
        {
            Vector3 dir = gem.transform.position - transform.position;
            if(dir.sqrMagnitude <= sqrCollectDist)// sqrMagnitued ��Ʈ �������.
            {
                Managers.Game.Gem += 1;
                Managers.Object.Despawn(gem);
            }
        }

        var findGemes = GameObject.Find("@Grid").GetComponent<Grid>().GatherObjects(transform.position, EnvCollectDist + 0.5f /*���� ũ��*/);

        Debug.Log($"SearchGems({findGemes.Count}) TotalGems({gems.Count})");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        MonsterController target = collision.gameObject.GetComponent<MonsterController>();
        if (target == null)
            return;
    }

    public override void OnDamaged(BaseController attacker, int damage)
    {
        base.OnDamaged(attacker, damage); //������ ����

        Debug.Log($"OnDamaged ! {Hp}");

        //TEMP 
        CreatureController cc = attacker as CreatureController;
        cc?.OnDamaged(this, 10000); // ������ ��
    }

    //TEMP
    #region FireProjectile
    Coroutine _coFireProjectile;

    void StartProjectile()
    {
        if (_coFireProjectile != null)
            StopCoroutine(_coFireProjectile);

        _coFireProjectile = StartCoroutine(CoStartProjectile());
    }

    IEnumerator CoStartProjectile()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);

        while (true)
        {
            ProjectileController pc = Managers.Object.Spawn<ProjectileController>(transform.position);
            if(pc == null)
            {
                Debug.Log("Error");
            }
            pc.SetInfo(1, this, _moveDir);

            yield return wait;
        }
    }
    #endregion
}