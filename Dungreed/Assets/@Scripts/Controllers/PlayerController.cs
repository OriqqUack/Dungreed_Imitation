using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : CreatureController
{
    Vector2 _moveDir = Vector2.zero;
    Vector2 _mouse;
    Rigidbody2D _rid2;
    SpriteRenderer _spr;
    Animator _anim;
    private bool _isJumpLong = false;
    private bool _isJump;
    private bool _isrun;
    private bool _canDash = true;
    private bool _isDashing;
    private float _dashingPower = 30f;
    private float _dashingTime = 0.2f;
    private float _dashingCooldown = 1f;
    private float _idle_run_ratio;
    private float _idle_jump_ratio;
    //--------------------------------------
    public float _weaponattackspeed = 0.1f;
    public bool _attack = false;

    [SerializeField]
    float _defalutSpeed;

    [SerializeField]
    private float _jumpPower;

    [SerializeField]
    private TrailRenderer _tr;

    public GameObject _weapon;
    public GameObject _sword_body;
    public GameObject _shot_Sword;

    float EnvCollectDist { get; set; } = 1.0f;

    private void Start()
    {
        _rid2 = GetComponent<Rigidbody2D>();
        _spr = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        _shot_Sword.SetActive(true);
    }

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
        if (_isDashing)
            return;

        Attack();
        MovePlayer();
        JumpPlayer();
        AnimPlayer();
        CollectEnv();
    }

    private void FixedUpdate()
    {
        if (_isDashing)
            return;

        DirPlayer();
        JumpControl();
    }

    IEnumerator cooltime(float cool)
    {
        yield return new WaitForSeconds(cool);
        _attack = false;
    }

    protected void Attack()
    {
        if (Input.GetMouseButtonDown(0) && !_attack)
        {
            _attack = true;
            StartCoroutine(cooltime(_weaponattackspeed));
            _weapon.GetComponent<Weapon>().Attack();
        }
    }

    void DirPlayer()
    {
        _mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (_mouse.x - this.transform.position.x > 0)
            _spr.flipX = false;
        else
            _spr.flipX = true;
    }

    void MovePlayer()
    {
        float hor = Input.GetAxis("Horizontal") * _defalutSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
        {
            _isrun = true;
            hor = _defalutSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _isrun = true;
            hor = -_defalutSpeed * Time.deltaTime;
        }
        
        this.transform.Translate(new Vector3(hor, 0f, 0f));

        if (Input.GetMouseButtonDown(1) && _canDash)
            StartCoroutine(Dash());

        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
            _isrun = false;
    }

    void JumpPlayer()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_isJump)
        {
            _isJumpLong = true;
            _isJump = true;
            _rid2.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
            _isJumpLong = false;
    }

    void JumpControl()
    {
        if (_isJumpLong && _rid2.velocity.y > 0)
            _rid2.gravityScale = 4.0f;
        else
            _rid2.gravityScale = 8.5f;
    }

    private IEnumerator Dash()
    {
        Vector2 _mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dashDirection = (_mouse - (Vector2)transform.position).normalized;

        _canDash = false;
        _isDashing = true;
        float originalGravity = _rid2.gravityScale;
        _rid2.gravityScale = 0f;

        _rid2.velocity = dashDirection.normalized * _dashingPower;

        _tr.emitting = true;
        yield return new WaitForSeconds(_dashingTime);
        _tr.emitting = false;
        _rid2.gravityScale = originalGravity;
        _isDashing = false;
        yield return new WaitForSeconds(_dashingCooldown);
        _canDash = true;
        _rid2.velocity = Vector2.zero;
    }

    void AnimPlayer()
    {
        if (!_isJump)
        {
            if (_isrun)
            {
                _idle_run_ratio = Mathf.Lerp(_idle_run_ratio, 1, 10.0f * Time.deltaTime);
                _anim.SetFloat("idle_run_ratio", 1);
                _anim.Play("Idle_Run");
            }
            else
            {
                _idle_run_ratio = Mathf.Lerp(_idle_run_ratio, 0, 10.0f * Time.deltaTime);
                _anim.SetFloat("idle_run_ratio", 0);
                _anim.Play("Idle_Run");
            }
        }
        else
        {
            _idle_jump_ratio = Mathf.Lerp(_idle_jump_ratio, 1, 10.0f * Time.deltaTime);
            _anim.SetFloat("idle_jump_ratio", 1);
            _anim.Play("Idle_Jump");
        }
    }

    void OnCollisionEnter2D(Collision2D collision) //충돌 감지
    {
        if (collision.gameObject.tag == "Floor")
        { //tag가 Floor인 오브젝트와 충돌했을 때
            _isJump = false; //isJumping을 다시 false로
        }
    }

    void CollectEnv()
    {
        float sqrCollectDist = EnvCollectDist * EnvCollectDist; //루트가 부하가 심하기에 이렇게 두번 곱해줌.
        List<GemController> gems = Managers.Object.Gem.ToList();
        foreach(GemController gem in gems)
        {
            Vector3 dir = gem.transform.position - transform.position;
            if(dir.sqrMagnitude <= sqrCollectDist)// sqrMagnitued 루트 씌우기전.
            {
                Managers.Game.Gem += 1;
                Managers.Object.Despawn(gem);
            }
        }

        var findGemes = GameObject.Find("@Grid").GetComponent<Grid>().GatherObjects(transform.position, EnvCollectDist + 0.5f /*보석 크기*/);

        Debug.Log($"SearchGems({findGemes.Count}) TotalGems({gems.Count})");
    }

    public override void OnDamaged(BaseController attacker, int damage)
    {
        base.OnDamaged(attacker, damage); //데미지 입음

        Debug.Log($"OnDamaged ! {Hp}");

        //TEMP 
        CreatureController cc = attacker as CreatureController;
        cc?.OnDamaged(this, 10000); // 데미지 줌
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
