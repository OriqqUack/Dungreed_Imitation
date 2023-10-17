using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///던전을 생성할때 맵이 생성될 자리를 미리 정해놓을때 사용 합니다.
///자신의 인덱스 번호와 연결된 방들의 정보를 가지고 있습니다.
/////////////////////////////////////////////////////////////////////

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

    }
    public void InitSttting(int num, int x, int y)
    {
        this.Num = num;
        this.indexX = x;
        this.indexY = y;
        this.type = MapType.NOMAL;

    }
}
