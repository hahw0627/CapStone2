using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ComicSceneManager : MonoBehaviour
{
    public static string nextScene = "DevScene";
    [System.Serializable]
    public class ComicCut
    {
        public Sprite image;
        [TextArea]
        public string description;
    }

    public List<ComicCut> comicCuts; // Inspector에서 컷 이미지 + 설명 연결
    public Image comicImage;         // 컷 이미지 보여줄 UI
    public Text descriptionText;     // 컷 설명 텍스트
    public Button screenButton;      // 화면 클릭 유도 버튼

    private int currentIndex = 0;

    private void Start()
    {
        ShowCurrentCut();
        screenButton.onClick.AddListener(OnNextCut);
        AudioManager.Instance.PlayBGM(AudioManager.Instance.lobbyBGM);
    }

    private void ShowCurrentCut()
    {
        comicImage.sprite = comicCuts[currentIndex].image;
        descriptionText.text = comicCuts[currentIndex].description;
    }

    private void OnNextCut()
    {
        currentIndex++;
        if (currentIndex < comicCuts.Count)
        {
            ShowCurrentCut();
        }
        else
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
