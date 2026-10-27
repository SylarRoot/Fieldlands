using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyAi : MonoBehaviour
    {
        public NavMeshAgent agent;
        public Transform player;
        public Transform gunEnd;
        public LayerMask whatIsGround, whatIsPlayer;

        public Animator animator;
        private bool isAttacking = false;
        public float attackDelay = 0.4f; // Retard per sincronitzar animació

        // Patroling
        public Vector3 walkPoint;
        private bool walkPointSet;
        public float walkPointRange;
        public float patrolPauseTime = 2f; // Temps de pausa entre rutes
        private float patrolTimer;

        // Attacking
        public float timeBetweenAttacks;
        private bool alreadyAttacked;
        public GameObject projectile;
        public float projectileSpeed = 32f; // Velocitat ajustable
        public float predictionStrength = 0.5f; // Predicció de moviment del jugador

        // Estats
        public float sightRange, attackRange;
        public bool playerInSightRange, playerInAttackRange;

        
        private string currentAnimState;
        private const string idle = "idle";
        private const string walk = "walk";
        private const string attack = "attack";

        private void Awake()
        {
            player = GameObject.Find("Player").transform;
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            patrolTimer = patrolPauseTime;
        }

        private void Update()
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!playerInSightRange && !playerInAttackRange) Patroling();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }

        
        private void ChangeAnimationState(string newState)
        {
           
            if (currentAnimState == newState) return;

            
            animator.Play(newState);

            
            currentAnimState = newState;
        }

        // ======================================== PATRULLA ========================================
        private void Patroling()
        {
            if (!walkPointSet)
            {
                if (patrolTimer <= 0)
                {
                    SearchWalkPoint();
                    patrolTimer = patrolPauseTime; //  Pausa abans de nova ruta
                }
                else
                {
                    patrolTimer -= Time.deltaTime; // Compte enrere per a pausa
                    agent.isStopped = true; // Atura el moviment durant la pausa

                    // Play idle animation when paused
                    ChangeAnimationState(idle);
                    return;
                }
            }

            agent.isStopped = false;
            agent.SetDestination(walkPoint);

            
            if (agent.velocity.magnitude > 0.1f)
            {
                ChangeAnimationState(walk);
            }
            else
            {
                ChangeAnimationState(idle);
            }

            Vector3 distanceToWalkPoint = transform.position - walkPoint;
            if (distanceToWalkPoint.magnitude < 1f)
            {
                walkPointSet = false;
                patrolTimer = patrolPauseTime; // Reinicia el timer
            }
        }

        private void SearchWalkPoint()
        {
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            float randomX = Random.Range(-walkPointRange, walkPointRange);
            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
                walkPointSet = true;
        }

        // ======================================== PERSECUCIÓ ========================================
        private void ChasePlayer()
        {
            agent.isStopped = false;

            // Predicció de la posició futura del jugador
            Vector3 playerVelocity = player.GetComponent<CharacterController>().velocity;
            Vector3 predictedPosition = player.position + playerVelocity * predictionStrength;

            agent.SetDestination(predictedPosition);

            
            ChangeAnimationState(walk);

        }

        // ======================================== ATAC ========================================
        private void AttackPlayer()
        {
            agent.SetDestination(transform.position);
            transform.LookAt(player);

            if (!alreadyAttacked && !isAttacking)
            {
                isAttacking = true;

                
                ChangeAnimationState(attack);

                StartCoroutine(ShootAtSpecificTime_Attack(0.75f));
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
            else if (!isAttacking && !alreadyAttacked)
            {
                
                ChangeAnimationState(idle);
            }
        }

        // Mètode separat per disparar
        private void FireProjectile()
        {
            Vector3 playerVelocity = player.GetComponent<CharacterController>().velocity;
            float distanceToPlayer = Vector3.Distance(gunEnd.position, player.position);
            float timeToHit = distanceToPlayer / projectileSpeed;
            Vector3 predictedAimPoint = player.position + playerVelocity * timeToHit;
            Vector3 shootDirection = (predictedAimPoint - gunEnd.position).normalized;

            Rigidbody rb = Instantiate(projectile, gunEnd.position, Quaternion.LookRotation(shootDirection)).GetComponent<Rigidbody>();
            rb.linearVelocity = shootDirection * projectileSpeed;
            rb.AddForce(transform.up * 15f, ForceMode.Impulse);

            isAttacking = false;
        }

        private void ResetAttack()
        {
            alreadyAttacked = false;
            isAttacking = false; // Resatejar l'estat d'atac

            
            ChangeAnimationState(idle);
        }

        private IEnumerator ShootAtSpecificTime_Attack(float delay)
        {
            yield return new WaitForSeconds(delay);
            FireProjectile();
        }
    }
}