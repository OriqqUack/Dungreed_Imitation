using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkedData
{
    ///모든 던전의 방들의 연결 정보 해당 정보에 따라 문들이 활성화되고
    ///캐릭터가 이동가능하게 된다.
    public GameObject RightMap = null;
    public GameObject LeftMap = null;
    public GameObject UpMap = null;
    public GameObject DownMap = null;


    public int Num;
    public int indexX;
    public int indexY;
}
