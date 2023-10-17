using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///미니맵 입니다.
///미니맵도 타일들로 이루어져 있습니다. 월드공간을 미니맵의 사이즈에 맞게
///축소시키고 화면 크기에 맞게 컬링을 진행해서 보여줍니다.
///모든 타일 정보들은 MapManager에서 초기화때 미니맵에 넘겨줍니다.
/////////////////////////////////////////////////////////////////////

public class Minimap : Singleton<Minimap>
{
    //캔버스에 붙여주면 알아서 작동하도록
    //타일은 보여질 크기만큼 전부 만들어 주고 미니맵타일의 정보만 바꿔주면 알아서 바뀌도록
    //몬스터나 캐릭터의 타일은 따로 움직이도록 여기서 조정

    public MinimapTile[] tiles = null;

    public List<MinimapTile> monstertiles = null;

    public List<Transform> monsterposlist = null;

    public MinimapTile playertile = null;

    public MinimapTile TileObj;

    public GameObject panel;

    public Transform PlayerPos = null;

    public Transform OriginBottomLeft = null;

    public Transform OriginTopRight = null;

    public int[] tileInfo = null;

    public MinimapOption _option = new MinimapOption();

    public Vector2Int Scroll;

    public Camera cam;

    public float MinimapMaxX;

    public float MinimapMaxY;

    public Vector3 MinimapBottomLeft;

    public Vector3 MinimapTopRight;

    public bool IsMapInfoExist = false;

    private bool isActive = true;

    public bool IsActive
    {
        get
        {
            return isActive;
        }
        set
        {
            isActive = value;
            root.SetActive(isActive);
            
        }
    }

    

    public GameObject root;

    [Header("Test")]
    public int mx;
    public int my;
    public Vector2Int worldPlayerindex;
    public Vector2Int tempscroll;
    public Vector3 worldplayerinterval;

    [System.Serializable]
    public class MinimapOption
    {
        [Tooltip("미니맵 최대 표현 크기")]
        public int MapMaxX;

        public int MapMaxY;

        public string EnemyTagName;

        public string PlayerTagName;

        [Tooltip("미니맵이 화면의 왼쪽상단에서 떨어져있을 거리")]
        public Vector2 Gappos;

        public float UpdataTiming;

    }

    private float lasttime;

    public int[] Ori_info;
    public int Ori_MaxX;
    public int Ori_MaxY;

    


    public void Init()
    {
        TileObj = Resources.Load<MinimapTile>("Prefabs/Map_Prefabs/MinimapPrefabs/MinimapTile");
        panel = Resources.Load<GameObject>("Prefabs/Map_Prefabs/MinimapPrefabs/MinimapPanel");
    }

