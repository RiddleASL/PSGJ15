using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class DungeonCon : MonoBehaviour
{
    public List<GameObject> roomPrefabs;
    public int numberOfRooms;
    public int seed;
    int currentRooms;

    //Hidden Variables
    List<GameObject> rooms = new List<GameObject>();
    List<GameObject> ignoreRooms = new List<GameObject>();
    int roomSize = 40;

    bool doorCheck;

    // Start is called before the first frame update
    void Start()
    {
        spawnStartRoom();
        startGen();

        if(seed == 0) seed = Random.Range(0, 1000000);
        Random.InitState(seed);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentRooms >= numberOfRooms){
            Debug.Log("Generation Complete");
            rooms.AddRange(ignoreRooms);
            ignoreRooms.Clear();
            
            if(!doorCheck){
                finalDoors();
                doorCheck = true;
                AstarPath.active.Scan();
            }
        }
    }

    void spawnStartRoom()
    {
        GameObject room = Instantiate(roomPrefabs[0], new Vector3(0, 0, 0), Quaternion.identity);
        rooms.Add(room);
        currentRooms++;
    }

    void startGen()
    {
        while(currentRooms < numberOfRooms){
            genRoom();
        }
    }

    void genRoom()
    {
        if(currentRooms >= numberOfRooms) return;

        //Get random room from list
        GameObject room = rooms[Random.Range(0, rooms.Count-1)];
        Room r = room.GetComponent<Room>();

        //Generate possible rooms
        List<GameObject> possibleRooms = new List<GameObject>();
        foreach(GameObject roomPrefab in roomPrefabs){
            Room testRoom = roomPrefab.GetComponent<Room>();
            if(r.doors.top == testRoom.doors.bottom) possibleRooms.Add(roomPrefab);
            if(r.doors.bottom == testRoom.doors.top) possibleRooms.Add(roomPrefab);
            if(r.doors.left == testRoom.doors.right) possibleRooms.Add(roomPrefab);
            if(r.doors.right == testRoom.doors.left) possibleRooms.Add(roomPrefab);
        }

        //Get possible spawn directions
        List<int> posDirs = new List<int>();
        for(int i = 0; i < 4; i++){
            if(r.doors.top) posDirs.Add(0);
            if(r.doors.right) posDirs.Add(1);
            if(r.doors.bottom) posDirs.Add(2);
            if(r.doors.left) posDirs.Add(3);
        }

        //random how many rooms to spawn
        int roomsToSpawn = Random.Range(1, posDirs.Count);

        //Spawn rooms
        for(int i = 0; i < roomsToSpawn; i++){
            int dir = posDirs[Random.Range(0, posDirs.Count-1)];
            Vector2 offsetDir = getOffset(dir);
            Vector3 finalOffset = (Vector2)room.transform.position + offsetDir;

            GameObject testRoom = rooms.Find(x => x.transform.position == finalOffset);
            GameObject testRoom2 = ignoreRooms.Find(x => x.transform.position == finalOffset);

            if(testRoom != null || testRoom2 != null){
                roomsToSpawn--;
                posDirs.Remove(dir);

                continue;
            }

            addRoom(room, finalOffset, possibleRooms);
        }

        //Add current room to ignore list
        ignoreRooms.Add(room);
        rooms.Remove(room);
    }

    void addRoom(GameObject currRoom, Vector3 finalOffset, List<GameObject> possibleRooms){
        //Generate room
        GameObject newRoom = Instantiate(possibleRooms[Random.Range(0, possibleRooms.Count-1)], new Vector3(finalOffset.x, finalOffset.y, 0), Quaternion.identity);
        rooms.Add(newRoom);

        currentRooms++;
    }
    
    Vector2 getOffset(int dir){
        Vector2 offset = new Vector2(0, 0);
        switch(dir){
            case 0:
                offset = new Vector2(0, roomSize);
                break;
            case 1:
                offset = new Vector2(roomSize, 0);
                break;
            case 2:
                offset = new Vector2(0, -roomSize);
                break;
            case 3:
                offset = new Vector2(-roomSize, 0);
                break;
        }
        return offset;
    }

    void finalDoors(){
        foreach(GameObject room in rooms){
            Room r = room.GetComponent<Room>();
            List<int> doorsToTurnOff = new List<int>();

            foreach(GameObject door in r.doorObjs){
                Door d = door.GetComponent<Door>();

                Vector2 offset = getOffset(door.GetComponent<Door>().dir);
                Vector3 finalPos = (Vector2)room.transform.position + offset;

                GameObject testRoom = rooms.Find(x => x.transform.position == finalPos);

                if(testRoom == null){
                    if(!doorsToTurnOff.Contains(d.dir)){
                        doorsToTurnOff.Add(d.dir);
                    }
                }
            }

            foreach(int door in doorsToTurnOff){
                GameObject d = r.doorObjs[door];
                d.AddComponent<BoxCollider2D>();
                d.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }
}
