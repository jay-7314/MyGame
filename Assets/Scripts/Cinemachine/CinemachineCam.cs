using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class CinemachineCam : MonoBehaviour
{
    CinemachineCamera vcam;

    private void Awake()
    {
        vcam = GetComponent<CinemachineCamera>();
    }

    private void Start()
    {
        StartCoroutine(WaitForPlayerAndFollow());
    }

    IEnumerator WaitForPlayerAndFollow()
    {
        GameObject player = null;

        // Player가 생성될 때까지 매 프레임 확인
        while (player == null)
        {
            player = GameObject.FindWithTag("Player");
            if (player == null)
                yield return null;   // 한 프레임 대기 후 다시 시도
        }

        vcam.Follow = player.transform;
        vcam.LookAt = player.transform;
        vcam.OnTargetObjectWarped(player.transform, player.transform.position - vcam.transform.position);
    }
}