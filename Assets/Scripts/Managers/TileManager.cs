using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public bool developMode;

    public CheesyGrid cheesyGrid;
    public GameObject tilePrefab;
    public GameObject rootOfTiles;
    public Sprite[] tileSprites;
    public List<TileController> tileControllers = new List<TileController>();
    public List<Transform> spawnPoints = new List<Transform>();
    public List<Transform> rowPoints = new List<Transform>();
    private List<int> spawnablesIndexes = new List<int>() { 0,1,2,3,4 };
    private List<TileController> clickedTileAllNeighbors = new List<TileController>();

    public float spaceBetweenTiles;
    public Vector3 firstTilePosition;

    private int width, height;
    private float startX, startY;
    private int spawnablesMaxLimit = 5;
    private string dataOfTileOrder;

    public static TileManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if(developMode)
        {
            width = cheesyGrid.width;
            height = cheesyGrid.height;
            dataOfTileOrder = cheesyGrid.data;
        }
        else
            Initialize(LevelManager.Instance.currentLevel);
    }
    public void Initialize(Level level)
    {
        dataOfTileOrder = level.dataOfTileOrder;
        width = level.width;
        height = level.height;
    }

    public void StartGame()
    {
        SetStartingPoint();

        SetSpawnables();

        GenerateBoard();
    }

    private void SetStartingPoint()
    {
        startX = (9 - width) * 0.25f;
        startY = (11 - height) * 0.25f;
        rootOfTiles.transform.localPosition = new Vector3(startX, startY, 0);
        spawnPoints[0].transform.parent.position = new Vector3(rootOfTiles.transform.position.x, spawnPoints[0].parent.transform.position.y);
    }

    public void GenerateBoard()
    {
        int dataOfTile = 0;
        int index = 0;
        float xPosition = firstTilePosition.x;
        float yPosition = firstTilePosition.y;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                dataOfTile = dataOfTileOrder[index] - '0';

                Vector3 startPosition = new Vector3(0, yPosition, 0);
                rowPoints[i].gameObject.SetActive(true);
                spawnPoints[i].gameObject.SetActive(true);
                GameObject tileGO = Instantiate(tilePrefab, rowPoints[i].transform);
                tileGO.transform.SetLocalPositionAndRotation(startPosition, Quaternion.identity);

                Vector2Int coordinates = new Vector2Int(i, j);

                Tile tileData = new Tile(coordinates, (TileType)dataOfTile, j);
                TileController tileController = tileGO.GetComponent<TileController>();
                tileController.Initialize(tileData);

                tileControllers.Add(tileController);
                yPosition += spaceBetweenTiles;

                index++;

            }

            yPosition = firstTilePosition.y;
            xPosition += spaceBetweenTiles;
        }

        StartCoroutine(BorderController.Instance.GenerateBorders(width, height, tileControllers));
    }

    
    
    private IEnumerator ReassignCoordinates()
    {
        yield return new WaitForEndOfFrame();

        foreach (TileController tileController in tileControllers)
        {
            int parentIndex = 0;

            for (int i = 0; i < rowPoints.Count; i++)
            {
                if (rowPoints[i] == tileController.transform.parent)
                {
                    parentIndex = i;
                    break;
                }
            }

            tileController.SetCoordinates(new Vector2Int(parentIndex, tileController.transform.GetSiblingIndex()));
            tileController.SetName(tileController.tile.coordinates);
            tileController.SetSortingOrder(tileController.tile.coordinates.y);
        }
    }

    private void ReassignAtSpecifiedCoordinate(TileController tileController, Vector2Int coordinates, int siblingIndex)
    {
        tileController.SetCoordinates(coordinates);
        tileController.SetName(tileController.tile.coordinates);
        tileController.transform.SetSiblingIndex(siblingIndex);
    }

    private IEnumerator UpdateSpawnPointHeight(Vector2Int coordinates)
    {
        spawnPoints[coordinates.x].transform.localPosition += Vector3.up;

        yield return new WaitForSeconds(0.5f);

        spawnPoints[coordinates.x].transform.localPosition -= Vector3.up;

    }
    private void GenerateTileAtColumn(Vector2Int coordinates)
    {
        GameObject tileGO = Instantiate(tilePrefab, rowPoints[coordinates.x]);

        Vector3 startPosition = new Vector3(0, spawnPoints[coordinates.x].transform.localPosition.y, 0);
        tileGO.transform.SetLocalPositionAndRotation(startPosition, Quaternion.identity);

        StartCoroutine(UpdateSpawnPointHeight(coordinates));
        


        int typeRandomized = Random.Range(0, spawnablesMaxLimit);
        Tile tileData = new Tile(coordinates, (TileType)spawnablesIndexes[typeRandomized], coordinates.y);
        TileController tileController = tileGO.GetComponent<TileController>();
        tileController.Initialize(tileData);
        tileControllers.Add(tileController);
    }

    private void SetSpawnables()
    {
        TileType goal1Type = GameManager.Instance.goal1Type;
        TileType goal2Type = GameManager.Instance.goal2Type;

        if (goal1Type == TileType.baloon || goal2Type == TileType.baloon)
            spawnablesIndexes.Add((int) TileType.baloon);

        if (goal1Type == TileType.duck || goal2Type == TileType.duck)
            spawnablesIndexes.Add((int) TileType.duck);

        spawnablesMaxLimit = spawnablesIndexes.Count;
    }
    private void GenerateTileAtClickedTile(TileController clickedTileController, bool isVertical)
    {
        Vector2Int coordinates = clickedTileController.tile.coordinates;
        Vector3 spawnPosition = new Vector3(clickedTileController.transform.localPosition.x, clickedTileController.transform.localPosition.y);
        GameObject tileGO = Instantiate(tilePrefab, rowPoints[clickedTileController.tile.coordinates.x].transform);
        tileGO.transform.SetLocalPositionAndRotation(
            spawnPosition, Quaternion.identity);

        Tile tileData = new Tile(coordinates, isVertical ? TileType.rocketV : TileType.rocketH, coordinates.y);
        TileController tileController = tileGO.GetComponent<TileController>();
        tileController.Initialize(tileData);

        tileControllers.Add(tileController);

        ReassignAtSpecifiedCoordinate(tileController, coordinates, clickedTileController.transform.GetSiblingIndex());
    }

    internal void TileClicked(TileController tileController)
    {
        bool isGoal = GameManager.Instance.IsTileOnGoal(tileController.tile.tileType);

        if(tileController.tile.tileType == TileType.rocketH || tileController.tile.tileType == TileType.rocketV)
        {
            LaunchRocket(tileController , tileController.tile.tileType == TileType.rocketH, false);

            GameManager.Instance.LowerRemainingMoves();

            return;
        }

        if (tileController.tile.tileType == TileType.duck)
        {
            tileControllers.Remove(tileController);

            GenerateTileAtColumn(tileController.tile.coordinates);

            if(isGoal)
            {
                tileController.ChangeToGoalObject();
                
                Vector3 goalPosition = GameplayUIController.Instance.GetGoalPosition(tileController.tile.tileType);
                SendTileToGoal(tileController, goalPosition);
            }

            clickedTileAllNeighbors.Clear();

            StartCoroutine(ReassignCoordinates());

            return;
        }


        clickedTileAllNeighbors = GetSameTypeNeighbors(tileController);

        bool isPowerUpCreated = clickedTileAllNeighbors.Count > 4;

        if (isPowerUpCreated)
        {
            bool randomBool = Random.Range(0, 2) == 0;
            GenerateTileAtClickedTile(tileController, randomBool);
        }

        if(clickedTileAllNeighbors.Count > 1)
        {
            StartCoroutine(CheckChain(tileController, isPowerUpCreated, isGoal));
            GameManager.Instance.LowerRemainingMoves();
        }

    }

    public void LaunchRocket(TileController tileController, bool isHorizontalRocket, bool launchedFromRocket)
    {
        List<Vector2Int> coordinatesList = new List<Vector2Int>();

        Vector2Int rocketTileCoordinates = tileController.tile.coordinates;

        if (isHorizontalRocket) // YATAY ROKET
        {
            int index = 0;

            for (int i = rocketTileCoordinates.x; i < width - 1; i++) // Sað Tarafa
            {
                index++;

                Vector2Int tileToDestroyCoordinates = new Vector2Int(rocketTileCoordinates.x + index, rocketTileCoordinates.y);

                TileController tileToDestroy = FindTileByCoordinates(tileToDestroyCoordinates);

                if (tileToDestroy.tile.tileType == TileType.duck)
                    continue;

                tileToDestroy.ChangeToDestroyableByRocket();

                tileControllers.Remove(tileToDestroy);

                coordinatesList.Add(tileToDestroyCoordinates);
            }

            index = 0;

            for (int i = rocketTileCoordinates.x; i > 0; i--) // Sol Tarafa
            {
                index++;

                Vector2Int tileToDestroyCoordinates = new Vector2Int(rocketTileCoordinates.x - index, rocketTileCoordinates.y);

                TileController tileToDestroy = FindTileByCoordinates(tileToDestroyCoordinates);

                if (tileToDestroy.tile.tileType == TileType.duck)
                    continue;

                tileToDestroy.ChangeToDestroyableByRocket();

                tileControllers.Remove(tileToDestroy);

                coordinatesList.Add(tileToDestroyCoordinates);
            }


        }
        else // DÝKEY ROKET
        {
            int index = 0;

            for (int i = rocketTileCoordinates.y; i < height - 1; i++) // Sað Tarafa
            {
                index++;

                Vector2Int tileToDestroyCoordinates = new Vector2Int(rocketTileCoordinates.x, rocketTileCoordinates.y + index);

                TileController tileToDestroy = FindTileByCoordinates(tileToDestroyCoordinates);

                if (tileToDestroy.tile.tileType == TileType.duck)
                    continue;

                tileToDestroy.ChangeToDestroyableByRocket();

                tileControllers.Remove(tileToDestroy);

                coordinatesList.Add(tileToDestroyCoordinates);
            }

            index = 0;

            for (int i = rocketTileCoordinates.y; i > 0; i--) // Sol Tarafa
            {
                index++;

                Vector2Int tileToDestroyCoordinates = new Vector2Int(rocketTileCoordinates.x, rocketTileCoordinates.y - index);

                TileController tileToDestroy = FindTileByCoordinates(tileToDestroyCoordinates);

                if (tileToDestroy.tile.tileType == TileType.duck)
                    continue;

                tileToDestroy.ChangeToDestroyableByRocket();

                tileControllers.Remove(tileToDestroy);

                coordinatesList.Add(tileToDestroyCoordinates);
            }
        }

        EffectManager.Instance.PlayRockets(tileController.transform.position, isHorizontalRocket);

        tileControllers.Remove(tileController);
        Destroy(tileController.gameObject);
        
        if(!launchedFromRocket)
            coordinatesList.Add(tileController.tile.coordinates);

        for (int i = 0; i < coordinatesList.Count; i++)
        {
            GenerateTileAtColumn(coordinatesList[i]);
        }

        StartCoroutine(ReassignCoordinates());
    }

    private IEnumerator CheckChain(TileController tileController, bool powerUpCreated, bool isGoal)
    {
        List<Vector2Int> coordinatesList = new List<Vector2Int>();

        foreach (TileController neighborTile in clickedTileAllNeighbors)
        {
            tileControllers.Remove(neighborTile);

            EffectManager.Instance.PlayCubeExplosionVFX(neighborTile.tile.tileType, neighborTile.transform.position);

            if(isGoal)
            {
                neighborTile.ChangeToGoalObject();
            }
            else
            {
                Destroy(neighborTile.gameObject);
            }

            if(powerUpCreated)
            {
                if (tileController != neighborTile)
                    coordinatesList.Add(neighborTile.tile.coordinates);
            }
            else
                coordinatesList.Add(neighborTile.tile.coordinates);

        }

        for (int i = 0; i < coordinatesList.Count; i++)
        {
            yield return null;
            GenerateTileAtColumn(coordinatesList[i]);

        }

        if (isGoal)
        {
            Vector3 goalPosition = GameplayUIController.Instance.GetGoalPosition(tileController.tile.tileType);
            List<TileController> goalTiles = new List<TileController>(clickedTileAllNeighbors);
            StartCoroutine(SendTilesToGoal(goalTiles, goalPosition));
        }

        clickedTileAllNeighbors.Clear();
        
        StartCoroutine(ReassignCoordinates());

    }

    public void SendTileToGoal(TileController tile, Vector3 endPosition)
    {
        tile.ChangeToGoalObject();

        tile.transform.DOMove(endPosition, 0.5f).OnComplete(() => DestroyedGoalObj(tile));
    }
    private IEnumerator SendTilesToGoal(List<TileController> goalTiles, Vector3 endPosition)
    {

        foreach (TileController tileController in goalTiles)
        {
            if(tileController.tile.tileType == TileType.baloon)
            {
                Destroy(tileController.gameObject);
            }
            else
            {
                tileController.transform.DOMove(tileController.transform.position - Vector3.up, 0.2f);
            }
        }

        yield return new WaitForSeconds(0.1f);

        foreach (TileController tileController in goalTiles)
        {
            if(tileController)
                tileController.transform.DOMove(endPosition, 0.5f).OnComplete(()=> DestroyedGoalObj(tileController));

            yield return new WaitForSeconds(0.065f);
        }

        yield return new WaitForSeconds(0.5f);

        EffectManager.Instance.PlayStarVFX(endPosition);
    }

    private void DestroyedGoalObj(TileController tileController)
    {
        AudioManager.Instance.PlayCubeTileCollectedSound();
        Destroy(tileController.gameObject);
    }

    public List<TileController> GetSameTypeNeighbors(TileController tileToCheck)
    {
        Vector2Int tileToCheckCoordinates = new Vector2Int(tileToCheck.tile.coordinates.x, tileToCheck.tile.coordinates.y);

        for (int x = -1; x <= 1; x++)
        {
            if (x == 0)
                continue;

            Vector2Int neighborTileCoordinates = new Vector2Int(tileToCheckCoordinates.x + x, tileToCheckCoordinates.y);

            TileController neighborTileController = FindTileByCoordinates(neighborTileCoordinates);

            if (neighborTileController != null)
            {
                if (neighborTileController.tile.tileType != tileToCheck.tile.tileType &&
                    neighborTileController.tile.tileType != TileType.baloon)
                    continue;

                if (clickedTileAllNeighbors.Contains(neighborTileController))
                    continue;

                clickedTileAllNeighbors.Add(neighborTileController);

                if (neighborTileController.tile.tileType != TileType.baloon)
                {
                    clickedTileAllNeighbors = GetSameTypeNeighbors(neighborTileController);

                }
            }
        }

        for (int y = -1; y <= 1; y++)
        {
            if (y == 0)
                continue;

            Vector2Int neighborTileCoordinates = new Vector2Int(tileToCheckCoordinates.x, tileToCheckCoordinates.y + y);

            TileController neighborTileController = FindTileByCoordinates(neighborTileCoordinates);

            if (neighborTileController != null)
            {
                if (neighborTileController.tile.tileType != tileToCheck.tile.tileType &&
                    neighborTileController.tile.tileType != TileType.baloon)
                    continue;

                if (clickedTileAllNeighbors.Contains(neighborTileController))
                    continue;

                clickedTileAllNeighbors.Add(neighborTileController);

                if(neighborTileController.tile.tileType != TileType.baloon)
                {
                    clickedTileAllNeighbors = GetSameTypeNeighbors(neighborTileController);

                }
            }
        }

        return clickedTileAllNeighbors;
    }

    public TileController FindTileByCoordinates(Vector2Int coordinates)
    {
        foreach (TileController tile in tileControllers)
        {
            if (tile.tile.coordinates == coordinates)
            {
                return tile;
            }
        }

        return null;
    }
}
