using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveZone : MonoBehaviour
{
    public int maxWaves = 5;

    private bool isWaveActive = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(isWaveActive)
        {
            return;
        }

        if ((other.CompareTag("Player")))
        {
            GameManager.instance.StartNextWave();
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
