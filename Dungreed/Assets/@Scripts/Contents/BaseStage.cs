using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;


/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///모든 던전의 방들의 가장 최상위 베이스클래스
///방의 모든 요소들(문, 함정, 몬스터 스폰 포인트, 상자스폰, 빠른이동 비석)을 관리하고
///방이 시작되고 클리어되는 것에 맞춰서 모든 요소들을 통제 합니다.
/////////////////////////////////////////////////////////////////////



public class BaseStage : MonoBehaviour
{
    public enum TileElement { BackGround, Wall, Moveable,Door, ElementMax };

    public enum Chests { Stash, Rare, Yellow, Gold, Boss,ChestMax };

    //던전에서 생성 가능한 상자들 오브젝트
    public GameObject[] ChestPrefabs;

    //자신의 모든 타일들을 벽인지, 배경인지, 빈 공간인지에 따라서 가지고 있는다.
    public Tilemap[] tiles;

    //모든 문
    public Door[] door;

    //모든 몬스터 스폰 포인트
    //public MonsterSpawnPoint[] spawnpoint;

    //모든 상자들
    public List<GameObject> ChestList;

    //각 방들 연결 정보
    public LinkedData StageLinkedData;

    //링크드데이터에 따라서 문이 2개 이상이면 일정 확률로 중간크기, 3개이상이면 일정확률로 큰방, 4개 이상이면 무조건 큰방 등등 의 처리를 해서 최종적으로 방을 생성해준다.
    //public enum STAGECLASS { SMALL, MEDIUM, LARGE };
    public MapManager.ROOMTYPE type;

    public MapManager.ROOMCLASS sizeclass;

    //방의 위치정보
    public Transform StartPos;
    public Vector3Int StartIndex;
    public Transform EndPos;
    public Vector3Int EndInedex;
    //public Rect RoomArea;

    //방의 다양한 변수들
    public GameObject playerobj;
    public MapManager.STAGE NowStage;
    public int NowFloor = 0;
    public int StageNum = -1;
    //방이 잠겼는지 아닌지 방에 스폰된 몬스터들을 모두 잡고 나면 락이 풀린다.
    public bool RoomLocked;
    public Vector2 RoomSize;
    public Transform bottomleft;
    public Transform topright;
    public Rect RoomActiveArea;
    public Rect RoomArea;
    public Vector3 CenterPos = new Vector3(-3000f, -3000f);
    public int MaxX;
    public int MaxY;

    public int[] Roominfo;

    //방 상태 변수&프로퍼티들
    public bool RoomIsClear;
    public bool NowSpawned;
    [SerializeField]
    private bool isteleporter;
    [SerializeField]
    private bool nowplayerenter = false;
    [SerializeField]
    private bool issearched = false;

    public bool IsSearched
    {
        get
        {
            return issearched;
        }
        set
        {
            issearched = value;
        }
    }


    public bool NowPlayerEnter
    {
        get
        {
            return nowplayerenter;
        }
        set
        {
            nowplayerenter = value;
            if(nowplayerenter)
            {
                if(IsSearched==false)
                {
                    IsSearched = true;
                }
                Minimap.Instance.MinimapInfoSetting(Roominfo, MaxX, MaxY, bottomleft, topright, playerobj.transform);
            }
        }
    }

    //public DungeonTeleporter Teleporter;


    public float UpdateSecond = 0.1f;
    private float mytime;


    public bool IsTeleporter
    {
        get
        {
            return isteleporter;
        }
        set
        {
            isteleporter = value;
            
            //Teleporter.IsActive = isteleporter;
        }
    }

    

    public void  LoadDoors()
    {
        door = new Door[(int)Door.DoorType.DoorMax];
        for (Door.DoorType i = Door.DoorType.Up; i < Door.DoorType.DoorMax; i++)
        {
            door[(int)i] = null;
        }

        Door[] temp = GetComponentsInChildren<Door>();
        foreach (var i in temp)
        {
            door[(int)i.type] = i;
            i.gameObject.SetActive(false);
        }
    }

