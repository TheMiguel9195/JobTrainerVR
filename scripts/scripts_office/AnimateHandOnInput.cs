using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour{

    public InputActionProperty triggerValue;
    public InputActionProperty gripValue;

    public Animator handAnimator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        float trigger = triggerValue.action.ReadValue<float>(); //leemos si se pulsa el trigger del mando

        float grip = gripValue.action.ReadValue<float>(); //leemos si se pulsa el grip del mando

        handAnimator.SetFloat("Trigger", trigger);
        handAnimator.SetFloat("Grip", grip);

    }
}
