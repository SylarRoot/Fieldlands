using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;

public class DashAbility : MonoBehaviour
{
   
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] public float dashCooldown = 2f;

    
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject MoonSword; 

    private PlayerMovement playerMovement;
    private CharacterController characterController;
    private PlayerAttack playerAttack;
    public bool isDashing = false;
    private bool canDash = true;
    private string currentAnimationState;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        characterController = GetComponent<CharacterController>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    public void TryDash()
    {
        if (canDash)
        {
                       
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        
        canDash = false;
        isDashing = true;
        MoonSword.SetActive(true); 
        playerAttack.Sword_Damage = 20; 

        // Activar efectes   

        ChangeAnimationState("Dash");
     

        // Moviment del dash  
        Vector3 dashDirection = transform.forward;
        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            characterController.Move(dashDirection * (dashDistance / dashDuration) * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        


        isDashing = false;
        //ReturnToIdleAfterAnimation("Dash");
        StartCoroutine(HideSword(0.5f)); 
        playerAttack.Sword_Damage = 10; 

        // Cooldown  
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState) return;
        animator.Play(newState);
        currentAnimationState = newState;
    }

    private IEnumerator ReturnToIdleAfterAnimation(string animationName)
    {
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(animationLength);

        ChangeAnimationState("Move");
    }
    private IEnumerator HideSword(float delay)
    {
        yield return new WaitForSeconds(delay);
        MoonSword.SetActive(false);
        StartCoroutine(ReturnToIdleAfterAnimation("Dash"));

    }

}

