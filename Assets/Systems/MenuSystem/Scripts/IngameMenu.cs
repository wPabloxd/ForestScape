using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class IngameMenu : MonoBehaviour
{
    private static IngameMenu _instance;
    public static IngameMenu Instance
    {
        get { return _instance; }
    }

    [Header("Menu References")]
    [SerializeField] GameObject menu;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject quitMenu;
    [SerializeField] GameObject deathMenu;
    [SerializeField] GameObject finishMenu;
    [SerializeField] GameObject tutorialMenu;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] GameObject[] playerHUD;
    [SerializeField] TextMeshProUGUI[] deadText;
    [SerializeField] Image menuBackground;
    [SerializeField] Color backgroundSelected;
    [SerializeField] Color backgroundUnselected;
    [SerializeField] Toggle enemyToggle;
    [SerializeField] GameObject[] enemyHPBars;
    bool ended;

    [Header("Inputs")]
    [SerializeField] InputActionReference tutorialInput;
    [SerializeField] InputActionReference pauseGame;

    bool notMainMenuButMenu;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this.gameObject);
        }
    }
    private void OnEnable()
    {
        tutorialInput.action.Enable();
        pauseGame.action.Enable();
        GameManager.Instance.Unpause();
    }
    void Start()
    {
        if (GameManager.Instance.enemyHPBar)
        {
            foreach (GameObject e in enemyHPBars)
            {
                e.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject e in enemyHPBars)
            {
                e.SetActive(false);
            }
        }
        GameManager.Instance.playerDetectedCounter = 0;
        enemyToggle.isOn = GameManager.Instance.enemyHPBar;
        GameManager.Instance.songPhase = 0;
        LoadPrefs();
        menu.SetActive(false);
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
        deathMenu.SetActive(false);
        foreach(GameObject h in playerHUD)
        {
            h.SetActive(true);
        }
        finishMenu.SetActive(false);
        TutorialMenu();
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void LoadPrefs()
    {
        float[] saves = GameManager.Instance.LoadPrefs();
        musicSlider.value = saves[0];
        sfxSlider.value = saves[1];
        sensitivitySlider.value = saves[2];
    }
    private void Update()
    {
        if (pauseGame.action.WasPerformedThisFrame() && !ended)
        {
            PauseButtonPressed();
        }
        else if (tutorialInput.action.WasPerformedThisFrame() && !ended)
        {
            TutorialMenu();
        }
    }
    public void EnemyHPBar()
    {
        if (GameManager.Instance.enemyHPBar)
        {
            foreach(GameObject e in enemyHPBars)
            {
                e.SetActive(false);
            }
            GameManager.Instance.enemyHPBar = false;
        }
        else
        {
            foreach (GameObject e in enemyHPBars)
            {
                e.SetActive(true);
            }
            GameManager.Instance.enemyHPBar = true;
        }
    }
    public void TutorialMenu()
    {
        if(tutorialMenu.activeSelf)
        {
            tutorialMenu.SetActive(false);
            Continue();
        }
        else
        {
            Pause();
            menu.SetActive(false);
            tutorialMenu.SetActive(true);
        }
    }
    public void PauseButtonPressed()
    {
        if (tutorialMenu.activeSelf)
        {
            Debug.Log("EGCAP");
            tutorialMenu.SetActive(false);
            Continue();
        }
        else if (!menu.activeSelf)
        {   
            Pause();
        }
        else if (notMainMenuButMenu)
        {
            MenuInGame();
        }
        else if (menu.activeSelf && !notMainMenuButMenu && !deathMenu.activeSelf)
        {
            Continue();
        }
    }
    public void Continue()
    {
        //tutorialTexts.SetActive(true);
        notMainMenuButMenu = false;
        foreach (GameObject h in playerHUD)
        {
            h.SetActive(true);
        }
        foreach (AudioSource audioSource in transform.GetComponentsInChildren<AudioSource>())
        {
            audioSource.UnPause();
        }
        menu.SetActive(false);
        GameManager.Instance.Unpause();
    }
    public void Pause()
    {
        //tutorialTexts.SetActive(false);
        menuBackground.color = backgroundSelected;
        foreach (GameObject h in playerHUD)
        {
            h.SetActive(false);
        }
        foreach (AudioSource audioSource in transform.GetComponentsInChildren<AudioSource>())
        {
            audioSource.Pause();
        }

        MenuInGame();
        GameManager.Instance.Pause();
    }
    public void MenuInGame()
    {
        notMainMenuButMenu = false;
        menu.SetActive(true);
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
        deathMenu.SetActive(false);
        finishMenu.SetActive(false);
    }
    public void Settings()
    {
        menuBackground.color = backgroundUnselected;
        notMainMenuButMenu = true;
        mainMenu.SetActive(true);
        settingsMenu.SetActive(true);
        quitMenu.SetActive(false);
        deathMenu.SetActive(false);
        finishMenu.SetActive(false);
    }
    public void Quit()
    {
        menuBackground.color = backgroundUnselected;
        notMainMenuButMenu = true;
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(true);
        deathMenu.SetActive(false);
        finishMenu.SetActive(false);
    }
    public void ConfirmQuit(bool confirm)
    {
        if (confirm)
        {
            GameManager.Instance.LoadScene(0);
        }
        else
        {
            MenuInGame();
        }
    }
    public void DeathMenu()
    {
        Cursor.lockState = CursorLockMode.Confined;
        ended = true;
        //tutorialTexts.SetActive(false);
        foreach (GameObject h in playerHUD)
        {
            h.SetActive(false);
        }
        menu.SetActive(true);
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
        deathMenu.SetActive(true);
        finishMenu.SetActive(false);
    }
    public void FinishMenu()
    {
        Pause();
        Cursor.lockState = CursorLockMode.Confined;
        ended = true;
        //tutorialTexts.SetActive(false);
        foreach (GameObject h in playerHUD)
        {
            h.SetActive(false);
        }
        menu.SetActive(true);
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
        deathMenu.SetActive(false);
        finishMenu.SetActive(true);
    }
    public void DeathMenuConfirm(int confirm)
    {
        if (confirm >= 1)
        {
            GameManager.Instance.LoadScene(confirm);
        }
        else
        {
            GameManager.Instance.LoadScene(0);
        }
    }
    public void SavePrefs()
    {
        GameManager.Instance.SavePrefs(musicSlider.value, sfxSlider.value, sensitivitySlider.value);
        GameManager.Instance.LoadPrefs();
    }
    public void LoadChallenge()
    {
        GameManager.Instance.LoadScene(2);
    }
    private void OnDisable()
    {
        tutorialInput.action.Disable();
        pauseGame.action.Disable();
    }
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}