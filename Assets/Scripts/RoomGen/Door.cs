using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOccupied = false;
    public int dir = 0;
    
    public Vector2 GetSpawnDir(){
        Vector2 spawnDir = Vector2.zero;
        switch(dir){
            case 0:
                spawnDir = new Vector2(0, 1);
                break;
            case 1:
                spawnDir = new Vector2(1, 0);
                break;
            case 2:
                spawnDir = new Vector2(0, -1);
                break;
            case 3:
                spawnDir = new Vector2(-1, 0);
                break;
        }

        return spawnDir;
    }
}
