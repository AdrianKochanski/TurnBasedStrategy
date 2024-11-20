using System;
using UnityEngine;

public class UnitAnimatorEvent : MonoBehaviour
{
    public event Action OnThrow;

    public void Throw()
    {
        OnThrow?.Invoke();
    }
}
