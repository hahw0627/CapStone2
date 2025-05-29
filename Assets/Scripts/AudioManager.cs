using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource bgmSource;
    public AudioSource sfxSource;

    public AudioClip attackClip;
    public AudioClip monsterDeadClip;
    public AudioClip playerHitClip;
    public AudioClip playerRevivalClip;

    // public AudioClip buttonClickClip;
    // public AudioClip gameOverClip;

    public AudioClip lobbyBGM;
    public AudioClip mainBGM;

    private void Awake()
    {
        // ½Ì±ÛÅæ ¼³Á¤
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ¾À ÀüÈ¯ ½Ã¿¡µµ À¯Áö
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayAttackSound()
    {
        PlaySFX(attackClip);
    }

    public void MonsterDeadSound()
    {
        PlaySFX(monsterDeadClip);
    }

    public void PlayerHitSound()
    {
        PlaySFX(playerHitClip);
    }

    public void PlayerRevivalSound()
    {
        PlaySFX(playerRevivalClip);
    }
    public void PlayBGM(AudioClip bgmClip)
    {
        if (bgmSource != null && bgmClip != null)
        {
            if (bgmSource.isPlaying)
            {
                bgmSource.Stop();
            }
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    /* public void PlayButtonClick()
    {
        PlaySFX(buttonClickClip);
    }*/

    /* public void PlayGameOver()
    {
        PlaySFX(gameOverClip);
    }*/

}
