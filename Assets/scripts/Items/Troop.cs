using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Troop : Item
{
    [Header("Stats")]
    public float Speed = 3f;
    public float RadiusAttack = 1.5f;
    public float Damage = 10f;
    public float attackCooldown = 1f;
    public float rotationSpeed = 10f;
    
    [Header("to fill")]
    protected Bullet bulletPrefab;
    protected Transform bulletSpawn;
    
    [Header("Runtime")]
    protected bool isAttacking;
    public Transform target;
    
    Animator animator;

    protected GridManager gridManager;

    protected List<Node> path = new();
    protected Transform lastTarget;
    protected float pathRefreshTimer;

    public bool alreadyTakeTP = false;

    protected const float PATH_REFRESH_TIME = 0.1f;
    
    public bool isFrozen;
    public bool isPoisened;
    
    public new bool IsMovementTarget => true;
    public new bool CanBeAttacked => true;
    
    public Vector3 lastPosition;
    public Vector3 velocity;
    public bool isMoving;

    [Header("Collision")]
    float troopPushForce = 10f;
    
    void OnEnable()
    {
        gridManager = GridManager.instance;
        if (gridManager == null)
        {
            Debug.LogWarning($"{name}: GridManager not found!");
            return;
        }
    
        gridManager = GridManager.instance;

        lastTarget = null;
        pathRefreshTimer = 0f;
        
        lastPosition = transform.position;
    }

    void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX 
                    | RigidbodyConstraints.FreezeRotationZ;
    }

protected virtual void Update()
{
    if (isFrozen)
        return;

    if (!enabled || !gridManager)
        return;

    pathRefreshTimer -= Time.deltaTime;

    if (target == null || pathRefreshTimer <= 0f)
        {
            Debug.Log($"[{transform.name}] Rescan... (P1={playerOneProperty})");
            
            Transform newTarget = Rescan();

            if (newTarget != target)
            {
                Debug.Log($"[{transform.name}] Changement de cible : {(target ? target.name : "null")} → {(newTarget ? newTarget.name : "null")}");
                
                target = newTarget;
                path.Clear();
                
                if (target != null)
                {
                    FindPath(transform.position, target.position);
                    lastTarget = target;
                }
            }
            
            pathRefreshTimer = PATH_REFRESH_TIME;
        }


    if (target && (path.Count == 0 || pathRefreshTimer <= 0f))
    {
        FindPath(transform.position, target.position);
        lastTarget = target;
        pathRefreshTimer = PATH_REFRESH_TIME;
    }

    if (!target)
        return;

    animator.SetBool("Walk", isMoving && !isAttacking);

    if (isAttacking)
    {
        if (target)
        {
            Vector3 directionToTarget = (target.position - transform.position);
            directionToTarget.y = 0;
            RotateTowards(directionToTarget);
        }
        return;
    }

    if (target.TryGetComponent<Item>(out var t))
    {
        float dist = Vector3.Distance(transform.position, target.position);
        
        if (dist <= RadiusAttack && t.CanBeAttacked)
        {
            isAttacking = true;
            animator.ResetTrigger("Throw");
            animator.SetBool("Walk", false);
            animator.SetTrigger("Throw");
        
            Vector3 directionToTarget = (target.position - transform.position);
            directionToTarget.y = 0;
            RotateTowards(directionToTarget);
            return;
        }
    }
    else if (target.TryGetComponent<Nexus>(out var n))
    {
        float dist = Vector3.Distance(transform.position, target.position);
        if (dist <= RadiusAttack && n.CanBeAttacked)
        {
            isAttacking = true;
            animator.ResetTrigger("Throw");
            animator.SetBool("Walk", false);
            animator.SetTrigger("Throw");
        
            Vector3 directionToTarget = (target.position - transform.position);
            directionToTarget.y = 0;
            RotateTowards(directionToTarget);
            return;
        }
    }

    if (path.Count > 0)
    {
        Chase();
    }

    print(transform.name + " ///////// " + playerOneProperty);
}
    
    protected void RotateTowards(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public void AttackEvent()
    {
        if (!isAttacking) return;
        Attack();
    }

    protected virtual void Attack()
    {
        print("Attack normal");
        AudioManager.instance.PlaySoundRandowPitch(AudioManager.instance.attackUnit,1.1f);

        print(target.transform.name + " in range, attack! : " + target.GetComponent<ITargetable>().CanBeAttacked);
    
        if (target && target.TryGetComponent<Item>(out var t) && t.CanBeAttacked)
        {
            t.TakeDamage(Damage);
        }else if (target && target.TryGetComponent<Nexus>(out var n))
        {
            n.TakeDamage(Damage);
        }
    
        StopAllCoroutines();
        StartCoroutine(Attacking());
    }
    
    protected virtual IEnumerator Attacking()
    {
        yield return new WaitForSeconds(attackCooldown);
    
        isAttacking = false;
    }

    public void GetFrozen(float duration)
    {
        isFrozen = true;
        StartCoroutine(FrozenCoroutine(duration));
    }

    IEnumerator FrozenCoroutine(float duration)
    {
        float elapsedtime = 0f;
        while (elapsedtime < duration)
        {
            elapsedtime += Time.deltaTime;
            yield return null;
        }
        isFrozen = false;
        currentHP.color = Color.green;
    }
    
    protected void Chase()
    {
        if (path == null || path.Count == 0)
        {
            isMoving = false;
            return;
        }

        isMoving = true;

        Vector3 nextPos = path[0].worldPosition;

        nextPos.y = -0.5f;

        Vector3 moveDir = (nextPos - transform.position).normalized;

        RotateTowards(moveDir);

        transform.position = Vector3.MoveTowards(
            transform.position,
            nextPos,
            Speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, nextPos) < 0.05f)
        {
            path.RemoveAt(0);
        }
    }
    
