using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneFadeIn : MonoBehaviour
{
    [Header("페이드 설정")]
    [SerializeField] float fadeOutDuration = 0.3f; 
    [SerializeField] float maxWaitTime = 2f;        

    Image overlayImage;

    void Awake()
    {
        CreateOverlay();
    }

    void CreateOverlay()
    {
        GameObject canvasGO = new GameObject("FadeInCanvas");
        canvasGO.transform.SetParent(transform, false);

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; 

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject imgGO = new GameObject("FadeOverlay");
        imgGO.transform.SetParent(canvasGO.transform, false);

        overlayImage = imgGO.AddComponent<Image>();
        overlayImage.color = Color.black;

        RectTransform rt = overlayImage.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        overlayImage.raycastTarget = true;
    }

    IEnumerator Start()
    {
        float timer = 0f;
        GameObject player = GameObject.FindWithTag("Player");

        while (player == null && timer < maxWaitTime)
        {
            yield return null;
            timer += Time.deltaTime;
            player = GameObject.FindWithTag("Player");
        }

        yield return null;
        yield return null;

        yield return StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float t = 0f;
        Color c = overlayImage.color;

        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
            overlayImage.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        overlayImage.color = new Color(c.r, c.g, c.b, 0f);
        overlayImage.raycastTarget = false; 
        Destroy(overlayImage.transform.parent.gameObject); 
    }
}