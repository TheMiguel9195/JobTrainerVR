using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class LobbyManager : MonoBehaviour{

    public static bool useTeleportation = true;

    [SerializeField]
    private TeleportationProvider teleportationProvider;

    [SerializeField]
    private ContinuousMoveProvider continousMoveProvider;

    [SerializeField]
    private XRRayInteractor leftRayInteractor;

    private void Awake(){
        
        if(useTeleportation){
            
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
    
    public void SupermarketScene(){
        
        SceneManager.LoadScene("VR Supermarket");

    }

    public void OfficeScene(){

        SceneManager.LoadScene("VR Office");

    }

    public void SelectTeleportation(){
        
        useTeleportation = true;

        Debug.Log("Se ha seleccionado movimiento por teletransporte");

        teleportationProvider.enabled = true;

        continousMoveProvider.enabled = false;

        leftRayInteractor.enabled = true;

    }

    public void SelectContinousMovement(){
        
        useTeleportation = false;

        Debug.Log("Se ha seleccionado movimiento por joystick");

        teleportationProvider.enabled = false;

        continousMoveProvider.enabled = true;

        leftRayInteractor.enabled = false;

    }

}
