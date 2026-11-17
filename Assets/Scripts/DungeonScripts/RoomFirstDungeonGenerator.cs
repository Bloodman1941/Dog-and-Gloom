using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField]
    private int minRoomWidth = 6, minRoomHeight = 6;
    [SerializeField]
    private int dungeonWidth = 60, dungeonHeight = 60;
    [SerializeField]
    [Range(0, 10)]
    private int offset = 1;
    [SerializeField]
    private bool randomWalkRooms = false;

    [Header("Player & Exit Spawning")]
    [SerializeField]
    private GameObject playerPrefab;
    private GameObject currentPlayer;
    private GameObject currentExitTrigger;

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        if (randomWalkRooms)
        {
            floor = CreateRoomsRandomly(roomsList);
        }
        else
        {
            floor = CreateSimpleRooms(roomsList);
        }

        // Clean up the dungeon by removing isolated floor tiles
        floor = CleanUpDungeon(floor);

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            int cx = room.min.x + room.size.x / 2;
            int cy = room.min.y + room.size.y / 2;
            roomCenters.Add(new Vector2Int(cx, cy));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);

        // Clean up the dungeon again after adding corridors
        floor = CleanUpDungeon(floor);

        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);

        SpawnPlayerAndExit(roomsList, floor);
    }

    private HashSet<Vector2Int> CleanUpDungeon(HashSet<Vector2Int> floorPositions)
    {
        HashSet<Vector2Int> cleanedFloor = new HashSet<Vector2Int>();
        foreach (var pos in floorPositions)
        {
            int neighborCount = 0;
            foreach (var dir in Direction2D.cardinalDirectionsList)
            {
                if (floorPositions.Contains(pos + dir))
                {
                    neighborCount++;
                }
            }
            if (neighborCount > 0)
            {
                cleanedFloor.Add(pos);
            }
        }
        return cleanedFloor;
    }

    private void SpawnPlayerAndExit(List<BoundsInt> roomsList, HashSet<Vector2Int> floorPositions)
    {
        if (roomsList == null || roomsList.Count < 2) return;

        GameObject[] oldPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (var oldPlayer in oldPlayers)
        {
            Destroy(oldPlayer);
        }
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }
        if (currentExitTrigger != null)
        {
            Destroy(currentExitTrigger);
        }

        int playerRoomIndex = Random.Range(0, roomsList.Count);
        BoundsInt playerRoom = roomsList[playerRoomIndex];
        Vector2Int playerSpawnPos = GetValidFloorInRoom(playerRoom, floorPositions);

        int exitRoomIndex;
        do
        {
            exitRoomIndex = Random.Range(0, roomsList.Count);

        } while (exitRoomIndex == playerRoomIndex);

        BoundsInt exitRoom = roomsList[exitRoomIndex];
        Vector2Int exitSpawnPos = GetValidFloorInRoom(exitRoom, floorPositions);

        if (playerPrefab != null)
        {
            currentPlayer = Instantiate(playerPrefab, new Vector3(playerSpawnPos.x, playerSpawnPos.y, 0), Quaternion.identity);
            currentPlayer.tag = "Player";

            // Directly assign camera target immediately upon spawn
            CameraFollowPlayer camFollow = Camera.main.GetComponent<CameraFollowPlayer>();
            if (camFollow != null)
            {
                camFollow.player = currentPlayer.transform;
            }
            else
            {
                Camera.main.gameObject.AddComponent<CameraFollowPlayer>().player = currentPlayer.transform;
            }
        }
        else
        {
            Debug.LogWarning("Player prefab is not assigned.");
        }

        tilemapVisualizer.PaintExitTile(exitSpawnPos);

        currentExitTrigger = new GameObject("DungeonExitTrigger");
        currentExitTrigger.transform.position = new Vector3(exitSpawnPos.x, exitSpawnPos.y, 0);
        BoxCollider2D boxCol = currentExitTrigger.AddComponent<BoxCollider2D>();
        boxCol.isTrigger = true;

        DungeonExit exitScript = currentExitTrigger.AddComponent<DungeonExit>();
        exitScript.Initialized(this);
    }

    private Vector2Int GetValidFloorInRoom(BoundsInt roomBounds, HashSet<Vector2Int> floorPositions)
    {
        // Calculate center safely using min and size
        int centerX = roomBounds.min.x + roomBounds.size.x / 2;
        int centerY = roomBounds.min.y + roomBounds.size.y / 2;
        Vector2Int center = new Vector2Int(centerX, centerY);

        if (floorPositions.Contains(center))
        {
            return center;
        }

        // Fallback: search for any valid floor tile inside the room
        for (int col = offset; col < roomBounds.size.x - offset; col++)
        {
            for (int row = offset; row < roomBounds.size.y - offset; row++)
            {
                Vector2Int pos = (Vector2Int)roomBounds.min + new Vector2Int(col, row);
                if (floorPositions.Contains(pos))
                {
                    return pos;
                }
            }
        }

        return center;
    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            int centerX = roomBounds.min.x + roomBounds.size.x / 2;
            int centerY = roomBounds.min.y + roomBounds.size.y / 2;
            var roomCenter = new Vector2Int(centerX, centerY);

            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (var position in roomFloor)
            {
                int xMin = roomBounds.min.x + offset;
                int xMax = roomBounds.min.x + roomBounds.size.x - offset;
                int yMin = roomBounds.min.y + offset;
                int yMax = roomBounds.min.y + roomBounds.size.y - offset;

                if (position.x >= xMin && position.x <= xMax && position.y >= yMin && position.y <= yMax)
                {
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
            }
            corridor.Add(position);
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }
            corridor.Add(position);
        }
        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }
}