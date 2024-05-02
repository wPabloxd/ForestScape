using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }
    public int playerDetectedCounter;
    public float sensitivity;
    public float musicVolume;
    public float sfxVolume;
    public bool paused;
    public int songPhase;
    public int spawnHealingProbability;
    public bool sensitivityChanged;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] GameObject volumeProfile;
    [SerializeField] AudioMixerGroup musicMixer;
    public int tutorialCompleted;
    public int bufferAmmoUSP;
    public int bufferAmmoM4;
    public int bufferAmmoSG;
    public bool enemyHPBar;
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
        DontDestroyOnLoad(this.gameObject);

        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 1f);
        }
        if (!PlayerPrefs.HasKey("SFXVolume"))
        {
            PlayerPrefs.SetFloat("SFXVolume", 1f);
        }
        if (!PlayerPrefs.HasKey("Sensitivity"))
        {
            PlayerPrefs.SetFloat("Sensitivity", 100f);
        }
    }
    private void Update()
    {
        if(playerDetectedCounter == 0)
        {
            songPhase = 0;
        }
        else
        {
            songPhase = 1;
        }
    }
    public float[] LoadPrefs()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
        sensitivity = PlayerPrefs.GetFloat("Sensitivity");
        float[] prefsValues = { musicVolume, sfxVolume, sensitivity };
        UpdateSettings();
        return prefsValues;
    }
    void UpdateSettings()
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
    }
    public void Pause()
    {
        paused = true;
        PauseSounds();
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
    }
    void PauseSounds()
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in allAudioSources)
        {
            if(audioSource.outputAudioMixerGroup == !musicMixer)
            {
                audioSource.Pause();
            }
        }
    }
    void ResumeSounds()
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in allAudioSources)
        {
            audioSource.UnPause();
        }
    }
    public void Unpause()
    {
        paused = false;
        ResumeSounds();
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void SavePrefs(float musicInput, float sfxInput, float sensitivityInput)
    {
        musicVolume = musicInput;
        sfxVolume = sfxInput;
        sensitivity = sensitivityInput;
        PlayerPrefs.SetFloat("MusicVolume", musicInput);
        PlayerPrefs.SetFloat("SFXVolume", sfxInput);
        PlayerPrefs.SetFloat("Sensitivity", sensitivityInput);
        PlayerPrefs.Save();
        UpdateSettings();
        sensitivityChanged = true;
    }
    public void LoadScene(int sceneId)
    {
        songPhase = 0;
        spawnHealingProbability = 0;
        SceneManager.LoadScene(sceneId);
    }
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}