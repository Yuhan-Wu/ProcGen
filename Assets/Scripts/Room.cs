using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector2Int MapPosition;

    [TextArea]
    public string StringRoom;

    public List<Tile> Tiles = new List<Tile>();
    public List<Door> Doors = new List<Door>();

    public bool isVictory = false;
}
