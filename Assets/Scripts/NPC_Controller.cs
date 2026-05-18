using DG.Tweening;
using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    Animator anim;
    SpriteRenderer sp;
    DOTweenPath dotweenpath;
    public float stopTiming;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sp = GetComponent<SpriteRenderer>();
        dotweenpath = GetComponent<DOTweenPath>();
    }

    private void Start()
    {
        anim.SetBool("isWalk", true);
    }

    public void OnStepComplete()
    {
        dotweenpath.tween.Pause();

        sp.flipX = !sp.flipX;

        anim.SetBool("isWalk", false);

        DOVirtual.DelayedCall(stopTiming, () =>
        {
            anim.SetBool("isWalk", true);
            dotweenpath.tween.Play();
        });
    }


}
