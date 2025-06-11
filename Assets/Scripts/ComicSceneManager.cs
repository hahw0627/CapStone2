using System.Collections;
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
        public string text;
    }

    public List<ComicCut> cuts; // Inspector에서 컷 이미지 + 설명 연결
    public Image cutImage;         // 컷 이미지 보여줄 UI
    public Text cutText;     // 컷 설명 텍스트
    public Button nextButton;      // 화면 클릭 유도 버튼

    private int currentIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;


    private void Start()
    {
        nextButton.onClick.AddListener(OnNextPressed);
        ShowCurrentCut();
    }

    private void ShowCurrentCut()
    {
        cutImage.sprite = cuts[currentIndex].image;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(cuts[currentIndex].text));
    }

    private IEnumerator TypeText(string fullText)
    {
        isTyping = true;
        cutText.text = "";

        foreach (char c in fullText)
        {
            cutText.text += c;
            yield return new WaitForSeconds(0.03f); // 타이핑 속도
        }

        isTyping = false;
    }

    private void OnNextPressed()
    {
        if (isTyping)
        {
            // 타이핑 중이면 전체 텍스트 바로 출력
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            cutText.text = cuts[currentIndex].text;
            isTyping = false;
            return;
        }

        currentIndex++;
        if (currentIndex < cuts.Count)
        {
            ShowCurrentCut();
        }
        else
        {
            SceneManager.LoadScene(nextScene);
        }
    }

}
