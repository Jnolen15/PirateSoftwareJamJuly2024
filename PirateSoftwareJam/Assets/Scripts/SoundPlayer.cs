using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private List<AudioClip> _sounds;

    public void PlayAtIndex(int i, bool randomizePitch = false)
    {
        if (_sounds.Count <= i)
            return;

        _audioSource.PlayOneShot(_sounds[i]);
    }

    public void PlayWithDelay(int i, float delay, bool randomizePitch = false)
    {
        if (randomizePitch)
            RandomizePitch();

        StartCoroutine(PlayDelay(i, delay));
    }

    private IEnumerator PlayDelay(int i, float delay)
    {
        yield return new WaitForSeconds(delay);

        _audioSource.PlayOneShot(_sounds[i]);
    }

    public void PlayRandom(bool randomizePitch)
    {
        if (randomizePitch)
            RandomizePitch();

        int rand = Random.Range(0, _sounds.Count);

        _audioSource.PlayOneShot(_sounds[rand]);
    }

    public void RandomizePitch()
    {
        _audioSource.pitch = Random.Range(0.8f, 1.2f);
    }

    public void PlayMusicalScale(int index, int count)
    {
        if(_audioSource.pitch < 2)
            _audioSource.pitch *= 1.059463f;

        _audioSource.PlayOneShot(_sounds[index]);
    }

    public void SetPitch(float pitch)
    {
        _audioSource.pitch = pitch;
    }
}
