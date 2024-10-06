using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Monster _monsterPrefab = null;
    [SerializeField] private Transform _spawnTransform = null;
    [SerializeField] private float _spawnInterval = 0.1f;

    void Start()
    {
        StartCoroutine(SpawnMonsterCoroutine());
    }

    IEnumerator SpawnMonsterCoroutine()
    {
        while (true)
        {
            // 몬스터 생성
            Instantiate(_monsterPrefab, _spawnTransform.position, _spawnTransform.rotation);
            // _spawnInterval 만큼 대기
            yield return new WaitForSeconds(_spawnInterval);
        }
    }
}