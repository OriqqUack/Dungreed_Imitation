using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour
{
    [SerializeField]
    private Vector3 _tmp;
    public GameObject _obj_swing;
    private GameObject _obj_copy;
    private float _rotateDegree;
    private Vector2 _localscale;
    // Start is called before the first frame update
    void Start()
    {
        _localscale = this.transform.localScale;
    }
    // Update is called once per frame
    void Update()
    {
        //SwingRotation();
    }
    void SwingRotation()
    {
        _tmp = this.transform.localPosition;
        _tmp.y = 0.2f;

        _obj_copy = Instantiate(_obj_swing);
        _obj_copy.transform.parent = this.transform;
        _obj_copy.transform.localRotation = Quaternion.Euler(0, 0, 8.377f);
        _obj_copy.transform.localPosition = _tmp;
        this.transform.localScale = _localscale;

        Vector3 mPosition = Input.mousePosition; 
        Vector3 oPosition = transform.position; 
        mPosition.z = oPosition.z - Camera.main.transform.position.z;
        Vector3 target = Camera.main.ScreenToWorldPoint(mPosition);
        float dy = target.y - oPosition.y;
        float dx = target.x - oPosition.x;
        _rotateDegree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.Euler(0f, 0f, _rotateDegree - 90);
    }

    public void SwingEffect()
    {
        SwingRotation();
        Destroy(_obj_copy, 0.2f);
    }

}
