using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PaperPile : MonoBehaviour{ //clase que representa el objeto PaperPile
    
    private XRGrabInteractable grabInteractable;

    private Rigidbody rb;

    private bool grabbed = false;

    private Vector3 initialPosition;

    private Quaternion initialRotation;

    public bool Grabbed => grabbed;

    private void Awake(){
        
        grabInteractable = GetComponent<XRGrabInteractable>();

        rb = GetComponent<Rigidbody>();

        initialPosition = transform.position; //guardamos la posicion inicial del paperpile

        initialRotation = transform.rotation; //guardamos rotacion inicial del paperpile

        SetMovable(false); //por defecto no se podran interactuar hasta que empiece la partida

        if(grabInteractable == null){
            
            Debug.LogError("No se ha encontrado componente XRGrabInteractable en el paperpile");

        }

        if(rb == null){
            
            Debug.LogError("No se ha encontrado componente Rigidbody en el paperpile");

        }

    }

    private void OnEnable(){

        if(grabInteractable != null){
            
            grabInteractable.selectEntered.AddListener(OnGrabbed);

            grabInteractable.selectExited.AddListener(OnReleased);

        }

    }

    private void OnDisable(){

        if(grabInteractable != null){
            
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);

            grabInteractable.selectExited.RemoveListener(OnReleased);

        }

    }

    private void OnGrabbed(SelectEnterEventArgs args){

        Debug.Log("PaperPile agarrado");

        grabbed = true;

    }

    private void OnReleased(SelectExitEventArgs args){
        
        Debug.Log("PaperPile soltado");

        grabbed = false;

    }

    public void SetMovable(bool movable){
        
        Rigidbody rb = GetComponent<Rigidbody>();

        if(rb != null){
            
            rb.isKinematic = !movable;

        }

        if(grabInteractable != null){
            
            grabInteractable.enabled = movable;

        }

    }

    public void RestorePaperPile(){

        if(rb != null){
            
            rb.linearVelocity = Vector3.zero;

            rb.angularVelocity = Vector3.zero;

        }
        
        transform.position = initialPosition;

        transform.rotation = initialRotation;

        SetMovable(true);

    }

}
