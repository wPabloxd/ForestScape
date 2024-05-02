using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening.Core.Easing;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject playMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject quitMenu;
    [SerializeField] TextMeshProUGUI challengeText;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Toggle enemyToggle;
    [SerializeField] Image mainMenuBackground;
    [SerializeField] Color backgroundSelected;
    [SerializeField] Color backgroundUnselected;
    void Start()
    {
        MainMenu();
        LoadPrefs();
        enemyToggle.isOn = GameManager.Instance.enemyHPBar;
    }
    public void LoadPrefs()
    {
        float[] saves = GameManager.Instance.LoadPrefs();
        musicSlider.value = saves[0];
        sfxSlider.value = saves[1];
        sensitivitySlider.value = saves[2];
    }
    public void PlayLevel(int levelIndex)
    {
        GameManager.Instance.LoadScene(levelIndex);
    }
    public void MainMenu()
    {
        mainMenuBackground.color = backgroundSelected;
        mainMenu.SetActive(true);
        playMenu.SetActive(false);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
    }
    public void PlayMenu()
    {
        mainMenuBackground.color = backgroundUnselected;
        mainMenu.SetActive(true);
        playMenu.SetActive(true);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
    }
    public void SettingsMenu()
    {
        mainMenuBackground.color = backgroundUnselected;
        mainMenu.SetActive(true);
        playMenu.SetActive(false);
        settingsMenu.SetActive(true);
        quitMenu.SetActive(false);
    }
    public void QuitMenu()
    {
        mainMenuBackground.color = backgroundUnselected;
        mainMenu.SetActive(true);
        playMenu.SetActive(false);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(true);
    }
    public void EnemyHPBar()
    {
        if (GameManager.Instance.enemyHPBar)
        {
            GameManager.Instance.enemyHPBar = false;
        }
        else
        {
            GameManager.Instance.enemyHPBar = true;
        }
    }
    public void ConfirmQuitGame(bool confirm)
    {
        if (confirm)
        {
            Application.Quit();
        }
        else
        {
            MainMenu();
        }
    }
    public void SavePrefs()
    {
        GameManager.Instance.SavePrefs(musicSlider.value, sfxSlider.value, sensitivitySlider.value);
        GameManager.Instance.LoadPrefs();
    }
}