using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageData
{
    public enum MapType { NOMAL, RESTAURANT, SHOP, MAPTYPEMAX };

    public MapType type;
    public StageData RightMap = null;
    public StageData LeftMap = null;
    public StageData UpMap = null;
    public StageData DownMap = null;

    public int Num;
    public int indexX;
    public int indexY;

    public void InitSttting(int num, int x, int y, MapType type)
    {
        this.Num = num;
        this.indexX = x;
        this.indexY = y;
        this.type = type;

    }// 맵 타입 적용.
    
    public void InitSttting(int num, int x, int y)
    {
        this.Num = num;
        this.indexX = x;
        this.indexY = y;
        this.type = MapType.NOMAL;
    }///////////////////////// 노말용
}
