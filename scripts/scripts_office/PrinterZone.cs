using UnityEngine;
using System.Collections;

public class PrinterZone : MonoBehaviour{ //instanciara 5 documentos cuando PaperPileZone indique que se ha cargado para simular la impresion
    
    [SerializeField]
    public int DocumentsQuantity = 5;

    [SerializeField]
    private PaperPileZone paperPileZone;

    [SerializeField]
    private GameObject documentPrefab; //prefab del documento

    public void PrintDocuments(){ //imprime los documentos

        if(paperPileZone.HasPrinted != true){
            
            for (int i = 0; i < DocumentsQuantity; i++){

                GameObject document = Instantiate(documentPrefab, transform.position + Vector3.up * 0.1f, transform.rotation * Quaternion.Euler(0f, 90f, 0f));

            }

        }

    }

}