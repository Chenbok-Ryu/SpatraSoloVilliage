using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; } // �̱��� �ν��Ͻ�

    private Coroutine waveRoutine;
    private int currentWave;
    private int totalWaves;

    [SerializeField]
    private List<GameObject> enemyPrefabs; // ������ �� ������ ����Ʈ

    [SerializeField]
    private List<Rect> spawnAreas; // ���� ������ ���� ����Ʈ

    [SerializeField]
    private Color gizmoColor = new Color(1, 0, 0, 0.3f); // ����� ����

    private List<EnemyController> activeEnemies = new List<EnemyController>(); // ���� Ȱ��ȭ�� ����

    private bool enemySpawnComplete;

    [SerializeField] private float timeBetweenSpawns = 0.2f;
    [SerializeField] private float timeBetweenWaves = 1f;

    GameManager gameManager;

    private void Awake()
    {
        Instance = this; // �̱��� �ν��Ͻ� ����
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

        // ù ���̺� �� ���ð�
        yield return new WaitForSeconds(timeBetweenWaves);

        while (currentWave < totalWaves)
        {
            // �� ���� ����
            SpawnRandomEnemy();

            currentWave++;
            // ���� ���̺� �� ���
            yield return new WaitForSeconds(timeBetweenWaves);
        }

        // ��� ���̺� ���� �Ϸ�
        StopWave();
    }

    public void StopWave()
    {
        if (waveRoutine != null)
            StopCoroutine(waveRoutine);

        // ���̺� ���� �� ���� �Ŵ����� �˸�
        gameManager.EndOfWave();
    }

    private void SpawnRandomEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0 ||
            spawnAreas == null || spawnAreas.Count == 0)
        {
            Debug.LogWarning("Enemy Prefabs �Ǵ� Spawn Areas�� �������� �ʾҽ��ϴ�.");
            return;
        }

        // ������ �� ������ ����
        GameObject randomPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        // ������ ���� ����
        Rect area = spawnAreas[Random.Range(0, spawnAreas.Count)];
        // ���� �� ���� ��ġ ���
        Vector2 pos = new Vector2(
            Random.Range(area.xMin, area.xMax),
            Random.Range(area.yMin, area.yMax)
        );

        // �� ����
        GameObject spawned = Instantiate(
            randomPrefab,
            new Vector3(pos.x, pos.y, 0f),
            Quaternion.identity
        );
        var enemyCtrl = spawned.GetComponent<EnemyController>();
        enemyCtrl.Init(this, gameManager.player.transform);
        activeEnemies.Add(enemyCtrl);
    }

    // ����� �׷� ������ �ð�ȭ (���õ� ��쿡�� ǥ��)
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