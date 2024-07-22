using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    [System.Serializable]
    public class Data
    {
        [Header("Character Data")]
        public int selectedCharacter;
        public List<CharacterData> characters;
        public StatChanges statChanges;
        public CollectedTreasures collectedTreasures;
    }

    [System.Serializable]
    public class CharacterData{
        [Header("Character Data")]
        public string name;
        public int maxHealth;
        public bool isRanged;
        public CharacterAbilities abilities;

        [Header("Movement Data")]
        public float speed;
        public float speedMod = 1;
        public float dashDist;
        public float dashMod = 1;

        [Header("Combat Data")]
        public float dmg;
        public float dmgMod = 1;
        public float attackSpeed;
        public float attackSpeedMod = 1;
        public float knockback;
        public float knockbackMod = 1;
        public float kbResist;
        public float kbResistMod = 1;
        public Vector2 hitboxSize;

        [Header("Buffer Data")]
        public float attackBuffer = .3f;
        public float attackBufferMod = 1;
        public float dashBuffer = .5f;
        public float dashBufferMod = 1;
    }

    [System.Serializable]
    public class CharacterAbilities
    {
        [Header("Primary Ability")]
        public Ability primary;

        [Header("Secondary Ability")]
        public Ability secondary;

        [Header("Passive Ability")]
        public Ability passive;
    }

    [System.Serializable]
    public class StatChanges
    {
        public float speedMod = 1;
        public float dashMod = 1;
        public float kbResistMod = 1;

        public float dmgMod = 1;
        public float attackSpeedMod = 1;
        public float knockbackMod = 1;
        
        public float attackBufferMod = 1;
        public float dashBufferMod = 1;
    }

    [System.Serializable]
    public class CollectedTreasures
    {
        public List<TreasureData> treasures;
    }

    [System.Serializable]
    public class TreasureData
    {
        public string name;
        public string description;
        public string funnyDescription;
        public List<TreasureStats> statChanges;
    }

    [System.Serializable]
    public class TreasureStats
    {
        public string stat;
        public float percentage;
    }

    public Data saveData = new Data();

    public void SaveToJson(int num){
        string data = JsonUtility.ToJson(saveData);
        string path = Application.persistentDataPath + "/saveData" + num + ".json";
        System.IO.File.WriteAllText(path, data);
    }

    public void LoadFromJson(int num){
        string path = Application.persistentDataPath + "/saveData"  + num + ".json";
        string data = System.IO.File.ReadAllText(path);
        saveData = JsonUtility.FromJson<Data>(data);
        LoadCharacterData();
    }

    //* Character Data
    public void LoadCharacterData(){
        if(GameObject.FindGameObjectWithTag("Player") == null) return;
        CharacterData character = saveData.characters[saveData.selectedCharacter];
        PlayerMotor pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
        AbilityHolder ah = GameObject.FindGameObjectWithTag("Player").GetComponent<AbilityHolder>();

        //Base Data
        pm.maxHealth = character.maxHealth;
        pm.isRanged = character.isRanged;

        //Movement Data
        pm.speed = character.speed;
        pm.speedMod = character.speedMod;
        pm.dashDist = character.dashDist;
        pm.dashMod = character.dashMod;

        //Combat Data
        pm.dmg = character.dmg;
        pm.dmgMod = character.dmgMod;
        pm.attackSpeed = character.attackSpeed;
        pm.attackSpeedMod = character.attackSpeedMod;
        pm.knockback = character.knockback;
        pm.knockbackMod = character.knockbackMod;
        pm.kbResist = character.kbResist;
        pm.kbResistMod = character.kbResistMod;
        pm.attackHitbox = character.hitboxSize;

        //Buffer Data
        pm.attackBuffer = character.attackBuffer;
        pm.attackBufferMod = character.attackBufferMod;
        pm.dashBuffer = character.dashBuffer;
        pm.dashBufferMod = character.dashBufferMod;


        //Ability Data
        ah.ResetChar();
        emptyStatChanges();
        ah.primaryAbility = character.abilities.primary;
        ah.secondaryAbility = character.abilities.secondary;
        ah.passiveAbility = character.abilities.passive;
    }

    //* Stat Changes
    public void applyStatChanges(){
        StatChanges changes = saveData.statChanges;
        PlayerMotor pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();

        //Movement Data
        pm.speedMod = changes.speedMod;
        pm.dashMod = changes.dashMod;
        pm.kbResistMod = changes.kbResistMod;

        //Combat Data
        pm.dmgMod = changes.dmgMod;
        pm.attackSpeedMod = changes.attackSpeedMod;
        pm.knockbackMod = changes.knockbackMod;

        //Buffer Data
        pm.attackBufferMod = changes.attackBufferMod;
        pm.dashBufferMod = changes.dashBufferMod;
    }

    public void emptyStatChanges(){
        saveData.statChanges.speedMod = 1;
        saveData.statChanges.dashMod = 1;
        saveData.statChanges.kbResistMod = 1;

        saveData.statChanges.dmgMod = 1;
        saveData.statChanges.attackSpeedMod = 1;
        saveData.statChanges.knockbackMod = 1;

        saveData.statChanges.attackBufferMod = 1;
        saveData.statChanges.dashBufferMod = 1;
    }

    public void changeStat(string stat, float value){
        switch(stat){
            case "speed":
                saveData.statChanges.speedMod += (saveData.statChanges.speedMod * (value/100));
                break;
            case "dash":
                saveData.statChanges.dashMod += (saveData.statChanges.speedMod * (value/100));
                break;
            case "kbResist":
                saveData.statChanges.kbResistMod += (saveData.statChanges.speedMod * (value/100));
                break;
            case "dmg":
                saveData.statChanges.dmgMod += (saveData.statChanges.speedMod * (value/100));
                break;
            case "attackSpeed":
                saveData.statChanges.attackSpeedMod += (saveData.statChanges.speedMod * (value/100));
                break;
            case "knockback":
                saveData.statChanges.knockbackMod += (saveData.statChanges.speedMod * (value/100));
                break;
            case "attackBuffer":
                saveData.statChanges.attackBufferMod += (saveData.statChanges.speedMod * (value/100));
                break;
            case "dashBuffer":
                saveData.statChanges.dashBufferMod += (saveData.statChanges.speedMod * (value/100));
                break;
        }
    }

    //* Treasure Data
    public void applyTreasureData(){
        emptyStatChanges();

        foreach(TreasureData treasure in saveData.collectedTreasures.treasures){
            foreach(TreasureStats stat in treasure.statChanges){
                changeStat(stat.stat, stat.percentage);
            }
        }
        applyStatChanges();
    }

    public void addTreasure(TreasureData treasure){
        saveData.collectedTreasures.treasures.Add(treasure);
    }
}
