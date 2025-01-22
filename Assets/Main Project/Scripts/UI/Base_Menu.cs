using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;

public class Base_Menu : MonoBehaviour
{
    public List<GameObject> settingsMenus;
    public TMP_Dropdown resolutionDropdown, qualityDropdown;
    public Slider volumeMasterSlider, volumeMusicSlider, volumeSoundEffectsSlider;
    public float buttonVolume = 0.5f;

    public void OnStart(){

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(GameController.instance.ResolutionOptions);
        resolutionDropdown.value = GameController.instance.currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        qualityDropdown.value = GameController.instance.currentQualityIndex;
        qualityDropdown.RefreshShownValue();

        for (int i = 0; i < GameController.instance.audioVolumes.Count; i++){
            if (GameController.instance.audioVolumes[i].key == "MasterVolume"){
                volumeMasterSlider.value = GameController.instance.audioVolumes[i].volumeLevel;
            }
            else if (GameController.instance.audioVolumes[i].key == "MusicVolume"){
                volumeMusicSlider.value = GameController.instance.audioVolumes[i].volumeLevel;
            }
            else if (GameController.instance.audioVolumes[i].key == "SoundEffectsVolume"){
                volumeSoundEffectsSlider.value = GameController.instance.audioVolumes[i].volumeLevel;
            }
            
        }
    }

    public void EnableMenu(List<GameObject> menus, string menuName){
        foreach (GameObject menu in menus){
            if (menu.name == menuName){
                menu.SetActive(true);
            }
            else{
                menu.SetActive(false);
            }
        }
    }

    public void LoadGameScene(){
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void LoadMenuScene(){
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void QuitGame(){
        GameController.SaveCoins();
        GameController.SaveWingsuitPurchased();
        GameController.SaveSettings();
        Application.Quit();
    }

    public void EnableSettingsMenu(string menuName){
        EnableMenu(settingsMenus, menuName);
    }

    public virtual void ClearPlayerData(){
        GameController.ResetPlayer();
    }

    public void ToggleFullScreen(bool toggle){
        GameController.ToggleFullScreen(toggle);
    }

    public void SetResolution(int resolutionIndex){
        GameController.SetResolution(resolutionIndex);
    }

    public void SetQuality(int qualityIndex){
        GameController.SetQuality(qualityIndex);
    }

    public void SetMasterVolume(float level){
        GameController.SetVolume("MasterVolume", level);
    }

    public void SetSFXVolume(float level){
        GameController.SetVolume("SoundEffectsVolume", level);
    }

    public void SetMusicVolume(float level){
        GameController.SetVolume("MusicVolume", level);
    }

    public void PlayButtonSound(){
        Audio_Controller.PlaySound(SoundType.Button, buttonVolume);
    }
}
