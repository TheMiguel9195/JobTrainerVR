using UnityEngine;
using System.Collections.Generic;

public class ChecklistManager : MonoBehaviour{

    private List<ChecklistEntry> checklistEntries = new List<ChecklistEntry>(); //conjunto de entradas de checklist que se deben mostrar al usuario

    [SerializeField]
    private TMPro.TextMeshProUGUI checklistText;

    private ProductDatabase productDatabase;

    private bool isGenerated = false;

    public bool IsGenerated => isGenerated;

    private void Awake(){
        
        productDatabase = FindAnyObjectByType<ProductDatabase>();

        if(productDatabase == null){
            
            Debug.LogError("No se ha encontrado ningún productDatabase en la escena");

        }

    }

    public void GenerateChecklist(List<Product> removedProducts){ //funcion que generará la checklist del usuario a partir de la lista removedProducts
        
        checklistEntries.Clear();

        Dictionary<string, int> checklistDictionary = new Dictionary<string, int>(); //creamos un diccionario para representar (productID x N)

        foreach (Product product in removedProducts){ //contabilizamos cada producto de la lista removedProducts

            if (checklistDictionary.ContainsKey(product.productID)){
                
                checklistDictionary[product.productID]++; //si ya contiene el elemento entonces aumentamos su cantidad.

            }
            else{
                
                checklistDictionary.Add(product.productID, 1); //introducimos el primero de ese tipo de producto en la checklist

            }
            
        }

        //una vez rellenada la checklistDictionary creamos una lista de ChecklistEntry

        foreach(KeyValuePair<string, int> entry in checklistDictionary){
            
            ChecklistEntry newEntry = new ChecklistEntry(); //creamos una nueva entrada para cada par de valores del dictionary

            newEntry.productID = entry.Key; //asignamos el productID del par de valores del dictionary

            newEntry.totalAmount = entry.Value; //asignamos la cantidad de ese productID del par de valores del dictionary

            newEntry.displayName = productDatabase.GetDisplayName(entry.Key); //hallamos el displayname del productID de este productentry

            checklistEntries.Add(newEntry); //añadimos la entrada a la lista

        }

        UpdateChecklistprogress(); //escribimos el contenido en la lista

        isGenerated = true;

    }

    public void SetChecklistResult(string result){

        checklistText.text = "¡TAREA COMPLETADA!\n\n";

        checklistText.text += (result + "\n\n ¡BUEN TRABAJO!");

    }

    public void SetText(string text){
        
        checklistText.text = text;

    }

    public void UpdateChecklistprogress(){ //cuando se modifica una shelfzone se llama a esta funcion para actualizar el progreso
        
        ShelfZone[] shelfZones = FindObjectsByType<ShelfZone>(); //localizamos todas las shelfzones para quedarnos solo con las del productID recien colocado/retirado

        List<ShelfZone> affectedShelfZones = new List<ShelfZone>(); //lista de shelfzones afectadas

        foreach (ChecklistEntry checklistentry in checklistEntries){ //para cada entrada en checklistEntries agrupamos las shelfzones afectadas y calculamos el estado en base a la informacion de las shelfzones

            affectedShelfZones.Clear();

            foreach (ShelfZone shelfzone in shelfZones){

                if(checklistentry.productID == shelfzone.ExpectedProductID){ //hemos encontrado un shelfzone afectado y lo añadimos a la lista de shelfzones de un mismo productID
                    
                    affectedShelfZones.Add(shelfzone);

                }

            }

            //aqui ya se deben tener agrupadas toda las shelfzones de un mismo productID por lo que calculamos la cantidad total de productos antes de pasar al siguiente checklistentry

            int missingProductsInShelfs = 0;

            foreach (ShelfZone shelfzone in affectedShelfZones){ //calculamos todo lo que falta por completar
                
                missingProductsInShelfs += shelfzone.MissingProducts;

            }

            checklistentry.completedAmount = checklistentry.totalAmount - missingProductsInShelfs; //si al total a completar le restamos lo que falta obtenemos lo que hemos completado

        }

        //una vez actualizado el completedAmount de cada checklistentry volvemos a imprimir la lista

        string checklistString = "";

        int allShelfZonesTotalAmount = 0;

        int allShelfZonesCompletedAmount = 0;

        foreach(ChecklistEntry entry in checklistEntries){

            checklistString += "- " + entry.displayName + " " + entry.completedAmount + "/" + entry.totalAmount + "\n";

            allShelfZonesTotalAmount += entry.totalAmount; //calculamos el total del todo

            allShelfZonesCompletedAmount += entry.completedAmount; //calculamos el completado del todo
            
        }

        checklistText.text = checklistString; //asignamos el string completo al textmesh

        checklistText.text += "COMPLETADO: " + (((float)allShelfZonesCompletedAmount / allShelfZonesTotalAmount) * 100).ToString("F1") +"%";

    }

}