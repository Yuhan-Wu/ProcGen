using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public int MaxDepth = 0;
    public int Seed = 0;
    public int MaxTries = 100;
    public Color VictoryColor;

    public List<TextAsset> RoomTexts = new List<TextAsset>();
    public List<TileType> TileTypes = new List<TileType>();

    public Map GameMap;

    private float[] Rotation = { 180, -90, 0, 90 };
    private Direction[] Directions = { Direction.N, Direction.E, Direction.S, Direction.W };

    [System.Serializable]
    public struct TileType
    {
        public char Name;
        public Tile TileObject;
    }

    public Map Generate()
    {
        if (Seed != 0)
        {
            UnityEngine.Random.InitState(Seed);
        }
        
        GameObject tileObj = new GameObject("TileBoard");
        GameMap = tileObj.AddComponent<Map>();

        string strRoom = RoomTexts[0].text;
        Room startRoom = GenerateRoom(strRoom, Vector2Int.zero);
        startRoom.Depth = 1;

        GenerateConnectedRoom(startRoom, 2);
        RemoveUnconnectedDoors();

        return GameMap;
    }

    private Room GenerateRoom(string p_RoomText, Vector2Int p_MapPosition)
    {
        GameObject roomObj = new GameObject($"Room {GameMap.Rooms.Count}");
        roomObj.transform.SetParent(GameMap.transform);
        roomObj.transform.position = new Vector3(p_MapPosition.x, p_MapPosition.y, 0);
        Room room = roomObj.AddComponent<Room>();
        GameMap.Rooms.Add(room);
        room.StringRoom = p_RoomText;
        room.MapPosition = p_MapPosition;

        string[] lines = p_RoomText.Split('\n');
        for(int i = 0; i < lines.Length; i++)
        {
            for(int j = 0; j < lines[i].Length; j++)
            {
                Direction curTileDirection = Direction.NONE;
                if (lines[i][j] == '0' || lines[i][j] == '1')
                {
                    if (i == 0 || lines[i - 1].Length <= j || lines[i - 1][j] == ' ' || lines[i - 1][j] == '\n')
                    {
                        curTileDirection = Direction.N;
                    }
                    else if (i == lines.Length - 1 || lines[i + 1].Length <= j || lines[i + 1][j] == ' ' || lines[i + 1][j] == '\n')
                    {
                        curTileDirection = Direction.S;
                    }
                    else if (j == 0 || lines[i][j - 1] == ' ' || lines[i][j - 1] == '\n')
                    {
                        curTileDirection = Direction.W;
                    }
                    else if (lines[i][j + 1] == '\n' || lines[i][j + 1] == ' ')
                    {
                        curTileDirection = Direction.E;
                    }
                }
                SpawnTile(lines[i][j], room, new Vector2Int(j, lines.Length - 1 - i), curTileDirection);
                // Debug.Log(lines[i][j]);
            }
        }
        
        return room;
    }


    private Room GenerateRoom(string p_Original, char[,] p_RoomText, Vector2Int p_MapPosition)
    {
        GameObject roomObj = new GameObject($"Room {GameMap.Rooms.Count}");
        roomObj.transform.SetParent(GameMap.transform);
        roomObj.transform.position = new Vector3(p_MapPosition.x, p_MapPosition.y, 0);
        Room room = roomObj.AddComponent<Room>();
        GameMap.Rooms.Add(room);
        room.StringRoom = p_Original;
        room.MapPosition = p_MapPosition;

        int row = p_RoomText.GetLength(0);
        int column = p_RoomText.GetLength(1);
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                Direction curTileDirection = Direction.NONE;
                if (p_RoomText[i, j] == '0' || p_RoomText[i, j] == '1')
                {
                    if (i == 0 || p_RoomText[i - 1, j] == ' ' || p_RoomText[i - 1, j] == '\n')
                    {
                        curTileDirection = Direction.N;
                    }
                    else if (i == row - 1 || p_RoomText[i + 1, j] == ' ' || p_RoomText[i + 1, j] == '\n')
                    {
                        curTileDirection = Direction.S;
                    }
                    else if (j == 0 || p_RoomText[i, j - 1] == ' ' || p_RoomText[i, j - 1] == '\n')
                    {
                        curTileDirection = Direction.W;
                    }
                    else if (j == column - 1 || p_RoomText[i, j + 1] == '\n' || p_RoomText[i, j + 1] == ' ')
                    {
                        curTileDirection = Direction.E;
                    }
                }
                SpawnTile(p_RoomText[i, j], room, new Vector2Int(j, row - 1 - i), curTileDirection);
                // Debug.Log(lines[i][j]);
            }
        }

        return room;
    }

    private void SpawnTile(char p_TileType, Room p_Room, Vector2Int p_RoomPosition, Direction p_Direction = Direction.NONE)
    {
        TileType curTile = TileTypes.Find(x => x.Name == p_TileType);
        Tile curObject = null;
        if (!curTile.TileObject)
        {
            // Generate default tile
            // Which is water tile
            return;
        }
        else
        {
            curObject = Instantiate(curTile.TileObject, p_Room.transform);
        }
        curObject.transform.localPosition = new Vector3(p_RoomPosition.x, p_RoomPosition.y, 0);
        curObject.Type = p_TileType;
        curObject.ParentRoom = p_Room;
        curObject.RoomPosition = p_RoomPosition;
        p_Room.Tiles.Add(curObject);

        Door door = curObject.GetComponent<Door>();
        if (door != null)
        {
            p_Room.Doors.Add(door);
            door.OnTile = curObject;
            door.DoorDirection = p_Direction;
        }
    }

    private bool Validate(char[,] p_Tiles, Vector2Int p_RoomOrigin, out char[,] o_Ret)
    {
        int row = p_Tiles.GetLength(0);
        int column = p_Tiles.GetLength(1);
        char[,] result = new char[row, column];
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                result[i, j] = p_Tiles[i, j];
            }
        }
        for(int i = 0; i < row; i++)
        {
            for(int j = 0; j < column; j++)
            {
                if(p_Tiles[i, j]!=' ' && p_Tiles[i, j]!='\n')
                {
                    Vector2Int pos = p_RoomOrigin + new Vector2Int(j, row - 1 - i);
                    foreach (Room room in GameMap.Rooms)
                    {
                        if(result[i, j]!=' ')
                        {
                            foreach (Tile tile in room.Tiles)
                            {
                                if (pos == room.MapPosition + tile.RoomPosition)
                                {
                                    char temp = p_Tiles[i, j];
                                    if (p_Tiles[i, j] == 'B' && tile.Type == 'B')
                                    {
                                        result[i, j] = ' ';
                                        break;
                                    }
                                    else if (p_Tiles[i, j] == '0' && tile.Type == '0')
                                    {
                                        break;
                                    }
                                    else if(p_Tiles[i,j]=='1'&& tile.Type == '1')
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        o_Ret = result;
                                        return false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                        
                    }
                }
            }
        }
        o_Ret = result;
        return true;
    }

    private void GenerateConnectedRoom(Room p_Room, int p_Depth)
    {
        if (p_Depth >= MaxDepth)
        {
            p_Room.isVictory = true;
            foreach(Tile tile in p_Room.Tiles)
            {
                tile.GetComponentInChildren<SpriteRenderer>().color = VictoryColor;
            }
            List<Tile> floor_tiles = p_Room.Tiles.FindAll(t => t.IsFloorTile);
            int i = UnityEngine.Random.Range(0, floor_tiles.Count);
            Tile to_be_removed = floor_tiles[i];
            p_Room.Tiles.Remove(to_be_removed);
            SpawnTile('V', p_Room, to_be_removed.RoomPosition);
            Destroy(to_be_removed.gameObject);

            return;
        }
        foreach(Door door in p_Room.Doors)
        {
            if (door.ConnectedDoor)
            {
                continue;
            }
            bool canWork = false;
            int tries = 0;
            while (!canWork)
            {
                tries++;
                if (tries > MaxTries)
                {
                    break;
                }
                int randRoom = UnityEngine.Random.Range(0, RoomTexts.Count);
                
                string adjacentRoomText = RoomTexts[randRoom].text;
                string[] lines = adjacentRoomText.Split('\n');
                int maxL = 0;
                for(int i = 0; i < lines.Length; i++)
                {
                    if (lines[i][lines[i].Length - 1] == '\n') lines[i] = lines[i].Substring(0, lines[i].Length - 1);
                    if (lines[i].Length > maxL) maxL = lines[i].Length;
                }

                char[,] rebuild = new char[lines.Length, maxL];
                for (int i = 0; i < lines.Length; i++)
                {
                    for (int j = 0; j < maxL; j++)
                    {
                        if (j > lines[i].Length - 1)
                        {
                            rebuild[i, j] = ' ';
                        }
                        else
                        {
                            rebuild[i, j] = lines[i][j];
                        }
                    }
                }

                for (int i = 0; i < rebuild.GetLength(0); i++)
                {
                    for (int j = 0; j < maxL; j++)
                    {
                        Direction curTileDirection = Direction.NONE;
                        if (rebuild[i, j] == door.OnTile.Type)
                        {
                            if (i == 0 || rebuild[i - 1, j] == ' ')
                            {
                                curTileDirection = Direction.N;
                            }
                            else if (i == rebuild.GetLength(0) - 1 || rebuild[i + 1, j] == ' ')
                            {
                                curTileDirection = Direction.S;
                            }
                            else if (j == 0 || rebuild[i, j - 1] == ' ')
                            {
                                curTileDirection = Direction.W;
                            }
                            else if (j == maxL - 1 || rebuild[i, j + 1] == '\n' || rebuild[i, j + 1] == ' ')
                            {
                                curTileDirection = Direction.E;
                            }

                            int delta = curTileDirection - door.DoorDirection;
                            if (delta < 0)
                            {
                                delta = delta + 4;
                            }
                            char[,] result = rebuild;
                            int row = rebuild.GetLength(0);
                            int column = rebuild.GetLength(1);
                            Vector2Int doorLocalPos = new Vector2Int(j, row - 1 - i);
                            
                            switch (delta)
                            {
                                case 0:
                                    result = Rotate180(rebuild);
                                    doorLocalPos = new Vector2Int(column - 1 - j, i);
                                    break;
                                case 1:
                                    result = Rotate90(rebuild);
                                    doorLocalPos = new Vector2Int(row - 1 - i, column - 1 - j);
                                    break;
                                case 2:
                                    // No rotation
                                    break;
                                case 3:
                                    result = RotateMinus90(rebuild);
                                    doorLocalPos = new Vector2Int(i, j);
                                    break;
                                default:
                                    break;
                            }
                            Vector2Int roomOrigin = p_Room.MapPosition + door.OnTile.RoomPosition - doorLocalPos;
                            char[,] reduced = new char[result.GetLength(0), result.GetLength(1)];
                            if (Validate(result, roomOrigin, out reduced))
                            {
                                // Generate room
                                Room nextRoom = GenerateRoom(adjacentRoomText, reduced, roomOrigin);
                                nextRoom.Depth = p_Depth;
                                foreach(Door otherDoor in nextRoom.Doors)
                                {
                                    if (otherDoor.OnTile.RoomPosition == doorLocalPos)
                                    {
                                        door.ConnectedDoor = otherDoor;
                                        otherDoor.ConnectedDoor = door;
                                    }
                                }
                                GenerateConnectedRoom(nextRoom, p_Depth + 1);
                                canWork = true;
                            }
                            if (canWork) break;
                        }
                        if (canWork) break;
                    }
                    
                }
                
            }
        }
    }

    private char[,] Rotate90(char[,] p_Input)
    {
        int row = p_Input.GetLength(0);
        int column = p_Input.GetLength(1);
        char[,] ret = new char[column, row];
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                ret[j, row - 1 - i] = p_Input[i, j];
            }
        }
        return ret;
    }

    private char[,] RotateMinus90(char[,] p_Input)
    {
        int row = p_Input.GetLength(0);
        int column = p_Input.GetLength(1);
        char[,] ret = new char[column, row];
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                ret[column - 1 - j, i] = p_Input[i, j];
            }
        }
        return ret;
    }

    private char[,] Rotate180(char[,] p_Input)
    {
        int row = p_Input.GetLength(0);
        int column = p_Input.GetLength(1);
        char[,] ret = new char[row, column];
        for(int i = 0; i < row; i++)
        {
            for(int j = 0; j < column; j++)
            {
                ret[row - 1 - i, column - 1 - j] = p_Input[i, j];
            }
        }
        // Debug.Log(ret.ToString());
        return ret;
    }

    private void RemoveUnconnectedDoors()
    {
        foreach(Room room in GameMap.Rooms)
        {
            foreach(Door door in room.Doors)
            {
                if (door.ConnectedDoor) continue;
                TileType curTile = TileTypes.Find(x => x.Name == 'B');
                Tile curObject = null;
                curObject = Instantiate(curTile.TileObject, room.transform);
                curObject.transform.localPosition = new Vector3(door.transform.localPosition.x, door.transform.localPosition.y, 0);
                curObject.Type = 'B';
                curObject.ParentRoom = room;
                room.Tiles.Add(curObject);
                Destroy(door.gameObject);
            }
        }
    }
}
