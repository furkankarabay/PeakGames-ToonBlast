using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;

    public GameObject starEffect;
    public List<Color> cubeColors = new List<Color>();
    public List<GameObject> explosionEffects = new List<GameObject>();
    public List<GameObject> animObjRockets = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PlayCubeExplosionVFX(TileType tileType, Vector3 position)
    {
        Color color = cubeColors[((int)tileType)];
        int randomExplosion = Random.Range(0, 2);

        GameObject particle = Instantiate(explosionEffects[randomExplosion]);
        particle.transform.position = position;

        ParticleSystem particleSystem = particle.GetComponent<ParticleSystem>();
        var main = particleSystem.main;
        main.startColor = color;

    }

    public void PlayStarVFX(Vector3 position)
    {
        GameObject particle = Instantiate(starEffect);
        particle.transform.position = position;

    }

    public void PlayRockets(Vector3 position, bool isHorizontal)
    {
        GameObject rockets = Instantiate(isHorizontal ? animObjRockets[0] : animObjRockets[1]);
        Destroy(rockets, 5);
        Transform rocket1 = rockets.transform.GetChild(0);
        Transform rocket2 = rockets.transform.GetChild(1);

        rockets.transform.position = position;

        if (isHorizontal)
        {

            rocket1.DOMoveX(position.x + 5, 0.6f).OnComplete(() => Destroy(rocket1.gameObject));
            rocket2.DOMoveX(position.x - 5, 0.6f).OnComplete(() => Destroy(rocket2.gameObject));
        }
        else
        {
            rocket1.DOMoveY(position.y + 5, 0.6f).OnComplete(() => Destroy(rocket1.gameObject));
            rocket2.DOMoveY(position.y - 5, 0.6f).OnComplete(() => Destroy(rocket2.gameObject));
        }
    }
}
