using UnityEngine;
using UnityEngine.Events;

public class pickup_trigger : MonoBehaviour
{
    [SerializeField] private UnityEvent _onPickup;

    public void OnPickup()
    {
        _onPickup.Invoke();
    }
}
