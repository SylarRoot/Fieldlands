using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyAi_melee : MonoBehaviour
    {
        public NavMeshAgent agent;
        public Transform player;
        public Transform weaponHolder;
        public Animator animator;
        public LayerMask whatIsGround, whatIsPlayer, whatIsEnemy;
        [SerializeField] private Collider Axe_collider;

        // Patrulla
        public Vector3 walkPoint;
        bool walkPointSet;
        public float walkPointRange;

        // Atac
        public float timeBetweenAttacks;
        private float timeSinceLastAttack;
        public float attackDelay = 0.4f; // Temps de retard per sincronitzar animació
        private bool isAttacking = false; // Control d'atacs

        public int attackDamage;
        public float attackRadius;

        // Estats
        public float sightRange, attackRange;
        public bool playerInSightRange, playerInAttackRange;

        // Separació
        public float separationRadius;
        public float separationForce;

        // Anims
        private const string Attack01 = "Attack01";

        private void Start()
        {
            player = GameObject.Find("Player").transform;
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            Axe_collider.enabled = false; 
            timeSinceLastAttack = timeBetweenAttacks;
        }

        private void Update()
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            AvoidOtherEnemies();
            animator.SetFloat("Speed", agent.velocity.magnitude);

            if (!playerInSightRange && !playerInAttackRange)
                Patroling();
            else if (playerInSightRange && !playerInAttackRange)
                ChasePlayer();
            else if (playerInAttackRange && !isAttacking) 
                AttackPlayer();

            timeSinceLastAttack += Time.deltaTime;
        }

        // ======================================== MOVIMENT ========================================
        private void Patroling()
        {
            if (!walkPointSet) SearchWalkPoint();
            if (walkPointSet) agent.SetDestination(walkPoint);

            Vector3 distanceToWalkPoint = transform.position - walkPoint;
            if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false;
        }

        private void SearchWalkPoint()
        {
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            float randomX = Random.Range(-walkPointRange, walkPointRange);
            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
                walkPointSet = true;
        }

        private void ChasePlayer()
        {
            agent.isStopped = false;

            // Offset aleatori per evitar moviment robòtic
            Vector3 randomOffset = new Vector3(
                Random.Range(-1f, 1f),
                0,
                Random.Range(-1f, 1f)
            );
            agent.SetDestination(player.position + randomOffset);
        }

        // ======================================== ATAC ========================================
        private void AttackPlayer()
        {
            agent.isStopped = true;
            transform.LookAt(player);

            if (timeSinceLastAttack >= timeBetweenAttacks && !isAttacking)
            {
                isAttacking = true;
                animator.Play(Attack01);
                Invoke(nameof(PerformAttack), attackDelay); 
                timeSinceLastAttack = 0f;
            }
        }

        private void PerformAttack()
        {
            // ⭐ Millor detecció amb posició de l'arma
            Collider[] hits = Physics.OverlapSphere(weaponHolder.position, attackRadius, whatIsPlayer);
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    hit.GetComponent<VidaPlayer>()?.TakeDamage(attackDamage);
                    Debug.Log("Dany aplicat!");
                }
            }
            isAttacking = false;
        }

        // mètodes per a Animation Events
        public void EnableAxeCollider() => Axe_collider.enabled = true;
        public void DisableAxeCollider() => Axe_collider.enabled = false;

        private void AvoidOtherEnemies()
        {
            Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, separationRadius, whatIsEnemy);
            Vector3 separationVector = Vector3.zero;
            int neighborCount = 0;

            foreach (Collider enemy in nearbyEnemies)
            {
                if (enemy.transform != transform)
                {
                    Vector3 directionAway = transform.position - enemy.transform.position;
                    float distance = directionAway.magnitude;

                    if (distance > 0)
                    {
                        float repulsionStrength = separationForce / distance;
                        separationVector += directionAway.normalized * repulsionStrength;
                        neighborCount++;
                    }
                }
            }

            if (neighborCount > 0)
            {
                separationVector /= neighborCount;
                separationVector.y = 0;
                agent.velocity += separationVector;
            }
        }


    }
}