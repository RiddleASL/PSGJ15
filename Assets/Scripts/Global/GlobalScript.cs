using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GlobalScript : MonoBehaviour
{
    [Header("Global Components")]
    [SerializeField] bool isGaming;
    public GameObject hitbox;
    public GameObject decoy;
    public GameObject barrier;
    UIController ui;
    AbilityHolder ah;
    [HideInInspector] public PlayerMotor pm;
    [HideInInspector] public SaveData sd;
    [HideInInspector] public globalData gd;
    


    // [Header("UI Controls")]
    public PlayerInput input;
    
    // Start is called before the first frame update
    void Start()
    {
        ui = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
        if(GameObject.FindGameObjectWithTag("Player") != null)
        {
            pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
            ah = GameObject.FindGameObjectWithTag("Player").GetComponent<AbilityHolder>();
        }

        sd = GetComponent<SaveData>();
        gd = GetComponent<globalData>();
        gd.LoadFromJson();

        if(isGaming) sd.LoadFromJson(gd.data.currSave);

        UnpauseGame();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        input.SwitchCurrentActionMap("Menu");
        ui.isPaused = true;
        ui.backgroundImg.SetActive(true);
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
        ui.isPaused = false;
        ui.curState = 0;
        ui.backgroundImg.SetActive(false);
        input.SwitchCurrentActionMap("Gameplay");
    }
}
