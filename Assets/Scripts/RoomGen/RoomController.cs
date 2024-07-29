using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("Generation Info")]
    public int desiredRooms = 10;
    int currentRooms;

    [Header("Room Prefabs")]
    public GameObject[] roomPrefabs;
    int roomSize = 20;
    bool redundantDoors = false;
    List<GameObject> spawnedRooms = new List<GameObject>();
    List<GameObject> finishedRooms = new List<GameObject>();
    List<GameObject> fullRooms = new List<GameObject>();

    [Header("Error Handling")]
    [SerializeField] float errorTimer = 2f;

    // Start is called before the first frame update
    void Start()
    {
        //Generate Start Room
        startRoom();
    }

    // Update is called once per frame
    void Update()
    {
        if(errorTimer > 0) errorTimer -= Time.deltaTime;
        if(errorTimer <= 0 && currentRooms < desiredRooms){
            Debug.LogError("Room Generation Error: Not enough rooms generated");
            ResetRoomGen();
            errorTimer = 2f;
        }

        if(currentRooms < desiredRooms){
            GenerateRoom();
        } else if(!redundantDoors){
            finishedRooms.AddRange(spawnedRooms);
            doorsOccupied();
            removeUnconnectedDoors();
            redundantDoors = true;
        }
    }

    void startRoom(){
        GameObject start = Instantiate(roomPrefabs[0], Vector3.zero, Quaternion.identity, transform);
        spawnedRooms.Add(start);
        currentRooms++;
    }

    void GenerateRoom(){
        //Get a random spawned room
        if(spawnedRooms.Count == 0){
            spawnedRooms = new List<GameObject>(finishedRooms);
            finishedRooms.Clear();
        }

        GameObject selectedRoom = spawnedRooms[Random.Range(0, spawnedRooms.Count-1)];
        Room room = selectedRoom.GetComponent<Room>();

        //Get number of doors
        int doors = room.doorObjs.Count;
        int roomsToSpawn = Random.Range(1, doors);
        if(spawnedRooms.Count == 1){
            roomsToSpawn = 4;
        }

        //Spawn rooms
        for(int i = 0; i < roomsToSpawn;){
            int doorIndex = Random.Range(0, doors-1);
            room.GetDoorObjs();
            Door door = room.doorObjs[doorIndex].GetComponent<Door>();
            if(door.isOccupied){
                roomsToSpawn--;
                continue;
            }

            Vector2 spawnDir = door.GetSpawnDir();
            Vector3 spawnPos = (Vector2)selectedRoom.transform.position + (spawnDir * roomSize);
            if(isOccupied(spawnPos)){
                roomsToSpawn--;
                continue;
            }
            AddRoom(spawnPos, door.dir);
            door.isOccupied = true;
            i++;
            if((i + 1) == roomsToSpawn){
                roomsToSpawn = 0;
            }
        }

        //check if room is finished
        if(roomsToSpawn == 0){
            finishedRooms.Add(selectedRoom);
            spawnedRooms.Remove(selectedRoom);
        }
    }

    void AddRoom(Vector3 spawnPos, int fromDir){
        List<GameObject> possibleRooms = new List<GameObject>();
        int availableSpaces = CalcSpaces(spawnPos);

        foreach(GameObject room in roomPrefabs){
            Room r = room.GetComponent<Room>();
            int doors = 0;
            if(r.doors.top) doors++;
            if(r.doors.right) doors++;
            if(r.doors.bottom) doors++;
            if(r.doors.left) doors++;

            if(doors > availableSpaces){
                continue;
            }

            bool top = r.doors.top;
            bool right = r.doors.right;
            bool bottom = r.doors.bottom;
            bool left = r.doors.left;

            if(fromDir == 0 && bottom){
                possibleRooms.Add(room);
            } else if(fromDir == 1 && left){
                possibleRooms.Add(room);
            } else if(fromDir == 2 && top){
                possibleRooms.Add(room);
            } else if(fromDir == 3 && right){
                possibleRooms.Add(room);
            }
        }

        GameObject newRoom = Instantiate(possibleRooms[Random.Range(0, possibleRooms.Count-1)], spawnPos, Quaternion.identity, transform);
        Room newR = newRoom.GetComponent<Room>();
        newR.GetDoorObjs();

        currentRooms++;
        errorTimer = 2f;

        int doorsTaken = 0;
        Debug.Log("Doors: " + newR.doorObjs.Count + " | newRoom: " + newRoom.gameObject.name);
        foreach(GameObject door in newR.doorObjs){
            Door d = door.GetComponent<Door>();
            Vector2 checkPos = Vector2.zero;
            switch(d.dir){
                case 0:
                    checkPos = (Vector2)newRoom.transform.position + new Vector2(0, roomSize);
                    break;
                case 1:
                    checkPos = (Vector2)newRoom.transform.position + new Vector2(roomSize, 0);
                    break;
                case 2:
                    checkPos = (Vector2)newRoom.transform.position + new Vector2(0, -roomSize);
                    break;
                case 3:
                    checkPos = (Vector2)newRoom.transform.position + new Vector2(-roomSize, 0);
                    break;
            }

            d.isOccupied = isOccupied(checkPos);
            if(isOccupied(checkPos)) doorsTaken++;
        }

        // Debug.Log("Doors Taken: " + doorsTaken + " | Doors: " + newR.doorObjs.Count);

        if(doorsTaken == newR.doorObjs.Count){
            fullRooms.Add(newRoom);
        } else {
            spawnedRooms.Add(newRoom);
        }
    }

    int CalcSpaces(Vector3 pos){
        int spaces = 4;
        
        foreach(GameObject room in spawnedRooms){
            Room r = room.GetComponent<Room>();
            if(r.transform.position == pos + new Vector3(0, roomSize, 0)){
                spaces--;
            }
            if(r.transform.position == pos + new Vector3(roomSize, 0, 0)){
                spaces--;
            }
            if(r.transform.position == pos + new Vector3(0, -roomSize, 0)){
                spaces--;
            }
            if(r.transform.position == pos + new Vector3(-roomSize, 0, 0)){
                spaces--;
            }
        }

        Debug.Log("Spaces: " + spaces);
        return spaces;
    }

    public bool isOccupied(Vector3 pos){
        foreach(GameObject room in spawnedRooms){
            if(room.transform.position == pos){
                return true;
            }
        }
        foreach(GameObject room in finishedRooms){
            if(room.transform.position == pos){
                return true;
            }
        }
        return false;
    }

    void doorsOccupied(){
        foreach(GameObject room in finishedRooms){
            Room r = room.GetComponent<Room>();
            foreach(GameObject door in r.doorObjs){
                Door d = door.GetComponent<Door>();
                Vector2 checkPos = Vector2.zero;

                switch(d.dir){
                    case 0:
                        checkPos = (Vector2)room.transform.position + new Vector2(0, roomSize);
                        break;
                    case 1:
                        checkPos = (Vector2)room.transform.position + new Vector2(roomSize, 0);
                        break;
                    case 2:
                        checkPos = (Vector2)room.transform.position + new Vector2(0, -roomSize);
                        break;
                    case 3:
                        checkPos = (Vector2)room.transform.position + new Vector2(-roomSize, 0);
                        break;
                }

                GameObject testRoom = spawnedRooms.Find(x => (Vector2)x.transform.position == checkPos);
                if(testRoom != null){
                    Room testR = testRoom.GetComponent<Room>();
                    foreach(GameObject testDoor in testR.doorObjs){
                        Door testD = testDoor.GetComponent<Door>();
                        if(testD.dir == (d.dir + 2) % 4){
                            testD.isOccupied = true;
                        }
                    }
                    
                }
            }
        }
    }

    void removeUnconnectedDoors(){
        foreach(GameObject room in finishedRooms){
            Room r = room.GetComponent<Room>();
            foreach(GameObject door in r.doorObjs){
                Door d = door.GetComponent<Door>();
                if(!d.isOccupied){
                    d.gameObject.SetActive(false);
                }
            }
        }
    }

    void ResetRoomGen(){
        currentRooms = 0;
        spawnedRooms.Clear();
        finishedRooms.Clear();
        fullRooms.Clear();
        redundantDoors = false;
        foreach(Transform child in transform){
            Destroy(child.gameObject);
        }
        
        startRoom();
    }
}