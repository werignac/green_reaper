using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource)),
    RequireComponent(typeof(RootMonster))]
public class RootMonsterSoundManager : MonoBehaviour
{
    private enum RootMonsterSoundTypes {POP = 0, HIT = 1, HIT_NO_COINS = 2, STEAL = 3}

    private AudioSource audioSource;
    private RootMonster monster;

    [SerializeField]
    private AudioClip[] popClips;

    [SerializeField]
    private AudioClip[] hitClips;

    [SerializeField]
    private AudioClip[] hitNoCoinsClips;

    [SerializeField]
    private AudioClip[] stealClips;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        monster = GetComponent<RootMonster>();

        PlayRandomClip(RootMonsterSoundTypes.POP);
    }

    private void PlayRandomClip(RootMonsterSoundTypes type)
    {

        AudioClip[] randomClips;

        switch (type)
        {
            case RootMonsterSoundTypes.POP:
                randomClips = popClips;
                break;
            case RootMonsterSoundTypes.HIT:
                randomClips = hitClips;
                break;
            case RootMonsterSoundTypes.HIT_NO_COINS:
                randomClips = hitNoCoinsClips;
                break;
            case RootMonsterSoundTypes.STEAL:
                randomClips = stealClips;
                break;
            default:
                randomClips = null;
                break;
        }

        audioSource.clip = randomClips[Random.Range(0, randomClips.Length)];
        audioSource.Play();
    }

    public void PlayHitSound()
    {
        PlayRandomClip((monster.HasStolenCoins()) ? RootMonsterSoundTypes.HIT : RootMonsterSoundTypes.HIT_NO_COINS);
    }

    public void PlayStealSound()
    {
        PlayRandomClip(RootMonsterSoundTypes.STEAL);
    }
}
