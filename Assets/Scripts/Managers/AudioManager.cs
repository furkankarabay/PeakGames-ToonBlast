using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] tileDestroyedSounds;
    public AudioClip tileCollectedSound;


    public static AudioManager Instance;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    public void PlayTileDestroySound(TileType tileType)
    {
        if (audioSource == null)
            return;

        int tileTypeIndex = (int)tileType;

        if (tileTypeIndex <= 4 && tileType >= 0) // CUBE
            audioSource.PlayOneShot(tileDestroyedSounds[0]);
        else if (tileTypeIndex == 5)
            audioSource.PlayOneShot(tileDestroyedSounds[1]); // BALOON
        else if (tileTypeIndex == 6)
            audioSource.PlayOneShot(tileDestroyedSounds[2]); // DUCK
    }

    public void PlayCubeTileCollectedSound()
    {
        audioSource.PlayOneShot(tileCollectedSound);
    }
}
