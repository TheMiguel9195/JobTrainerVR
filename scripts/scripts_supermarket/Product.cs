using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;


public class Product : MonoBehaviour{

    public string productID; //aqui almacenaremos el identificador del producto/item del supermercado para que mas adelante lo puedan detectar las estanterias

    public bool basketable = false;

    private XRGrabInteractable grabInteractable;

    private Vector3 initialPosition; //posicion para restaurar objetos perdidos
    private Quaternion initialRotation; //posicion para restaurar objetos perdidos

    [SerializeField]
    private bool penalizeBadPlacement = true;

    public bool PenalizeBadPlacement => penalizeBadPlacement;

    private void Awake(){
        
        SetMovable(false); //al comenzar la escena todos los productos ignoran fisicas y XR grab interactable

        grabInteractable = GetComponent<XRGrabInteractable>();

        initialPosition = transform.position;

        initialRotation = transform.rotation;

    }

    private void Update(){

        if(transform.position.y <= -50f){ //en este caso el producto ha caido del escenario de alguna forma y vamos a restaurarlo al almacen
            
            RestoreObjectFromLimbo();

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
        
        Debug.Log("Producto agarrado");

        basketable = false;

        if(transform.parent != null && transform.parent.name == "inside_products"){
            
            transform.SetParent(null ,true);

            Debug.Log("Producto agarrado");

        }

    }

    private void OnReleased(SelectExitEventArgs args){
        
        Debug.Log("Producto soltado");

        basketable = true;

    }

    public void SetMovable(bool movable){ //nos permitira desactivar la fisica de los productos cuando sea necesario
        
        Rigidbody rb = GetComponent<Rigidbody>();

        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        if(rb != null){
            
            rb.isKinematic = !movable;

        }

        if(grabInteractable != null){
            
            grabInteractable.enabled = movable;

        }

        if(movable){

            basketable = true;

        }else{

            basketable = false;
            
        }

    }

    private void RestoreObjectFromLimbo(){

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null){ //borramos el momentun alcanzado por la caida del producto al vacio

            rb.linearVelocity = Vector3.zero;

            rb.angularVelocity = Vector3.zero;

        }

        this.transform.SetParent(null, true);
        
        transform.position = initialPosition;

        transform.rotation = initialRotation;

        Debug.Log(this.productID + "restaurado correctamente");

    }

    public bool CheckYaxys(){
        
        float angle = Vector3.Angle(transform.up, Vector3.up);

        Debug.Log("El angulo del producto recien colocado es: " + angle + " por tanto la funcion devuelve " + (angle<=30f));

        return angle <= 30f; //tenemos tolerancia de hasta 30º de inclinacion del eje Y para hacer que los productos que esten tumbados penalicen al no estar bien colocados

    }

}
