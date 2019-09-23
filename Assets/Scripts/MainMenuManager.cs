using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public Text totalPointsText;
    public Text asteroidsDestroyedText;
    public Text ufosDestroyedText;
    public Text wavesClearedText;

    public Image whiteFade;

    public AudioSource playButtonSFX;
    public AudioSource menuButtons;

    public float timeToFade;


    void Start()
    {
        Fade(true, 1f);
        SetTextStats();
    }

    public void ResetAllStats()
    {
        menuButtons.Play();
        PlayerPrefs.DeleteAll();
        SetTextStats();
    }

    void SetTextStats()
    {
        totalPointsText.text = "TOTALPOINTS GAINED: " + PlayerPrefs.GetInt("TotalPoints");
        asteroidsDestroyedText.text = "ASTEROIDS DESTROYED: " + PlayerPrefs.GetInt("AsteroidsDestroyed");
        ufosDestroyedText.text = "UFOS DESTROYED: " + PlayerPrefs.GetInt("UFOsDestroyed");
        wavesClearedText.text = "WAVES CLEARED: " + PlayerPrefs.GetInt("WavesCleared");
    }

    public void Fade(bool fadeOut, float fadeTime)
    {
        switch (fadeOut)
        {
            case true:
                whiteFade.CrossFadeAlpha(0, fadeTime, false);
                break;
            case false:
                whiteFade.CrossFadeAlpha(1, fadeTime, false);
                break;
        }
    }
    public void PlayGame()
    {
        playButtonSFX.Play();
        Fade(false, timeToFade);
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        yield return new WaitForSeconds(timeToFade);
        SceneManager.LoadScene("MainLevel");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
