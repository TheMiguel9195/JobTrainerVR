using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class OfficeGameManager : MonoBehaviour{

    private float startTime;

    public bool gameRunning = false;

    private PaperPile[] paperPiles;

    private Document[] documents;

    private PaperPileZone[] paperPileZones;

    private PrinterZone[] printerZones;

    private PlayerFeedback playerFeedback;

    [SerializeField]
    private Collider startButtonTrigger;

    [SerializeField]
    private Collider restartButtonTrigger;

    [SerializeField]
    private Collider finalizeButtonTrigger;

    [SerializeField]
    private Collider lobbyButtonTrigger;

    [SerializeField]
    private TextMeshProUGUI boardText; //texto pizarra tutorial

    [SerializeField]
    private TeleportationProvider teleportationProvider;

    [SerializeField]
    private ContinuousMoveProvider continousMoveProvider;

    [SerializeField]
    private XRRayInteractor leftRayInteractor;

    private void Awake(){
        
        startButtonTrigger.enabled = true;

        restartButtonTrigger.enabled = false;

        finalizeButtonTrigger.enabled = false;

        lobbyButtonTrigger.enabled = true;

        paperPiles = FindObjectsByType<PaperPile>(); //encuentra todos los paperpile para poder luego restaurarlos a su sitio al reiniciar

        playerFeedback = FindAnyObjectByType<PlayerFeedback>();

        paperPileZones = FindObjectsByType<PaperPileZone>();

        printerZones = FindObjectsByType<PrinterZone>();

        if(playerFeedback == null){
            
            Debug.LogError("No se ha encontrado ningún componente playerfeedback en la escena");

        }

        if(paperPiles == null){
            
            Debug.LogError("No se ha encontrado ningún paperpile en la escena");

        }

        if(paperPileZones == null){
            
            Debug.LogError("No se ha encontrado ningún paperpilezone en la escena");

        }

        if(printerZones == null){
            
            Debug.LogError("No se ha encontrado ningún printerZone en la escena");

        }

        if(LobbyManager.useTeleportation){
            
            teleportationProvider.enabled = true;

            continousMoveProvider.enabled = false;

            leftRayInteractor.enabled = true;

        }
        else{
            
            teleportationProvider.enabled = false;

            continousMoveProvider.enabled = true;

            leftRayInteractor.enabled = false;

        }

    }

    public void StartGame(){
        
        gameRunning = true;

        Debug.Log("Inicio StartGame: " + gameRunning);

        restartButtonTrigger.enabled = true;

        finalizeButtonTrigger.enabled = true;

        lobbyButtonTrigger.enabled = false;

        startTime = Time.time; //aqui empezamos el cronometro a contar el tiempo que tarda el jugador

        startButtonTrigger.enabled = false;

        foreach (PaperPile paperpile in paperPiles){ //hacemos que los paperpiles sean interactuables
            
            paperpile.SetMovable(true);

        }

        //mostrar documentos clasificados en la pizarra (al inicio deberian ser 0)

        int numero_impresoras = printerZones.Length; //numero de impresoras

        int numero_documentos_generados = 0;

        foreach (PrinterZone printerZone in printerZones){
            
            numero_documentos_generados += printerZone.DocumentsQuantity; //va sumando los documentos que genera cada impresora del escenario para saber el total posible

        }

        boardText.text = "";

        boardText.text = "Auxiliar de Oficina." + "\n\n" + "Paso 1: Recarga las impresoras." + "\n\n" + "Paso 2: Recoge los documentos." + "\n\n" + "Paso 3: Coloca los documentos." + "\n\n" + "Paso 4: Repite."+ "\n\n" + "Documentos colocados: " + "0/" + numero_documentos_generados;

    }

    public void RestartGame(){
        
        gameRunning = false; //volvemos al estado de revisar el supermercado

        Debug.Log("Inicio RestartGame: " + gameRunning);

        startButtonTrigger.enabled = true;

        restartButtonTrigger.enabled = false;

        finalizeButtonTrigger.enabled = false;

        lobbyButtonTrigger.enabled = true;

        foreach(PaperPile paperpile in paperPiles){ //devolvemos cada paperpile a su sitio inicial
            
            paperpile.RestorePaperPile();

            paperpile.SetMovable(false);

        }

        if(!(documents == null)){
            
            foreach (Document document in documents){
            
                Destroy(document.gameObject);

            }

        }

        foreach(PaperPileZone paperpilezone in paperPileZones){

            paperpilezone.RestorePaperPileZone();

        }

        boardText.text = "Auxiliar de Oficina." + "\n\n" + "Paso 1: Recarga las impresoras." + "\n\n" + "Paso 2: Recoge los documentos." + "\n\n" + "Paso 3: Coloca los documentos." + "\n\n" + "Paso 4: Repite.";

    }

    public void EndGame(){
        
        gameRunning = false;

        Debug.Log("Inicio EndGame: " + gameRunning);

        playerFeedback.PlayEndGameClip(); //sonido de finalizacion de prueba

        float elapsedTime = Time.time - startTime; //calculamos la duracion de la prueba

        int minutes = Mathf.FloorToInt(elapsedTime / 60); //formateamos a minutos

        int seconds = Mathf.FloorToInt(elapsedTime % 60); //formateamos en segundos

        boardText.text = "Tiempo: " + minutes + "min " + seconds + "s\n\n";

        //GENERAR METRICAS DE COMO HA SALIDO TODO

        int documentosCorrectos = 0;

        int documentos_totales = 0;

        foreach (PrinterZone printerZone in printerZones){
            
            documentos_totales += printerZone.DocumentsQuantity; // se calculan los totales que deben ser clasificados

        }

        documents = FindObjectsByType<Document>(); //los buscamos aqui y no en Awake ya que al inicio de la escena no existen aun

        if(!(documents == null)){ //si existen documentos en la escena entonces empezamos a valorar
            
            foreach (Document document in documents){

                if(document.correctlyPlaced == true){
                    
                    documentosCorrectos++;

                }

            }

        }

        int incorrectos = documentos_totales - documentosCorrectos; //incorrectos

        float accuracy = (float)documentosCorrectos / (float)documentos_totales;

        accuracy = Mathf.Round(accuracy * 10000f) / 100f;

        boardText.text += "Porcentaje de aciertos: " + accuracy + "%\n\n" + "CORRECTOS: " + documentosCorrectos + "\n\n" + "INCORRECTOS: " + incorrectos;

    }

    public void CheckGameEnd(){

        PaperPileZone[] paperpileZones = FindObjectsByType<PaperPileZone>();

        foreach (PaperPileZone paperPileZone in paperpileZones){

            if(!paperPileZone.HasPrinted){ //se comprueba que las 4 impresoras hayan imprimido sus documentos
                
                return;

            }

        }
        
        Document[] documents = FindObjectsByType<Document>();

        foreach(Document document in documents){

            if(!document.correctlyPlaced){ //se comprueba que cada documento impreso ha sido colocado correctamente
                
                return;

            }

        }

        EndGame(); //si todos salen true se dispara el final de partida

    }

    public void UpdateBoard(){
        
        int documentosCorrectos = 0;

        int documentos_totales = 0;

        foreach (PrinterZone printerZone in printerZones){
            
            documentos_totales += printerZone.DocumentsQuantity; // se calculan los totales que deben ser clasificados

        }

        documents = FindObjectsByType<Document>(); //los buscamos aqui y no en Awake ya que al inicio de la escena no existen aun

        if(!(documents == null)){ //deberia calcular los nuevos documentos de la escena cada vez que se ejecuta
            
            foreach (Document document in documents){

                if(document.correctlyPlaced == true){ //siempre se quedara con los nuevos colocados aumentando el progreso
                    
                    documentosCorrectos++;

                }

            }

        }

        boardText.text = "";

        boardText.text = "Auxiliar de Oficina." + "\n\n" + "Paso 1: Recarga las impresoras." + "\n\n" + "Paso 2: Recoge los documentos." + "\n\n" + "Paso 3: Coloca los documentos." + "\n\n" + "Paso 4: Repite." + "\n\n"+ "Documentos colocados: " + documentosCorrectos + "/" + documentos_totales;

    }

    public void SendToLobby(){
        
        SceneManager.LoadScene("VR Lobby");

    }

}
