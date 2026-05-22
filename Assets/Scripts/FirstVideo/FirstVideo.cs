using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class FirstVideo : MonoBehaviour
{
    [SerializeField] VideoPlayer vp;
    [SerializeField] RawImage fadeImg;
    [SerializeField] float fadeDuration;

    private void OnEnable()
    {
        vp = GetComponent<VideoPlayer>();
        vp.loopPointReached += OnVideoFinished;
    }

    private void OnDisable()
    {
        vp.loopPointReached -= OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer player)
    {
        StartCoroutine(FadeInLoad());
    }

    IEnumerator FadeInLoad()
    {
        float elapsed = 0f;
        Color color = fadeImg.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImg.color = color;
            yield return null;
        }

        SceneManager.LoadScene("Intro");
    }
}

