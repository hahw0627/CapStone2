using UnityEngine;
using UnityEngine.UI;

public class SettingsUi : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // 초기값 설정 (저장된 값 불러오기 등)
        bgmSlider.value = AudioManager.Instance.bgmSource.volume;
        sfxSlider.value = AudioManager.Instance.sfxSource.volume;

        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetBGMVolume(float volume)
    {
        AudioManager.Instance.SetBGMVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        AudioManager.Instance.SetSFXVolume(volume);
    }
}
