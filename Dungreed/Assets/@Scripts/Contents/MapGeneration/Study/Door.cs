using UnityEngine.Tilemaps;
using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    public enum DoorType { Up, Down, Right, Left, DoorMax };

    public Tilemap WallTilemap;
    public Tilemap BackTilemap;
    public Tilemap MoveableTilemap;


    public Tilemap MainWallTilemap;
    public Tilemap MainBackTilemap;
    public Tilemap MainMoveableTilemap;

    public Tile tile;

    public Transform StartPos;
    public Transform EndPos;

    public Vector3Int DoorStartIndex;
    public Vector3Int DoorEndIndex;

    public Vector3Int MainStartIndex;
    public Vector3Int MainEndIndex;


    public Vector2Int size;

    public GameObject NextRoom;

    public BoxCollider2D coll;
    public LayerMask PlayerMask;
    public DoorType type;

    public BoxCollider2D collision;
    public GameObject ParentStage;
    public Transform TeleportPos;

    public bool nowdoorlocked;

    //public GameObject SealStone;
    public Animator SealStoneAni;
    public AnimationClip SealStoneOpenAni;
    public AnimationClip SealStoneCloseAni;
    public ParticleSystem Doorparticle;
    [Header("==================Test==================")]
    public Transform MapInfoPos;
    public Vector3Int StartIndex;
    public Vector3Int ParentStartIndex;
    public bool NowDoorLocked
    {
        get
        {
            return nowdoorlocked;
        }
        set
        {
            nowdoorlocked = value;
            if (Doorparticle != null && SealStoneAni != null)
            {
                if (nowdoorlocked)
                {
                    Doorparticle.gameObject.SetActive(false);
                    StartCoroutine(SealStoneActive());
                }
                else
                {
                    StartCoroutine(SealStoneInactive());
                    Doorparticle.gameObject.SetActive(true);
                }
            }
        }
    }


    //�ִϸ��̼ǿ� ���� �ڿ������� ���� ���� �ڷ�ƾ �Լ��� ���� �Ͽ���
    IEnumerator SealStoneActive()
    {
        //SealStoneAni.gameObject.SetActive(true);
        if (SealStoneAni.gameObject.activeSelf == false)
        {
            SealStoneAni.gameObject.SetActive(true);
        }
        if (SealStoneAni.gameObject.GetComponent<SpriteRenderer>().enabled == false)
        {
            SealStoneAni.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }
        SealStoneAni.SetTrigger("SealClose");
        yield return null;
    }

    //�ִϸ��̼ǿ� ���� �ڿ������� ���� ���� �ڷ�ƾ �Լ��� ���� �Ͽ���
    IEnumerator SealStoneInactive()
    {
        //SealStoneAni.gameObject.SetActive(false);
        SealStoneAni.SetTrigger("SealOpen");
        yield return new WaitForSeconds(SealStoneOpenAni.length);
        SealStoneAni.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        SealStoneAni.gameObject.SetActive(false);
    }


    //���� ������ Ÿ�ϸ��� �ε�����ȣ�� �Ѱ��ָ� �ش� �ε����� ��ġ���� ���� �����Ѵ�.
    public void CreateDoor(GameObject tilemaps)
    {
        ParentStage = tilemaps;


        Tilemap[] temp = tilemaps.GetComponentsInChildren<Tilemap>();
        foreach (var a in temp)
        {
            if (a.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                MainWallTilemap = a;
            }
            else if (a.gameObject.layer == LayerMask.NameToLayer("BackGround"))
            {
                MainBackTilemap = a;
            }
            else if (a.gameObject.layer == LayerMask.NameToLayer("Moveable"))
            {
                MainMoveableTilemap = a;
            }
        }

        if (this.type == DoorType.Up)
        {
            NextRoom = tilemaps.GetComponent<BaseStage>().StageLinkedData.UpMap;
        }
        else if (this.type == DoorType.Down)
        {
            NextRoom = tilemaps.GetComponent<BaseStage>().StageLinkedData.DownMap;
        }
        else if (this.type == DoorType.Right)
        {
            NextRoom = tilemaps.GetComponent<BaseStage>().StageLinkedData.RightMap;
        }
        else if (this.type == DoorType.Left)
        {
            NextRoom = tilemaps.GetComponent<BaseStage>().StageLinkedData.LeftMap;
        }

        GetTileInfo(type);

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                Vector3Int index = new Vector3Int(x, y * -1, 0);
                index = MainStartIndex + index;

                MainWallTilemap.SetTile(index, null);

                MainBackTilemap.SetTile(index, null);

                if (MoveableTilemap != null)
                {
                    MainMoveableTilemap.SetTile(index, null);
                }
            }
        }

        BaseStage stagedata = tilemaps.GetComponent<BaseStage>();
        SetStageDoorInfo();
    }

    //���� ����� ���� �ش� ���� ��ġ�ϴ� ���� ������ �������� ���� �־��ش�.
    public void SetStageDoorInfo()
    {
        int x = 0;
        int y = 0;
        RaycastHit2D[] hit;
        int size = ParentStage.GetComponent<BaseStage>().MaxX;
        StartIndex = MainWallTilemap.WorldToCell(MapInfoPos.position);

        Vector3 temp = MainWallTilemap.GetCellCenterWorld(StartIndex);
        if (this.type == DoorType.Right || this.type == DoorType.Left)
        {
            for (y = 0; y < 4; y++)
            {
                temp.x = MapInfoPos.position.x + x;
                temp.y = MapInfoPos.position.y + y + 0.3f;
                hit = Physics2D.RaycastAll(temp, new Vector2(1, 1), 0f);

                Vector3Int index = StartIndex;
                index.y = index.y + y;
                //stagedata.Roominfo[x + (y * maxX)] = 0;
                foreach (var a in hit)
                {
                    if (a)
                    {
                        if (a.transform.gameObject.layer == LayerMask.NameToLayer("Door"))
                        {
                            ParentStage.GetComponent<BaseStage>().Roominfo[index.x + (index.y * size)] = (int)BaseStage.TileElement.Door;
                            break;
                        }

                    }
                }

            }
        }
        else if (this.type == DoorType.Up || this.type == DoorType.Down)
        {
            for (x = 0; x < 4; x++)
            {
                temp.x = MapInfoPos.position.x + x;
                temp.y = MapInfoPos.position.y + y + 0.3f;
                hit = Physics2D.RaycastAll(temp, new Vector2(1, 1), 0f);

                Vector3Int index = StartIndex;
                index.x = index.x + x;
                //stagedata.Roominfo[x + (y * maxX)] = 0;


                foreach (var a in hit)
                {
                    if (a)
                    {
                        if (a.transform.gameObject.layer == LayerMask.NameToLayer("Door") || a.transform.gameObject.layer == LayerMask.NameToLayer("Moveable"))
                        {
                            ParentStage.GetComponent<BaseStage>().Roominfo[index.x + (index.y * size)] = (int)BaseStage.TileElement.Door;
                            break;
                        }

                    }
                }
            }
        }
    }


    //�� Ÿ�Կ� ���� �������� �޾Ƽ� �ش�. ���μ��� ũ��, ������ �ִ� ���� ���� �ε��� ��ȣ ���
    public void GetTileInfo(DoorType type)
    {
        //Tilemap[] temp = DoorPrefabs[(int)type].GetComponentsInChildren<Tilemap>();
        Tilemap[] temp = GetComponentsInChildren<Tilemap>();//�ڱ� �ڽ��� Ÿ�ϸ� ������ �޾ƿ´�.
        foreach (var a in temp)
        {
            if (a.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                WallTilemap = a;
            }
            else if (a.gameObject.layer == LayerMask.NameToLayer("BackGround"))
            {
                BackTilemap = a;
            }
            else if (a.gameObject.layer == LayerMask.NameToLayer("Moveable"))
            {
                MoveableTilemap = a;
            }
        }

        StartPos = transform.Find("StartPos");
        EndPos = transform.Find("EndPos");

        DoorStartIndex = WallTilemap.WorldToCell(StartPos.position);
        DoorEndIndex = WallTilemap.WorldToCell(EndPos.position);

        MainStartIndex = MainWallTilemap.WorldToCell(StartPos.position);
        MainEndIndex = MainWallTilemap.WorldToCell(EndPos.position);

        size.x = DoorEndIndex.x - DoorStartIndex.x + 1;
        size.y = DoorStartIndex.y - DoorEndIndex.y + 1;

    }

    //ĳ���Ͱ� ���� ������ �̾��� �ִ� ������ �̵� ���� �ش�.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!NowDoorLocked)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                //SoundManager.Instance.bgmSource.PlayOneShot(SoundManager.Instance.UI_Audio[6]);
                GoNextMap(collision.gameObject);
            }
        }

    }


    public void GoNextMap(GameObject Player)
    {
        Door pos = null;
        GameObject temp = null;
        if (this.type == DoorType.Up)
        {
            temp = ParentStage.GetComponent<BaseStage>().StageLinkedData.UpMap;
            pos = NextRoom.GetComponent<BaseStage>().door[(int)DoorType.Down];
        }
        else if (this.type == DoorType.Down)
        {
            temp = ParentStage.GetComponent<BaseStage>().StageLinkedData.DownMap;
            pos = NextRoom.GetComponent<BaseStage>().door[(int)DoorType.Up];
        }
        else if (this.type == DoorType.Right)
        {
            temp = ParentStage.GetComponent<BaseStage>().StageLinkedData.RightMap;
            pos = NextRoom.GetComponent<BaseStage>().door[(int)DoorType.Left];
        }
        else if (this.type == DoorType.Left)
        {
            temp = ParentStage.GetComponent<BaseStage>().StageLinkedData.LeftMap;
            pos = NextRoom.GetComponent<BaseStage>().door[(int)DoorType.Right];
        }
        string name = temp.gameObject.name;

        NextRoom = GameObject.Find(name);

        ParentStage.GetComponent<BaseStage>().NowPlayerEnter = false;

        NextRoom.GetComponent<BaseStage>().NowPlayerEnter = true;

        Player.transform.position = pos.TeleportPos.position;


    }

    void Start()
    {
        WallTilemap = null;
        BackTilemap = null;
        MoveableTilemap = null;


        MainWallTilemap = null;
        MainBackTilemap = null;
        MainMoveableTilemap = null;

        SealStoneAni = GetComponentInChildren<Animator>();
        Doorparticle = GetComponentInChildren<ParticleSystem>();

        NowDoorLocked = false;
    }
}
