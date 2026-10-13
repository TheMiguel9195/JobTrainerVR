using UnityEngine;

[System.Serializable]
public class ProductEntry{

    [SerializeField]
    private string productID;

    [SerializeField]
    private Product prefab;

    [SerializeField]
    private string displayName; //nombre que mostraremos en la checklist del jugador

    public string ProductID => productID;

    public Product Prefab => prefab;

    public string DisplayName => displayName;
    
}
