using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///미니맵 입니다.
///미니맵도 타일들로 이루어져 있습니다. 월드공간을 미니맵의 사이즈에 맞게
///축소시키고 화면 크기에 맞게 컬링을 진행해서 보여줍니다.
///모든 타일 정보들은 MapManager에서 초기화때 미니맵에 넘겨줍니다.
/////////////////////////////////////////////////////////////////////
public class MinimapTile : MonoBehaviour
{
    public enum MinimapElement { BackGround, Wall, Moveable, Door, Player, Monster, ElementMax };

    [SerializeField]
    private MinimapElement tiletype;

    public Sprite[] ElementSprite;

    public Sprite PlayerSprite;

    public Sprite EmenySprite;

    public bool IsPlayer;
    //public Transform player;
    public bool IsMonster;

    public Image image;

    Vector2Int index;

    Vector3 worldpos;



    public MinimapElement p_tiletype
    {
        get
        {
            return tiletype;
        }
        set
        {
            tiletype = value;
            image.sprite = ElementSprite[(int)value];
        }

    }

    private void Awake()
    {
        image = GetComponent<Image>();
    }

}
