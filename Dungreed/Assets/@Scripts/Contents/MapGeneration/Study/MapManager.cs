using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager 
{
    public enum STAGE { Stage1, Stage2, Stage3, Stage4, StageMax };
    public enum ROOMTYPE { Start, Restaurant, Shop, End, Boss, NOMAL, MAX };
    public enum ROOMCLASS { SMALL, MEDIUM, LARGE };

    //public enum 
    public StageData[] LinkedData = null;
    //public int arrsize;

    public GameObject[] StageArr = null;
    public int arrsize;

    [SerializeField]
    private STAGE nowstage = STAGE.Stage1;
    [SerializeField]
    private int nowfloor = 0;

    public GameObject[] SpecialRoom;
    public GameObject startRoom;
    public GameObject[] largeroom;
    public GameObject[] mediumroom;
    public GameObject[] smallroom;
    public GameObject EndRoom;

    public int NowFloor
    {
        get
        {
            return nowfloor;
        }
        set
        {
            nowfloor = value;
            if (nowfloor % 2 != 0 && nowfloor != 1)
            {
                NowStage = NowStage + 1;
            }
        }
    }


    public STAGE NowStage
    {
        get
        {
            return nowstage;
        }
        set
        {
            nowstage = value;
            LoadStagesToPrefabs(value);
        }
    }


    public void LoadStagesToPrefabs(STAGE stage)
    {
        largeroom = Resources.LoadAll<GameObject>($"Prefabs/Map_Prefabs/MapPrefabs/{stage.ToString()}/Large");
        mediumroom = Resources.LoadAll<GameObject>($"Prefabs/Map_Prefabs/MapPrefabs/{stage.ToString()}/Medium");
        smallroom = Resources.LoadAll<GameObject>($"Prefabs/Map_Prefabs/MapPrefabs/{stage.ToString()}/Small");

        SpecialRoom[(int)ROOMTYPE.Start] = Resources.Load<GameObject>($"Prefabs/Map_Prefabs/MapPrefabs/{stage.ToString()}/Start/Stage1_Start");
        SpecialRoom[(int)ROOMTYPE.Shop] = Resources.Load<GameObject>($"Prefabs/Map_Prefabs/MapPrefabs/{stage.ToString()}/Shop/Stage1_Shop");
        SpecialRoom[(int)ROOMTYPE.Restaurant] = Resources.Load<GameObject>($"Prefabs/Map_Prefabs/MapPrefabs/{stage.ToString()}/Restaurant/Stage1_Restaurant");
        SpecialRoom[(int)ROOMTYPE.End] = Resources.Load<GameObject>($"Prefabs/Map_Prefabs/MapPrefabs/{stage.ToString()}/End/Stage1_End");
        SpecialRoom[(int)ROOMTYPE.Boss] = Resources.Load<GameObject>($"Prefabs/Map_Prefabs/MapPrefabs/{stage.ToString()}/{ROOMTYPE.Boss.ToString()}/Stage1_Boss");
        Debug.Log("로드 완료!");
    }



    public void StageSetting(GameObject[] rooms, int size)
    {
        StageArr = rooms;
        arrsize = size;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (rooms[x + (y * size)] != null)
                {
                    rooms[x + (y * size)]?.GetComponent<BaseStage>().Initsetting();
                }
            }
        }
    }


    public GameObject StageLoad(ROOMTYPE type)
    {
        return SpecialRoom[(int)type];
    }

    public GameObject StageLoad(ROOMTYPE type, ROOMCLASS roomclass)
    {
        //랜덤으로 뽑아서 하나를 넘겨준다.
        int count = 0;
        if (roomclass == ROOMCLASS.SMALL) count = smallroom.Length;
        else if (roomclass == ROOMCLASS.MEDIUM) count = mediumroom.Length;
        else if (roomclass == ROOMCLASS.LARGE) count = largeroom.Length;

        int rnd = Random.Range(0, count);

        if (roomclass == ROOMCLASS.SMALL) return smallroom[rnd];
        else if (roomclass == ROOMCLASS.MEDIUM) return mediumroom[rnd];
        else return largeroom[rnd];
    }


    public void InitSetting()
    {
        SpecialRoom = new GameObject[(int)ROOMTYPE.MAX];
    }

    //현재 만들어져 있는  스테이지를 모두 없애주고 새로운 방들을 만들어 준다.
    public void NextStage()
    {
        DestroyRooms(StageArr);
        NowFloor++;
        MapSpawner.Instance.SpawnStart(NowFloor);
    }

    public void DestroyRooms(GameObject[] stage)
    {
        for (int i = 0; i < stage.Length; i++)
        {
            GameObject.Destroy(stage[i]);
        }
    }

    private void Awake()
    {
        InitSetting();
        NowStage = STAGE.Stage1;
        NowFloor = 1;
    }

}
