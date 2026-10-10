using UnityEngine;
using System.Collections.Generic;

public class ProductDatabase : MonoBehaviour{
    
    [SerializeField]
    private List<ProductEntry> productEntries = new List<ProductEntry>(); //lista con todos los productEntry

    public Product GetProductPrefab(string productID){

        for (int i = 0 ; i < productEntries.Count ; i++){

            if(productEntries[i].ProductID == productID){
                
                return productEntries[i].Prefab; //devuelve el prefab si coincide el productID

            }

        }

        Debug.LogWarning($"No existe un prefab asociado al ProductID: {productID}");

        return null; //en caso de no encontrarlo (DEBERIA DE ENCONTRARLO YA QUE CONTIENE TODOS LOS PRODUCTS DEL ESCENARIO)

    }

    public string GetDisplayName(string productID){
        
        foreach (ProductEntry entry in productEntries){
        
            if(entry.ProductID == productID){
                
                return entry.DisplayName; //si coincide devolvemos el displayName

            }

        }

        return productID; //en caso de no encontrarlo

    }

}
