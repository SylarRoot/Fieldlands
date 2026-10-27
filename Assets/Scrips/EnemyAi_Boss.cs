using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyAi_Boss : MonoBehaviour
    {
        public NavMeshAgent agent;
        public Transform player;
        public Animator animator;
        public LayerMask whatIsGround, whatIsPlayer;

        // Atacs
        public float timeBetweenAttacks;
        private float timeSinceLastAttack;
        public float attackDelay = 0.5f;
        private bool isAttacking = false;
        public int attackDamage ;
        public float attackRadius ;
        public Transform leftHand;
        public Transform rightHand;
        [SerializeField] private Collider leftHandCollider;
        [SerializeField] private Collider rightHandCollider;
        private bool alternateAttack = true; // Alternar entre atacs

        // Estats
        public float sightRange, attackRange;
        public bool playerInSightRange, playerInAttackRange;

        // Referència al component VidaPlayer
        private VidaPlayer playerHealth;

        private void Start()
        {
            player = GameObject.Find("Player").transform;
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            timeSinceLastAttack = timeBetweenAttacks;

            // Obtenim la referència al component VidaPlayer
            if (player != null)
            {
                playerHealth = player.GetComponent<VidaPlayer>();
                if (playerHealth == null)
                {
                    Debug.LogError("No s'ha trobat el component VidaPlayer al jugador!");
                }
            }

            // Desactivem els colliders al principi
            if (leftHandCollider != null) leftHandCollider.enabled = false;
            if (rightHandCollider != null) rightHandCollider.enabled = false;
        }

        private void Update()
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            // Actualitzar el paràmetre "Speed" segons si es mou o està aturat
            animator.SetFloat("Speed", agent.velocity.magnitude > 0.1f ? 1f : 0f);

            if (!playerInSightRange && !playerInAttackRange)
                Patroling();
            else if (playerInSightRange && !playerInAttackRange)
                ChasePlayer();
            else if (playerInAttackRange && !isAttacking)
                AttackPlayer();

            timeSinceLastAttack += Time.deltaTime;
        }

        private void Patroling()
        {
            agent.isStopped = false;
        }

        private void ChasePlayer()
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }

        private void AttackPlayer()
        {
            agent.isStopped = true;
            transform.LookAt(player);

            if (timeSinceLastAttack >= timeBetweenAttacks && !isAttacking)
            {
                isAttacking = true;

                if (alternateAttack)
                {
                    animator.Play("Attack02"); // Canvia a la teva animació esquerra real
                }
                else
                {
                    animator.Play("Attack03"); // Canvia a la teva animació dreta real
                }
                alternateAttack = !alternateAttack;

                Invoke(nameof(PerformAttack), attackDelay);
                timeSinceLastAttack = 0f;
            }
        }

        private void PerformAttack()
        {
            Collider[] hits;
            Transform attackHand = alternateAttack ? rightHand : leftHand;

            // Utilitzem una esfera per detectar el jugador al voltant de la mà
            hits = Physics.OverlapSphere(attackHand.position, attackRadius, whatIsPlayer);

            if (hits.Length > 0)
            {
                ApplyDamage(hits);
            }

            isAttacking = false;
        }

        // Aquests mètodes seran cridats des de les animacions
        public void EnableLeftHandCollider()
        {
            if (leftHandCollider != null)
            {
                leftHandCollider.enabled = true;
            }
        }

        public void DisableLeftHandCollider()
        {
            if (leftHandCollider != null)
            {
                leftHandCollider.enabled = false;
            }
        }

        public void EnableRightHandCollider()
        {
            if (rightHandCollider != null)
            {
                rightHandCollider.enabled = true;
            }
        }

        public void DisableRightHandCollider()
        {
            if (rightHandCollider != null)
            {
                rightHandCollider.enabled = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                VidaPlayer playerVida = other.GetComponent<VidaPlayer>();
                if (playerVida != null)
                {
                    playerVida.TakeDamage(attackDamage);
                    Debug.Log($"Boss ha fet mal al jugador amb el collider! Dany: {attackDamage}");
                }
            }
        }

        private void ApplyDamage(Collider[] hits)
        {
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    VidaPlayer playerVida = hit.GetComponent<VidaPlayer>();
                    if (playerVida != null)
                    {
                        playerVida.TakeDamage(attackDamage);
                        Debug.Log($"Boss ha fet mal al jugador! Dany: {attackDamage}");
                    }
                }
            }
        }
    }
}