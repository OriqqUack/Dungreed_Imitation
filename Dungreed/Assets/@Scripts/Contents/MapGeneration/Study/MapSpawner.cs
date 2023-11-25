using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class MapSpawner : Singleton<MapSpawner>
{
    public enum DIRECTION { NODATA = -1, UP, RIGHT, DOWN, LEFT, MAX };

    public Dictionary<DIRECTION, Vector2Int> dirIndex = new Dictionary<DIRECTION, Vector2Int>()
    {
        { DIRECTION.UP,new Vector2Int(0,1)},
        { DIRECTION.RIGHT,new Vector2Int(1,0)},
        { DIRECTION.DOWN,new Vector2Int(0,-1)},
        { DIRECTION.LEFT,new Vector2Int(-1,0)},
    };

    [Serializable]
    public class SpawnOption
    {
        [Tooltip("�ִ�� ������ ���� ����")]
        public int MaxNum;
        [Tooltip("�ּҷ� ������ ���� ����")]
        public int MinNum;
        [Tooltip("�ϳ��� ���� ������ ������ ���� ����")]
        public int MaxRestaurant;
        [Tooltip("�ϳ��� ���� ������ ���� ���� ����")]
        public int MaxShop;

        [Range(0.00f, 1.00f), Tooltip("�� ���� �ڷ����� ���� Ȯ��")]
        public float teleporterPercent;
        [Range(0.00f, 1.00f), Tooltip("�� ���� �����ڽ� ���� Ȯ��")]
        public float BronzeChestPercent;
        [Range(0.00f, 1.00f), Tooltip("�� ���� �ǹ��ڽ� ���� Ȯ��")]
        public float SilverChestPercent;
        [Range(0.00f, 1.00f), Tooltip("�� ���� ���ڽ� ���� Ȯ��")]
        public float GoldChestPercent;

        [Tooltip("���簢�� ũ���� 2���� �迭�� ũ��")]
        public int MaxListSize = 5;
    }

    [Serializable]
    public class CurrentValue
    {
        public int NowCount;
        public int RestaurantCount;
        public int ShopCount;
        [SerializeField]
        public StageData[] Maplist;
        //public GameObject[] MapPrefabsList;
        public List<GameObject> CurrentRoomObjs;
        public GameObject[] MapObjList;
    }

    [SerializeField]
    public SpawnOption option = new SpawnOption();
    [SerializeField]
    public CurrentValue current = new CurrentValue();

    bool[] dir = new bool[(int)Door.DoorType.DoorMax];

    void Awake()
    {
        InitSetting();
    }
    void Start()
    {
        SpawnStart(0);
    }


    public void InitSetting()
    {
        current.Maplist = new StageData[option.MaxListSize * option.MaxListSize];
        current.MapObjList = new GameObject[option.MaxListSize * option.MaxListSize];
        current.CurrentRoomObjs = new List<GameObject>();
        for (int i = 0; i < option.MaxListSize * option.MaxListSize - 1; i++)
        {
            current.Maplist[i] = null;
            //current.MapPrefabsList[i] = null;
            current.MapObjList[i] = null;
        }
    }

    public void SpawnStart(int NowFloor)
    {
        current.NowCount = 0;
        //������ ������Ű����� �����ڸ��� DFSŽ�� �˰����� ���ؼ� �̴´�
        Vector2Int lastindex = MapSpawn(0, 2, null, 0);
        //�������� �ڸ� ��� ���� ������ �������ش�.
        StageSetting();

        //¦����° �������� �������� ��Ÿ����.
        if (Managers.Map.NowFloor % 2 == 0)
        {
            SetBossRoom(Managers.Map.NowStage);
        }

        ShowMaps();

        Managers.Map.StageSetting(current.MapObjList, option.MaxListSize);

        //���� ����� ���� ��ü�ʵ� �����
        //DungeonMapUI.Instance.Body.SetRoomTiles(current.MapObjList, option.MaxListSize);

        Debug.Log($"������� ���� ����{current.NowCount}");
    }


    #region StageSetting
    //������ġ, �������, ����, ����, ������ ���� ���� ����� �ش�.
    public void StageSetting()
    {
        
        //direction.Initialize();
        bool flag = false;
        int size = option.MaxListSize;

        StageSettingStart(size, flag);
        

    }

    void StageSettingStart(int size, bool flag)
    {
        for (int i = 0; i < size * size; i++)
        {
            flag = false;

            DirDoorInit();

            PercentageMapGenerator(i);

            if (current.MapObjList[i] != null)
            {
                for (int a = 0; a < (int)Door.DoorType.DoorMax; a++)
                {
                    //���� ���� �ؾ� �ϴµ� ������ ���� �ش���ġ�� ���� ���� ���̸� �ٽṵ̂� �Ѵ�.
                    if (dir[a])
                    {
                        if (current.MapObjList[i].GetComponent<BaseStage>().door[a] == null)
                        {
                            flag = true;
                            GameObject.Destroy(current.MapObjList[i]);
                            break;
                        }
                    }
                }
            }

            if (flag)
            {
                i--;
                continue;
            }

            //������ ����Ʈ�� ���� ������ �ֺ��� �ִ� ����� �˻��ؼ� ��ũ�����͸� �־��ش�.
            if (current.MapObjList[i] != null)
            {
                LinkedData data = SetLinkingData(i);
                current.MapObjList[i].GetComponent<BaseStage>().StageLinkedData = data;
                //obj.transform.position = new Vector3(transform.position.x + (x * interval), transform.position.y + ((y * interval) * -1));
                Debug.Log($"{i}���� ��ũ����");
            }

        }//for
    }

    void DirDoorInit()
    {
        for (int b = 0; b < dir.Length; b++)
        {
            dir[b] = false;
        }
    }

    void PercentageMapGenerator(int i)
    {
        int count = 0;

        //DirDoorInit(ref dir);

        if (current.Maplist[i] != null)
        {
            //ó���� ���۹�
            if (current.Maplist[i].Num == 0)
            {
                StageLoadAndContain(MapManager.ROOMTYPE.Start, i);
                return;
            }
            if (current.Maplist[i].Num == current.NowCount - 1)//������ ���� �������� �Ѵ�.
            {
                StageLoadAndContain(MapManager.ROOMTYPE.End, i);
                return;
            }
            //0����� ������ ���� ����� ����� ���� ����(���������� �����ϴ� ���� ����)�� ���� ���� Ŭ������ �������� �����ش�.

            if (current.Maplist[i].RightMap != null)
            {
                dir[(int)Door.DoorType.Right] = true;
                count++;
            }
            if (current.Maplist[i].LeftMap != null)
            {
                dir[(int)Door.DoorType.Left] = true;
                count++;
            }
            if (current.Maplist[i].UpMap != null)
            {
                dir[(int)Door.DoorType.Up] = true;
                count++;
            }
            if (current.Maplist[i].DownMap != null)
            {
                dir[(int)Door.DoorType.Down] = true;
                count++;
            }
            //�ֺ��� �����ϴ� ����� ���� ������ ������ ���� ũ�⸦ �����Ѵ�.

            //100% ū��
            if (count <= 2)//���� �ϳ� �Ǵ� �ΰ��� ���� ������ ������ �̸鼭 ������, �������� ������ ������ �������� �ȴ�.
            {
                int rnd = UnityEngine.Random.Range(0, 100);
                if (current.RestaurantCount < option.MaxRestaurant)
                {
                    StageLoadAndContain(MapManager.ROOMTYPE.Restaurant, i);
                    current.RestaurantCount++;
                    //Debug.Log($"{current.Maplist[i].Num}��°�� �������");

                    return;
                }

                if (current.ShopCount < option.MaxShop)
                {
                    StageLoadAndContain(MapManager.ROOMTYPE.Shop, i);
                    current.ShopCount++;
                    //Debug.Log($"{current.Maplist[i].Num}��°�� ����")

                    return;
                }
                else
                {
                    if (rnd < 70)
                    {
                        StageLoadAndContain(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.SMALL, i);
                        //Debug.Log($"{current.Maplist[i].Num}��°�� ������");
                        return;
                    }
                    if (rnd >= 70)
                    {
                        StageLoadAndContain(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.MEDIUM, i);
                        //Debug.Log($"{current.Maplist[i].Num}��°�� �߰���");
                        return;
                    }
                }
            }

            if (count <= 3)//70%�߰���,30%ū��
            {
                int rnd = UnityEngine.Random.Range(0, 100);
                if (rnd < 70)
                {
                    StageLoadAndContain(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.MEDIUM, i);
                    //Debug.Log($"{current.Maplist[i].Num}��°�� �߰���");
                    return;
                }
                if (rnd >= 70)
                {
                    StageLoadAndContain(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.LARGE, i);
                    //Debug.Log($"{current.Maplist[i].Num}��°�� ū��");
                    return;
                }
            }

            if (count <= 4)//���� �ΰ��ִ¹浵 ��������� �������� ������ ��ȣ�� ���� ���� ��������� ���������� ������.
            {
                StageLoadAndContain(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.LARGE, i);
                //Debug.Log($"{current.Maplist[i].Num}��°�� ū��");
            }
        }
    }

    void StageLoadAndContain(MapManager.ROOMTYPE roomType, int i)
    {
        GameObject obj = Managers.Map.StageLoad(roomType);
        current.MapObjList[i] = GameObject.Instantiate(obj);
    }

    void StageLoadAndContain(MapManager.ROOMTYPE roomType, MapManager.ROOMCLASS roomClass, int i)
    {
        GameObject obj = Managers.Map.StageLoad(roomType,roomClass);
        current.MapObjList[i] = GameObject.Instantiate(obj);
    }
    #endregion

    public void ShowMaps()
    {
        int interval = 70;
        int yval = option.MaxListSize;

        for (int y = 0; y < option.MaxListSize; y++)
        {
            for (int x = 0; x < option.MaxListSize; x++)
            {
                if (current.MapObjList[x + (y * yval)] != null)
                {
                    GameObject obj = current.MapObjList[x + (y * yval)];
                    obj.transform.position = new Vector3(transform.position.x + (x * interval), transform.position.y + ((y * interval) * -1));

                    int num = obj.GetComponent<BaseStage>().StageLinkedData.Num;
                    obj.name = $"Room_{num}";

                    obj.GetComponent<BaseStage>().StageNum = num;
                }
            }
        }
    }


    //����� Ư������� �ƴ� �Ϲݹ���� ã�Ƽ� ���߿� �ϳ��� ���������� ���Ѵ�.
    public void SetBossRoom(MapManager.STAGE stage)
    {
        int size = option.MaxListSize;
        List<int> list = new List<int>();
        for (int i = 0; i < size * size; i++)
        {
            if (current.MapObjList[i] != null)
            {
                BaseStage stagescript = current.MapObjList[i].GetComponent<BaseStage>();
                if (stagescript.type == MapManager.ROOMTYPE.NOMAL)
                {
                    list.Add(i);
                }
            }
        }
        int rnd = UnityEngine.Random.Range(0, list.Count);
        int index = list[rnd];
        GameObject.Destroy(current.MapObjList[index]);
        GameObject obj = Managers.Map.StageLoad(MapManager.ROOMTYPE.Boss);
        current.MapObjList[index] = GameObject.Instantiate(obj);
        current.MapObjList[index].GetComponent<BaseStage>().StageLinkedData = SetLinkingData(index);
    }

    //linkeddata���� ��������� ����Ǿ� �ִ� �� Ȯ���ϰ�
    //prefabs���� �ش���⿡ ���� ���� ������� �ִ��� Ȯ���ϰ�
    //���� �����Ѵ�.
    public LinkedData SetLinkingData(int index)
    {
        LinkedData linkeddata = new LinkedData();
        int yval = option.MaxListSize;
        int x = index % yval;
        int y = index / yval;
        linkeddata.Num = current.Maplist[index].Num;

        if (current.Maplist[index].LeftMap != null)
        {
            if (current.MapObjList[(x - 1) + (y * yval)] != null)
            {
                linkeddata.LeftMap = current.MapObjList[(x - 1) + (y * yval)];
                if (current.MapObjList[(x - 1) + (y * yval)].GetComponent<BaseStage>().StageLinkedData != null)
                {
                    current.MapObjList[(x - 1) + (y * yval)].GetComponent<BaseStage>().StageLinkedData.RightMap = current.MapObjList[(x) + (y * yval)];
                }
            }
        }
        if (current.Maplist[index].RightMap != null)
        {
            if (current.MapObjList[(x + 1) + (y * yval)] != null)
            {
                linkeddata.RightMap = current.MapObjList[(x + 1) + (y * yval)];

                if (current.MapObjList[(x + 1) + (y * yval)].GetComponent<BaseStage>().StageLinkedData != null)
                {
                    current.MapObjList[(x + 1) + (y * yval)].GetComponent<BaseStage>().StageLinkedData.LeftMap = current.MapObjList[(x) + (y * yval)];
                }
            }
        }
        if (current.Maplist[index].UpMap != null)
        {
            if (current.MapObjList[x + ((y - 1) * yval)] != null)
            {
                linkeddata.UpMap = current.MapObjList[x + ((y - 1) * yval)];

                if (current.MapObjList[x + ((y - 1) * yval)].GetComponent<BaseStage>().StageLinkedData != null)
                {
                    current.MapObjList[x + ((y - 1) * yval)].GetComponent<BaseStage>().StageLinkedData.DownMap = current.MapObjList[(x) + (y * yval)];
                }
            }
        }
        if (current.Maplist[index].DownMap != null)
        {
            if (current.MapObjList[x + ((y + 1) * yval)] != null)
            {
                linkeddata.DownMap = current.MapObjList[x + ((y + 1) * yval)];

                if (current.MapObjList[x + ((y + 1) * yval)].GetComponent<BaseStage>().StageLinkedData != null)
                {
                    current.MapObjList[x + ((y + 1) * yval)].GetComponent<BaseStage>().StageLinkedData.UpMap = current.MapObjList[(x) + (y * yval)];
                }
            }
        }

        return linkeddata;
    }


    //�����ʿ��� ���� ������ �ؼ� �� �迭�� ����� �ְ� �ش� �迭�� �� �Ŵ����� �Ѱ��ش�.
    //public int MapSpawn(int x, int y, StageData Parent)
    public Vector2Int MapSpawn(int x, int y, StageData Parent, int depth)
    {
        //���� �ִ� �������� ����� ���� �����Ѵ�.
        if (current.NowCount >= option.MaxNum)
        {
            return new Vector2Int(x, y);
        }

        int RandNum;
        int yval = option.MaxListSize;

        //������ �ش� ��ġ�� ���� ����� 
        //������, ����, �� ũ��, ���� ���� ���� �����Ѵ�.
        //�������� �־�� �ϸ� �����浵 ����
        if (current.Maplist[x + (y * yval)] == null)
        {
            current.Maplist[x + (y * yval)] = new StageData();
            current.Maplist[x + (y * yval)].InitSttting(current.NowCount, x, y);
            current.NowCount++;

            //�� ����� ���� ����Ǿ �̵��� �� �־�� ������ ������ �ִٰ� �׻� ����Ǿ� �־�� �ϴ°��� �ƴϴ�
            //���� Ž���� �Ҷ� �ڽ��� ���� Ž���� ��ġ���� ������ �־��� ��ġ�� �Ѱ������ν�
            //Ž���� ������ ��ΰ� ���� �̾��ִ� ��ΰ� �� �� �ֵ��� ���ش�.
            if (Parent != null)
            {
                if (Parent.indexX == x)
                {
                    if (Parent.indexY > y)//�Ʒ��ʰ� ����
                    {
                        current.Maplist[x + (y * yval)].DownMap = Parent;
                        Parent.UpMap = current.Maplist[x + (y * yval)];
                    }
                    else//���ʰ� ����
                    {
                        current.Maplist[x + (y * yval)].UpMap = Parent;
                        Parent.DownMap = current.Maplist[x + (y * yval)];
                    }
                }
                else if (Parent.indexY == y)
                {
                    if (Parent.indexX > x)//�����ʰ� ����
                    {
                        current.Maplist[x + (y * yval)].RightMap = Parent;
                        Parent.LeftMap = current.Maplist[x + (y * yval)];
                    }
                    else//���ʰ� ����
                    {
                        current.Maplist[x + (y * yval)].LeftMap = Parent;
                        Parent.RightMap = current.Maplist[x + (y * yval)];
                    }
                }
            }

        }

        //õ��° ���� �׻� ���������ΰ���.
        if (current.NowCount == 1)
        {
            MapSpawn(x + 1, y, current.Maplist[(x) + (y * yval)], depth + 1);
            //return option.MaxNum;
            return new Vector2Int(x + 1, y);
        }

        //����
        RandNum = UnityEngine.Random.Range(0, 100);
        //Debug.Log($"����{RandNum}");
        if (RandNum <= 70)
        {
            if (x - 1 >= 1 && current.Maplist[(x - 1) + (y * yval)] == null)
            {
                MapSpawn(x - 1, y, current.Maplist[(x) + (y * yval)], depth + 1);
            }
        }

        //������
        RandNum = UnityEngine.Random.Range(0, 100);

        if (RandNum <= 70)
        {
            if (x + 1 <= option.MaxListSize - 1 && current.Maplist[(x + 1) + (y * yval)] == null)
            {
                MapSpawn(x + 1, y, current.Maplist[(x) + (y * yval)], depth + 1);
            }
        }

        //����
        RandNum = UnityEngine.Random.Range(0, 100);
        if (RandNum <= 70)
        {
            if (y - 1 >= 0 && current.Maplist[x + ((y - 1) * yval)] == null)
            {
                MapSpawn(x, y - 1, current.Maplist[(x) + (y * yval)], depth + 1);
            }
        }

        //�Ʒ���
        RandNum = UnityEngine.Random.Range(0, 100);

        if (RandNum <= 70)
        {
            if (y + 1 <= option.MaxListSize - 1 && current.Maplist[x + ((y + 1) * yval)] == null)
            {
                MapSpawn(x, y + 1, current.Maplist[(x) + (y * yval)], depth + 1);
            }
        }

        //�̷��� Ȯ���� �����̵��� �ϸ� �ּҰ����� ����� ���� ������ �ֱ� ������ �ּ� ������ ä������ ������ 4 ������ ����ִ� ���� ã�Ƽ� ������ �������� �ݴϴ�.
        if (current.NowCount < option.MinNum)
        {
            for (int i = 0; i < 4; i++)
            {
                int tempx = x + dirIndex[(DIRECTION)i].x;
                int tempy = y + dirIndex[(DIRECTION)i].y;
                if (tempx >= 0 && tempx < option.MaxListSize - 1 && tempy >= 0 && tempy < option.MaxListSize - 1)
                {
                    if (current.Maplist[tempx + ((tempy + 1) * yval)] == null)
                    {
                        MapSpawn(tempx, tempy, current.Maplist[(tempx) + (tempy * yval)], depth + 1);
                    }
                }
            }
        }

        return new Vector2Int(x, y);
    }


    /*public void CreateStageData(int x, int y)
    {
        int yval = option.MaxListSize;
        if (current.Maplist[x + (y * yval)] == null)
        {

        }
    }*/
    //�ʽ����ϰ� ������ ���� �� �Ŵ������� �Ѱ��ش�.
}
