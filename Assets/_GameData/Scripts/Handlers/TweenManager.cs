using NUnit.Compatibility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenManager : MonoBehaviour
{
    [SerializeField] TweenProfile lobby;

    private void Start()
    {
        lobby.ActivateTween();
    }
}
