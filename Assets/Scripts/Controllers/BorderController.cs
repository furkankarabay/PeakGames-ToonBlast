using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderController : MonoBehaviour
{
    public static BorderController Instance;

    public GameObject upperLeft, upperRight, lowerRight, lowerLeft;
    public GameObject straightHorizontal, straightVertical;

    private List<TileController> controllerList; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public TileController FindTileByCoordinates(Vector2Int coordinates)
    {
        foreach (TileController tile in controllerList)
        {
            if (tile.tile.coordinates == coordinates)
            {
                return tile;
            }
        }
        return null;
    }

    public IEnumerator GenerateBorders(int width, int height, List<TileController> tileControllers)
    {
        controllerList = new List<TileController>(tileControllers);

        yield return new WaitForSeconds(0.1f);

        GameObject tileUpperLeft = FindTileByCoordinates(new Vector2Int(0, height - 1)).gameObject;
        GameObject borderUpperLeft = Instantiate(upperLeft, transform);
        borderUpperLeft.transform.position = tileUpperLeft.transform.position;

        GameObject tileUpperRight = FindTileByCoordinates(new Vector2Int(width - 1, height - 1)).gameObject;
        GameObject borderUpperRight = Instantiate(upperRight, transform);
        borderUpperRight.transform.position = tileUpperRight.transform.position;

        GameObject tileLowerRight = FindTileByCoordinates(new Vector2Int(width - 1, 0)).gameObject;
        GameObject borderLowerRight = Instantiate(lowerRight, transform);
        borderLowerRight.transform.position = tileLowerRight.transform.position;

        GameObject tileLowerLeft = FindTileByCoordinates(new Vector2Int(0, 0)).gameObject;
        GameObject borderLowerLeft = Instantiate(lowerLeft, transform);
        borderLowerLeft.transform.position = tileLowerLeft.transform.position;

        for (int i = 1; i <= width - 2; i++)
        {
            GameObject upperTile = FindTileByCoordinates(new Vector2Int(i, height - 1)).gameObject;
            GameObject borderUpperTile = Instantiate(straightHorizontal, transform);
            borderUpperTile.transform.position = upperTile.transform.position;
            borderUpperTile.transform.rotation = Quaternion.Euler(0, 0, 180);

            GameObject lowerTile = FindTileByCoordinates(new Vector2Int(i, 0)).gameObject;
            GameObject borderLowerTile = Instantiate(straightHorizontal, transform);
            borderLowerTile.transform.position = lowerTile.transform.position;
        }

        for (int i = 1; i <= height - 2; i++)
        {
            GameObject rightTile = FindTileByCoordinates(new Vector2Int(width - 1, i)).gameObject;
            GameObject borderRightTile = Instantiate(straightVertical, transform);
            borderRightTile.transform.position = rightTile.transform.position;
            borderRightTile.transform.rotation = Quaternion.Euler(0, 0, 180);

            GameObject leftTile = FindTileByCoordinates(new Vector2Int(0, i)).gameObject;
            GameObject borderLeftTile = Instantiate(straightVertical, transform);
            borderLeftTile.transform.position = leftTile.transform.position;
        }

    }
}
