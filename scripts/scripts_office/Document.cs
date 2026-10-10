using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;


public class Document : MonoBehaviour{

    [SerializeField]
    private TextMeshProUGUI destinationText; //nombre del departamento destino del documento

    public bool correctlyPlaced = false; //se volverá true al colocarse correctamente (al final de la prueba se comprobara cuantos de los instanciados son correctos o no y entonces se generaran las estadisticas)

    [SerializeField]
    private string destination;

    [SerializeField]
    private string desk;

    private bool grabbed;

    private XRGrabInteractable grabInteractable;

    public string GetDepartment => destination;

    public string GetDesk => desk;

    public bool Grabbed => grabbed;

    private void Awake(){ // al crearse un documento se le asigna aleatoriamente un destino
        
        SetRandomDestination();

        grabbed = false;

        grabInteractable = GetComponent<XRGrabInteractable>();

        if(grabInteractable == null){
            
            Debug.LogError("No se encontró ningún componente grabInteractable en el documento");

        }

        SetMovable(true);

    }

    public enum Departamentos{ //indica el departamento del documento
        
        RecursosHumanos,
        Contabilidad,
        Administración,
        AtenciónAlCliente

    }

    public enum Desks{ //indica en que mesa va el documento
        
        A,
        B,
        C,
        D

    }

    private void SetRandomDestination(){ //funcion que asigna de forma aleatoria departamento destino para el documento
        
        Departamentos[] departamentos = (Departamentos[])System.Enum.GetValues(typeof(Departamentos));

        Desks[] desks = (Desks[])System.Enum.GetValues(typeof(Desks));

        Departamentos randomDepartment = departamentos[Random.Range(0, departamentos.Length)]; //departamento aleatorio

        Desks randomDesk = desks[Random.Range(0, desks.Length)]; //desk aleatorio

        destination = randomDepartment.ToString();

        desk = randomDesk.ToString();

        if(destination == "RecursosHumanos"){
            
            destination = "Recursos Humanos";

        }

        if(destination == "AtenciónAlCliente"){
            
            destination = "Atención al Cliente";

        }

        destinationText.text = "Departamento: " + destination + "\n" + "Mesa: " + desk;

    }

    public void SetMovable(bool movable){
        
        Rigidbody rb = GetComponentInParent<Rigidbody>();

        rb.isKinematic = !movable;

        grabInteractable.enabled = movable;

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

        Debug.Log("Document agarrado");

        grabbed = true;

    }

    private void OnReleased(SelectExitEventArgs args){
        
        Debug.Log("Document soltado");

        grabbed = false;

    }

}
