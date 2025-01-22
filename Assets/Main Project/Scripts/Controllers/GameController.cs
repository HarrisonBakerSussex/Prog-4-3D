using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameController : MonoBehaviour
{
    public List<Wingsuit> wingsuits = new List<Wingsuit>(1);
    public List<AudioVolume> audioVolumes = new List<AudioVolume>(3);
    private Resolution[] resolutions;
    public List<string> ResolutionOptions { get; private set; }
    public int currentResolutionIndex {get; private set;}
    public int currentQualityIndex {get; private set;}
    public AudioMixer audioMixer;
    [SerializeField] private int startCoins = 200;
    public int currentWingsuit = -1;
    public int PlayerCoins {get { return playerCoins; }}
    [SerializeField] private int playerCoins = 0;
    [SerializeField] private string coinsKey = "Coins";
    [SerializeField] private string previousPBKey = "PreviousPB";
    public int PreviousPb {get { return previousPb; }}
    private bool isFullscreen = false;
    private int previousPb = 0;
    private int defaultResolution;
    public static GameController instance {get; private set;}

    private void Awake(){
        // Need to not destroy on load
        if (instance != null){
            Destroy(gameObject);
        }
        else{
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        ResolutionOptions = new List<string>();
        resolutions = Screen.resolutions;
        defaultResolution = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            ResolutionOptions.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                defaultResolution = i;
        }
    }

    private void OnEnable(){
        GameController.LoadWingsuitPurchased();
        GameController.LoadCoins();
        GameController.LoadSettings();
        GameController.LoadPreviousPB();
    }

    private void OnDisable(){
        GameController.SaveWingsuitPurchased();
        GameController.SaveSettings();
        GameController.SaveCoins();
    }

    public static void ResetPlayer(){
        // Need to delete player data for wingsuits and coins
        GameController.DeleteWingsuitsPurchased();
        PlayerPrefs.DeleteKey(instance.coinsKey);
        PlayerPrefs.DeleteKey(instance.previousPBKey);
        instance.currentWingsuit = -1;
        GameController.LoadWingsuitPurchased();
        GameController.LoadCoins();
    }

    private static void LoadWingsuitPurchased(){
        List<Wingsuit> wingsuits = instance.wingsuits;
        for(int i = 0; i < wingsuits.Count; i++){
            wingsuits[i].purchased = false;
            if (PlayerPrefs.HasKey(wingsuits[i].name)){
                wingsuits[i].purchased = PlayerPrefs.GetInt(wingsuits[i].name) == 0 ? false : true;
            }
        }
        instance.wingsuits = wingsuits;
    }

    public static void SelectWingsuit(int wingsuitNumber){
        instance.currentWingsuit = wingsuitNumber;
    }

    public static void SaveWingsuitPurchased(){
        List<Wingsuit> wingsuits = instance.wingsuits;
        for(int i = 0; i < wingsuits.Count; i++){
            PlayerPrefs.SetInt(wingsuits[i].name, wingsuits[i].purchased == false ? 0 : 1);
        }
    }

    private static void DeleteWingsuitsPurchased(){
        List<Wingsuit> wingsuits = instance.wingsuits;
        for(int i = 0; i < wingsuits.Count; i++){
            PlayerPrefs.DeleteKey(wingsuits[i].name);
        }
    }

    private static void LoadCoins(){
        instance.playerCoins = instance.startCoins;
        if (PlayerPrefs.HasKey(instance.coinsKey)){
            instance.playerCoins = PlayerPrefs.GetInt(instance.coinsKey);
        }
    }
    
    public static void AddCoins(int coins){
        instance.playerCoins += coins;
        GameController.SaveCoins();
    }

    public static void SaveCoins(){
        PlayerPrefs.SetInt(instance.coinsKey, instance.playerCoins);
    }

    private static void LoadPreviousPB(){
        instance.previousPb = 0;
        if (PlayerPrefs.HasKey(instance.previousPBKey)){
            instance.previousPb = PlayerPrefs.GetInt(instance.previousPBKey);
        }
    }

    public static void SetNewPB(int newPB){
        PlayerPrefs.SetInt(instance.previousPBKey, newPB);
    }

    public static bool TryPurchase(int wingsuitNum, out string deductedCoinsText){
        bool success = instance.playerCoins >= instance.wingsuits[wingsuitNum].cost;
        if (success){
            instance.playerCoins -= instance.wingsuits[wingsuitNum].cost;
            instance.wingsuits[wingsuitNum].purchased = true;
        }
        deductedCoinsText = "-"+instance.wingsuits[wingsuitNum].cost;
        return success;
    }

    public static void LoadSettings(){
        instance.isFullscreen = false;
        if (PlayerPrefs.HasKey("Fullscreen")) instance.isFullscreen = PlayerPrefs.GetInt("Fullscreen") == 0 ? false : true;
        Screen.fullScreen = instance.isFullscreen;

        instance.currentResolutionIndex = instance.defaultResolution;
        if (PlayerPrefs.HasKey("ResolutionIndex")) instance.currentResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
        Resolution resolution = instance.resolutions[instance.currentResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        instance.currentQualityIndex = 0;
        if (PlayerPrefs.HasKey("QualityIndex")) instance.currentQualityIndex = PlayerPrefs.GetInt("QualityIndex");
        QualitySettings.SetQualityLevel(instance.currentQualityIndex);

        List<AudioVolume> audioVolumes = instance.audioVolumes;
        for (int i = 0; i < audioVolumes.Count; i++){
            audioVolumes[i].volumeLevel = 0.5f;
            if (PlayerPrefs.HasKey(audioVolumes[i].key)) audioVolumes[i].volumeLevel = PlayerPrefs.GetFloat(audioVolumes[i].key);
            instance.audioMixer.SetFloat(audioVolumes[i].key, Mathf.Log10(audioVolumes[i].volumeLevel) * 20f);
        }
        instance.audioVolumes = audioVolumes;
    }

    public static void SaveSettings(){
        PlayerPrefs.SetInt("Fullscreen", instance.isFullscreen ? 1 : 0);
        PlayerPrefs.SetInt("ResolutionIndex", instance.currentResolutionIndex);
        PlayerPrefs.SetInt("QualityIndex", instance.currentQualityIndex);

        for (int i = 0; i < instance.audioVolumes.Count; i++){
            PlayerPrefs.SetFloat(instance.audioVolumes[i].key, instance.audioVolumes[i].volumeLevel);
        }
    }

    public static void ToggleFullScreen(bool toggle){
        instance.isFullscreen = toggle;
        Screen.fullScreen = toggle;
    }

    public static void SetResolution(int resolutionIndex){
        instance.currentResolutionIndex = resolutionIndex;
        Resolution resolution = instance.resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public static void SetQuality(int qualityIndex){
        instance.currentQualityIndex = qualityIndex;
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public static void SetVolume(string volumeKey, float level){
        for (int i = 0; i < instance.audioVolumes.Count; i++){
            if (instance.audioVolumes[i].key == volumeKey) instance.audioVolumes[i].volumeLevel = level; // Volume is the slider
        }
        instance.audioMixer.SetFloat(volumeKey, Mathf.Log10(level) * 20f);
    }
}

[System.Serializable]
public class AudioVolume {
    public string key;
    public float volumeLevel;
}
