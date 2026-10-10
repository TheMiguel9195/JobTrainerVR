using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class PaperPileZone : MonoBehaviour{

    [Header("DEBUG")]
    [SerializeField]
    private bool paperLoaded = false; //por defecto no tienen papel las impresoras
    
    private PlayerFeedback playerFeedback;

    [SerializeField]
    private PrinterZone printerZone;

    private bool hasPrinted = false;

    public bool HasPrinted => hasPrinted;

    private void Awake(){
        
        playerFeedback = FindAnyObjectByType<PlayerFeedback>();

        if(playerFeedback == null){
            
            Debug.Log("No se ha encontrado ningún componente playerfeedback en la escena");

        }

    }
    private void OnTriggerEnter(Collider other){

    PaperPile paperPile = other.GetComponent<PaperPile>();

    if (paperPile == null){

        return;

    }

    paperLoaded = true;

    Debug.Log("Se ha cargado la impresora con un PaperPile: " + paperLoaded);

    StartCoroutine(blockPlacedPaperPile(paperPile));

}

    private IEnumerator blockPlacedPaperPile(PaperPile paperPile){

    while (paperPile.Grabbed == true){

        yield return null;

    }

    yield return new WaitForSeconds(3f);

    Debug.Log("Ejecutando Coroutine");

    if (paperPile != null && CheckPaperPileOrientation()){

        paperPile.SetMovable(false);

        playerFeedback.PlayCorrectClip();

        Debug.Log("Se ha ejecutado Setmovable(false) del paperpile");

        printerZone.PrintDocuments();

        Debug.Log("Se ha lanzado la impresión");

        hasPrinted = true;

    }

}

    private bool CheckPaperPileOrientation(){ //comprueba que la pila de folios este colocada correctamente
        
        float angle = Vector3.Angle(transform.up, Vector3.up);

        return angle <= 30f || angle >= 150f;

    }

    public void RestorePaperPileZone(){
        
        paperLoaded = false;

        hasPrinted = false;

    }

}
