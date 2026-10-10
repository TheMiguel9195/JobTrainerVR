using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BasketBehaviour : MonoBehaviour{
    private void OnTriggerEnter(Collider other)
    {
        Product product = other.GetComponent<Product>();

        if (product == null){

            return;

        }

        if(product.transform.parent == transform){
            
            return;

        }

        if(product.basketable == true){
            
            product.transform.SetParent(transform, true);

            Debug.Log(product.name + " ha entrado en la cesta");

        }

    }

    private void OnTriggerExit(Collider other)
    {
        Product product = other.GetComponent<Product>();

        if (product == null){

            return;

        }

        // Si el producto ha salido físicamente de la cesta
        // y todavía era hijo de ella, lo liberamos.
        if (product.transform.parent == transform){

            product.transform.SetParent(null, true);

            Debug.Log(product.name + " ha salido de la cesta");
            
        }
    }
}