using System;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX, gridY;
    public int gCost;
    public int hCost;
    public Node parent;
    
    public int fCost => gCost + hCost;
    
    public Node(bool _wakable, Vector3 worldPos, int X, int Y)
    {
        walkable = _wakable;
        worldPosition = worldPos;
        gridX = X;
        gridY = Y;
    }
}

public class GridManager : MonoBehaviour
{
	public static GridManager instance;
	
	[SerializeField] LayerMask unWalkableMask;
	[SerializeField] Vector2   gridWorldSize;
	[SerializeField] float     nodeRadius;
	Node[,]                    grid;
	int                        gridSizeX, gridSizeY;
	float                      nodeDiameter;

	public List<Node> path;

	void Awake()
	{
		instance = this;
		
		nodeDiameter = nodeRadius * 2f;

		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

		CreateGrid();
	}
	
	void CreateGrid()
	{
		grid = new Node[gridSizeX, gridSizeY];

		Vector3 worldBottomLeft = transform.position - Vector3.right   * (gridWorldSize.x * 0.5f - nodeRadius)
													 - Vector3.forward * (gridWorldSize.y * 0.5f - nodeRadius);

		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y < gridSizeY; y++)
			{
				Vector3 worldPoint = worldBottomLeft + Vector3.right   * (x * nodeDiameter)
													 + Vector3.forward * (y * nodeDiameter);

				bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unWalkableMask);

				
				RaycastHit hit;
				
				grid[x, y] = new Node(walkable, worldPoint, x, y);
			}
		}
	}

	public List<Node> GetNeighbours(Node node)
	{
		List<Node> neighbours = new();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0)
				{
					continue;
				}

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
				{
					neighbours.Add(grid[checkX, checkY]);
				}
			}
		}

		return neighbours;
	}

	public Node NodeFromWorldPosition(Vector3 worldPosition)
	{
		float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

		return grid[x, y];
	}
}
