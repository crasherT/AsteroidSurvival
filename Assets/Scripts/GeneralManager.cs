using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralManager : MonoBehaviour {

    public static GeneralManager instance;

    public int points;
    public int asteroidsDestroyed;
    public int ufosDestroyed;
    public int wavesBeaten;

    public float timeToFade;
    private void Awake()
    {
        instance = this;
    }

    
    public void GoBackToMainMenu()
    {
        UIManager.instance.Flash(false, timeToFade);
        StartCoroutine(LoadMenu());
    }

    IEnumerator LoadMenu()
    {
        yield return new WaitForSeconds(timeToFade);
        SceneManager.LoadScene("MainMenu");
    }

    public void SaveStats()
    {
        int totalPointsGainedSoFar = PlayerPrefs.GetInt("TotalPoints");
        int totalWavesCleared = PlayerPrefs.GetInt("WavesCleared");
        int ufosDestroyed = PlayerPrefs.GetInt("UFOsDestroyed");
        int asteroidsDestroyed = PlayerPrefs.GetInt("AsteroidsDestroyed");

        totalPointsGainedSoFar += Player.instance.points;
        totalWavesCleared += WaveSpawner.instance.currentWave - 1;
        ufosDestroyed += Player.instance.ufoKills;
        asteroidsDestroyed += Player.instance.asteroidsDestroyed;

        PlayerPrefs.SetInt("TotalPoints", totalPointsGainedSoFar);
        PlayerPrefs.SetInt("WavesCleared", totalWavesCleared);
        PlayerPrefs.SetInt("UFOsDestroyed", ufosDestroyed);
        PlayerPrefs.SetInt("AsteroidsDestroyed", asteroidsDestroyed);
        PlayerPrefs.Save();
    }
}
