using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;

public class TileController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Tile tile;

    private bool willDestoryWithRocket = false;
    private bool isGoalObj = false;

    public void Initialize(Tile tile)
    {
        this.tile = tile;
        spriteRenderer.sprite = TileManager.Instance.tileSprites[(int)tile.tileType];
        spriteRenderer.sortingOrder = tile.orderInLayer;
        SetName(tile.coordinates);
    }

    public void SetCoordinates(Vector2Int newCoordinates)
    {
        tile.coordinates = newCoordinates;
    }
    public void SetName(Vector2Int tileCoordinates)
    {
        name = string.Format("Tile: {0}, {1}", tile.coordinates.x.ToString(), tile.coordinates.y.ToString());
    }

    public void SetSortingOrder(int order)
    {
        tile.orderInLayer = order;
        spriteRenderer.sortingOrder = tile.orderInLayer;
    }


    public void ChangeToGoalObject()
    {
        isGoalObj = true;
        AudioManager.Instance.PlayTileDestroySound(tile.tileType);

        transform.SetParent(null);
        spriteRenderer.sortingLayerName = "Gameplay";
        spriteRenderer.sortingOrder = 1;
        GetComponent<BoxCollider2D>().isTrigger = true;
        GetComponent<Rigidbody2D>().gravityScale = 0;
    }

    public void ChangeToDestroyableByRocket()
    {
        willDestoryWithRocket = true;

        Destroy(gameObject, 3);
        GetComponent<BoxCollider2D>().isTrigger = true;
        GetComponent<Rigidbody2D>().gravityScale = 0;

        transform.SetParent(null);
    }

    private void OnMouseDown()
    {
        if (!GameManager.Instance.CheckEnoughMoves())
            return;

        if (willDestoryWithRocket)
            return;

        if (tile.tileType == TileType.baloon)
            return;

        if (tile.tileType == TileType.duck)
            return;

        TileManager.Instance.TileClicked(this);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnTileDestroyed(tile.tileType, transform.position);

        if (!isGoalObj)
            AudioManager.Instance.PlayTileDestroySound(tile.tileType);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Floor"))
        {
            if (tile.tileType == TileType.duck)
            {
                TileManager.Instance.TileClicked(this);
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rocket"))
        {
            if (tile.tileType != TileType.duck)
            {
                if (tile.tileType == TileType.rocketH)
                {
                    TileManager.Instance.LaunchRocket(this, true, true);
                }
                else if (tile.tileType == TileType.rocketV)
                {
                    TileManager.Instance.LaunchRocket(this, false, true);
                }

                if (willDestoryWithRocket)
                    Destroy(gameObject);
            }

        }
    }
}
