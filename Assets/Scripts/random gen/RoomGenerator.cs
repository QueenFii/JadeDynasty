﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public enum roomType
    {
        Regular,
        Boss,
        Treasure,
        
    }
    public roomType type;

    public int roomIndexKilled; //used for boss/treasue/special rooms

    public int posX;          //the x position of this room in the roomGrid array
    public int posY;          //the y position of this room in the roomGrid array
    public int roomIndex = 0; //this room's index in the array of rooms in the scene, used as an identifier
    public bool done = false; //is this room done creating rooms?

    [SerializeField] private GameObject corridor;             //storage of the corridor prefab
    public GameObject[] rooms;                                //storage of the different room prefabs
    public List<GameObject> doors = new List<GameObject>();   //the doors in this room
    //public List<GameObject> enemies = new List<GameObject>(); //the enemies in this room
    private GenerationManager genManage;                      //reference to the manager
    public GameObject roomRef;                               //debug: for remembering what room begat what other room
    public bool completed = false;

    public bool start;

    public List<GameObject> roomsMade;

    public int doorsConnected;

    void Start()
    {
        start = true;
        //assign variables
        genManage = GameObject.FindWithTag("GameController").GetComponent<GenerationManager>();
        rooms = genManage.rooms;

        doors = FindChildrenWithTag(gameObject.transform, "door");

        posX = GetRoomPosition()[0];
        posY = GetRoomPosition()[1];
    }

    void Update()
    {
        if (start && type == roomType.Regular)
        {
            name = name + " " + roomIndex;
            start = false;
        }
        else if (start && type == roomType.Boss)
        {
            name = name + " " + roomIndexKilled;
            start = false;
        }


        if (type == roomType.Boss)
        {
            doorsConnected = 0;
            for (int i = 0; i < doors.Count; i++)
            {
                if (doors[i].GetComponent<DoorControl>().active)
                {
                    doorsConnected++;
                }
            }
            if (doorsConnected >= 2)
            {
                print("hi" + doorsConnected);
                genManage.RetryLevel();
            }
        }
        
        //main room creation code. It tries to make more rooms every frame until it is finished, or there are enough rooms
        try
        {
            if (genManage.roomsCreated < genManage.minRooms && !done && type != roomType.Boss)
            {
                int roomsToCreate = Mathf.FloorToInt(Random.Range(1, 4)); //how many rooms am I going to make
                genRooms(roomsToCreate);
                
            }
        }
        catch
        {
            //if the generation tries to go out of the grid, give up trying to make it
            done = true;
        }
        //manually reset level
        if (Input.GetKeyDown(KeyCode.T))
        {
            genManage.RetryLevel();
        }

            completed = true;

            for(int i = 0; i < doors.Count; i++)
            {
                doors[i].GetComponent<DoorControl>().locked = false;
            }
    }
    public void genRooms(int numOfRooms)
    {
        if (numOfRooms == 0)
        {
            return;
        }

        int room1;

            room1 = Mathf.FloorToInt(Random.Range(0, 4)); //which direction am I going to make this room in?

            if (doors[room1].gameObject.name == "door1") //interprets the value from 0-3 as a door object in the room, since the doors are always in the same position relative to each other
            {
                if (!genManage.roomGrid[posX, posY + 1] && !genManage.roomGrid[posX - 1, posY + 1] && !genManage.roomGrid[posX + 1, posY + 1]) //is there already a room there or adjacent?
                {
                    roomRef = Instantiate(rooms[Mathf.FloorToInt(Random.Range(1, rooms.Length))], genManage.roomPositions[posX, posY + 1], Quaternion.identity); //instantiate the room
                    doors[room1].GetComponent<DoorControl>().active = true; //obsolete I think
                    genManage.roomGrid[posX, posY + 1] = true; //update grid
                    done = true; //done with generation
                    genManage.roomsCreated++; //increment how many rooms have been made

                //print(gameObject.name + ": " + roomRef.name);
                roomsMade.Add(roomRef);
            }
            }

            if (doors[room1].gameObject.name == "door2")
            {
                if (!genManage.roomGrid[posX - 1, posY] && !genManage.roomGrid[posX - 1, posY + 1] && !genManage.roomGrid[posX - 1, posY - 1])
                {
                    roomRef = Instantiate(rooms[Mathf.FloorToInt(Random.Range(1, rooms.Length))], genManage.roomPositions[posX - 1, posY], Quaternion.identity);
                    doors[room1].GetComponent<DoorControl>().active = true;
                    genManage.roomGrid[posX - 1, posY] = true;
                    done = true;
                    genManage.roomsCreated++;

                //print(gameObject.name + ": " + roomRef.name);
                roomsMade.Add(roomRef);
            }
            }

            if (doors[room1].gameObject.name == "door1 (1)")
            {
                if (!genManage.roomGrid[posX, posY - 1] && !genManage.roomGrid[posX - 1, posY - 1] && !genManage.roomGrid[posX + 1, posY + 1])
                {
                    roomRef = Instantiate(rooms[Mathf.FloorToInt(Random.Range(1, rooms.Length))], genManage.roomPositions[posX, posY - 1], Quaternion.identity);
                    doors[room1].GetComponent<DoorControl>().active = true;
                    genManage.roomGrid[posX, posY - 1] = true;
                    done = true;
                    genManage.roomsCreated++;

                //print(gameObject.name + ": " + roomRef.name);
                roomsMade.Add(roomRef);
            }
            }

            if (doors[room1].gameObject.name == "door2 (1)")
            {
                if (!genManage.roomGrid[posX + 1, posY] && !genManage.roomGrid[posX + 1, posY + 1] && !genManage.roomGrid[posX + 1, posY - 1])
                {
                    roomRef = Instantiate(rooms[Mathf.FloorToInt(Random.Range(1, rooms.Length))], genManage.roomPositions[posX + 1, posY], Quaternion.identity);
                    doors[room1].GetComponent<DoorControl>().active = true;
                    genManage.roomGrid[posX + 1, posY] = true;
                    done = true;
                    genManage.roomsCreated++;

                //print(gameObject.name + ": " + roomRef.name);
                roomsMade.Add(roomRef);
            }
            }
        genRooms(numOfRooms - 1);
    }
    private int[] GetRoomPosition() //finds room's position on the grid based on its position in the scene and returns it as an array of 2 values; [0] is x and [1] is y
    {
        int[] pos = new int[2]; //array to return

        //iterates through the lookup table to find the position that matches the room's position and then, when it's found, breaks out of the loop and returns it
        for (int x = 0; x < genManage.roomPositions.GetLength(0); x++)
        {
            for (int y = 0; y < genManage.roomPositions.GetLength(1); y++)
            {
                if (genManage.roomPositions[x, y] == transform.position)
                {
                    pos[0] = x;
                    pos[1] = y;
                    goto Done;
                }
            }
        }

    Done:
        return pos;
    }

    public List<GameObject> FindChildrenWithTag(Transform parent, string tag) //simple function to find all children of a game object "parent" with tag "tag" (why is this not built in???)
    {
        List<GameObject> children = new List<GameObject>();

        for(int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).gameObject.CompareTag(tag))
            {
                children.Add(parent.GetChild(i).gameObject);
            }
        }

        return children;
    }
}
