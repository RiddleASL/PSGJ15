using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    GlobalScript gs;
    public SaveData.TreasureData treasureData = new SaveData.TreasureData();

    [SerializeField] float pickupRange = 1;

    void Start()
    {
        gs = GameObject.FindGameObjectWithTag("Global").GetComponent<GlobalScript>();
    }

    void Update() {
        Vector2 target = gs.pm.transform.position;
        if(Vector2.Distance(transform.position, target) < pickupRange)
        {
            Destroy(gameObject);
            Collect();
        }
    }

    void Collect()
    {
        gs.sd.saveData.collectedTreasures.treasures.Add(treasureData);
        gs.sd.applyTreasureData();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
