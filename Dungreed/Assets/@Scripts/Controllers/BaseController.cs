using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseController : MonoBehaviour
{
    public ObjectType ObjectType { get; protected set; } // 나만 세팅.

    void Awake()
    {
        Init();
    }

    bool _init = false;
    public virtual bool Init()
    {
        if (_init)
            return false;

        _init = true;
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateController();
    }

    public virtual void UpdateController()
    {

    }
}
