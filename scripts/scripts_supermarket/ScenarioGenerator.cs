using UnityEngine;
using System.Collections.Generic; //para poder usar List

public class ScenarioGenerator : MonoBehaviour{

    [SerializeField]
    private ShelfZone[] shelfZone; //array que almacenara la informacion de todos los shelfzone de la escena

    [SerializeField]
    private List<Product> removedProducts = new List<Product>();

    [SerializeField]
    private ProductDatabase productDatabase; //"base de datos" de los productos con IDs y prefabs

    [SerializeField]
    private List<SpawnArea> spawnAreas = new List<SpawnArea>(); //lista con areas del almacen
    
    private GameManager gameManager;

    private ChecklistManager checklistManager;

    private void Awake(){
        
        gameManager = FindAnyObjectByType<GameManager>(); //encuentra el gamemanager de la escena

        checklistManager = FindAnyObjectByType<ChecklistManager>(); //encuentra el checklistmanager

        if(gameManager == null){
            
            Debug.LogError("No se encontró ningún gameManager en la escena");

        }

        if(checklistManager == null){
            
            Debug.LogError("No se encontró ningún checklistManager en la escena");

        }

    }

    private void ShuffleList<T>(List<T> items){ //utiliza el algoritmo Fisher-Yates de coste O(n) para mezclar la listas (utilizamos plantilla para que sea genérico para cualquier tipo de List)
        
        for (int i = items.Count - 1; i > 0; i--){

        int randomIndex = Random.Range(0, i + 1);

        T temp = items[i];

        items[i] = items[randomIndex];

        items[randomIndex] = temp;

        }

    }

    public void GenerateScenario(){

        shelfZone = null; //limpiamos todas las shelfs de anteriores ejecuciones

        removedProducts.Clear(); //limpiamos la lista de productos de anteriores ejecuciones
        
        shelfZone = FindObjectsByType<ShelfZone>(); //localizar todas las shelfzones

        int totalRemoved = 0; //total de instancias eliminadas

        int storageCapacity = GetStorageCapacity(); //cantidad de espacio disponible en almacen para instanciar los productos eliminados

        Debug.Log("ShelfZones encontradas: " + shelfZone.Length); //debug para ver si se han localizado todas (son 68)

        foreach (ShelfZone shelf in shelfZone){ //mostrar la info del contenido de cada shelfzone
            
            Debug.Log(

                $"Shelf: {shelf.name} | " +
                $"Estanteria (puede no ser de estanteria): {shelf.Estanteria} | " +
                $"Isla (puede no ser de isla): {shelf.Isle} | " +
                $"Producto esperado: {shelf.ExpectedProductID} | " +
                $"Cantidad esperada: {shelf.ExpectedAmount} | " +
                $"Correctos: {shelf.CorrectProducts}"

            );

        }

        //elegir aleatoriamente varias shelfzone (34 numeros aleatorios por ejemplo)

        List<ShelfZone> shuffleditems = new List<ShelfZone>(shelfZone); //copia del array de shelfzones a la que aplicaremos Fisher-Yates

        ShuffleList(shuffleditems); //se le aplica el algoritmo para estar aleatorizado

        for(int i = 0 ; i < (shuffleditems.Count/2) ; i++){ //recorremos la mitad de shelfzones y eliminamos productos

            if(totalRemoved == storageCapacity){
                
                break;

            } 
            
            List<Product> productsFromShelf = new List<Product>(shuffleditems[i].ProductsInShelf); //seleccionamos todos los productos de cada shelf copiando la lista de productos

            ShuffleList(productsFromShelf); //hacemos shuffle para eliminar en cada ocasion diferentes instancias de los productos.

            int amountToEliminate = productsFromShelf.Count/2; //eliminamos siempre la mitad del total del productos de la shelf

            for(int j = 0 ; j < amountToEliminate ; j++){

                if(totalRemoved == storageCapacity){
                
                    break;

                }

                shuffleditems[i].ProductsInShelf.Remove(productsFromShelf[j]); //eliminamos la instancia del producto que vamos a desactivar del escenario ya que Unity no asegura que al desactivarlo se lance la llamada a OnTriggerExit
                
                productsFromShelf[j].gameObject.SetActive(false); //desactivamos de la escena cada producto tras el shuffle y solo la mitad de la cantidad total de productos

                removedProducts.Add(productsFromShelf[j]); //añadimos el product a la lista

                totalRemoved++; //aumentamos la cantidad de instancias eliminadas

                Debug.Log($"Quitando {productsFromShelf[j].productID} de {shuffleditems[i].name}, Estanteria: {shuffleditems[i].Estanteria}, Isla: {shuffleditems[i].Isle}");

            }

            Debug.Log("SE HAN QUITADO TODOS LOS OBJETOS");

        }

        checklistManager.GenerateChecklist(removedProducts); //con removedProducts listo podemos llamar a GenerateChecklist()

        //como ya han sido borrados todos los productos y existe una checklist llamamos para instanciar los productos borrados y ver si se colocan correctamente en el almacen

        SpawnRemovedProducts();

        foreach (ShelfZone shelf in shelfZone){

            shelf.EnableShelfFeedback(); //activamos para cada shelf el feedback evitando asi que suenen al inicio de la escena

            shelf.UpdateProgressUI(); //actualizamos el feedbackTextual

        }

    }

    private void SpawnRemovedProducts(){

        foreach(Product removedProduct in removedProducts){
            
            Product prefab = productDatabase.GetProductPrefab(removedProduct.productID); //obtenemos el prefab de cada objeto borrado de la escena para poder instanciarlos en el almacen

            foreach(SpawnArea spawnArea in spawnAreas){

                if(spawnArea.SpawnProduct(prefab)){ //instanciamos el prefab en el primer spawnArea que sea posible y una vez hecho volvemos al foreach de los products
                    
                    break;

                }

            }

        }

    }

    private int GetStorageCapacity(){
        
        int capacity = 0;

        foreach(SpawnArea spawnArea in spawnAreas){
            
            capacity += spawnArea.Capacity; //la suma de las capacidades de cada spawnArea es el total de capacidad del almacen

        }

        return capacity;

    }

    public void CheckScenarioCompleted(){ //comprueba si todas las shelfzones estan completadas correctamente

        foreach (SpawnArea spawnArea in spawnAreas){ //comprobar almacen vacio

            if (!spawnArea.IsEmpty()){
                
                return;

            }

        }

        foreach (ShelfZone shelfzone in shelfZone){ //que todas las baldas esten perfectas

            if(!shelfzone.IsCompleted){
                
                return;

            }
            
        }

        //aqui solo debe llegar el flujo cuando el almacen esta vacio y se ha colocado el ultimo producto

        //si se cumple lo anterior se dispara el fin de prueba automaticamente, si falla algo el jugador pulsara el botón de finalizar

        if(gameManager.gameRunning == true){
            
            gameManager.EndGame(); //como se ha completado todo correctamente se acaba la prueba

        }
        
    }

    public void RestoreRemovedProducts(){

        foreach (ShelfZone shelfZone in shelfZone){
            
            shelfZone.ResetShelf();

        }
        
        foreach (Product product in removedProducts){
            
            if(product != null){
                
                product.gameObject.SetActive(true);

            }

        }

        removedProducts.Clear();

    }

}
