using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class SpawnArea : MonoBehaviour{

    [Header("Configuración de la matriz")]

    [SerializeField]
    private int filas = 5; //numero de filas de la matriz de productos que se generará en el SpawnArea

    [SerializeField]
    private int columnas = 5; //numero de columnas de la matriz de productos generada en el SpawnArea
    
    [SerializeField]
    private float spacingX = 0; //espacio de separacion entre productos eje X

    [SerializeField]
    private float spacingZ = 0; //espacio de separacion entre productos eje Z

    [SerializeField]
    private float marginX = 0.20f; //margen eje X

    [SerializeField]
    private float marginZ = 0.20f; //margen eje Z

    [Header("Debug")]

    [SerializeField]
    private List<Product> spawnedProducts = new List<Product>(); //lista de productos que hemos instanciado

    [SerializeField]
    private List<Product> productsInSpawnArea = new List<Product>(); //lista que mantiene el estado de los productos que aun se encuentran en el almacen

    private BoxCollider boxCollider; //BoxCollider del SpawnArea

    public int Capacity => (filas * columnas);

    private Vector3 firstSpawnPosition;

    private int nextSlot = 0; //contador para calcular las posiciones de la matriz de prodcutos de SpawnArea

    private void Awake(){ //obtiene el componente boxcollider de SpawnArea

        boxCollider = GetComponent<BoxCollider>();

        firstSpawnPosition = GetStartPosition(); //guardamos la posicion inicial de la matriz

        CalculateSpacing(); //calculamos el espacio entre productos en base al tamaño del SpawnArea
        
    }

    private void CalculateSpacing(){
        
        float usableWidth = (boxCollider.bounds.size.x - 2 * marginX); //ancho utilizable en ejeX
        float usableDepth = (boxCollider.bounds.size.z - 2 * marginZ); //ancho utilizable en ejeZ

        spacingX = usableWidth / (columnas - 1); //espacio en ejeX de separacion entre productos
        spacingZ = usableDepth / (filas - 1); //espacio en ejeZ de separacion entre productos

    }

    private Vector3 GetStartPosition(){ //devuelve la posicion en la que se debe empezar a instanciar los productos para formar una matriz de productos sobre el spawnArea
        
        if(boxCollider == null){
            
            boxCollider = GetComponent<BoxCollider>();

        }

        Vector3 center = boxCollider.bounds.center;

        float longitud_x = boxCollider.bounds.size.x;

        float longitud_z = boxCollider.bounds.size.z;

        Vector3 start = center - transform.right * (longitud_x/2f) - transform.forward * (longitud_z/2f);

        start += transform.right * marginX;

        start += transform.forward * marginZ;

        return start;

    }

    private Vector3 GetSpawnPosition(int slot){ //calcula la posicion en base al slot de la matriz que se pide
        
        int fila = slot / columnas;

        int columna = slot % columnas;

        Vector3 position = firstSpawnPosition + transform.right * (columna * spacingX) + transform.forward * (fila * spacingZ);

        return position;

    }

    public bool HasFreeSpace(){
        
        return nextSlot < Capacity; //si nextslot es menor que la capacidad entonces quedan espacios libres en ese spawnArea

    }

    public bool SpawnProduct(Product productPrefab){

        if(HasFreeSpace() == true){
            
            Vector3 spawnPosition = GetSpawnPosition(nextSlot); //calculamos la posicion de spawn del siguiente slot (desde el primero hasta el ultimo hasta llenar el spawnArea)

            //instanciar prefab en la posicion

            Product instance = Instantiate(productPrefab, spawnPosition, Quaternion.identity);

            //guardar la instancia en spawnedProducts

            instance.SetMovable(true); //hacemos que los productos generados si respondan a las fisicas

            spawnedProducts.Add(instance); //de esta forma podemos ver desde el inspector de unity si se han generado todos los productos que debian

            nextSlot++; //aumentamos en 1 el valor de nextSlot ya que al instanciar hemos ocupado un espacio del spawnArea

            return true;

        }
        else{
            
            return false;

        }

    }

    private void OnTriggerEnter(Collider other){
        
        Product product = other.GetComponent<Product>();

        if(product == null){
            
            return;

        }

        Debug.Log(product.productID + " ha entrado en el almacén");

        if(!productsInSpawnArea.Contains(product)){
            
            productsInSpawnArea.Add(product);

        }

    }

    private void OnTriggerExit(Collider other){
        
        Product product = other.GetComponent<Product>();

        if(product == null){
            
            return;

        }

        Debug.Log(product.productID + " ha sido retirado del almacén");

        productsInSpawnArea.Remove(product);

    }

    public bool IsEmpty(){
        
        if(productsInSpawnArea.Count == 0){
            
            return true;

        }
        else{
            
            return false;

        }

    }

    public void DestroySpawnedProducts(){
            
        foreach (Product product in spawnedProducts){
                
            Destroy(product.gameObject);

        }

        spawnedProducts.Clear();

        productsInSpawnArea.Clear();

        nextSlot = 0;

    }

    private void OnDrawGizmosSelected(){

        boxCollider = GetComponent<BoxCollider>();

        if(boxCollider == null){
            
            return;

        } 

        CalculateSpacing();

        Gizmos.color = Color.red;

        firstSpawnPosition = GetStartPosition();

        Gizmos.DrawSphere(firstSpawnPosition, 0.05f);

        Gizmos.color = Color.green;

        for(int i = 1 ; i < Capacity ; i++){
                
            Gizmos.DrawSphere(GetSpawnPosition(i), 0.03f);

        }

    }

}
