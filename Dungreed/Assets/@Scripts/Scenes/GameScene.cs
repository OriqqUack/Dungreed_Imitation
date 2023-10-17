using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    void Start()
    {
        Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalCount) =>
        {
            //(비동기)함수가 로딩이 끝나고 안으로 들어오는 것이기 때문에
            Debug.Log($"{key} {count}/{totalCount}");

            if (count == totalCount)
            {
                StartLoaded();
            }
        });
        //여기서 StartLoaded()는 하면 안된다. // 언젠가 로딩이 끝나면 testfunc를 호출 해줌.
    }

    /*void StartLoaded()
    {
        var player = Managers.Resource.Instantiate("Slime_01.prefab");
        player.AddComponent<PlayerController>();

        var snake = Managers.Resource.Instantiate("Snake_01.prefab");
        var goblin = Managers.Resource.Instantiate("Goblin_01.prefab");
        var joystick = Managers.Resource.Instantiate("UI_Joystick.prefab");
        joystick.name = "@UI_Joystick";

        var map = Managers.Resource.Instantiate("Map.prefab");
        map.name = "@Map";
        Camera.main.GetComponent<CameraController>().Target = player;
        //오브젝트 매니저에서 관리하자.
    }*/

    SpawningPool _spawningPool;

    void StartLoaded()
    {
        #region 다른 코드
        /*Managers.Data.Init();

        _spawningPool = gameObject.AddComponent<SpawningPool>();

        var player = Managers.Object.Spawn<PlayerController>(Vector3.zero);

       *//* for(int i = 0; i< 10; i++)
        {
            Vector3 randPos = new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
            MonsterController mc = Managers.Object.Spawn<MonsterController>(randPos,Random.Range(0, 2));
            mc.transform.position = new Vector2(Random.Range(-5,5), Random.Range(-5,5));
        }
*//*
        //var joystick = Managers.Resource.Instantiate("UI_Joystick.prefab");
        //joystick.name = "@UI_Joystick";

        //var map = Managers.Resource.Instantiate("Map.prefab");
        //map.name = "@Map";
        Camera.main.GetComponent<CameraController>().Target = player.gameObject;
        //오브젝트 매니저에서 관리하자.

        //Data Test
        //Managers.Data.Init();

        *//*foreach(var playerData in Managers.Data.PlayerDic.Values)
        {
            Debug.Log($"Lv1 : {playerData.level}, Hp{playerData.maxHp}");
        }*/
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
