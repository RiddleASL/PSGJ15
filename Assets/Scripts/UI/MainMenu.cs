using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    GlobalScript gs;

    //Bools
    [HideInInspector] public int curState; //0 MENU, 1 SETTINGS, 2 START, 3 CHAR SELECT
    public GameObject mainMenu,charSelect, settingsMenu, startMenu;
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

    [Header("Selection Arrow")]
    public GameObject selectionArrow;
    public Vector2 arrowOffset;

    [Header("Selection Border")]
    public GameObject selectionBorder;
    public Vector2 borderOffset;
    public float borderSize;

    [Header("Save File 1")]
    SaveData sd1 = new SaveData();
    public TextMeshProUGUI data;
    public TextMeshProUGUI newOverwrite;
    public GameObject loadButton;
    bool save1Exists;

    [Header("Save File 2")]
    SaveData sd2 = new SaveData();
    public TextMeshProUGUI data2;
    public TextMeshProUGUI newOverwrite2;
    public GameObject loadButton2;
    bool save2Exists;


    void Start() {
        gs = GameObject.FindGameObjectWithTag("Global").GetComponent<GlobalScript>();
        checkForSaves();

        //Settings
        displayRatio.Add("16 : 9"); displayRes.Add(new Vector2(1920, 1080)); displayRes.Add(new Vector2(1280, 720)); displayRes.Add(new Vector2(854, 480));
        displayRatio.Add("16 : 10"); displayRes.Add(new Vector2(1920, 1200)); displayRes.Add(new Vector2(1280, 800)); displayRes.Add(new Vector2(854, 480));
        displayRatio.Add("4 : 3"); displayRes.Add(new Vector2(1600, 1200)); displayRes.Add(new Vector2(1024, 768)); displayRes.Add(new Vector2(800, 600));
    }

    void Update() {
        handleUI();    
    }

    void checkForSaves(){
        if(System.IO.File.Exists(Application.persistentDataPath + "/saveData0.json")){
            save1Exists = true;
            sd1.LoadFromJson(0);
        } else {
            save1Exists = false;
        }

        if(System.IO.File.Exists(Application.persistentDataPath + "/saveData1.json")){
            save2Exists = true;
            sd2.LoadFromJson(1);
        } else {
            save2Exists = false;
        }
    }

    public void newGame(int saveFile){
        gs.gd.data.currSave = saveFile;
        gs.sd.SaveToJson(saveFile);
        gs.gd.SaveToJson();
        
        ChangeState(3);
    }

    public void loadGame(int saveFile){
        //load game
        gs.gd.data.currSave = saveFile;
        gs.gd.SaveToJson();
        SceneManager.LoadScene(1);
    }

    public void SelectChar(int charNum){
        gs.sd.saveData.selectedCharacter = charNum;
        gs.sd.SaveToJson(gs.gd.data.currSave);
        SceneManager.LoadScene(1);
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
        Debug.Log("Mouse Hover: " + pos.gameObject);
        selectionBorder.transform.localScale = pos.localScale * borderSize;
        selectionBorder.transform.position = pos.position + new Vector3(borderOffset.x, borderOffset.y, 0);
        selectionBorder.SetActive(true);
    }

    public void mouseExitBorder(){
        Debug.Log("Mouse Exit");
        selectionBorder.SetActive(false);
    }


    public void ChangeState(int state){
        mouseExitBorder();
        mouseExitArrow();
        
        curState = state;
    }

    public void ExitGame(){
        Application.Quit();
    }

    void handleUI() {
        switch (curState)
        {
            case 0:
                mainMenu.SetActive(true);
                charSelect.SetActive(false);
                settingsMenu.SetActive(false);
                startMenu.SetActive(false);
                break;
            case 1:
                mainMenu.SetActive(false);
                charSelect.SetActive(false);
                settingsMenu.SetActive(true);
                startMenu.SetActive(false);
                break;
            case 2:
                mainMenu.SetActive(false);
                charSelect.SetActive(false);
                settingsMenu.SetActive(false);
                startMenu.SetActive(true);
                break;
            case 3:
                mainMenu.SetActive(false);
                charSelect.SetActive(true);
                settingsMenu.SetActive(false);
                startMenu.SetActive(false);
                break;
        }

        //Settings
        ratioText.text = displayRatio[tempRatio];
        resText.text = displayRes[tempRes + (tempRatio * 3)].x + " x " + displayRes[tempRes + (tempRatio * 3)].y;

        //Save File 1
        if(save1Exists){
            int currChar = sd1.saveData.selectedCharacter;
            data.text = sd1.saveData.characters[currChar].name;
            newOverwrite.text = "Overwrite";
            loadButton.SetActive(true);
        } else {
            data.text = "Empty";
            newOverwrite.text = "new game";
            loadButton.SetActive(false);
        }

        //Save File 2
        if(save2Exists){
            int currChar = sd2.saveData.selectedCharacter;
            data2.text = sd2.saveData.characters[currChar].name;
            newOverwrite2.text = "Overwrite";
            loadButton2.SetActive(true);
        } else {
            data2.text = "Empty";
            newOverwrite2.text = "new game";
            loadButton2.SetActive(false);
        }
    }
}
