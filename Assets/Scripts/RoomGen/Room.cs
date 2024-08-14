using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [System.Serializable]
    public struct Doors
    {
        public bool top, bottom, left, right;
    }

    [Header("Room Info")]
    public Doors doors;
    public List<GameObject> doorObjs;


    // Start is called before the first frame update
    void Start()
    {
        GetDoorObjs();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetDoorObjs()
    {
        //* Doors
        doorObjs.Clear();
        Transform doorParent = transform.Find("Doors");
        foreach (Transform child in doorParent)
        {
            doorObjs.Add(child.gameObject);
            if (child.GetComponent<Door>() == null) child.gameObject.AddComponent<Door>();
        }

        doorObjs[0].GetComponent<Door>().dir = 0;
        doorObjs[1].GetComponent<Door>().dir = 1;
        doorObjs[2].GetComponent<Door>().dir = 2;
        doorObjs[3].GetComponent<Door>().dir = 3;
    }
}
