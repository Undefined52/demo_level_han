using System;
using System.Collections;

using UnityEngine;

using Random = UnityEngine.Random;

public class wiggle : MonoBehaviour
{
    [SerializeField] private float constantDelay = 5;
    [SerializeField] private float variableDelay = 5;

    private Animator _animator;
    
    
    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        StartCoroutine(Animate());
    }
    
    IEnumerator Animate()
    {
        while (true) {
            float delay = Random.value* variableDelay + constantDelay;
            yield return new WaitForSeconds(delay);
            
            _animator.SetTrigger("Wiggle");
        }
    }
}