    //자신의 아래에 있는 문들을 받아온다.
    public void Initsetting()
    {
        //70%확률로 텔레포터 활성화
        
        int rand = UnityEngine.Random.Range(0, 100);
        IsTeleporter = (rand <= 70);


        

        Vector2 temp1 = bottomleft.position + new Vector3(2, 1, 0);
        Vector2 temp2 = topright.position + new Vector3(-2, -1, 0);
        RoomActiveArea.x = temp1.x;
        RoomActiveArea.y = temp1.y;
        RoomActiveArea.width = temp2.x - temp1.x;
        RoomActiveArea.height = temp2.y - temp1.y;


        RoomArea.x = bottomleft.position.x;
        RoomArea.y = bottomleft.position.y;
        RoomArea.width = topright.position.x - bottomleft.position.x;
        RoomArea.height = topright.position.y - bottomleft.position.y;

        CenterPos = (topright.position - bottomleft.position) * 0.5f;
        

        //링크정보가 있는데 해당 위치에 문이 없으면 
        //해당 방을 다른 방이랑 교체 한다.
        //링크정보가 있고 해당 방향에 문도 존재하면 문을 만들어 준다.
        //문을 활성화 할때 맵정보에 문도 집어넣어 준다.
        if (StageLinkedData.RightMap!=null)
        {
            if(door[(int)Door.DoorType.Right]!=null)
            {
                door[(int)Door.DoorType.Right].gameObject.SetActive(true);
                door[(int)Door.DoorType.Right].CreateDoor(this.gameObject);
            }
        }
        if(StageLinkedData.LeftMap!=null)
        {
            if (door[(int)Door.DoorType.Left] != null)
            {
                door[(int)Door.DoorType.Left].gameObject.SetActive(true);
                door[(int)Door.DoorType.Left].CreateDoor(this.gameObject);
            }
        }
        if(StageLinkedData.UpMap!=null)
        {
            if (door[(int)Door.DoorType.Up] != null)
            {
                door[(int)Door.DoorType.Up].gameObject.SetActive(true);
                door[(int)Door.DoorType.Up].CreateDoor(this.gameObject);
            }
        }
        if(StageLinkedData.DownMap!=null)
        {
            if (door[(int)Door.DoorType.Down] != null)
            {
                door[(int)Door.DoorType.Down].gameObject.SetActive(true);
                door[(int)Door.DoorType.Down].CreateDoor(this.gameObject);
            }
        }

        if (type == MapManager.ROOMTYPE.Start)
        {
           
            StartPos = transform.Find($"{MapManager.Instance.NowStage}StartGate");
            
            if (playerobj != null)
            {
                playerobj.transform.position = StartPos.position;
                NowPlayerEnter = true;
            }
 
        }
        else if(type == MapManager.ROOMTYPE.End)
        {
            EndPos = transform.Find($"{MapManager.Instance.NowStage}EndGate");
           
        }

    }

    public bool IsDoorExist(Door.DoorType type)
    {
        //bool flag = false;
        if (door[(int)type] == null)
        {
            return false;
        }
        else
        {
            return true;
        }

    }


