using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        //Generate Start Room
        GameObject start = Instantiate(roomPrefabs[0], Vector3.zero, Quaternion.identity, transform);
        spawnedRooms.Add(start);
        currentRooms++;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentRooms < desiredRooms){
            GenerateRoom();
        } else if(!redundantDoors){
            foreach(GameObject room in spawnedRooms){
                Room r = room.GetComponent<Room>();
                foreach(GameObject door in r.transform.Find("Doors")){
                    Door d = door.GetComponent<Door>();
                    if(!d.isOccupied){
                        redundantDoors = false;
                        return;
                    }
                    
                }
            }
            redundantDoors = true;
        }
    }

    void GenerateRoom(){
        //Get a random spawned room
        GameObject selectedRoom = spawnedRooms[Random.Range(0, spawnedRooms.Count)];
        Room room = selectedRoom.GetComponent<Room>();

        //Get number of doors
        int doors = room.doorObjs.Count;
        int roomsToSpawn = Random.Range(1, doors);

        //Spawn rooms
        for(int i = 0; i < roomsToSpawn;){
            int doorIndex = Random.Range(0, doors-1);
            Door door = room.doorObjs[doorIndex].GetComponent<Door>();
            if(door.isOccupied){
                roomsToSpawn--;
                continue;
            }

            Vector2 spawnDir = door.GetSpawnDir();
            Vector3 spawnPos = selectedRoom.transform.position + new Vector3(spawnDir.x * roomSize, spawnDir.y * roomSize, 0);
            if(spawnedRooms.Exists(x => x.transform.position == spawnPos)){
                roomsToSpawn--;
                continue;
            }
            AddRoom(spawnPos, door.dir);
            door.isOccupied = true;
            i++;
        }
    }

    public void AddRoom(Vector3 spawnPos, int fromDir){
        int toDoor = 0;
        switch(fromDir){
            case 0:
                toDoor = 2;
                break;
            case 1:
                toDoor = 3;
                break;
            case 2:
                toDoor = 0;
                break;
            case 3:
                toDoor = 1;
                break;
        }

        List<GameObject> possibleRooms = new List<GameObject>();
        int availableSpaces = CalcSpaces(spawnPos);

        foreach(GameObject room in roomPrefabs){;
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
        Room newRR = newRoom.GetComponent<Room>();

        foreach(GameObject door in newRR.doorObjs){
            if(door.GetComponent<Door>().dir == toDoor){
                door.GetComponent<Door>().isOccupied = true;
            }
        }

        currentRooms++;
        spawnedRooms.Add(newRoom);
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
}

