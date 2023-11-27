using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public SpriteRenderer _sprite;
    public Transform _player;
    public Transform _sword;
    public GameObject _swing;
    private Vector2 _localscale;
    [SerializeField]
    private float _rotateDegree;
    [SerializeField]
    private float _f_tmp = 0;
    private bool _motion = false;

    void Start()
    {
        _localscale = this.transform.localScale;
    }
    public void Attack()
    {
        if (_motion == false)
        {
            _f_tmp = -180; //0~90
            _motion = true;
        }
        else if (_motion == true)
        {
            _f_tmp = 0;
            _motion = false;
        }
        _swing.GetComponent<Swing>().SwingEffect();
    }

    void Update()
    {
        if(!_sprite.flipX)
        {
            _localscale.x = -1f;
            _localscale.y = 1f;
        }
        else
        {
            _localscale.x = 1f;
            _localscale.y = -1f;
        }

        this.transform.localScale = _localscale;
        Vector3 mPosition = Input.mousePosition;
        Vector3 oPosition = transform.position;

        mPosition.z = oPosition.z - Camera.main.transform.position.z;
        Vector3 target = Camera.main.ScreenToWorldPoint(mPosition);

        float dy = target.y - oPosition.y;
        float dx = target.x - oPosition.x;

        _rotateDegree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, (_rotateDegree + 90) + _f_tmp);
    }
}
