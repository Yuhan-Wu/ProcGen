using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Generator MapGenerator;
    public Map GameMap;

    public Camera MainCamera;

    public Unit Player;

    public List<Unit> CurRoomUnits = new List<Unit>();
    public Room CurRoom;
    public Color HighlightColor;
    public Tile HighlightTile = null;

    private void Start()
    {
        if(MainCamera == null)
        {
            MainCamera = Camera.main;
        }
        GameMap = MapGenerator.Generate();
        Room startRoom = GameMap.Rooms[0];
        MoveToRoom(startRoom);
    }

    private void Update()
    {
        ProcessPlayerInput();
    }

    void ProcessPlayerInput()
    {

    }

    void MoveToRoom(Room p_Room, Door p_Entry = null)
    {
        Tile startTile = p_Entry != null ? p_Entry.OnTile : p_Room.Tiles.Find(t => t.IsFloorTile);
        CurRoom = p_Room;
        Player.transform.position = startTile.transform.position;

        FocusCameraOnRoom(p_Room);
        
        //TODO SPAWN UNITS
    }

    private void FocusCameraOnRoom(Room p_Room)
    {
        Vector3 min = p_Room.Tiles[0].transform.position;
        Vector3 max = min;

        foreach(Tile tile in p_Room.Tiles)
        {
            Vector3 lp = tile.transform.position;
            if (lp.x < min.x) min.x = lp.x;
            else max.x = lp.x;
            if (lp.y < min.y) min.y = lp.y;
            else max.y = lp.y;
        }

        Vector3 center = (max - min) / 2.0f + min;
        
        MainCamera.transform.position = new Vector3(center.x, center.y, MainCamera.transform.position.z);
    }

   
}
