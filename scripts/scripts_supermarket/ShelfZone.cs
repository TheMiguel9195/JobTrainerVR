using UnityEngine;
using System.Collections.Generic; //para poder usar List
using System.Collections; //para usar coroutine
using TMPro; //para los carteles que indican la cantidad restante

[RequireComponent(typeof(BoxCollider))] //indicamos que el script requiere de un box collider para funcionar, en caso de no haber box collider en alguna shelf podremos localizar el error mas facilmente
public class ShelfZone : MonoBehaviour{

    [SerializeField]
    private string expectedProductID; //string identificativo del producto esperado

    [SerializeField]
    private int expectedAmount; //cantidad en números enteros del producto esperado

    [SerializeField] //visible desde el editor de unity aunque sea privada
    private List<Product> productsInShelf = new List<Product>(); //lista de objetos que hay en una estanteria, se va modificando segun las colisiones que se detecten

    private PlayerFeedback playerFeedback; //encontramos la fuente del audio del jugador para dar feedback de lo que se coloca

    private bool feedbackEnabled = false; //para desactivar el sonido al inicio de la escena

    [SerializeField]
    private List<Product> initialProducts = new List<Product>(); //lista que guardara los productos iniciales para restaurarlos al reiniciar

    private TMP_Text feedbackText;

    private ScenarioGenerator scenarioGenerator;

    private GameManager gameManager;

    private ChecklistManager checklistManager;

    private int badPlacementPenalization = 0;

    public int BadPlacementPenalization => badPlacementPenalization;

    public string ExpectedProductID => expectedProductID; //metodo para obtener la propiedad expectedProductID

    public int ExpectedAmount => expectedAmount; //metodo para obtener la propiedad expectedAmount

    public List<Product> ProductsInShelf => productsInShelf; //metodo para poder acceder a la lista de productos de cada shelf

    public Transform Estanteria => transform.parent.parent; //para encontrar estanteria por diseño de la escena es parent.parent

    public Transform Isle => transform.parent; //en caso de no ser estanteria es una isla y entonces por diseño es solo parent y el "estanteria" sera null

    private void Start(){
        
        StartCoroutine(SaveInitialProductsCoroutine());

    }

    private void Awake(){
        
        playerFeedback = FindAnyObjectByType<PlayerFeedback>(); //al inicio de la escena encontramos la fuente del feedback

        scenarioGenerator = FindAnyObjectByType<ScenarioGenerator>(); //al inicio de la escena detecta el scenariogenerator para poder avisar de comprobar si la tarea ha terminado

        gameManager = FindAnyObjectByType<GameManager>(); //al inicio de la escena detecta el gameManager para poder comprobar si gameRunning es true y asi comprobar el estado de la prueba

        feedbackText = GetComponentInChildren<TMP_Text>(); //encuentra el componente TMP text en el hijo del shelfzone

        checklistManager = FindAnyObjectByType<ChecklistManager>(); //encuentra el checklist para poder llamar al update

        if(playerFeedback == null){

            Debug.LogError("No se encontró ningún PlayerFeedback en la escena.");

        }

        if(scenarioGenerator == null){
            
            Debug.LogError("No se encontró ningún scenarioGenerator en la escena");

        }

        if(feedbackText == null){
            
            Debug.LogError("No se encontró el componente TMP Text en el hijo de shelfzone");

        }

        if(checklistManager == null){
            
            Debug.LogError("No se encontró ningún checklistManager en la escena");

        }

    }

    public int CorrectProducts{

        get{
            
            int count = 0;

            foreach(Product product in productsInShelf){
                
                if(product.productID == expectedProductID){ //por cada elemento de la lista actualizada cuyo ID coincida con el del objeto esperado, aumentamos el contador
                    
                    count++;

                }

            }

            return count; //retornamos los productos correctos contados

        }

    } //cuenta el numero de productos correctamente situados de la shelf

