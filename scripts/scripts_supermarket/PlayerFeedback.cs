using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFeedback : MonoBehaviour{
    
    [SerializeField]
    private AudioSource audioSource;

    private float feedbackCooldown = 2f; //cantidad de tiempo entre feedbacks

    private float lastFeedbackTime = 0f; //ultimo momento en que se reprodujo un feedback

    [SerializeField]
    private AudioClip correctPlacementClip;

    [SerializeField]
    private AudioClip wrongPlacementClip;

    [SerializeField]
    private AudioClip EndGameClip;

    public void PlayCorrectClip(){

        if(Time.time - lastFeedbackTime < feedbackCooldown){
            
            return;

        }

        lastFeedbackTime = Time.time; //actualizamos momento de ultimo feedback
        
        audioSource.PlayOneShot(correctPlacementClip);

    }

    public void PlayWrongClip(){

        if(Time.time - lastFeedbackTime < feedbackCooldown){
            
            return;

        }

        lastFeedbackTime = Time.time;
        
        audioSource.PlayOneShot(wrongPlacementClip);

    }

    public void PlayEndGameClip(){
        
        audioSource.PlayOneShot(EndGameClip);

    }

}