public Transform Rescan()
{
    Vector3 myPos = transform.position;
    
    Debug.Log($"[{transform.name}] P1={playerOneProperty} - Recherche de cible...");
    
    // ========== PRIORITÉ 1 : TROUPES ENNEMIES ACCESSIBLES ==========
    List<Item> enemyTroops = playerOneProperty 
        ? GameManager.instance.placedItemsP2 
        : GameManager.instance.placedItemsP1;
    
    Transform closestTroop = null;
    float minTroopDistance = Mathf.Infinity;
    
    foreach (Item item in enemyTroops)
    {
        if (item == null) continue;
        if (item.playerOneProperty == playerOneProperty) continue; // Double check
                
        // ✅ VÉRIFIER SI ON EST DU MÊME CÔTÉ DU TERRAIN
        if (!IsOnSameSideOfTerrain(item.transform))
        {
            Debug.Log($"[{transform.name}] {item.name} est de l'autre côté du terrain, ignoré");
            continue;
        }
        
        float dist = Vector3.SqrMagnitude(item.transform.position - myPos);
        
        if (dist < minTroopDistance)
        {
            minTroopDistance = dist;
            closestTroop = item.transform;
        }
    }
    
    // Si on a trouvé une troupe ennemie accessible, on la cible en priorité
    if (closestTroop != null)
    {
        Debug.Log($"[{transform.name}] ✅ PRIORITÉ 1 - Troupe accessible trouvée : {closestTroop.name}");
        return closestTroop;
    }
    
    Debug.Log($"[{transform.name}] Aucune troupe ennemie accessible, recherche TP/Nexus...");
    
    // ========== PRIORITÉ 2 : TP_TROOP ET NEXUS ==========
    List<ITargetable> secondaryTargets = new List<ITargetable>();
    
    // Ajouter les TP_Troop (pour changer de côté)
    if (!alreadyTakeTP)
    {
        foreach (TP_Troop tp in VariablesManager.instance.tps)
        {
            // On prend les TP de NOTRE côté pour pouvoir traverser
            if (IsOnSameSideOfTerrain(tp.transform))
                secondaryTargets.Add(tp);
        }
    }
    
    // Ajouter les Nexus ennemis accessibles
    foreach (Nexus nexus in VariablesManager.instance.nexus)
    {
        if (nexus.playerOneProperty != playerOneProperty && IsOnSameSideOfTerrain(nexus.transform))
            secondaryTargets.Add(nexus);
    }
    
    if (secondaryTargets.Count == 0)
    {
        Debug.LogWarning($"[{transform.name}] ❌ AUCUNE CIBLE DISPONIBLE !");
        return null;
    }
    
    // Chercher la cible secondaire la plus proche
    Transform closestSecondary = null;
    float minSecondaryDistance = Mathf.Infinity;
    
    foreach (ITargetable t in secondaryTargets)
    {
        Transform tTransform = ((MonoBehaviour)t).transform;
        float dist = Vector3.SqrMagnitude(tTransform.position - myPos);
        
        if (dist < minSecondaryDistance)
        {
            minSecondaryDistance = dist;
            closestSecondary = tTransform;
        }
    }
    
    if (closestSecondary != null)
    {
        Debug.Log($"[{transform.name}] ✅ PRIORITÉ 2 - Cible secondaire : {closestSecondary.name}");
    }
    
    return closestSecondary;
}