    public int WrongProducts{

        get{
            
            int count = 0;

            foreach(Product product in productsInShelf){
                
                if(product.productID != expectedProductID){ //por cada elemento de la lista actualizada cuyo ID coincida con el del objeto esperado, aumentamos el contador
                    
                    count++;

                }

            }

            return count; //retornamos productos incorrectos contados

        }

    } //cuenta el numero de productos mal situados de la shelf

    public int MissingProducts{

        get{
            
            return (expectedAmount - CorrectProducts);

        }

    } //cuenta el numero de productos faltantes de la shelf

    public bool IsCompleted{

        get{
            
            if((MissingProducts == 0 && WrongProducts == 0) || (productsInShelf.Count >= expectedAmount)){ //completada correctamente o llena de objetos aunque incorrectos
                
                return true;

            }
            else{
                
                return false;

            }

        }

    } //indicara si la shelf esta completada

    private void OnTriggerEnter(Collider other){
        
        Product product = other.GetComponent<Product>();

        if (product == null){
            
            return;

        }

        Debug.Log(product.productID + " ha entrado en " + gameObject.name);

        if (!productsInShelf.Contains(product)){ //si esa instancia del product no se encontraba previamente en la shelf entonces la añadimos a la lista
            
            productsInShelf.Add(product);

            UpdateProgressUI(); //actualizamos el feedback textual

            if(checklistManager.IsGenerated == true){
                
                checklistManager.UpdateChecklistprogress();

            }

            CheckPlacement(product);

            //comprobamos si se ha repuesto todo con una llamada a ScenarioGenerator
            if(gameManager.gameRunning == true){

                scenarioGenerator.CheckScenarioCompleted();

            }

        }

    }

    private void OnTriggerExit(Collider other){
        
        Product product = other.GetComponent<Product>();

        if (product == null){
            
            return;

        }
        
        productsInShelf.Remove(product); //cuando el product deja de colisionar con la shelf obtenemos el componente product para eliminar la instancia de la lista de productos que estan en la shelf

        Debug.Log(product.productID + " ha salido de " + gameObject.name);

        UpdateProgressUI(); //actualizamos el feedback textual

    }

    public void EnableShelfFeedback(){ //llamado desde scenario generator para evitar sonidos al inicio de la escena
        
        feedbackEnabled = true;

    }

    private void CheckPlacement(Product product){

        if(productsInShelf.Contains(product)){ //como el producto sigue dentro comprobamos si es correcto o no y reproducimos sonido
            
            if(product.productID == expectedProductID && (CorrectProducts <= expectedAmount)){

                if(feedbackEnabled == true){
                    
                    playerFeedback.PlayCorrectClip();

                    if(product.PenalizeBadPlacement == true && product.CheckYaxys() == false){ //debe ser penalizable y que este mal puesto para considerar penalizacion
                        
                        badPlacementPenalization++;

                    }

                    StartCoroutine(LockCorrectProduct(product));

                }

            }
            else{

                if(feedbackEnabled == true){
                    
                    playerFeedback.PlayWrongClip();

                }

            }

        }

    }

    public void ResetShelf(){
        
        productsInShelf.Clear();

        productsInShelf.AddRange(initialProducts);

        UpdateProgressUI();

        checklistManager.UpdateChecklistprogress();

    }

    public void UpdateProgressUI(){
        
        feedbackText.text = $"{CorrectProducts}/{expectedAmount}";

    }

    private IEnumerator SaveInitialProductsCoroutine(){ //corutina al comienzo que permite capturar el estado inicial de cada shelfzone
        
        yield return new WaitForSeconds(0.5f);

        initialProducts = new List<Product>(productsInShelf); //guardamos lo que hay en cada shelfzone justo al empezar para restaurar las listas correctamente al reiniciar la partida

        UpdateProgressUI(); //actualizamos el feedback textual

    }

    private IEnumerator LockCorrectProduct(Product product){
        
        yield return new WaitForSeconds(3f); //espera 3 segundos para que el jugador trate de colocar el producto derechito

        if(productsInShelf.Contains(product) && product.productID == expectedProductID){
            
            product.SetMovable(false);

            //desactivar product para evitar k se peguen a la cesta 

            product.basketable = false;

        }

    }
    
}
