using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    void Start()
    {
        Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalCount) =>
        {
            //(�񵿱�)�Լ��� �ε��� ������ ������ ������ ���̱� ������
            Debug.Log($"{key} {count}/{totalCount}");

            if (count == totalCount)
            {
                StartLoaded();
            }
        });
        //���⼭ StartLoaded()�� �ϸ� �ȵȴ�. // ������ �ε��� ������ testfunc�� ȣ�� ����.
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
        //������Ʈ �Ŵ������� ��������.
    }*/

    SpawningPool _spawningPool;

    void StartLoaded()
    {
        #region �ٸ� �ڵ�
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
        //������Ʈ �Ŵ������� ��������.

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
