using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class MultiTweenProfile : MonoBehaviour
{
    #region Serialized Variables & Structs

    [Serializable]
    public struct TweenElement
    {
        [TabGroup("Callbacks")]
        public string id;
        [TabGroup("Transforms")]
        public GameObject tweenObj;
        [TabGroup("Transforms")]
        public RectTransform ActiveTweenTransform;
        [TabGroup("Transforms")]
        public RectTransform InactiveTweenTransform;
        [TabGroup("Callbacks")]
        public UnityEvent OnObjInteraction;
    }

    [TabGroup("Tab1", "Tween Times")]
    [PropertyTooltip("The time it takes for each element to complete its movement aspect of the tween.")]
    [SerializeField] float moveTweenTime = 0f;
    [TabGroup("Tab1", "Tween Times")]
    [PropertyTooltip("The time it takes for each element to complete its rotation aspect of the tween.")]
    [SerializeField] float rotationTweenTime = 0f;
    [TabGroup("Tab1", "Tween Times")]
    [PropertyTooltip("The time it takes for each element to complete its scale aspect of the tween.")]
    [SerializeField] float scaleTweenTime = 0f;

    [TabGroup("Tab1", "Tween Elements")]
    [PropertyTooltip("A list of all the elements that will be tweened together as part of a group.")]
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "id")]
    [SerializeField] List<TweenElement> tweenElements = new();

    #endregion

    #region Public Functions

    public async void ActiveTweenLoop()
    {
        foreach (TweenElement tweenElement in tweenElements)
        {
            await ActiveTween(tweenElement);
        }
    }

    public async void InactiveTweenLoop(string tweenObjID)
    {
        Stack<TweenElement> stack = new Stack<TweenElement>(tweenElements);
        foreach (TweenElement tweenElement in stack)
        {
            await InactiveTween(tweenElement);
        }

        foreach (TweenElement tweenElement in tweenElements)
        {
            if (tweenElement.id == tweenObjID)
            {
                tweenElement.OnObjInteraction.Invoke();
            }
        }
    }

    #endregion

    #region Private Functions

    #region Activation Tween Functions

    async Task ActiveTween(TweenElement tweenElement)
    {
        Transform transform = tweenElement.tweenObj.transform;
        Vector3 position = tweenElement.ActiveTweenTransform.position;
        Quaternion rotation = tweenElement.ActiveTweenTransform.rotation;
        Vector3 scale = tweenElement.ActiveTweenTransform.localScale;

        if (tweenElement.ActiveTweenTransform.position == tweenElement.InactiveTweenTransform.position)
        {
            await ActiveTweenNoLocation(tweenElement);
            return;
        }

        await CentreForActivation(tweenElement);
        tweenElement.tweenObj.SetActive(true);

        Sequence tweenSequence = DOTween.Sequence();

        tweenSequence.Join(transform.DOMove(position, moveTweenTime))
        .Join(transform.DORotateQuaternion(rotation, rotationTweenTime))
        .Join(transform.DOScale(scale, scaleTweenTime));

        await tweenSequence.AsyncWaitForCompletion();
    }

    async Task ActiveTweenNoLocation(TweenElement tweenElement)
    {
        Transform transform = tweenElement.tweenObj.transform;
        Quaternion rotation = tweenElement.ActiveTweenTransform.rotation;
        Vector3 scale = tweenElement.ActiveTweenTransform.localScale;

        await CentreForActivation(tweenElement);
        tweenElement.tweenObj.SetActive(true);

        Sequence tweenSequence = DOTween.Sequence();

        tweenSequence.Join(transform.DORotateQuaternion(rotation, rotationTweenTime))
        .Join(transform.DOScale(scale, scaleTweenTime));

        await tweenSequence.AsyncWaitForCompletion();
    }

    async Task CentreForActivation(TweenElement tweenElement)
    {
        Transform transform = tweenElement.tweenObj.transform;
        Vector3 position = tweenElement.InactiveTweenTransform.position;
        Quaternion rotation = tweenElement.InactiveTweenTransform.rotation;
        Vector3 scale = tweenElement.InactiveTweenTransform.localScale;

        Sequence readyingSeq = DOTween.Sequence();

        readyingSeq.Join(transform.DOMove(position, 0f))
        .Join(transform.DORotateQuaternion(rotation, 0f))
        .Join(transform.DOScale(scale, 0f));

        await readyingSeq.AsyncWaitForCompletion();
    }

    #endregion

    #region Deactivation Tween Functions

    async Task InactiveTween(TweenElement tweenElement)
    {
        Transform transform = tweenElement.tweenObj.transform;
        Vector3 position = tweenElement.InactiveTweenTransform.position;
        Quaternion rotation = tweenElement.InactiveTweenTransform.rotation;
        Vector3 scale = tweenElement.InactiveTweenTransform.localScale;

        if (tweenElement.ActiveTweenTransform.position == tweenElement.InactiveTweenTransform.position)
        {
            await InactiveTweenNoLocation(tweenElement);
            return;
        }

        await CentreForInactivation(tweenElement);

        Sequence tweenSequence = DOTween.Sequence();

        tweenSequence.Join(transform.DOMove(position, moveTweenTime))
        .Join(transform.DORotateQuaternion(rotation, rotationTweenTime))
        .Join(transform.DOScale(scale, scaleTweenTime));

        await tweenSequence.AsyncWaitForCompletion();

        tweenElement.tweenObj.SetActive(false);
    }

    async Task InactiveTweenNoLocation(TweenElement tweenElement)
    {
        Transform transform = tweenElement.tweenObj.transform;
        Quaternion rotation = tweenElement.InactiveTweenTransform.rotation;
        Vector3 scale = tweenElement.InactiveTweenTransform.localScale;

        await CentreForInactivation(tweenElement);

        Sequence tweenSequence = DOTween.Sequence();

        tweenSequence.Join(transform.DORotateQuaternion(rotation, rotationTweenTime))
        .Join(transform.DOScale(scale, scaleTweenTime));

        await tweenSequence.AsyncWaitForCompletion();

        tweenElement.tweenObj.SetActive(false);
    }

    async Task CentreForInactivation(TweenElement tweenElement)
    {
        Transform transform = tweenElement.tweenObj.transform;
        Vector3 position = tweenElement.ActiveTweenTransform.position;
        Quaternion rotation = tweenElement.ActiveTweenTransform.rotation;
        Vector3 scale = tweenElement.ActiveTweenTransform.localScale;

        Sequence readyingSeq = DOTween.Sequence();

        readyingSeq.Join(transform.DOMove(position, 0f))
        .Join(transform.DORotateQuaternion(rotation, 0f))
        .Join(transform.DOScale(scale, 0f));

        await readyingSeq.AsyncWaitForCompletion();
    }

    #endregion

    #endregion
}
