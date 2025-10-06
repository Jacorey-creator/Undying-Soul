using UnityEngine;

public static class AudioHelper
{
    public static void PlaySound(AudioClip clip, AudioSource audioSource)
    {
        if (audioSource == null || clip == null)
            return;

        // Stop current sound if playing
        if (audioSource.isPlaying)
            audioSource.Stop();

        // Slight random pitch for natural variation
        audioSource.pitch = Random.Range(0.95f, 1.05f);

        // Play sound
        audioSource.PlayOneShot(clip);
    }
}
