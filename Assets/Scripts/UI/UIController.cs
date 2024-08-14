using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    GlobalScript gs;

    //Bools
    public bool isPaused;
    [HideInInspector] public int curState; //0 pause, 1 settings
    public GameObject pauseMenu, settingsMenu, infoMenu, deathScreen;
    public GameObject backgroundImg;

    [Header("Settings")]
    [SerializeField] GameObject settingsArrow;
    public List<string> displayRatio = new List<string>();
    public List<Vector2> displayRes = new List<Vector2>();
    [SerializeField] TextMeshProUGUI ratioText;
    [SerializeField] TextMeshProUGUI resText;
    int currRatio = 0;
    int tempRatio = 0;
    int currRes = 0;
    int tempRes = 0;

    [Header("HUD")]
    public GameObject hud;
    public TextMeshProUGUI healthText;
    int maxHp;
    int currHp;

    [Header("Selection Arrow")]
    public GameObject selectionArrow;
    public Vector2 arrowOffset;

    [Header("Selection Border")]
    public GameObject selectionBorder;
    public Vector2 borderOffset;
    public float borderSize;




    void Start() {
        gs = GameObject.FindGameObjectWithTag("Global").GetComponent<GlobalScript>();
        isPaused = false;


        //Settings
        displayRatio.Add("16 : 9"); displayRes.Add(new Vector2(1920, 1080)); displayRes.Add(new Vector2(1280, 720)); displayRes.Add(new Vector2(854, 480));
        displayRatio.Add("16 : 10"); displayRes.Add(new Vector2(1920, 1200)); displayRes.Add(new Vector2(1280, 800)); displayRes.Add(new Vector2(854, 480));
        displayRatio.Add("4 : 3"); displayRes.Add(new Vector2(1600, 1200)); displayRes.Add(new Vector2(1024, 768)); displayRes.Add(new Vector2(800, 600));

        //HUD
        maxHp = (int)gs.pm.maxHealth;
        currHp = (int)gs.pm.currHealth;
    }

    void Update() {
        handleUI();    
        handleHUD();
    }

    //! Settings
    //Resolution
    public void increaseRes(){
        if(tempRes >= 2){
            tempRes = 0;
        } else {
            tempRes++;
        }
    }
    public void decreaseRes(){
        if(tempRes <= 0){
            tempRes = 2;
        } else {
            tempRes--;
        }
    }

    public void moveArrows(Transform pos){
        settingsArrow.transform.position = pos.position;
    }

    //Ratio
    public void increaseRatio(){
        if(tempRatio >= 2){
            tempRatio = 0;
        } else {
            tempRatio++;
        }
    }
    public void decreaseRatio(){
        if(tempRatio <= 0){
            tempRatio = 2;
        } else {
            tempRatio--;
        }
    }

    public void ApplySettings(){
        Screen.SetResolution((int)displayRes[currRes + (currRatio * 3)].x, (int)displayRes[currRes + (currRatio * 3)].y, true);
        currRes = tempRes;
        currRatio = tempRatio;
        ChangeState(0);
    }
    public void CancelSettings(){
        tempRes = currRes;
        tempRatio = currRatio;
        ChangeState(0);
    }

    public void mouseHoverArrow(Transform pos){
        Debug.Log("Mouse Hover");
        selectionArrow.SetActive(true);
        selectionArrow.transform.position = pos.position + new Vector3(arrowOffset.x, arrowOffset.y, 0);
    }

    public void mouseExitArrow(){
        Debug.Log("Mouse Exit");
        selectionArrow.SetActive(false);
    }

    public void mouseHoverBorder(Transform pos){
        Debug.Log("Mouse Hover");
        selectionBorder.transform.localScale = pos.localScale * borderSize;
        selectionBorder.SetActive(true);
        selectionBorder.transform.position = pos.position + new Vector3(borderOffset.x, borderOffset.y, 0);
    }

    public void mouseExitBorder(){
        Debug.Log("Mouse Exit");
        selectionBorder.SetActive(false);
    }

    //Pause Menu Stuff
    public void resumeGame(){
        gs.UnpauseGame();
    }

    public void ChangeState(int state){
        curState = state;
        selectionArrow.SetActive(false);
        selectionBorder.SetActive(false);
    }

    public void ExitToMenu(){
        SceneManager.LoadScene(0);
    }

    void handleUI() {
        if(isPaused){
            switch (curState)
            {
                case 0:
                    pauseMenu.SetActive(true);
                    settingsMenu.SetActive(false);
                    infoMenu.SetActive(false);
                    break;
                case 1:
                    pauseMenu.SetActive(false);
                    settingsMenu.SetActive(true);
                    infoMenu.SetActive(false);
                    break;
                case 2:
                    pauseMenu.SetActive(false);
                    settingsMenu.SetActive(false);
                    infoMenu.SetActive(true);
                    break;
                case 3:
                    pauseMenu.SetActive(false);
                    settingsMenu.SetActive(false);
                    infoMenu.SetActive(true);
                    break;
            }

            ratioText.text = displayRatio[tempRatio];
            resText.text = displayRes[tempRes + (tempRatio * 3)].x + " x " + displayRes[tempRes + (tempRatio * 3)].y;
        } else {
            selectionArrow.SetActive(false);
            selectionBorder.SetActive(false);
            pauseMenu.SetActive(false);
            settingsMenu.SetActive(false);
            infoMenu.SetActive(false);
        }
    }

    void handleHUD(){
        if(gs.pm != null){
            healthText.text = gs.pm.currHealth + " / " + maxHp;
        }

        if(gs.pm.currHealth <= 0){
            death();
        }
    }

    void death(){
        //hide hud
        hud.SetActive(false);

        //hide ui
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        infoMenu.SetActive(false);
        backgroundImg.SetActive(false);

        //show death screen
        deathScreen.SetActive(true);
    }
}