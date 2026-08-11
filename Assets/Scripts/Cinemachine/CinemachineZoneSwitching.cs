using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CinemachineZoneSwitching : MonoBehaviour
{
    [SerializeField] CinemachineCamera vcam;
    [SerializeField] CinemachineConfiner2D confiner;
    [SerializeField] Collider2D targetBoundingShape;
    [SerializeField] float transitionDuration = 0.6f;

    private Coroutine routine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(SwitchZone());
    }

    private IEnumerator SwitchZone()
    {
        Vector3 start = vcam.State.RawPosition;

        confiner.BoundingShape2D = targetBoundingShape;
        confiner.InvalidateBoundingShapeCache();

        yield return null; // 한 프레임 대기 후 클램프된 목표 위치 확인
        Vector3 target = vcam.State.RawPosition;

        float t = 0f;
        while (t < transitionDuration)
        {
            t += Time.deltaTime;
            float lerp = t / transitionDuration;
            vcam.transform.position = Vector3.Lerp(start, target, lerp);
            yield return null;
        }
    }
}