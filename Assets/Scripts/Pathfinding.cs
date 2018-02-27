﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Component that provides path finding functionality (A*) to other classes
public class Pathfinding : MonoBehaviour {

	public static Pathfinding Instance;

	public class Node {

		public int x;
		public int y;

		public bool traversable;
		public float pathingCost;

		public float gCost;
		public float hCost;
		public float fCost {
			get { return gCost + hCost; }
		}

		public Node parentNode;

		public Node(bool traversable, int pathingCost, int x, int y) {
			this.traversable = traversable;
			this.pathingCost = pathingCost;
			this.x = x;
			this.y = y;
		}

	}

	public delegate void NodeEventHandler(Node node);
	public event NodeEventHandler onNodeChange;

	public bool allowDiagonalTraversal;

	const int directTraverseCost = 10;
	const int diagonalTraverseCost = 14;

	MapManager mapManager;

	Node[,] nodeGrid;

	List<Node> drawPath;
	
	public void CreateNodeGrid() {
		nodeGrid = new Node[mapManager.MapWidth, mapManager.MapHeight];

		for(int x = 0; x < mapManager.MapWidth; x++) {
			for(int y = 0; y < mapManager.MapHeight; y++) {
				Tile currentTile = mapManager.GetTileAt(x, y);
				UpdateNode(currentTile, x, y);
			}
		}
	}

	public void TileChanged(Tile tile) {
		UpdateNode(tile, tile.X, tile.Y);
		if(onNodeChange != null)
			onNodeChange(nodeGrid[tile.X, tile.Y]);
	}

	public List<Node> GetNeighbours(Node node) {
		List<Node> neighbours = new List<Node>();

		for(int x = -1; x <= 1; x++) {
			for(int y = -1; y <= 1; y++) {
				if(x == 0 && y == 0)
					continue;

				if(!allowDiagonalTraversal && (x + y == 2 || x + y == -2 || x + y == 0))
					continue;

				int checkX = node.x + x;
				int checkY = node.y + y;

				if(checkX >= 0 && checkX < mapManager.MapWidth && checkY >= 0 && checkY < mapManager.MapHeight) {
					neighbours.Add(nodeGrid[checkX, checkY]);
				}
			}
		}

		return neighbours;
	}

	public List<Node> FindPath(Tile startTile, Tile targetTile) {
		return FindPath(nodeGrid[startTile.X, startTile.Y], nodeGrid[targetTile.X, targetTile.Y]);
	}

	public List<Tile> FindPathTiles(Tile startTile, Tile targetTile) {
		List<Node> nodePath = FindPath(nodeGrid[startTile.X, startTile.Y], nodeGrid[targetTile.X, targetTile.Y]);
		return NodesToTiles(nodePath);
	}

	public List<Tile> FindPathTiles(Node startNode, Node targetNode) {
		List<Node> nodePath = FindPath(startNode, targetNode);
		return NodesToTiles(nodePath);
	}

	public List<Node> FindPath(Node startNode, Node targetNode) {
		List<Node> openSet = new List<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);

		List<Node> path = new List<Node>();

		while(openSet.Count > 0) {
			Node currentNode = openSet[0];

			for(int i = 1; i < openSet.Count; i++) {
				if(openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
					currentNode = openSet[i];
			}

			openSet.Remove(currentNode);
			closedSet.Add(currentNode);

			if(currentNode == targetNode) {
				path = RetracePath(startNode, targetNode);
				return path;
			}

			foreach(Node neighbour in GetNeighbours(currentNode)) {
				if(!neighbour.traversable || closedSet.Contains(neighbour))
					continue;

				float newMovementCostToNeighbour = currentNode.gCost + GetTraverseCost(currentNode, neighbour);
				if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains (neighbour)) {
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parentNode = currentNode;

					if(!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}

		return path;
	}

	List<Node> RetracePath(Node startNode, Node targetNode) {
		List<Node> path = new List<Node>();
		Node currentNode = targetNode;

		while(currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parentNode;
		}

		path.Reverse();
		return path;
	}

	List<Tile> NodesToTiles(List<Node> nodePath) {
		List<Tile> tilePath = new List<Tile>();
		for(int i = 0; i < nodePath.Count; i++) {
			tilePath.Add(mapManager.GetTileAt(nodePath[i].x, nodePath[i].y));
		}
		return tilePath;
	}

	float GetTraverseCost(Node node, Node neighbour) {
		if(node.x == neighbour.x || node.y == neighbour.y)
			return directTraverseCost * neighbour.pathingCost;
		return diagonalTraverseCost * neighbour.pathingCost;
	}

	float GetDistance(Node nodeA, Node nodeB) {
		return Mathf.Abs(nodeA.x - nodeB.x) + Mathf.Abs(nodeA.y - nodeB.y);
	}

	void UpdateNode(Tile tile, int x, int y) {
		if(nodeGrid[x, y] == null) {
			nodeGrid[x, y] = new Node(tile.Traversable, tile.PathingCost, x, y);
		} else {
			nodeGrid[x, y].traversable = tile.Traversable;
			nodeGrid[x, y].pathingCost = tile.PathingCost;
		}
	}

	void Start() {
		mapManager = MapManager.Instance;
		Instance = this;
		//mapManager.map.onTileChange += TileChanged;
	}

}
