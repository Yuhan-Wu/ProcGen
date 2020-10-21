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
    public Color DefaultColor;
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
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);
        float d;
        if(plane.Raycast(ray, out d))
        {
            Vector3 worldPos = ray.GetPoint(d);
            Vector3 localPos = worldPos - CurRoom.transform.position;
            Vector2Int roomPos = new Vector2Int((int)localPos.x, (int)localPos.y);
            Tile tile = CurRoom.GetTileAt(roomPos);
            Highlight(tile);

            if (Input.GetMouseButtonDown(0) && CanMoveTo(Player, tile))
                MoveUnitToTile(Player, tile);
            
        }
    }

    bool CanMoveTo(Unit p_Unit, Tile p_Tile)
    {
        if (p_Tile == null || !p_Tile.IsFloorTile || p_Tile.CurUnit != null)
        {
            return false;
        }

        if (p_Tile.IsAdjacentTo(p_Unit.CurTile) && !p_Tile.IsLavaTile) return true;
        return false;
    }

    void MoveUnitToTile(Unit p_Unit, Tile p_Tile)
    {
        if (p_Unit.CurTile != null)
        {
            p_Unit.CurTile.CurUnit = null;
        }

        p_Unit.CurTile = p_Tile;
        p_Tile.CurUnit = p_Unit;

        p_Unit.transform.position = p_Tile.transform.position;
    }

    void Highlight(Tile p_Tile)
    {
        if (p_Tile == HighlightTile)
        {
            return;
        }
        if (HighlightTile != null)
        {
            HighlightTile.GetComponentInChildren<SpriteRenderer>().color = DefaultColor;
            HighlightTile = null;
        }
        if (p_Tile && !p_Tile.IsFloorTile) return;

        HighlightTile = p_Tile;
        if (p_Tile != null)
        {
            p_Tile.GetComponentInChildren<SpriteRenderer>().color = HighlightColor;
        }
    }

    void MoveToRoom(Room p_Room, Door p_Entry = null)
    {
        // TODO
        Tile startTile = p_Entry != null ? p_Entry.OnTile : p_Room.Tiles.Find(t => t.IsFloorTile);
        CurRoom = p_Room;
        Player.transform.position = startTile.transform.position;
        Player.CurTile = startTile;
        startTile.CurUnit = Player;

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
