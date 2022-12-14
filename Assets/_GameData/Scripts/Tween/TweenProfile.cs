using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Sirenix.OdinInspector;

public class TweenProfile : MonoBehaviour
{
    #region Serialized Variables
    [TabGroup("Tab1", "Transforms")]
    [HorizontalGroup("Tab1/Transforms/Hoz1", LabelWidth = 105)]
    [PropertyTooltip("RectTransform of the Position, Rotation and Scale you wish the object to be in when the tween is complete.")]
    [SerializeField] RectTransform activationRect;
    [HorizontalGroup("Tab1/Transforms/Hoz1")]
    [PropertyTooltip("ReactTransform of the Position, Rotation and Scale you wish the object to return to when the tween is reversed.")]
    [SerializeField] RectTransform deactivationRect;

    [PropertySpace(10)]
    [TabGroup("Tab1", "Transforms")]
    [PropertyTooltip("The time it takes for the movement aspect of the tween to complete.")]
    [SerializeField] float moveTweenTime = 0f;
    [TabGroup("Tab1", "Transforms")]
    [PropertyTooltip("The time it takes for the rotation aspect of the tween to complete.")]
    [SerializeField] float rotationTweenTime = 0f;
    [TabGroup("Tab1", "Transforms")]
    [Tooltip("The time it takes for the scale aspect of the tween to complete.")]
    [SerializeField] float scaleTweenTime = 0f;

    [TabGroup("Tab1", "Completeion Events")]
    [HorizontalGroup("Tab1/Completeion Events/Hoz1")]
    [PropertyTooltip("Bool for if you want events to be called after the activation tween has completed.")]
    [SerializeField] bool OnCompleteActive = false;
    [HorizontalGroup("Tab1/Completeion Events/Hoz1")]
    [PropertyTooltip("Bool for if you want events to be called after the deactivation tween has completed.")]
    [SerializeField] bool OnCompleteInactive = false;

    [TabGroup("Tab1", "Completeion Events")]
    [HideIf("@!OnCompleteActive")]
    [PropertyTooltip("Events to be called when the activation tween has completed.")]
    [SerializeField] UnityEvent OnCompleteActiveEvents;
    [TabGroup("Tab1", "Completeion Events")]
    [HideIf("@!OnCompleteInactive")]
    [PropertyTooltip("Events to be called when the deactivation tween is completed.")]
    [SerializeField] UnityEvent OnCompleteInactiveEvents;

    #endregion

    #region Insecptor Button Functions

    [PropertySpace(20)]
    [HorizontalGroup("Tab1/Transforms/Hoz2")]
    [Button(ButtonSizes.Medium)]
    private void TestActivationTween()
    {
        ActivateTween();
    }

    [PropertySpace(20)]
    [HorizontalGroup("Tab1/Transforms/Hoz2")]
    [Button(ButtonSizes.Medium)]
    private void TestDeactivationTween()
    {
        DeactivateTween();
    }

    #endregion

    #region Private Variables

    Vector3 ActiveTweenPosition;
    Vector3 NonactiveTweenPosition;

    Quaternion ActiveTweenRotation;
    Quaternion NonactiveTweenRotation;

    Vector3 ActiveTweenScale;
    Vector3 NonactiveTweenScale;

    #endregion

    #region Life Cycle Functions

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    #endregion

    #region Public Functions

    public async void ActivateTween()
    {
        await Setup();

        if (ActiveTweenPosition == NonactiveTweenPosition)
        {
            await ActiveTweenNoLocation();
            return;
        }

        await CentreForActivation();
        gameObject.SetActive(true);

        Sequence tweenSequence = DOTween.Sequence();

        tweenSequence.Join(transform.DOMove(ActiveTweenPosition, moveTweenTime))
        .Join(transform.DORotateQuaternion(ActiveTweenRotation, rotationTweenTime))
        .Join(transform.DOScale(ActiveTweenScale, scaleTweenTime))
        .OnComplete(() => { if (OnCompleteActive) OnCompleteActiveEvents.Invoke(); });

        await Task.Yield();
    }

    public async void DeactivateTween()
    {
        await Setup();

        if (ActiveTweenPosition == NonactiveTweenPosition)
        {
            await UnactiveTweenNoLocation();
            return;
        }

        await CentreForInactivation();

        Sequence tweenSequence = DOTween.Sequence();

        tweenSequence.Join(transform.DOMove(NonactiveTweenPosition, moveTweenTime))
        .Join(transform.DORotateQuaternion(NonactiveTweenRotation, rotationTweenTime))
        .Join(transform.DOScale(NonactiveTweenScale, scaleTweenTime))
        .OnComplete(() => { if (OnCompleteInactive) OnCompleteInactiveEvents.Invoke(); });

        await tweenSequence.AsyncWaitForCompletion();

        gameObject.SetActive(false);

        await Task.Yield();
    }

    #endregion

    #region Private Functions

    #region Active Tween Functions

    async Task ActiveTweenNoLocation()
    {
        await CentreForActivation();
        gameObject.SetActive(true);

        Sequence tweenSequence = DOTween.Sequence();

        tweenSequence.Join(transform.DORotateQuaternion(ActiveTweenRotation, rotationTweenTime))
        .Join(transform.DOScale(ActiveTweenScale, scaleTweenTime))
        .OnComplete(() => { if (OnCompleteActive) OnCompleteActiveEvents.Invoke(); });

        await Task.Yield();
    }

    async Task CentreForActivation()
    {
        Sequence readyingSeq = DOTween.Sequence();

        readyingSeq.Join(transform.DOMove(NonactiveTweenPosition, 0f))
        .Join(transform.DORotateQuaternion(NonactiveTweenRotation, 0f))
        .Join(transform.DOScale(NonactiveTweenScale, 0f));

        await readyingSeq.AsyncWaitForCompletion();
    }

    #endregion

    #region Unactive Tween Functions

    async Task UnactiveTweenNoLocation()
    {
        await CentreForInactivation();

        Sequence tweenSequence = DOTween.Sequence();

        tweenSequence.Join(transform.DORotateQuaternion(NonactiveTweenRotation, rotationTweenTime))
        .Join(transform.DOScale(NonactiveTweenScale, scaleTweenTime))
        .OnComplete(() => { if (OnCompleteInactive) OnCompleteInactiveEvents.Invoke(); });

        await tweenSequence.AsyncWaitForCompletion();

        gameObject.SetActive(false);

        await Task.Yield();
    }

    async Task CentreForInactivation()
    {
        Sequence readyingSeq = DOTween.Sequence();

        readyingSeq.Join(transform.DOMove(ActiveTweenPosition, 0f))
        .Join(transform.DORotateQuaternion(ActiveTweenRotation, 0f))
        .Join(transform.DOScale(ActiveTweenScale, 0f));

        await readyingSeq.AsyncWaitForCompletion();
    }

    #endregion

    private async Task Setup()
    {
        ActiveTweenPosition = activationRect.position;
        NonactiveTweenPosition = deactivationRect.position;

        ActiveTweenRotation = activationRect.rotation;
        NonactiveTweenRotation = deactivationRect.rotation;

        ActiveTweenScale = activationRect.localScale;
        NonactiveTweenScale = deactivationRect.localScale;

        Debug.Log($"Awake variable setting called for {gameObject.name}");

        await Task.Yield();
    }

    #endregion
}
