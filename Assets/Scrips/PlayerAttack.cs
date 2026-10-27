using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // Temps de rec�rrega per a l'atac b�sic
    [SerializeField] private float Attack01_CD;
    // Velocitat del projectil
    [SerializeField] private float projectileSpeed;
    // Dany base del projectil
    [SerializeField] public int projectileDamage;

    // Refer�ncia a l'espasa i al punt de sortida dels projectils
    [SerializeField] private GameObject MoonSword;
    [SerializeField] private Transform gunEnd;
    [SerializeField] private GameObject Ability_2_End;
    [SerializeField] private Camera playerCamera;

    // Prefabs de les habilitats
    [SerializeField] public GameObject iceLancePrefab;
    [SerializeField] public GameObject fireballPrefab;
    [SerializeField] public GameObject zapPrefab;

    // Danys de les habilitats
    [SerializeField] public int Ability_2_Damage;
    [SerializeField] public int Sword_Damage;

    // Temps de rec�rrega per a cada habilitat
    [SerializeField] public int Ability_2_CD;
    [SerializeField] public int Ability_1_CD;
    [SerializeField] public int SwordCD;
    [SerializeField] public float DashCD;

    // UI per mostrar temps de rec�rrega
    [SerializeField] TextMeshProUGUI A1_CD;
    [SerializeField] TextMeshProUGUI A2_CD;
    

    // Ascens vertical inicial dels projectils 
    private float projectileUpward = 0.35f;
    private AutoAim autoAim;

    private Animator animator;
    private Player_Controler controls;
    private DashAbility dashAbility;

    // Temporitzadors per controlar els temps de rec�rrega
    private float nextFire_P;
    private float nextFire_A1;
    private float nextFire_A2;
    private float nextFire_Sword;
   

    // Estat actual de l'animaci� i tipus d'arma
    private string currentAnimationState;
    public string currentWeaponType;

    // Noms dels estats d'animaci�
    private const string Attack01 = "Attack01";
    private const string Move = "Move";
    private const string Ability_1 = "Ability_1";
    private const string Ability_2 = "Ability_2";
    private const string Sword = "Sword";



    private void Awake()
    {
        // Inicialitza components
        animator = GetComponent<Animator>();
        controls = new Player_Controler();
        dashAbility = GetComponent<DashAbility>();
        autoAim = GetComponent<AutoAim>();

        // Assigna els controls a cada acci�
        controls.Player.Attack.performed += ctx => Sword_Attack();
        controls.Player.Shoot.performed += ctx => Shoot();
        controls.Player.Ability_1.performed += ctx => Shoot_Ability_1();
        controls.Player.Ability_2.performed += ctx => Shoot_Ability_2();
        controls.Player.Dash.performed += ctx => dashAbility.TryDash();


    }

    private void Update()
    {
        // Actualitza els textos del temps de rec�rrega de les habilitats
        if (A1_CD != null)
        {
            A1_CD.text = $"Ability 1 CD: {nextFire_A1 - Time.time:F0}s";
            if (nextFire_A1 < Time.time)
            {
                A1_CD.color = Color.white;
                A1_CD.text = "Ability 1 CD: 0s";
            }
        }
        if (A2_CD != null)
        {
            A2_CD.text = $"Ability 2 CD: {nextFire_A2 - Time.time:F0}s";
            if (nextFire_A2 < Time.time)
            {
                A2_CD.color = Color.white;
                A2_CD.text = "Ability 2 CD: 0s";
            }
        }


    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Start()
    {
        // Inicialment amaga l'espasa i la posici� de final d'habilitat 2
        MoonSword.SetActive(false);
        Ability_2_End.SetActive(false);
    }

    public void UpdateCurrentWeaponType(int projectileIndex)
    {
        // Actualitza el tipus d'arma segons l'�ndex
        switch (projectileIndex)
        {
            case 0: currentWeaponType = "Projectile_Fire"; break;
            case 1: currentWeaponType = "Projectile_Ice"; break;
            case 2: currentWeaponType = "Projectile_Lightning"; break;
        }
    }

    private void Sword_Attack()
    {
        // Comprova si est� en temps de rec�rrega
        if (Time.time < nextFire_Sword) return;

        MoonSword.SetActive(true);
        ChangeAnimationState(Sword);
        nextFire_Sword = Time.time + SwordCD;
        StartCoroutine(HideSword(0.5f));
    }

    private void Shoot()
    {
        if (Time.time < nextFire_P) return;

        ChangeAnimationState(Attack01);
        nextFire_P = Time.time + Attack01_CD;
        StartCoroutine(ShootAtSpecificTime_Attack01(0.25f));
    }

    private void Shoot_Ability_1()
    {
        if (Time.time < nextFire_A1 && A1_CD != null)
        {
            A1_CD.color = Color.red;
            return;
        }

        ChangeAnimationState(Ability_1);
        nextFire_A1 = Time.time + Ability_1_CD;
        StartCoroutine(ShootAtSpecificTime_Ability_1(0.18f));
    }

    private void Shoot_Ability_2()
    {
        if (Time.time < nextFire_A2 && A2_CD != null)
        {
            A2_CD.color = Color.red;
            return;
        }

        ChangeAnimationState(Ability_2);
        nextFire_A2 = Time.time + Ability_2_CD;
        Ability_2_End.SetActive(true);
        StartCoroutine(ShootAtSpecificTime_Ability_2(0.8f));
    }

    // L�gica per disparar un projectil simple (atac b�sic)
    private void Projectile()
    {
        StaffSwitcher staffSwitcher = GetComponent<StaffSwitcher>();
        if (staffSwitcher == null || !gunEnd || playerCamera == null) return;

        GameObject currentProjectile = staffSwitcher.Projectiles[staffSwitcher.currentProcjectileIndex];
        if (!currentProjectile) return;

        GameObject projectile = Instantiate(currentProjectile, gunEnd.position, Quaternion.identity);

        if (projectile.GetComponent<Projectile>() == null)
            projectile.AddComponent<Projectile>();

        Physics.IgnoreCollision(projectile.GetComponent<Collider>(), GetComponent<Collider>());

        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Vector3 targetPoint = ray.origin + ray.direction * 100f;
        Vector3 shootDirection = (targetPoint - gunEnd.position).normalized;

        if (autoAim != null && autoAim.HasTarget())
            shootDirection = autoAim.GetAdjustedShootDirection(shootDirection);

        if (rb != null)
        {
            rb.useGravity = true;
            Vector3 finalDirection = (shootDirection + Vector3.up * projectileUpward).normalized;
            rb.linearVelocity = finalDirection * projectileSpeed;
            projectile.transform.rotation = Quaternion.LookRotation(finalDirection);
        }

        Destroy(projectile, 5f);
    }

    // Disparar 4 projectils amb angle lleugerament diferent (habilitat 1)
    private void Ability1()
    {
        StaffSwitcher staffSwitcher = GetComponent<StaffSwitcher>();
        if (staffSwitcher == null || !gunEnd) return;

        GameObject currentProjectile = staffSwitcher.Projectiles[staffSwitcher.currentProcjectileIndex];
        if (!currentProjectile) return;

        Vector3 baseShootDirection;
        AutoAim autoAim = GetComponent<AutoAim>();
        Transform targetEnemy = null;
  

        if (autoAim != null && autoAim.HasTarget())
        {
            targetEnemy = autoAim.GetCurrentTarget();
            baseShootDirection = (targetEnemy.position - gunEnd.position).normalized;
        }
        else
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            Vector3 targetPoint = ray.origin + ray.direction * 100f;
            baseShootDirection = (targetPoint - gunEnd.position).normalized;
        }

        for (int i = 0; i < 4; i++)
        {
            float angleOffset = (i - 1.5f) * 5f;
            Quaternion rotationOffset = Quaternion.Euler(0, angleOffset, 0);
            Vector3 shootDirection = rotationOffset * baseShootDirection;

            GameObject projectile = Instantiate(currentProjectile, gunEnd.position, Quaternion.LookRotation(shootDirection));
            if (projectile.GetComponent<Projectile>() == null)
                projectile.AddComponent<Projectile>();

            Physics.IgnoreCollision(projectile.GetComponent<Collider>(), GetComponent<Collider>());

            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 finalDirection = (shootDirection + Vector3.up * projectileUpward).normalized;
                rb.linearVelocity = finalDirection * projectileSpeed;
                projectile.transform.rotation = Quaternion.LookRotation(finalDirection);
            }

            Destroy(projectile, 5f);
        }
    }

    // Executa l�habilitat 2 segons el tipus de bast� seleccionat
    private void Ability2()
    {
        StaffSwitcher staffSwitcher = GetComponent<StaffSwitcher>();
        if (staffSwitcher == null || !gunEnd || !Ability_2_End) return;

        GameObject currentProjectile = staffSwitcher.Projectiles[staffSwitcher.currentProcjectileIndex];
        if (currentProjectile == null) return;

        Vector3 effectPosition = Ability_2_End.transform.position;
        Quaternion effectRotation = Ability_2_End.transform.rotation;

        AutoAim autoAim = GetComponent<AutoAim>();
        if (autoAim != null && autoAim.HasTarget())
        {
            Transform target = autoAim.GetCurrentTarget();
            if (staffSwitcher.currentProcjectileIndex != 2)
            {
                Vector3 directionToTarget = (target.position - effectPosition).normalized;
                effectRotation = Quaternion.LookRotation(directionToTarget);
            }
        }

        switch (staffSwitcher.currentProcjectileIndex)
        {
            case 0:
                currentWeaponType = "Ability2_Fire";
                Instantiate(fireballPrefab, effectPosition, effectRotation);
                break;
            case 1:
                currentWeaponType = "Ability2_Ice";
                Instantiate(iceLancePrefab, effectPosition, effectRotation);
                break;
            case 2:
                currentWeaponType = "Ability2_Lightning";
                Instantiate(zapPrefab, effectPosition, Quaternion.identity);
                break;
            default:
                Debug.LogWarning("Index de bast� no reconegut");
                break;
        }
    }

    // Coroutines per disparar projectils a temps espec�fic segons l'animaci�
    private IEnumerator ShootAtSpecificTime_Attack01(float delay)
    {
        yield return new WaitForSeconds(delay);
        Projectile();
        StartCoroutine(ReturnToIdleAfterAnimation(Attack01));
    }

    private IEnumerator ShootAtSpecificTime_Ability_1(float delay)
    {
        yield return new WaitForSeconds(delay);
        Ability1();
        StartCoroutine(ReturnToIdleAfterAnimation(Ability_1));
    }

    private IEnumerator ShootAtSpecificTime_Ability_2(float delay)
    {
        yield return new WaitForSeconds(delay);
        Ability2();
        StartCoroutine(ReturnToIdleAfterAnimation(Ability_2));
    }

    private IEnumerator HideSword(float delay)
    {
        yield return new WaitForSeconds(delay);
        MoonSword.SetActive(false);
        StartCoroutine(ReturnToIdleAfterAnimation(Sword));
    }

    private IEnumerator ReturnToIdleAfterAnimation(string animationName)
    {
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);
        ChangeAnimationState(Move);
        Ability_2_End.SetActive(false);
    }

    private void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState) return;
        animator.Play(newState);
        currentAnimationState = newState;
    }
}
