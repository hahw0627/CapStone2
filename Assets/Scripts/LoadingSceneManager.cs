using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene = "DevScene";

    [SerializeField] private Slider progressSlider;
    [SerializeField] private Text touchText; // 일반 Text일 경우 UnityEngine.UI.Text
    private bool isLoadComplete = false;
    private float blinkTimer = 0f;

    private void Start()
    {
        touchText.gameObject.SetActive(false);
        StartCoroutine(LoadScene());
        AudioManager.Instance.PlayBGM(AudioManager.Instance.lobbyBGM);
    }

    private void Update()
    {
        if (isLoadComplete)
        {
            // 텍스트 깜빡임
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= 0.5f)
            {
                touchText.gameObject.SetActive(!touchText.gameObject.activeSelf);
                blinkTimer = 0f;
            }

            // 클릭 시 씬 전환
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                SceneManager.LoadScene(nextScene);
            }
        }
    }

    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0.0f;

        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;

            if (op.progress < 0.9f)
            {
                progressSlider.value = Mathf.Lerp(progressSlider.value, op.progress, timer);
                if (progressSlider.value >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                progressSlider.value = Mathf.Lerp(progressSlider.value, 1f, timer);
                if (progressSlider.value >= 0.99f)
                {
                    progressSlider.gameObject.SetActive(false);
                    touchText.gameObject.SetActive(true);
                    isLoadComplete = true;
                    yield break; // 코루틴 종료 → Update에서 터치 대기
                }
            }
        }
    }
}
