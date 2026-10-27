using UnityEngine;
using UnityEngine.InputSystem;

public class StaffSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject staffFire;
    [SerializeField] private GameObject staffIce;
    [SerializeField] private GameObject staffLightning;

    [SerializeField] private GameObject projectileFire;
    [SerializeField] private GameObject projectileIce;
    [SerializeField] private GameObject projectileLightning;


    private Player_Controler player; 
    private int currentStaffIndex = 0; // 0 = Fire, 1 = Ice, 2 = Lightning
    private GameObject[] Staffs;

    [HideInInspector]
    public GameObject[] Projectiles;
    [HideInInspector]
    public int currentProcjectileIndex;

    private void Start()
    {
        Staffs[currentStaffIndex].SetActive(true);
        Projectiles[currentProcjectileIndex].SetActive(true);
    }

    private void Awake()
    {
        player = new Player_Controler();
        player.Player.SwitchStaff.performed += ctx => SwitchStaff();

        Staffs = new GameObject[] { staffFire, staffIce, staffLightning };
        Projectiles = new GameObject[] { projectileFire, projectileIce, projectileLightning };
    }

    private void OnEnable() => player.Enable();
    private void OnDisable() => player.Disable();

    private void SwitchStaff()
    {
      
        // Desactiva el staff y  el proyectil acutal
        Staffs[currentStaffIndex].SetActive(false);
        Projectiles[currentProcjectileIndex].SetActive(false);



        // Cambia al seguent staff y  el proyectil
        currentStaffIndex = (currentStaffIndex + 1) % Staffs.Length;
        currentProcjectileIndex = (currentProcjectileIndex + 1) % Projectiles.Length;

        PlayerAttack playerAttack = GetComponent<PlayerAttack>();
        if (playerAttack != null)
        {
            playerAttack.UpdateCurrentWeaponType(currentProcjectileIndex);
        }


        // Activa el nou  staff y  el proyectil
        Staffs[currentStaffIndex].SetActive(true);
        Projectiles[currentProcjectileIndex].SetActive(true);


    }

}
