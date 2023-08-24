using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            SavePlayerData();
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            LoadPlayerData();
        }

    }

    public void SavePlayerData()
    {
        Save(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }

    public void LoadPlayerData()
    {
        Load(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }

    public void Save(Object data,string key)
    {
        var jsonData = JsonUtility.ToJson(data,true);
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.Save();
    }

    public void Load(Object data, string key) 
    {
        if(PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key),data);
        }
    }
}
