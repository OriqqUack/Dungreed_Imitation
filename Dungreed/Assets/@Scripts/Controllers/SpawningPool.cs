using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningPool : MonoBehaviour
{
    //리스폰 주기
    //몬스터 최대 개수
    // 스톱?
    float _spawnInterval = 2.0f;
    int _maxMonsterCount = 100;
    Coroutine _coUpdateSpawningPool;

    void Start()
    {
        _coUpdateSpawningPool = StartCoroutine(CoUpdateSpawningPool());
    }

    IEnumerator CoUpdateSpawningPool()
    {
        while (true)
        {
            TrySpawn();
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    void TrySpawn()
    {
        int monsterCount = Managers.Object.Monster.Count;
        if (monsterCount >= _maxMonsterCount)
            return;

        //TEMP : DataId ?
        Vector3 randPos = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5));
        MonsterController mc = Managers.Object.Spawn<MonsterController>(randPos,Random.Range(0, 2));
        mc.transform.position = new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
    }
}
