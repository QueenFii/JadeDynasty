﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    [Header("Configurable Generation Variables")]
    public float timerOG;  //CheckLevel when timer done
    public int minRooms;
    public int startingMinRooms;
    public float roomWidth, roomHeight; // every 3 is about 1 second of travel between rooms (current default(15, 9)) (width/height determined by half of the total room size in tiles)

    [Header("The Rest")]
    public int roomsCreated = 0;            //how many rooms have been made
    public GameObject[] rooms;              //storage of different room prefabs
    public GameObject[] bigRooms;           //storage of big room prefabs
    public GameObject[] createdRooms;       //to keep track of the rooms that exist in the level
    public GameObject corridor;             //storage of the corridor prefab
    public GameObject[] bossRooms;          //storage of the boss room prefabs
    public GameObject treasureRoom;         //storage of the treasure room prefab
    private bool corridorsSpawned = false;  //have the corridors been instantiated yet?
    public GameObject[] doors;// Array;//to keep track of all the doors in the scene
    //public List<GameObject> doors;
    public int floor = 0;                   //what floor the player is on

    //checklist for generation
    public bool genComplete = false;        //main generation of floor
    public bool bossRoomAssigned = false;   //assign boss room
    public bool tRoomAssigned = false;      //assign treasure room

    public List<DoorControl> doorControlList; // stupid shit for boss room checking
    public float timer;  
    
    //Representation of the level as a grid. Each bool value corresponds to whether there is a room in that position on the grid
    public bool[,] roomGrid = new bool[10, 10] { 
        { false, false, false, false, false, false, false, false, false, false },
        { false, false, false, false, false, false, false, false, false, false },
        { false, false, false, false, false, false, false, false, false, false },
        { false, false, false, false, false, false, false, false, false, false },
        { false, false, false, false, false, false, false, false, false, false },
        { false, false, false, false, false, false, false, false, false, false },
        { false, false, false, false, false, false, false, false, false, false },
        { false, false, false, false, false, false, false, false, false, false },
        { false, false, false, false, false, false, false, false, false, false },
        { false, false, false, false, false, false, false, false, false, false }};

    //Lookup table of the actual in-engine transform positions for each position on the grid above
    public Vector3[,] roomPositions = new Vector3[10, 10];
    public List<Vector3> roomPositionsToAssign;
    public GameObject dummyObject;

    void Start()
    {
        timer = timerOG;
        //int xForDebug = 0;
        //int yForDebug = 0;
        for (float y = roomHeight*4*2; y >= -roomHeight*5*2; y -= roomHeight*2) //(start y(baseNum * 4) * 2, y >= end y(-baseNum * 5) * 2, base height num * 2)
        {
            for (float x = -roomWidth*4*2; x <= roomWidth*5*2; x += roomWidth*2) //(start x(-baseNum*4) * 2, y >= end x(baseNum*5) * 2, base width num * 2)
            {
                roomPositionsToAssign.Add(new Vector3(x, y, 0));
               // print("buttwrinkle");
                //GameObject g = Instantiate(dummyObject, new Vector3(x, y, 0), Quaternion.identity);
                //g.GetComponent<debugBoolChecker>().posX = xForDebug;
                //g.GetComponent<debugBoolChecker>().posY = yForDebug;
                //xForDebug++;
            }
            //xForDebug = 0;
            //yForDebug++;
        }
        int iRoomPos = 0;
        for (int y2 = 0; y2 < 10; y2++)
        {
            for (int x2 = 0; x2 < 10; x2++)
            {
                roomPositions[x2, y2] = roomPositionsToAssign[iRoomPos];
                iRoomPos++;
            }
        }

        //create starting room
        GameObject g = Instantiate(rooms[0], roomPositions[4, 4], Quaternion.identity);
        GameManager.GM.currentRoom = g.GetComponent<RoomGenerator>();
        roomGrid[4, 4] = true;
        roomsCreated = 1;
        startingMinRooms = minRooms;
        //for (int i = 0; i < doorsArray.Length; i++)
        //{
        //    doors.Add(doorsArray[i]);
        //}
    }

    void Update()
    {
        //create corridors, once there are at least 7 rooms
        //if(roomsCreated >= 7 && !corridorsSpawned)
        //{
        //    //iterates through the roomGrid and checks for neighbors. If it finds them, instantiate a corridor in the appropriate position
        //    for(int x = 0; x < roomGrid.GetLength(0); x++)
        //    {
        //        for(int y = 0; y < roomGrid.GetLength(1); y++)
        //        {
        //            if(roomGrid[x, y])
        //            {
        //                if(roomGrid[x + 1, y])
        //                {
        //                    Instantiate(corridor, new Vector3(roomPositions[x, y].x, 2.02f, roomPositions[x, y].z - 25), Quaternion.AngleAxis(90, Vector3.up));
        //                }

        //                if (roomGrid[x, y + 1])
        //                {
        //                    Instantiate(corridor, new Vector3(roomPositions[x, y].x + 25, 2.02f, roomPositions[x, y].z), Quaternion.identity);
        //                }
        //            }
        //        }
        //    }

        //    corridorsSpawned = true;
        //}

        //populates these arrays every frame, since the floor generation happens across multiple frames
        doors = GameObject.FindGameObjectsWithTag("door");
        createdRooms = GameObject.FindGameObjectsWithTag("room");
        minRooms = startingMinRooms + (2 * floor);

        //determines whether doors should be "real" or not
        for(int i = 0; i < doors.Length; i++)
        {
            //checks the grid for neighbors to each door. If it finds one, sets the door to be active ("real")
            if(doors[i].name == "doorSouth")// || doors[i].name == "doorSouthWestDown"  || doors[i].name == "doorSouthEastDown")
            {
                try
                {
                    if (roomGrid[doors[i].GetComponent<DoorControl>().posX, doors[i].GetComponent<DoorControl>().posY + 1])
                    {
                        doors[i].GetComponent<DoorControl>().active = true;
                    }
                }
                catch
                {
                    //in case the door borders the edge of the grid
                    print("exception caught");
                    doors[i].GetComponent<DoorControl>().active = false;
                }
            }

            if (doors[i].name == "doorWest")// || doors[i].name == "doorSouthWestLeft" || doors[i].name == "doorNorthWestLeft")
            {
                try
                {
                    if (roomGrid[doors[i].GetComponent<DoorControl>().posX - 1, doors[i].GetComponent<DoorControl>().posY])
                    {
                        doors[i].GetComponent<DoorControl>().active = true;
                    }
                }
                catch
                {
                    //in case the door borders the edge of the grid
                    print("exception caught");
                    doors[i].GetComponent<DoorControl>().active = false;
                }
            }

            if (doors[i].name == "doorNorth")// || doors[i].name == "doorNorthWestUp" || doors[i].name == "doorNorthEastUp")
            {
                try
                {
                    if (roomGrid[doors[i].GetComponent<DoorControl>().posX, doors[i].GetComponent<DoorControl>().posY - 1])
                    {
                        doors[i].GetComponent<DoorControl>().active = true;
                    }
                }
                catch
                {
                    //in case the door borders the edge of the grid
                    print("exception caught");
                    doors[i].GetComponent<DoorControl>().active = false;
                }
            }

            if (doors[i].name == "doorEast")// || doors[i].name == "doorNorthEastRight" || doors[i].name == "doorSouthEastRight")
            {
                try
                {
                    if (roomGrid[doors[i].GetComponent<DoorControl>().posX + 1, doors[i].GetComponent<DoorControl>().posY])
                    {
                        doors[i].GetComponent<DoorControl>().active = true;
                    }
                }
                catch
                {
                    //in case the door borders the edge of the grid
                    print("exception caught");
                    doors[i].GetComponent<DoorControl>().active = false;
                }
            }
        }

        //tells each room what its index is in the main array that tracks each room
        for(int i = 0; i < createdRooms.Length; i++)
        {
            createdRooms[i].GetComponent<RoomGenerator>().roomIndex = i;
        }

        //checklist update
        if(roomsCreated >= minRooms && !genComplete)
        {
            genComplete = true;
            
        }

        if(genComplete && !bossRoomAssigned)
        {
            AssignBossRoom(floor);
        }

        if(genComplete && bossRoomAssigned && !tRoomAssigned)
        {
            AssignTreasureRoom();
        }

        if (genComplete && bossRoomAssigned)
        {
            doorChecker();
            //nullRemover(doors);
            foreach(GameObject g in createdRooms){
                nullRemover(g.GetComponent<RoomGenerator>().doors);
            }

            GameManager.GM.fadeObject.GetComponent<Animator>().speed = 1;
            //GameManager.GM.CheckMinimapObjects("GenManager");

            //GameManager.GM.minimapCheckingNumerator = GameManager.GM.StartCoroutine(GameManager.GM.CheckMinimapObjects("genman"));
        }

    }
    public void nullRemover(List<GameObject> l)
    {
        for(int i = 0; i < l.Count; i++){
            if(l[i] == null){
                l.RemoveAt(i);
            }
        }
    }
    //     public void nullRemover(GameObject[] a)
    // {
    //     for(int i = 0; i < a.Length; i++){
    //         if(a[i] == null){
    //             a.;
    //         }
    //     }
    // }
    private void LateUpdate()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            StopCoroutine(CheckLevel());
            StartCoroutine(CheckLevel());
        }
        
    
    }

    public IEnumerator CheckLevel()
    {
        yield return new WaitForSeconds(0.25f);
        if (!(roomsCreated >= minRooms))
        {
            RetryLevel();
            yield return null;
            StopCoroutine(CheckLevel());
        }
        
    }

    public void doorChecker()
    {
        for (int i = 0; i < doors.Length; i++)
        {
            if (!doors[i].GetComponent<DoorControl>().active)
            {
                doors[i].GetComponent<SpriteRenderer>().enabled = false;
                doors[i].GetComponent<Collider2D>().enabled = false;
                doors[i].GetComponent<Animator>().enabled = false;

                //Destroy(doors[i]);
            }
            if (doors[i].GetComponent<DoorControl>().active)
            {
                doors[i].GetComponent<DoorControl>().ConnectToDoor();
            }
        }
        //GameManager.GM.CheckMinimapObjects("GenManager");
    }

    public void AssignBossRoom(int floor) //picks the room furthest from the player's starting room and replaces it with the appropriate boss room (based on floor)
    {
        int roomToReplaceIndex = 0; //which room gets replaced (identified by its index in the createdRooms array)
        float roomDist = 0f;        //stores the highest distance that has been found, to compare other rooms against

        RoomGenerator rG;

        for(int i = 0; i < createdRooms.Length; i++)
        {
            rG = createdRooms[i].GetComponent<RoomGenerator>();
            if(rG.type == RoomGenerator.roomType.RegularX2)
            {
                continue;
            }
            //iterate through all the rooms, every time a room is found that is further from the start than the previous furthest update which room to replace
            if(Vector3.Distance(createdRooms[i].transform.position, createdRooms[0].transform.position) > roomDist)
            {
                roomDist = Vector3.Distance(createdRooms[i].transform.position, createdRooms[0].transform.position);
                roomToReplaceIndex = i;
            }
        }
        //int numOfDoors = 0;

        // for (int i = 0; i < createdRooms[roomToReplaceIndex].GetComponent<RoomGenerator>().doors.Count; i++)
        // {
        //    doorControlList.Add(createdRooms[roomToReplaceIndex].GetComponent<RoomGenerator>().doors[i].GetComponent<DoorControl>());
        // }
        //for (int i = 0; i < doorControlList.Count; i++)
        //{
        //    if (doorControlList[i].active)
        //    {
        //        numOfDoors++;
        //    }
        //}
        //if (numOfDoors >= 2)
        //{
        //    AssignBossRoom(floor);
        //}

        //actually replace the room
        GameObject g = Instantiate(bossRooms[0], createdRooms[roomToReplaceIndex].transform.position, Quaternion.identity); //HI, JOHN HERE. LOOK BACK AT THIS. BOSS ROOMS USED TO BE INSTANIATED BASED ON FLOOR BUT NOW EVERY FLOOR HAS SAME BOSSROOM
        Destroy(createdRooms[roomToReplaceIndex]);

        g.GetComponent<RoomGenerator>().roomIndexKilled = roomToReplaceIndex;

        //update checklist
        bossRoomAssigned = true;
    }

    public void AssignTreasureRoom() //picks a random room that isn't the starting room and isn't the boss room and replaces it with the treasure room
    {
        //because the boss room was just assigned, we know it is the last room in the array, and the starting room is always 0, so we just exclude the ends of the array
        int roomToReplaceIndex = Random.Range(1, createdRooms.Length - 1);
        RoomGenerator rG;
        rG = createdRooms[roomToReplaceIndex].GetComponent<RoomGenerator>();
        if (rG.type == RoomGenerator.roomType.RegularX2)
        {
            AssignTreasureRoom();
            return;
        }
        //replace room
        Instantiate(treasureRoom, createdRooms[roomToReplaceIndex].transform.position, Quaternion.identity);
        Destroy(createdRooms[roomToReplaceIndex]);

        //update checklist
        tRoomAssigned = true;
    }

    public void RetryLevel() //retry the level generation, mostly for debug
    {
        
        //find all the stuff that was generated
        GameObject[] roomsToDestroy = GameObject.FindGameObjectsWithTag("room");
      //  GameObject[] corridorsToDestroy = GameObject.FindGameObjectsWithTag("corridor");

        //reset checklist
        genComplete = false;
        bossRoomAssigned = false;
        tRoomAssigned = false;
        timer = timerOG;
        GameManager.GM.fadeObject.SetActive(false);
        GameManager.GM.fadeObject.SetActive(true);
        GameManager.GM.fadeObject.GetComponent<Animator>().speed = 0;

        if(GameManager.GM.minimapCheckingNumerator != null)
        GameManager.GM.StopCoroutine(GameManager.GM.minimapCheckingNumerator);
        //destroy all the stuff that was generated
        for (int i = 0; i < roomsToDestroy.Length; i++)
        {
            Destroy(roomsToDestroy[i]);
        }

        //for (int i = 0; i < corridorsToDestroy.Length; i++)
        //{
        //    Destroy(corridorsToDestroy[i]);
        //}

        //reset the grid
        for (int x = 0; x < roomGrid.GetLength(0); x++)
        {
            for(int y = 0; y < roomGrid.GetLength(1); y++)
            {
                roomGrid[x, y] = false;
            }
        }

        //reset the last variables
        roomsCreated = 0;
        corridorsSpawned = false;

        //create new seed room
        GameObject g = Instantiate(rooms[0], roomPositions[4, 4], Quaternion.identity);
        GameManager.GM.currentRoom = g.GetComponent<RoomGenerator>();
        roomGrid[4, 4] = true;
        roomsCreated = 1;

        //put the player back at the beginning
        GameObject.FindWithTag("Player").transform.position = new Vector3(0, 0, 0);
    }
}
