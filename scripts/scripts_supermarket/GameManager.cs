using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GameManager : MonoBehaviour{

    [SerializeField]
    private ScenarioGenerator scenarioGenerator;

    private float startTime; //momento en que se inicia la prueba

    public bool gameRunning = false; //nos indica cuando la prueba ha terminado
    
    private ChecklistManager checklistManager;

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
    private TeleportationProvider teleportationProvider;

    [SerializeField]
    private ContinuousMoveProvider continousMoveProvider;

    [SerializeField]
    private XRRayInteractor leftRayInteractor;

    [SerializeField]
    private SpawnArea[] spawnAreas; //busca todas las spawnAreas de la escena

    [SerializeField]
    private ShelfZone[] shelfZones; //busca todas las shelfzones de la escena

    private void Awake(){

        startButtonTrigger.enabled = true;

        restartButtonTrigger.enabled = false;

        finalizeButtonTrigger.enabled = false;

        lobbyButtonTrigger.enabled = true;
        
        checklistManager = FindAnyObjectByType<ChecklistManager>();

        playerFeedback = FindAnyObjectByType<PlayerFeedback>();

        spawnAreas = FindObjectsByType<SpawnArea>(); //busca todas las spawnAreas

        shelfZones = FindObjectsByType<ShelfZone>(); //busca todas las shelfzone de la escena

        if(checklistManager == null){
            
            Debug.LogError("No se ha encontrado ningún ChecklistManager en la escena");

        }

        if(playerFeedback == null){
            
            Debug.LogError("No se ha encontrado ningún PlayerFeedback en la escena");

        }

        if(scenarioGenerator == null){
            
            Debug.LogError("No se ha encontrado ningún ScenarioGenerator en la escena");

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

    public void StartGame(){ //funcion a la que llamaremos para iniciar el entorno

        gameRunning = true;

        restartButtonTrigger.enabled = true;

        finalizeButtonTrigger.enabled = true;
        
        scenarioGenerator.GenerateScenario();

        startTime = Time.time; //aqui empezamos el cronometro a contar el tiempo que tarda el jugador

        startButtonTrigger.enabled = false;

        lobbyButtonTrigger.enabled = false;

    }

    public void EndGame(){

        gameRunning = false;

        playerFeedback.PlayEndGameClip(); //sonido de finalizacion de prueba

        float accuracy = CalculateMetrics();

        float elapsedTime = Time.time - startTime; //calculamos la duracion de la prueba

        int minutes = Mathf.FloorToInt(elapsedTime / 60); //formateamos a minutos

        int seconds = Mathf.FloorToInt(elapsedTime % 60); //formateamos en segundos

        int missing = 0;

        int incorrect = 0;

        int penalizacionTotal = 0;

        foreach (ShelfZone shelfzone in shelfZones){
            
            missing += shelfzone.MissingProducts; //productos que faltan

            incorrect += shelfzone.WrongProducts; //productos mal colocados

            penalizacionTotal += shelfzone.BadPlacementPenalization; //cuantos estan mal situados en su sitio

        }

        int totalCapacity = 0;

        int correct = 0;

        foreach (SpawnArea spawnArea in spawnAreas){
            
            totalCapacity += spawnArea.Capacity; //total de objetos a colocar

        }

        correct = totalCapacity - missing; //son correctos todos aquellos que queden al quitar los missing

        Debug.Log("Penalizacion total de: " + penalizacionTotal);

        Debug.Log("numero que deberia salir: " + (accuracy - (penalizacionTotal/2f) * (1/totalCapacity)/2f) * 100);

        accuracy -= ((penalizacionTotal/2f)*(((1f/totalCapacity)/2f) * 100)); //añade todas las penalizaciones antes de mostrar el porcentaje final

        checklistManager.SetChecklistResult("Tiempo: " + minutes + " min " + seconds + " s" + "\n\n" + "Porcentaje de aciertos: " + accuracy + "%" + "\n\n" + "CORRECTOS: " + correct + "\n\n" + "INCORRECTOS: " + incorrect + "\n\n" + "PENALIZACION: " + (-((penalizacionTotal/2f)*(((1f/totalCapacity)/2f) * 100))) + "%");

        restartButtonTrigger.enabled = true;

        lobbyButtonTrigger.enabled = true;

    }

    public void RestartGame(){ //se ejecuta cuando gameRunning = false tras acabar una partida

        gameRunning = false; //volvemos al estado de revisar el supermercado

        Debug.Log("Inicio RestartGame: " + gameRunning);

        startButtonTrigger.enabled = true;

        restartButtonTrigger.enabled = false;

        finalizeButtonTrigger.enabled = false;

        lobbyButtonTrigger.enabled = true;

        foreach(SpawnArea spawnArea in spawnAreas){
            
            spawnArea.DestroySpawnedProducts(); //borramos todos los objetos instanciados por cada spawnArea y sus listas

        }

        Debug.Log("productos instanciados destruidos");

        scenarioGenerator.RestoreRemovedProducts(); //reponemos productos

        Debug.Log("Productos originales restaurados");

        checklistManager.SetText($"BOTONES\n\nBotón Rojo: Empezar prueba.\n\nBotón azul: Reintentar.\n\nBotón verde: Finalizar\n\nREPONER PRODUCTOS\n\n1. Coge el producto.\n\n2. Busca su sitio.\n\n3. Colócalo en su sitio.\n\n4. Repite los pasos."); //indicamos texto de inicio

        Debug.Log("Final RestartGame: " + gameRunning);

    }

    public void SendToLobby(){
        
        SceneManager.LoadScene("VR Lobby");

    }

    private float CalculateMetrics(){

        int totalAttempts = 0;

        int totalErrors = 0;

        foreach (SpawnArea spawnArea in spawnAreas){
            
            totalAttempts += spawnArea.Capacity; //sumamos la capacidad de cada spawnArea para saber el numero de objetos que ha colocado el usuario

        }

        foreach (ShelfZone shelfZone in shelfZones){
            
            totalErrors += shelfZone.WrongProducts;

            totalErrors += shelfZone.MissingProducts; //la suma de los productos erroneos mas los faltantes serian el numero total de errores

        }

        float accuracy = ((float)(totalAttempts - totalErrors) / totalAttempts) * 100f; //porcentaje de acierto

        return accuracy;

    }

}
