using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; } // 싱글톤 인스턴스

    private Coroutine waveRoutine;
    private int currentWave;
    private int totalWaves;

    [SerializeField]
    private List<GameObject> enemyPrefabs; // 생성할 적 프리팹 리스트

    [SerializeField]
    private List<Rect> spawnAreas; // 적을 생성할 영역 리스트

    [SerializeField]
    private Color gizmoColor = new Color(1, 0, 0, 0.3f); // 기즈모 색상

    private List<EnemyController> activeEnemies = new List<EnemyController>(); // 현재 활성화된 적들

    private bool enemySpawnComplete;

    [SerializeField] private float timeBetweenSpawns = 0.2f;
    [SerializeField] private float timeBetweenWaves = 1f;

    GameManager gameManager;

    private void Awake()
    {
        Instance = this; // 싱글톤 인스턴스 설정
    }

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public void StartWave(int waveCount)
    {
        if (waveRoutine != null)
            StopCoroutine(waveRoutine);

        currentWave = 0;
        totalWaves = waveCount;
        waveRoutine = StartCoroutine(SpawnWave());
    }


    private IEnumerator SpawnWave()
    {
        enemySpawnComplete = false;

        // 첫 웨이브 전 대기시간
        yield return new WaitForSeconds(timeBetweenWaves);

        while (currentWave < totalWaves)
        {
            // 적 생성 로직
            SpawnRandomEnemy();

            currentWave++;
            // 다음 웨이브 전 대기
            yield return new WaitForSeconds(timeBetweenWaves);
        }

        // 모든 웨이브 생성 완료
        StopWave();
    }

    public void StopWave()
    {
        if (waveRoutine != null)
            StopCoroutine(waveRoutine);

        // 웨이브 종료 후 게임 매니저에 알림
        gameManager.EndOfWave();
    }

    private void SpawnRandomEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0 ||
            spawnAreas == null || spawnAreas.Count == 0)
        {
            Debug.LogWarning("Enemy Prefabs 또는 Spawn Areas가 설정되지 않았습니다.");
            return;
        }

        // 랜덤한 적 프리팹 선택
        GameObject randomPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        // 랜덤한 영역 선택
        Rect area = spawnAreas[Random.Range(0, spawnAreas.Count)];
        // 영역 내 랜덤 위치 계산
        Vector2 pos = new Vector2(
            Random.Range(area.xMin, area.xMax),
            Random.Range(area.yMin, area.yMax)
        );

        // 적 생성
        GameObject spawned = Instantiate(
            randomPrefab,
            new Vector3(pos.x, pos.y, 0f),
            Quaternion.identity
        );
        var enemyCtrl = spawned.GetComponent<EnemyController>();
        enemyCtrl.Init(this, gameManager.player.transform);
        activeEnemies.Add(enemyCtrl);
    }

    // 기즈모를 그려 영역을 시각화 (선택된 경우에만 표시)
    private void OnDrawGizmosSelected()
    {
        if (spawnAreas == null) return;

        Gizmos.color = gizmoColor;
        foreach (var area in spawnAreas)
        {
            var center = new Vector3(area.x + area.width / 2, area.y + area.height / 2);
            var size = new Vector3(area.width, area.height);
            Gizmos.DrawCube(center, size);
        }
    }

    public void RemoveEnemyOnDeath(EnemyController enemy)
    {
        activeEnemies.Remove(enemy);
        if (enemySpawnComplete && activeEnemies.Count == 0)
        {
            gameManager.EndOfWave();
        }
    }
}