using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager //게임 공통 변수
{
    Vector2 _moveDir;

    public event Action<Vector2> OnMoveDirChanged;

    #region 재화
    public int Gold { get; set; }
    public int Gem { get; set; }
    #endregion

    #region 이동
    public Vector2 MoveDir
    {
        get { return _moveDir; }
        set 
        { 
            _moveDir = value;
            OnMoveDirChanged?.Invoke(_moveDir);
        }
    }
    #endregion
}
