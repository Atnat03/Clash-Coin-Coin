using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Troop : Item
{
    public float Speed;
    public float RaduisAttack;
    public bool isAttaking = false;
    public float Damage = 10;
    
    public Troop(int  id, string name, float maxPV, float speed, float raduisAttack)
    {
        this.id = id;
        this.name = name;
        this.maxPV = maxPV;
        PV = maxPV;
        Speed = speed;
        RaduisAttack = raduisAttack;
    }
    
    Transform target;

	GridManager gridManager;
	
	private float t = 1;

	private void Start()
	{
		gridManager = GridManager.instance;
		target = Rescan();
	}

	void Update()
	{
		if (target == null) return;

		if (isAttaking) return;
		
		currentHP.fillAmount = PV / maxPV;
		
		FindPath(transform.position, target.position);
		
		if (t <= 0)
		{
			target = Rescan();
			t = 1;
		}
		else
		{
			t -= Time.deltaTime;
		}

		if (Vector3.Distance(transform.position, target.position) < RaduisAttack)
		{
			StartCoroutine(Attack());
		}else
		{
			Chase();
		}
	}

	IEnumerator Attack()
	{
		isAttaking = true;
		yield return new WaitForSeconds(1);
		
		target.GetComponent<ITargetable>().TakeDamage(Damage);
		
		isAttaking = false;
	}
	
	public void Chase()
	{
		if (gridManager.path != null)
		{
			if (gridManager.path.Count > 0)
			{
				transform.position = Vector3.MoveTowards(transform.position, gridManager.path[0].worldPosition, Speed * Time.deltaTime);
			}
		}
	}

	private Transform Rescan()
	{
		Collider[] hits = Physics.OverlapSphere(transform.position, 1000000);

		Transform closestTarget = null;
		float closestDistanceSqr = float.MaxValue;

		foreach (Collider hit in hits)
		{
			if (!hit.TryGetComponent<ITargetable>(out _))
				continue;
			
			if(hit.GetComponent<ITargetable>().playerOneProperty == playerOneProperty)
				continue;

			Vector3 delta = hit.transform.position - transform.position;
			float distanceSqr = delta.sqrMagnitude;

			if (distanceSqr < closestDistanceSqr)
			{
				closestDistanceSqr = distanceSqr;
				closestTarget = hit.transform;
			}
		}

		return closestTarget;
	}

	void FindPath(Vector3 startPos, Vector3 endPos)
	{
		Node startNode = gridManager.NodeFromWorldPosition(startPos - gridManager.transform.position);
		Node endNode   = gridManager.NodeFromWorldPosition(endPos - gridManager.transform.position);

		List<Node>    openNodes   = new();
		HashSet<Node> closedNodes = new();

		openNodes.Add(startNode);

		while (openNodes.Count > 0)
		{
			Node currentNode = GetLowestFCostNode(openNodes);

			openNodes.Remove(currentNode);
			closedNodes.Add(currentNode);

			if (currentNode == endNode)
			{
				RetracePath(startNode, endNode);
				return;
			}

			foreach (Node neighbour in gridManager.GetNeighbours(currentNode))
			{
				if (!neighbour.walkable || closedNodes.Contains(neighbour))
				{
					continue;
				}

				int newCost = currentNode.gCost + GetDistance(currentNode, neighbour);

				if (newCost < neighbour.gCost || !openNodes.Contains(neighbour))
				{
					neighbour.gCost  = newCost;
					
					neighbour.hCost  = GetDistance(neighbour, endNode);
					neighbour.parent = currentNode;

					if (!openNodes.Contains(neighbour))
					{
						openNodes.Add(neighbour);
					}
				}
			}
		}
	}

	int GetDistance(Node nodeA, Node nodeB)
	{
		const int diagonalCost = 14;
		const int straightCost = 10;

		int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		return distX > distY
			? diagonalCost * distY + straightCost * (distX - distY)
			: diagonalCost * distX + straightCost * (distY - distX);
	}

	void RetracePath(Node startNode, Node endNode)
	{
		List<Node> path = new();

		Node currentNode = endNode;

		while (currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}

		path.Reverse();
		gridManager.path = path;
	}

	Node GetLowestFCostNode(List<Node> nodes)
	{
		Node bestNode = nodes[0];

		foreach (Node node in nodes)
		{
			if (node.fCost < bestNode.fCost || (node.fCost == bestNode.fCost && node.hCost < bestNode.hCost))
			{
				bestNode = node;
			}
		}

		return bestNode;
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, RaduisAttack);
	}
}
