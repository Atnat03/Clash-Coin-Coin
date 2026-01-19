using System;
using System.Collections.Generic;
using UnityEngine;

public class Asta : MonoBehaviour
{
	Transform target;

	GridManager gridManager;
	[SerializeField] private Seeker player;

	private float t = 1;


	void Update()
	{
		if (t >= 0)
		{
			t = 1;
		}
		
		FindPath(transform.position, target.position);
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
}