    //맵정보가 들어오지 않으면 미니맵을 표시하지 않는다. 맵정보가 들어오면 그때부터 미니맵을 보여주기 시작한다
    //npc ,적 , 플레이어등은 태그를 이용해서 검색해서 찾는다. 찾아올때 각각의 객체에 색별번호를 부여한다. 해당 식별번호가 사라지면 표시를 멈추도록
    //맵에서 사용할 타일들은 프리팹으로 만들어서 인스펙터 창에서 배열에 넣어준다.
    public void MinimapInfoSetting(int[] roominfo, int maxX, int maxY, Transform Ori_B_L_Pos, Transform Ori_T_R_Pos,Transform playerpos)
    {
        
        Ori_MaxX = maxX;
        Ori_MaxY = maxY;
        OriginBottomLeft = Ori_B_L_Pos;
        OriginTopRight = Ori_T_R_Pos;
        Ori_info = roominfo;
        this.PlayerPos = playerpos;

        if (TileObj == null)
            TileObj = Resources.Load<MinimapTile>("Prefabs/Map_Prefabs/MinimapPrefabs/MinimapTile");

        if (panel == null)
            panel = Resources.Load<GameObject>("Prefabs/Map_Prefabs/MinimapPrefabs/MinimapPanel");


        mx = (_option.MapMaxX <= maxX) ? _option.MapMaxX : maxX;
        my = (_option.MapMaxY <= maxY) ? _option.MapMaxY : maxY;

        //cam.pixelWidth
        float tempx = cam.pixelWidth  - _option.Gappos.x;
        float tempy = cam.pixelHeight - _option.Gappos.y;
        Vector2 startpos = new Vector2(tempx, tempy);

        if(root==null)
        {
            root = GameObject.Instantiate(panel);
            root.transform.parent = this.transform;
        }

        if(tiles!=null)
        {
            DestroyTiles();
        }
        

        tiles = new MinimapTile[mx * my];

        //배열과 맞추기 위해서 왼쪽 밑에서부터 생성하기 시작한다.
        for (int y = 0; y < my; y++)
        {
            for (int x = 0; x < mx; x++)
            {
                
                MinimapTile temp = GameObject.Instantiate(TileObj);
                
                temp.p_tiletype = (MinimapTile.MinimapElement)roominfo[x + (y * maxX)];
                
                temp.transform.parent = root.transform;
               
                temp.transform.position = new Vector3(startpos.x + (x * 10), startpos.y + (y * 10), 0);
             
                tiles[x + (y * mx)] = temp;
                //temp.transform.position = startpos + new Vector3(x, y, 0);
            }
        }

        if (playertile != null)
        {
            Destroy(playertile.gameObject);
        }
        playertile = GameObject.Instantiate(TileObj);
        playertile.gameObject.name = "Player";

        playertile.transform.parent = root.transform;
        playertile.p_tiletype = MinimapTile.MinimapElement.Player;

        MinimapMaxX = tiles[mx - 1].transform.position.x - tiles[0].transform.position.x;
        MinimapMaxY = tiles[(my - 1) * mx].transform.position.y - tiles[0].transform.position.y;
        MinimapBottomLeft = tiles[0].transform.localPosition;
        MinimapTopRight = tiles[(mx - 1) + (my - 1) * mx].transform.localPosition;


        IsMapInfoExist = true;
    }


    
    public void DestroyTiles()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] != null)
            {
                Destroy(tiles[i].gameObject);
            }

        }
    }

    public void CreateMonsterTile(List<Transform> monsterlist)
    {
        monsterposlist = monsterlist;
        for(int i=0;i< monsterlist.Count;i++)
        {
            MinimapTile tile = GameObject.Instantiate(TileObj);
            tile.transform.parent = root.transform;
            tile.gameObject.name = "Monster";
            tile.p_tiletype = MinimapTile.MinimapElement.Monster;
            monstertiles.Add(tile);
        }
    }

    //
    public void ElementsMove()
    {
        float x1, y1;
        float originX = OriginTopRight.position.x - OriginBottomLeft.position.x;
        float originY = OriginTopRight.position.y - OriginBottomLeft.position.y;
        Vector2 temp;
        Vector3 val;

        //몬스터가 죽거나 해서 몬스터리스트가 타일리스트보다 작아지면 타일 하나를 삭제해준다.
        if (monsterposlist.Count < monstertiles.Count)
        {
            MinimapTile t = monstertiles[0];
            monstertiles.Remove(t);
            Destroy(t.gameObject);
        }

        if (monsterposlist.Count > 0)
        {
            for (int i = 0; i < monstertiles.Count; i++)
            {
                //monsterposlist[i].position = this.transform.position;
                temp = monsterposlist[i].position - OriginBottomLeft.position;
                x1 = temp.x / originX;//원래 월드의 맵에서 해당 객체의 상대적 위치
                y1 = temp.y / originY;

                //미니맵 에서의 위치로 변환해준다.

                val = new Vector3(MinimapMaxX * x1, MinimapMaxY * y1, 0);

                monstertiles[i].transform.localPosition = MinimapBottomLeft + val;

            }
        }

        //플레이어 타일 이동
        temp = PlayerPos.position - OriginBottomLeft.position;
        //worldplayerinterval = temp;
        x1 = temp.x / originX;//원래 월드의 맵에서 해당 객체의 상대적 위치
        y1 = temp.y / originY;

        val = new Vector3(MinimapMaxX * x1, MinimapMaxY * y1, 0);

        playertile.transform.localPosition = MinimapBottomLeft + val;

        
    }


    //스크롤값에 따라 컬링 진행
    public void MinimapMove()
    {
        if (Ori_MaxX > mx || Ori_MaxY > my)
        {
            Vector3 temp = PlayerPos.position - OriginBottomLeft.position;
            worldplayerinterval = temp;
            worldPlayerindex = new Vector2Int((int)temp.x, (int)temp.y);
            SetScroll(new Vector3Int((int)temp.x, (int)temp.y, 0));

        }
    }

    //컬링이 필요한 경우에만 호출이 된다.
    //플레이어의 월드에서의 타일위치를 넣어주면 거기에 따라서 컬링을 진행
    public void SetScroll(Vector3Int Worldplayerindex)
    {
        Vector2Int tempstart;
        tempstart = new Vector2Int((Worldplayerindex.x - mx / 2)+12, Worldplayerindex.y - my / 2);
        if (tempstart.x < 0) tempstart.x = 0;//bottomleft 걸리는지 확인
        if (tempstart.y < 0) tempstart.y = 0;

        if (tempstart.x + mx >= Ori_MaxX) tempstart.x = Ori_MaxX - mx;//topright 걸리는지 확인
        if (tempstart.y + my >= Ori_MaxY) tempstart.y = Ori_MaxY - my;


        tempscroll = tempstart;
        Vector2Int index;
        for (int x = 0; x < mx; x++)
        {
            for (int y = 0; y < my; y++)
            {
                index = new Vector2Int(tempstart.x + x, tempstart.y + y);
                //Debug.Log($"맵 {x},{y} = {Ori_info[(index.x) + (index.y) * Ori_MaxX]}");
                tiles[x + (y * mx)].p_tiletype = (MinimapTile.MinimapElement)Ori_info[(index.x) + (index.y) * Ori_MaxX];

            }
        }

    }

    public void CheckElement()
    {
        if(IsMapInfoExist)
        {
            if (Time.time >= lasttime)
            {
                lasttime = Time.time + _option.UpdataTiming;

                ElementsMove();
                MinimapMove();

            }
        }
        
    }

    

    private void Awake()
    {
        Init();

    }

    // Update is called once per frame
    void Update()
    {
        CheckElement();
    }
}
