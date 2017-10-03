﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour {

    public static SaveManager data;

    string filePath;
    string fileOne;
    string fileTwo;
    string fileThree;

    #region Data to Save/Load
    int scene;

    public int Scene
    {
        get
        {
            return scene;
        }

        set
        {
            scene = value;
        }
    }

    int bullets;

    public int Bullets
    {
        get
        {
            return bullets;
        }

        set
        {
            bullets = value;
        }
    }

    float xPosition;

    public float XPosition
    {
        get
        {
            return xPosition;
        }

        set
        {
            xPosition = value;
        }
    }

    float yPosition;

    public float YPosition
    {
        get
        {
            return yPosition;
        }

        set
        {
            yPosition = value;
        }
    }

    float zPosition;

    public float ZPosition
    {
        get
        {
            return zPosition;
        }

        set
        {
            zPosition = value;
        }
    }

    float zRotation;

    public float ZRotation
    {
        get
        {
            return zRotation;
        }

        set
        {
            zRotation = value;
        }
    }

    List<GameObject> items;

    public List<GameObject> Items
    {
        get
        {
            return items;
        }

        set
        {
            items = value;
        }
    }

    bool isPowerOn;

    public bool IsPowerOn
    {
        get
        {
            return isPowerOn;
        }

        set
        {
            isPowerOn = value;
        }
    }
    #endregion

    void Awake()
    {
        if (data == null)
        {
            DontDestroyOnLoad(gameObject);
            data = this;
        }
        else if (data != this)
        {
            Destroy(gameObject);
        }

        filePath = Application.persistentDataPath;
        fileOne = filePath + "/GameDataOne.zomb";
        fileTwo = filePath + "/GameDataTwo.zomb";
        fileThree = filePath + "/GameDataThree.zomb";

        LoadData();
    }

    public void SaveData(/*int fileNumber*/)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(fileOne);
        //switch (fileNumber)
        //{
        //    case 1:
        //        file = File.Create(fileOne);
        //        break;
        //    case 2:
        //        file = File.Create(fileTwo);
        //        break;
        //    case 3:
        //        file = File.Create(fileThree);
        //        break;
        //    default:
        //        Debug.LogError("File number outside of 1-3");
        //        return;
        //}

        Data data = new Data(scene, bullets, xPosition, yPosition, zPosition, zRotation, items, isPowerOn);

        bf.Serialize(file, data);
        file.Close();
    }

    // Deserializes the save file from binary and sets the variables based on the data on file.
    // Returns true if the file exists so that anyone accessing the function knows if the file exists or not.
    public bool LoadData(/*int fileNumber*/)
    {
        //String filePath;
        //switch (fileNumber)
        //{
        //    case 1:
        //        filePath = fileOne;
        //        break;
        //    case 2:
        //        filePath = fileTwo;
        //        break;
        //    case 3:
        //        filePath = fileThree;
        //        break;
        //    default:
        //        Debug.LogError("File number outside of 1-3");
        //        return false;
        //}
        if (File.Exists(fileOne))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(fileOne, FileMode.Open);
            Data data = (Data)bf.Deserialize(file);

            scene = data.currentScene;
            bullets = data.bulletsLeft;
            xPosition = data.xPos;
            yPosition = data.yPos;
            zPosition = data.zPos;
            zRotation = data.zRot;
            items = data.currentItems;
            isPowerOn = data.isPowerOn;

            file.Close();
            return true;
        }

        return false;
    }

    [Serializable]
    struct Data
    {
        public int currentScene;
        public int bulletsLeft;
        public float xPos;
        public float yPos;
        public float zPos;
        public float zRot;
        public List<GameObject> currentItems;
        public bool isPowerOn;

        public Data(int scene, int bullets, float xPosition, float yPosition, float zPosition, float zRotation, List<GameObject> items, bool _isPowerOn)
        {
            currentScene = scene;
            bulletsLeft = bullets;
            xPos = xPosition;
            yPos = yPosition;
            zPos = zPosition;
            zRot = zRotation;
            currentItems = items;
            isPowerOn = _isPowerOn;
        }
    }
}