using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class GameController : MonoBehaviour
{
    public Generator MapGenerator;
    public Map GameMap;

    public Camera MainCamera;

    public Unit Player;
    public GameObject Warrior;
    public GameObject Archer;

    public List<Unit> CurRoomUnits = new List<Unit>();
    public Room CurRoom;
    public Color HighlightColor;
    public Color MarkColor;
    public Color DefaultColor;
    public Tile HighlightTile = null;
    public List<Tile> MarkTiles = new List<Tile>();
    public Text Health = null;
    public Text Mana = null;
    public Text RoomDepth = null;


    public bool PlayerTurn = true;
    public bool Over = false;
    public List<Unit> Enemies = new List<Unit>();

    private void Start()
    {
        Player.Context = this;
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
        if (!Over)
        {
            if (PlayerTurn)
            {
                Player.TakeTurn();
            }
            else
            {
                foreach (Unit enemy in Enemies)
                {
                    if (enemy) enemy.TakeTurn();
                }
                PlayerTurn = true;
            }
        }
        
        Health.text = Player.GetStat("Health").CurrentValue.ToString() + " / " + Player.GetStat("Health").MaxValue.ToString();
        Mana.text = Player.GetStat("Mana").CurrentValue.ToString() + " / " + Player.GetStat("Mana").MaxValue.ToString();
    }

    public void MoveUnitToTile(Unit p_Unit, Tile p_Tile)
    {
        Tile from = null;
        if (p_Unit)
        {
            if (p_Unit.CurTile != null)
            {
                from = p_Unit.CurTile;
                p_Unit.CurTile.CurUnit = null;
            }

            p_Unit.CurTile = p_Tile;
            p_Tile.CurUnit = p_Unit;

            StartCoroutine(MoveTo(p_Unit.transform, p_Tile.transform.position, 0.25f));
            OnUnitEnterTile(p_Unit, p_Tile, from);
        }
    }

    void OnUnitEnterTile(Unit p_Unit, Tile p_To, Tile p_From)
    {
        if(p_To.IsDoorTile)
        {
            
            Door door = p_To.GetComponent<Door>();
            Room room = door.ConnectedDoor.OnTile.ParentRoom;
            p_Unit.CurTile = door.ConnectedDoor.OnTile;
            door.ConnectedDoor.OnTile.CurUnit = p_Unit;
            MoveToRoom(room, door.ConnectedDoor);
        }else if (p_To.IsLavaTile)
        {
            Enemies.Remove(p_Unit);
            p_Unit.Die();
        }else if (p_To.IsWallTile)
        {
            p_Unit.OnAttack();
        }else if (p_To.IsPowerUpTile)
        {
            if(p_Unit == Player)
            {
                if (!p_To.GetComponent<TilePowerUp>().Used)
                {
                    p_To.GetComponent<TilePowerUp>().ShowOptions();
                    p_To.GetComponent<TilePowerUp>().Used = true;
                }
            }
        }
        
    }

    // Highlight and mark tiles
    public void Highlight(Tile p_Tile)
    {
        if (p_Tile == HighlightTile)
        {
            return;
        }
        if (HighlightTile != null)
        {
            if (CurRoom.isVictory)
            {
                HighlightTile.GetComponentInChildren<SpriteRenderer>().color = MapGenerator.VictoryColor;
            }
            else
            {
                HighlightTile.GetComponentInChildren<SpriteRenderer>().color = DefaultColor;
            }
            

            HighlightTile = null;
        }
        if (p_Tile == null)
        {
            HighlightTile = p_Tile;
            return;
        }
        if (!p_Tile.IsFloorTile) return;

        HighlightTile = p_Tile;
        p_Tile.GetComponentInChildren<SpriteRenderer>().color = HighlightColor;
    }

    public void Mark(Tile p_Tile)
    {
        if (!(p_Tile.IsFloorTile || p_Tile.IsDoorTile || p_Tile.IsVictoryTile || p_Tile.IsPowerUpTile)) return;

        p_Tile.GetComponentInChildren<SpriteRenderer>().color = MarkColor;
        p_Tile.Marked = true;
        MarkTiles.Add(p_Tile);
    }

    public void UnMark()
    {
        Color color = DefaultColor;
        if (CurRoom.isVictory)
        {
            color = MapGenerator.VictoryColor;
        }
        foreach(Tile tile in MarkTiles)
        {
            tile.GetComponentInChildren<SpriteRenderer>().color = DefaultColor;
            tile.Marked = false;
        }

        MarkTiles.Clear();
    }

    void MoveToRoom(Room p_Room, Door p_Entry = null)
    {
        List<Tile> tiles = p_Room.Tiles.FindAll(t => t.IsFloorTile);

        // TODO hide other rooms
        Tile startTile = tiles[0];
        if(p_Entry != null)
        {
            startTile = p_Entry.OnTile;
        }
        else
        {
            tiles.Remove(tiles[0]);
        }
        CurRoom = p_Room;
        Player.transform.position = startTile.transform.position;
        Player.CurTile = startTile;
        startTile.CurUnit = Player;

        FocusCameraOnRoom(p_Room);

        DestroyAllEnemies();

        for(int i = 0; i < p_Room.Depth + 1; i++)
        {
            if (i % 2 == 0) SpawnEnemy(tiles, Warrior);
            else if (i % 2 == 1) SpawnEnemy(tiles, Archer);
        }

        Player.SetStat("Mana", Player.GetStat("Mana").MaxValue);
        RoomDepth.text = CurRoom.Depth.ToString();
    }

    private void SpawnEnemy(List<Tile> p_Tiles, GameObject p_Enemy)
    {
        int index = 0;
        bool result = false;
        int tries = 0;
        while (!result && tries!=100)
        {
            tries++;
            index = UnityEngine.Random.Range(0, p_Tiles.Count);
            Tile temp = p_Tiles[index];

            Tile left = temp.ParentRoom.GetTileAt(temp.RoomPosition - new Vector2Int(1, 0));
            if (left && left.IsDoorTile)
            {
                result = false;
                continue;
            }

            Tile right = temp.ParentRoom.GetTileAt(temp.RoomPosition + new Vector2Int(1, 0));
            if (right && right.IsDoorTile)
            {
                result = false;
                continue;
            }

            Tile up = temp.ParentRoom.GetTileAt(temp.RoomPosition + new Vector2Int(0, 1));
            if (up && up.IsDoorTile)
            {
                result = false;
                continue;
            }

            Tile down = temp.ParentRoom.GetTileAt(temp.RoomPosition - new Vector2Int(0, 1));
            if (down && down.IsDoorTile)
            {
                result = false;
                continue;
            }

            result = true;
        }
        GameObject enemy = Instantiate(p_Enemy);
        enemy.transform.position = p_Tiles[index].transform.position;
        enemy.GetComponent<Unit>().CurTile = p_Tiles[index];
        enemy.GetComponent<Unit>().Context = this;
        p_Tiles[index].CurUnit = enemy.GetComponent<Unit>();
        p_Tiles.RemoveAt(index);
        Enemies.Add(enemy.GetComponent<Unit>());
    }

    public void DestroyAllEnemies()
    {
        while (Enemies.Count != 0)
        {
            Unit temp = Enemies[0];
            Enemies.RemoveAt(0);
            if (temp) temp.Die();
        }
    }

    private void FocusCameraOnRoom(Room p_Room)
    {
        Vector3 min = p_Room.Tiles[0].transform.position;
        Vector3 max = min;

        foreach(Tile tile in p_Room.Tiles)
        {
            if (tile)
            {
                Vector3 lp = tile.transform.position;
                if (lp.x < min.x) min.x = lp.x;
                else if(lp.x > max.x) max.x = lp.x;
                if (lp.y < min.y) min.y = lp.y;
                else if(lp.y > max.y) max.y = lp.y;
            } 
        }

        Vector3 center = (max - min) / 2.0f + min;

        StartCoroutine(MoveTo(MainCamera.transform, new Vector3(center.x, center.y, MainCamera.transform.position.z), 0.3f));
    }

    IEnumerator MoveTo(Transform p_Target, Vector3 p_To, float p_Duration)
    {
        float time = 0;
        Vector3 start = p_Target.position;
        while (time <= p_Duration)
        {
            float t = Mathf.Clamp01(time / p_Duration);
            if(p_Target) p_Target.transform.position = Vector3.Lerp(start, p_To, t);
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }

        transform.position = p_To;
    }
}
