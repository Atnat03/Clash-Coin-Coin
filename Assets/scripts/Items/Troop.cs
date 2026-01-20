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

    [Header("Runtime")]
    bool isAttacking;
    Transform target;

    GridManager gridManager;

    List<Node> path = new();
    Transform lastTarget;
    float pathRefreshTimer;

    public bool alreadyTakeTP = false;

    const float PATH_REFRESH_TIME = 0.5f;
    
    public bool isFrozen;
    
    public new bool IsMovementTarget => true;
    public new bool CanBeAttacked => true;
    
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
    }

    void Update()
    {
        if (isFrozen)
            return;
        
        if (!enabled || !gridManager)
            return;
        
        target = Rescan();
        print("Target : " + target.name);

        if (target && path.Count == 0)
        {
            FindPath(transform.position, target.position);
            lastTarget = target;
            pathRefreshTimer = PATH_REFRESH_TIME;
        }

        if (!target)
            return;

        pathRefreshTimer -= Time.deltaTime;
        if (pathRefreshTimer <= 0f || target != lastTarget || path.Count == 0)
        {
            FindPath(transform.position, target.position);
            lastTarget = target;
            pathRefreshTimer = PATH_REFRESH_TIME;
        }
        
        if (path.Count == 0)
        {
            if (target.TryGetComponent<ITargetable>(out var t) && t.CanBeAttacked)
            {
                float dist = Vector3.Distance(transform.position, target.position);
                if (dist <= RadiusAttack && !isAttacking)
                    StartCoroutine(Attack());
            }
        }
        else
        {
            Chase();
        }



        currentHP.fillAmount = PV / maxPV;
    }
    
    IEnumerator Attack()
    {
        isAttacking = true;

        yield return new WaitForSeconds(attackCooldown);

        if (target &&
            target.TryGetComponent<ITargetable>(out var t) &&
            t.CanBeAttacked)
        {
            t.TakeDamage(Damage);
        }

        isAttacking = false;
    }


    void Chase()
    {
        if (path == null || path.Count == 0)
            return;

        Vector3 nextPos = path[0].worldPosition;

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
        ITargetable[] targets = FindObjectsOfType<MonoBehaviour>()
            .OfType<ITargetable>()
            .Where(t =>
            {
                MonoBehaviour mb = (MonoBehaviour)t;
                return mb.gameObject.activeInHierarchy
                       && mb != this;
            })
            .ToArray();

        if (targets.Length == 0)
            return null;

        Transform closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 myPos = transform.position;

        foreach (ITargetable t in targets)
        {
            if (t.playerOneProperty == playerOneProperty)
                continue;
            
            if(alreadyTakeTP)
                if(t is TP_Troop)
                    continue;
            
            Transform tTransform = ((MonoBehaviour)t).transform;
            float dist = Vector3.SqrMagnitude(tTransform.position - myPos);

            if (dist < minDistance)
            {
                minDistance = dist;
                closest = tTransform;
            }
        }

        return closest;
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

        bool stopAtAttackRange =
            target.TryGetComponent<ITargetable>(out var t) &&
            t.CanBeAttacked &&
            !t.IsMovementTarget;


        if (stopAtAttackRange)
        {
            while (path.Count > 0 &&
                   Vector3.Distance(path[path.Count - 1].worldPosition, target.position) <= RadiusAttack)
            {
                path.RemoveAt(path.Count - 1);
            }
        }

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
    
    public void ForceRecalculatePath()
    {
        StopAllCoroutines();
        isAttacking = false;

        path.Clear();
        lastTarget = null;
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

}
