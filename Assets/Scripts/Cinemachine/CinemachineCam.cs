using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCam : MonoBehaviour
{
    CinemachineCamera vcam;

    private void Awake()
    {
        vcam = GetComponent<CinemachineCamera>();
    }

    private void Start()
    {
        FindCine();
    }

    void FindCine()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            vcam.Follow = player.transform;
            vcam.LookAt = player.transform;
        }
    }
}
