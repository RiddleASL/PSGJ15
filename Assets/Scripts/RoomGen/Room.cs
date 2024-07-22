using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [System.Serializable]
    public struct Doors{
        public bool top, bottom, left, right;
    }

    [Header("Room Info")]
    public Doors doors;
    public List<GameObject> doorObjs;
    

    // Start is called before the first frame update
    void Start()
    {
        //* Doors
        Transform doorParent = transform.Find("Doors");
        foreach(Transform child in doorParent){
            child.gameObject.SetActive(false);
        }

        if(doors.top){
            GameObject door = doorParent.GetChild(0).gameObject;
            door.SetActive(true);
            door.AddComponent<Door>().dir = 0;
            doorObjs.Add(door);
        }
        if(doors.right){
            GameObject door = doorParent.GetChild(1).gameObject;
            door.SetActive(true);
            door.AddComponent<Door>().dir = 1;
            doorObjs.Add(door);
        }
        if(doors.bottom){
            GameObject door = doorParent.GetChild(2).gameObject;
            door.SetActive(true);
            door.AddComponent<Door>().dir = 2;
            doorObjs.Add(door);
        }
        if(doors.left){
            GameObject door = doorParent.GetChild(3).gameObject;
            door.SetActive(true);
            door.AddComponent<Door>().dir = 3;
            doorObjs.Add(door);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
