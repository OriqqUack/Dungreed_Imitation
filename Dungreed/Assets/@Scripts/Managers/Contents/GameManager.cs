using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager //���� ���� ����
{
    Vector2 _moveDir;

    public event Action<Vector2> OnMoveDirChanged;

    #region ��ȭ
    public int Gold { get; set; }
    public int Gem { get; set; }
    #endregion

    #region �̵�
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