bool IsOnSameSideOfTerrain(Transform other)
{
    float divisionX = 0f;
    bool imOnRight = transform.position.x > divisionX;
    bool otherOnRight = other.position.x > divisionX;
    
    Debug.Log($"[IsOnSameSide] {transform.name} (x={transform.position.x:F2}, right={imOnRight}) vs {other.name} (x={other.position.x:F2}, right={otherOnRight}) → Même côté? {imOnRight == otherOnRight}");
    
    return imOnRight == otherOnRight;
}


    public void FindPath(Vector3 startPos, Vector3 endPos)
    {
        Node startNode = gridManager.NodeFromWorldPosition(startPos);
        Node endNode = gridManager.NodeFromWorldPosition(endPos);

        if (startNode == null || endNode == null)
            return;

        // Reset nodes
        foreach (Node n in gridManager.grid)
        {
            n.gCost = int.MaxValue;
            n.hCost = 0;
            n.parent = null;
        }

        startNode.gCost = 0;

        List<Node> open = new();
        HashSet<Node> closed = new();

        open.Add(startNode);

        while (open.Count > 0)
        {
            Node current = GetLowestFCostNode(open);

            open.Remove(current);
            closed.Add(current);

            if (current == endNode)
            {
                RetracePath(startNode, endNode);
                return;
            }

            foreach (Node neighbour in gridManager.GetNeighbours(current))
            {
                if (!neighbour.walkable || closed.Contains(neighbour))
                    continue;

                int newCost = current.gCost + GetDistance(current, neighbour);

                if (newCost < neighbour.gCost)
                {
                    neighbour.gCost = newCost;
                    neighbour.hCost = GetDistance(neighbour, endNode);
                    neighbour.parent = current;

                    if (!open.Contains(neighbour))
                        open.Add(neighbour);
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        path.Clear();

        Node current = endNode;

        while (current != startNode && current != null)
        {
            path.Add(current);
            current = current.parent;
        }

        path.Reverse();
    }

    Node GetLowestFCostNode(List<Node> nodes)
    {
        Node best = nodes[0];

        foreach (Node n in nodes)
        {
            if (n.fCost < best.fCost ||
                (n.fCost == best.fCost && n.hCost < best.hCost))
            {
                best = n;
            }
        }

        return best;
    }

    int GetDistance(Node a, Node b)
    {
        int dx = Mathf.Abs(a.gridX - b.gridX);
        int dy = Mathf.Abs(a.gridY - b.gridY);

        const int diag = 14;
        const int straight = 10;

        return dx > dy
            ? diag * dy + straight * (dx - dy)
            : diag * dx + straight * (dy - dx);
    }
    
    public override void SetActive(bool state)
    {
        base.SetActive(state);
        enabled = state;
    }
    
    public void ForceRecalculatePath(bool ignoreLastTarget = false)
    {
        StopAllCoroutines();
        isAttacking = false;

        target = null;

        path.Clear();
        lastTarget = ignoreLastTarget ? null : lastTarget;
        pathRefreshTimer = -1f;

        target = Rescan();

        if (target != null)
        {
            FindPath(transform.position, target.position);
            lastTarget = target;
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, RadiusAttack);

        if (path != null)
        {
            Gizmos.color = Color.cyan;
            foreach (Node n in path)
            {
                Gizmos.DrawCube(n.worldPosition, Vector3.one * 0.2f);
            }
        }
    }

    public void ResetTroup()
    {
        target = null;             
        lastTarget = null;         
        path.Clear();             
        pathRefreshTimer = 0f; 
        ForceRecalculatePath();
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Troop>(out Troop otherTroop))
        {
            if (otherTroop == this)
                return;

            Vector3 pushDir = transform.position - otherTroop.transform.position;
            pushDir.y = 0f;

            if (pushDir.sqrMagnitude < 0.001f)
                return;

            pushDir.Normalize();

            transform.position += pushDir * troopPushForce * Time.deltaTime;
        }
    }
}
