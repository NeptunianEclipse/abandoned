using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {

	public static MapManager Instance;

	public int MapWidth = 50;
	public int MapHeight = 50;

	public int EntranceX { get { return map.EntranceX; } }
	public int EntranceY { get { return map.EntranceY; } }

	public delegate void MapEventHandler();
	public event MapEventHandler OnMapChange;

	public delegate void TileEventHandler(Tile tile);
	public event TileEventHandler OnTileChange;

	private Map map;

	private Pathfinding pathfinding;

	void Awake() {
		Instance = this;
	}

	void Start() {
		pathfinding = Pathfinding.Instance;

		map = new Map(MapWidth, MapHeight);

		if(OnMapChange != null) {
			OnMapChange();
		}

		pathfinding.CreateNodeGrid();
	}

	public Tile GetTileAt(int x, int y) {
		return map.GetTileAt(x, y);
	}

	public List<Tile> GetNeighbours(Tile tile, bool includeDiagonals) {
		return map.GetNeighbours(tile, includeDiagonals);
	}

	public void SetVisible(Tile tile) {
		tile.SetVisible();
	}

	public void TilesChanged() {
		if(OnMapChange != null) {
			OnMapChange();
		}
	}

	public void TilesChanged(List<Tile> tileList) {
		foreach(Tile tile in tileList) {
			TileChanged(tile);
		}
	}

	public void TileChanged(Tile tile) {
		pathfinding.TileChanged(tile);
		if(OnTileChange != null) {
			OnTileChange(tile);
		}
	}

}
