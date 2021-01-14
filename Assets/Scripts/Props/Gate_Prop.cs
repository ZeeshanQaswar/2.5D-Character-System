using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Gate_Prop : MonoBehaviour, IInteractable
{
    private DOTweenAnimation _doTweenAnim;


    public void Interaction()
    {
        if (gameObject.TryGetComponent<DOTweenAnimation>(out _doTweenAnim))
        {
            _doTweenAnim.DOPlay();
        }
    }
}