    public void LoadMapInfo()
    {
        //Ray ray;
        RaycastHit2D[] hit;
        
        Vector3Int bottomleftindex = tiles[(int)TileElement.Wall].WorldToCell(bottomleft.position);
        //Debug.Log($"{name}번방 로드 시작 bottomleft = {bottomleft.position.x},{bottomleft.position.y},{bottomleft.position.z}");
        Vector3Int toprightindex = tiles[(int)TileElement.Wall].WorldToCell(topright.position);
        //Debug.Log($"{name}번방 로드 시작 topright = {topright.position.x},{topright.position.y},{topright.position.z}");
        int maxX = toprightindex.x - bottomleftindex.x + 1;
        MaxX = maxX;
        int maxY = toprightindex.y - bottomleftindex.y + 1;
        MaxY = maxY;
        Roominfo = new int[maxX * maxY];

        Vector3 temp = tiles[(int)TileElement.Wall].GetCellCenterWorld(bottomleftindex);

        for (int x = 0; x < maxX; x++)
        {
            for (int y = 0; y < maxY; y++)
            {
                temp.x = bottomleft.position.x + x;
                temp.y = bottomleft.position.y + y + 0.3f;
              

                hit = Physics2D.RaycastAll(temp, new Vector2(1, 1), 0f);



                Roominfo[x + (y * maxX)] = 0;
                foreach(var a in hit)
                {
                    if (a)
                    {
                        if (a.transform.gameObject.layer == LayerMask.NameToLayer("Wall"))
                        {
                            Roominfo[x + (y * maxX)] = (int)TileElement.Wall;

                          
                        }
                        else if (a.transform.gameObject.layer == LayerMask.NameToLayer("Moveable"))
                        {
                            Roominfo[x + (y * maxX)] = (int)TileElement.Moveable;
                            break;
                            
                        }
                        //if (a.transform.gameObject.layer == LayerMask.NameToLayer("Door"))
                        //{
                        //    Roominfo[x + (y * maxX)] = /*(int)TileElement.Door10;

                        //    GameObject obj = GameObject.Find("TestCircle");
                        //    GameObject copy = GameObject.Instantiate(obj);
                        //    copy.transform.position = a.point;
                        //    copy.name = "Door";
                        //    break;
                        //}

                    }
                }
               

            }
        }


    }

    
    public void CheckRoomState()
    {
        if(NowPlayerEnter)
        {
            if (mytime <= Time.time)
            {
                mytime = Time.time + UpdateSecond;
                //Debug.Log("들어옴");

                //아직 몬스터들이 스폰되지 않았거나 방이 클리어되지 않았을때 캐릭터의 입장을 검사한다.
                if (NowSpawned == false && RoomIsClear == false)
                {
                    //Debug.Log("들어옴");
                    //플레이어가 방 영역 안으로 들어오면 해당 문을 닫고 해당방의 몬스터들을 소환한다. 만약 이미 몬스터들이 다 죽었거나 방이 클리어 되어 있거나 몬스터가 없는 방이면 그냥 그대로 둔다. 
                    if (RoomActiveArea.Contains(playerobj.transform.position))
                    //if (IsEnterPlayArea()) 
                    {
                        //Debug.Log("플레이영역에들어옴");
                        RoomStart();

                    }
                }

                //몹들이 스폰됐을때는 몬스터가 다 죽었는지를 검사한다.
                if (NowSpawned == true)
                {
                    /*if(this.type!=MapManager.ROOMTYPE.Boss)
                    {
                        if (SpawnManager.Instance.NowSpawndList.Count <= 0)
                        {
                            //스폰된 몬스터 전부 죽음
                            RoomClear();
                        }
                    }
                    else
                    {
                        if(BossManager.Instance.cur_Boss.isDie)
                        {
                            RoomClear();
                        }
                    }*/

                }


            }
        }
    }
    //플레이어가 방에 들어왔을때 방을 시작 해준다.
    public void RoomStart()
    {
        
        if (/*spawnpoint.Length != 0*//* &&*/ RoomIsClear == false)
        {
            NowSpawned = true;
            for (int i = 0; i < door.Length; i++)
            {
                if (door[i] != null)
                {
                    //활성화 되어있는 문들을 닫아주고 몬스터들을 스폰한다.
                    if (door[i].gameObject.activeSelf)
                    {
                        door[i].NowDoorLocked = true;
                    }
                }
                
            }

            //텔레포트도 비활성화 시켜준다.
            if (IsTeleporter)
            {
                //Teleporter.IsActive = false;
            }

            /*//몬스터들을 스폰 시켜준다.
            if (spawnpoint[0].type != SpawnManager.MonsterType.Boss)
            {
                for (int j = 0; j < spawnpoint.Length; j++)
                {
                    SpawnManager.Instance.Spawn(spawnpoint[j].type, spawnpoint[j].transform);
                    //MonsterCount++;
                }
            }
            else
            {
                BossManager.Instance.Boss_Start = true;
            }
            Enm_Hp_Bar_Manager.Instance.UpadateList();
            //몬스터에 정보를 넘겨준다.
            Minimap.Instance.CreateMonsterTile(SpawnManager.Instance.NowSpawndList);*/
        }

        //스폰될 몬스터가 없는 방은 바로 클리어 해준다.
        /*if (spawnpoint.Length <= 0)
        {
            RoomIsClear = true;
        }*/

    }

    //방이 클리어 되었을때 다시 텔레포트랑 문들을 활성화 시켜주고 방의 클래스에 따라 확률로 상자를 스폰해준다.
    public void RoomClear()
    {
        if(!RoomIsClear)
        {
            RoomIsClear = true;
            for (int i = 0; i < door.Length; i++)
            {
                if (door[i] != null)
                {
                    //활성화 되어있는 문들을 다시 열어준다.
                    if (door[i].gameObject.activeSelf)
                    {
                        door[i].NowDoorLocked = false;
                    }
                }
            }
            if (IsTeleporter)
            {
                //Teleporter.IsActive = true;
            }
            ChestSpawn();
        }
        
    }

