using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public enum WingsuitSelectState { Select, Selected, Purchase }
public class Game_Menu_Main : Base_Menu
{
    [SerializeField] private List<GameObject> menus = new List<GameObject>();
    //[SerializeField] private List<Button> glidersSelection = new List<Button>();
    [SerializeField] private Button purchasedOrSelected, jump;
    [SerializeField] private RawImage purchasedOrSelectedRaw;
    [SerializeField] private Texture2D selectText, selectedText, purchaseText;
    [SerializeField] private TMP_Text balance, balanceSubtract;

    private WingsuitSelectState wingsuitSelectState = WingsuitSelectState.Select;
    public int currentWingsuitSelected = -1;
    
    public void Start(){
        OnStart();
        EnableMainMenu("MainMenu");
        balance.text = "£"+ GameController.instance.PlayerCoins;
    }

    public void EnableMainMenu(string menuName){
        EnableMenu(menus, menuName);
    }

    public override void ClearPlayerData(){
        base.ClearPlayerData();
        // Update UI's
        balance.text = "£"+ GameController.instance.PlayerCoins;
        purchasedOrSelectedRaw.texture = selectText;
        purchasedOrSelected.interactable = false;
        jump.interactable = false;

    }

    public void WingsuitClicked(int wingsuitNum){
         // Update text accordingly on button
        bool wingsuitPurchased = GameController.instance.wingsuits[wingsuitNum].purchased;
        Texture2D displayText = selectText;
        // If already selected then display as selected
        if (GameController.instance.currentWingsuit == wingsuitNum){
            wingsuitSelectState = WingsuitSelectState.Selected;
            displayText = selectedText;
            purchasedOrSelected.interactable = false; // Button is disabled
        }
        else{
            if (wingsuitPurchased){
                wingsuitSelectState = WingsuitSelectState.Select;
                displayText = selectText;
                purchasedOrSelected.interactable = true;
                // Button can be pressed, if so is selection
            }
            else 
            {
                wingsuitSelectState = WingsuitSelectState.Purchase;
                displayText = purchaseText;
                purchasedOrSelected.interactable = true;
                // Button can be pressed, if so is purchase
            }
        }


        currentWingsuitSelected = wingsuitNum;
        purchasedOrSelectedRaw.texture = displayText;
    }

    public void PurchaseOrSelectWingsuit(){
        switch(wingsuitSelectState){
            case WingsuitSelectState.Select:
                GameController.SelectWingsuit(currentWingsuitSelected);
                purchasedOrSelectedRaw.texture = selectedText;
                purchasedOrSelected.interactable = false;
                jump.interactable = true;
            break;
            case WingsuitSelectState.Purchase:
                // Check if player has money for purchase
                bool boughtWingsuit = GameController.TryPurchase(currentWingsuitSelected, out string deductedCoinsText);
                if (boughtWingsuit){
                    GameController.SelectWingsuit(currentWingsuitSelected);
                    balance.text = "£"+GameController.instance.PlayerCoins;
                    balanceSubtract.text = deductedCoinsText;
                    balanceSubtract.transform.gameObject.SetActive(true);
                    purchasedOrSelectedRaw.texture = selectedText;
                    purchasedOrSelected.interactable = false;
                    jump.interactable = true;
                }
                else{
                    balanceSubtract.text = "Not Enough £!!";
                    balanceSubtract.transform.gameObject.SetActive(true);
                }
            break;
        }
    }
}
