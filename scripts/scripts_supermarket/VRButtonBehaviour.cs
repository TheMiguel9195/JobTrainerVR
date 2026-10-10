using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class VRButtonBehaviour : MonoBehaviour{
    
    [SerializeField]
    private float deadTime = 1.0f; //tiempo en el que el botón está inactivo tras soltar

    private bool deadTimeActive = false; //booleano utilizado para inhabilitar el boton en su deadtime

    [SerializeField]
    private AudioSource audioSource; //fuente del sonido asignada desde el inspector

    [SerializeField]
    private AudioClip ButtonClickAudio; //clip de sonido del boton al presionar

    public UnityEvent onPressed, onReleased; //eventos de unity para botones

    private void OnTriggerEnter(Collider other){ //detecta cuando el botón se presiona
        
        if(other.tag == "Button" && !deadTimeActive){
            
            onPressed?.Invoke();

            audioSource.PlayOneShot(ButtonClickAudio); //reproducimos el sonido

            Debug.Log("el botón ha sido presionado");

        }

    }

    private void OnTriggerExit(Collider other){ //detecta cuando el boton deja de ser presionado y lo inhabilita tanto tiempo como deadtime utilizando una corutina
        
        if(other.tag == "Button" && !deadTimeActive){
            
            onReleased?.Invoke();

            Debug.Log("El botón ha sido soltado");

            StartCoroutine(WaitForDeadTime());

        }

    }

    private IEnumerator WaitForDeadTime(){ //corutina que inhabilita el uso del boton durante el deadtime
        
        deadTimeActive = true;

        yield return new WaitForSeconds(deadTime);

        deadTimeActive = false;

    }

}