    //방이 클리어 되었을때 해당 방의 타입과 크기에 따라 상자를 스폰 해준다. 
    public void ChestSpawn()
    {
        int per; //해당 방에 스폰될 확률
        int rnd;
        int Crnd;
        int[] CPer = new int[(int)Chests.ChestMax];
        for(int i=0;i<CPer.Length;i++)
        {
            CPer[i] = 0;
        }

        if(type!=MapManager.ROOMTYPE.Boss)
        {
            if (sizeclass == MapManager.ROOMCLASS.SMALL)
            {
                per = 20;
                CPer[(int)Chests.Stash] = 85;
                CPer[(int)Chests.Rare] = 15;
                CPer[(int)Chests.Yellow] = 5;
            }
            else if (sizeclass == MapManager.ROOMCLASS.LARGE)
            {
                per = 60;
                CPer[(int)Chests.Stash] = 70;
                CPer[(int)Chests.Rare] = 20;
                CPer[(int)Chests.Yellow] = 10;
                CPer[(int)Chests.Gold] = 15;
            }
            else
            {
                per = 100;
                CPer[(int)Chests.Stash] = 40;
                CPer[(int)Chests.Rare] = 35;
                CPer[(int)Chests.Yellow] = 25;
                CPer[(int)Chests.Gold] = 35;
            }


            rnd = UnityEngine.Random.Range(0, 100);

            if (rnd <= per)
            {
                Crnd = UnityEngine.Random.Range(0, 100);
                for (int i = (int)Chests.Yellow; i >= (int)Chests.Stash; i--)
                {
                    if(Crnd<=CPer[i])
                    {
                        GameObject obj = GameObject.Instantiate(ChestPrefabs[i]);
                        //obj.transform.position = spawnpoint[spawnpoint.Length - 1].transform.position;
                        ChestList.Add(obj);
                        break;
                    }
                }
                if(Crnd<=CPer[(int)Chests.Gold])
                {
                    GameObject obj = GameObject.Instantiate(ChestPrefabs[(int)Chests.Gold]);
                    //obj.transform.position = spawnpoint[spawnpoint.Length - 1].transform.position + new Vector3(1, 0, 0);
                    ChestList.Add(obj);
                }

            }
        }
        else
        {
            GameObject obj = GameObject.Instantiate(ChestPrefabs[(int)Chests.Boss]);
            //obj.transform.position = spawnpoint[spawnpoint.Length - 1].transform.position;
            ChestList.Add(obj);
        }
    }

    public bool IsEnterPlayArea()
    {
        bool flag = false;
        Vector3 pos = playerobj.transform.position;
        if (pos.x >= RoomActiveArea.xMin && pos.x <= RoomActiveArea.xMax) 
        {
            if (pos.y <= RoomActiveArea.yMin && pos.y >= RoomActiveArea.yMax) 
            {
                flag = true;
            }
        }
        return flag;
    }




    private void Awake()
    {
        playerobj = GameObject.Find("Player");
        //Teleporter = GetComponentInChildren<DungeonTeleporter>();
        bottomleft = transform.Find("BottomLeft");
        topright = transform.Find("TopRight");
        tiles = new Tilemap[(int)TileElement.ElementMax];
        //ChestPrefabs = (GameObject[])Resources.LoadAll("Prefabs/Ui_Prefabs/Inventory/Tresure");
        Tilemap[] temp = GetComponentsInChildren<Tilemap>();
        foreach (var a in temp)
        {
            if (a.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                tiles[(int)TileElement.Wall] = a;
            }
            else if (a.gameObject.layer == LayerMask.NameToLayer("BackGround"))
            {
                tiles[(int)TileElement.BackGround] = a;
            }
            else if (a.gameObject.layer == LayerMask.NameToLayer("Moveable"))
            {
                tiles[(int)TileElement.Moveable] = a;
            }
        }
        //spawnpoint = GetComponentsInChildren<MonsterSpawnPoint>();
        LoadMapInfo();
        LoadDoors();
    }

    int count = 0;
    private void Update()
    {
        CheckRoomState();
        if(ChestList.Count>0)
        {
            /*for(int i=0;i<ChestList.Count;i++)
            {
                if (ChestList[i].GetComponent<GoldTresure>().Open)
                {
                    count++;  
                }
                else
                {
                    
                    break;
                }
            }
            if(count>=ChestList.Count)
            {
                for(int i=0;i<ChestList.Count;i++)
                {
                    Destroy(ChestList[i].gameObject, 5f);
                }
                ChestList.Clear();
            }*/
        }
    }
}
