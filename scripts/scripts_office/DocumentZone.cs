using UnityEngine;
using System.Collections;

public class DocumentZone : MonoBehaviour{

    [SerializeField]
    private string desk; //escritorio

    [SerializeField]
    private string department; //departamento

    private PlayerFeedback playerFeedback;

    private OfficeGameManager officeGameManager;

    private void Awake(){
        
        playerFeedback = FindAnyObjectByType<PlayerFeedback>();

        officeGameManager = FindAnyObjectByType<OfficeGameManager>();

        if(playerFeedback == null){
            
            Debug.LogError("No se encontró ningún componente playerfeedback en el escenario");

        }

        if(officeGameManager == null){
            
            Debug.LogError("No se encontró ningún componente OfficeGameManager en la escena");

        }

    }

    private void OnTriggerEnter(Collider other){ //detecta la entrada de document y si es el correcto reproduce sonido OK y bloquea el documento

        Document document = other.GetComponentInParent<Document>();

        Debug.Log("documento ha entrado en documentZone");

        if(document == null){

            Debug.Log("document null por alguna razon");
            
            return;

        }

        StartCoroutine(CheckDocument(document));

    }

    private IEnumerator CheckDocument(Document document){

        Debug.Log("Corutina de colocacion de documento lanzada");

        while (document.Grabbed == true){

            yield return null;

        }

        yield return new WaitForSeconds(3f);
        
        bool correctDepartment = document.GetDepartment == department;

        bool correctDesk = document.GetDesk == desk;

        if(correctDepartment && correctDesk){
            
            document.correctlyPlaced = true; //se marca como correcto

            Debug.Log("Documento Colocado correctamente");

            playerFeedback.PlayCorrectClip();

            document.SetMovable(false);

            officeGameManager.UpdateBoard(); //actualiza el progreso en la whiteboard del escenario

            officeGameManager.CheckGameEnd(); //comprueba si tras colocar correctamente un documento ya estan todos bien colocados

        }
        else{
            
            Debug.Log("Documento Colocado incorrectamente");

            playerFeedback.PlayWrongClip();

        }

    }
    
}