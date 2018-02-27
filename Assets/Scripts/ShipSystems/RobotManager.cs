using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RobotManager : ShipSystem {

	public static RobotManager Instance;

	public float RobotTileTraverseTime = 5;
	public float TileSizeInMetres = 10;

	public RectTransform MapDisplay;
	public GameObject Tile_Wall;
	public GameObject Tile_Entrance;
	public GameObject Tile_BeaconPart;
	public GameObject Tile_FoodStore;
	public GameObject Tile_Invisible;

	public RectTransform RobotContainer;
	public GameObject Robot;

	public RectTransform PathContainer;
	public GameObject PathNode;

	public Text BaseSpeedText;
	public Text DistanceText;
	public Text TimeText;
	public Text InventoryText;

	public GameObject BrokenCover;
	public GameObject NoPowerCover;

	public float MapWorldWidth;
	public float MapWorldHeight;

	public float FoodPerStore = 100;

	public Transform MapDisplayTopLeftMarker;
	public Transform MapDisplayBottomRightMarker;

	public enum RobotState {
		Idle,
		Collecting,
		Moving
	}
	public RobotState State { get; protected set; }

	public int RobotX { get; protected set; }
	public int RobotY { get; protected set; }

	public Tile RobotTile {
		get {
			return mapManager.GetTileAt(RobotX, RobotY);
		}
	}

	public enum RobotInventoryState {
		Empty,
		Food,
		Part
	}
	private RobotInventoryState inventoryState;
	public RobotInventoryState InventoryState {
		get {
			return inventoryState;
		}
		protected set {
			inventoryState = value;
			if(inventoryState == RobotInventoryState.Empty) {
				InventoryText.text = "Empty";
			} else if(inventoryState == RobotInventoryState.Food) {
				InventoryText.text = "Human ingestible material (" + FoodPerStore + ")";
			} else {
				InventoryText.text = "Beacon part";
			}
		}
	}

	public delegate void RobotPositionEventHandler(int x, int y);
	public event RobotPositionEventHandler OnRobotMoved;

	public delegate void RobotGeneralEventHandler();
	public event RobotGeneralEventHandler OnRobotPartReturn;

	public delegate void RobotFloatEventHandler(float amount);
	public event RobotFloatEventHandler OnRobotFoodReturn;
		
	private MapManager mapManager;
	private Pathfinding pathfinding;
	private ShipResourceManager shipResources;

	private Dictionary<Tile, GameObject> tileGameObjectMap;

	private List<Tile> robotPath;
	private Tile currentDestTile;
	private float currentTraverseProgress;

	private float displayTileWidth;
	private float displayTileHeight;

	void Awake() {
		Instance = this;
	}

	protected override void Start() {
		base.Start();

		mapManager = MapManager.Instance;
		pathfinding = Pathfinding.Instance;
		shipResources = ShipResourceManager.Instance;

		mapManager.OnMapChange += MapChanged;
		mapManager.OnTileChange += TileChanged;
		OnRobotMoved += RobotMoved;
		SystemBreakDown += OnBroken;
		SystemFixed += OnFixed;
		SystemPowerAvailable += OnPowerAvailable;
		SystemPowerUnavailable += OnPowerUnavailable;

		State = RobotState.Idle;
		RobotX = mapManager.EntranceX;
		RobotY = mapManager.EntranceY;

		displayTileWidth = MapDisplay.rect.width / mapManager.MapWidth;
		displayTileHeight = MapDisplay.rect.height / mapManager.MapHeight;

		tileGameObjectMap = new Dictionary<Tile, GameObject>();

		if(OnRobotMoved != null) {
			OnRobotMoved(RobotX, RobotY);
		}

		BaseSpeedText.text = TileSizeInMetres / (RobotTileTraverseTime / 60) + " m/hour";
		DistanceText.text = "-";
		TimeText.text = "-";
		InventoryText.text = "Empty";
		BrokenCover.SetActive(false);
		NoPowerCover.SetActive(false);
		
		DrawMap();
		RobotMoved(RobotX, RobotY);
	}

	protected override void Update() {
		base.Update();

		if(State == RobotState.Moving) {
			currentTraverseProgress += (1 / RobotTileTraverseTime) * TimeManager.Instance.GameDeltaTime;

			float tileDistance = robotPath.Count + (1 - currentTraverseProgress);
			DistanceText.text = Mathf.Round(tileDistance * TileSizeInMetres) + " m";
			TimeText.text = Mathf.Round(((tileDistance * RobotTileTraverseTime) / 60) * 10) / 10 + " hours";

			if(currentTraverseProgress >= 1) {
				RobotX = currentDestTile.X;
				RobotY = currentDestTile.Y;
				if(OnRobotMoved != null) {
					OnRobotMoved(RobotX, RobotY);
				}

				if(robotPath.Count > 0) {
					currentDestTile = robotPath[0];
					robotPath.RemoveAt(0);
					currentTraverseProgress = 0;
				} else {
					FinishMoving();
				}
			}
		}
	}

	public void MoveRobotTo(int x, int y) {
		MoveRobotTo(mapManager.GetTileAt(x, y));
	}

	public void MoveRobotTo(Tile destTile) {
		List<Tile> newPath = pathfinding.FindPathTiles(RobotTile, destTile);

		if(newPath != null && newPath.Count > 0) {
			State = RobotState.Moving;
			robotPath = newPath;
			DrawPath();

			currentDestTile = robotPath[0];
			robotPath.RemoveAt(0);
			currentTraverseProgress = 0;
		}
	}

	public void DisplayClick(Vector3 pos) {
		if(Active) {
			Tile tile = WorldCoordsToDisplayedTile(pos);

			if(tile != null) {
				if(tile.Visible) {
					MoveRobotTo(tile);
				}
			}
		}
	}

	Tile WorldCoordsToDisplayedTile(Vector3 pos) {
		float mapWorldX = MapDisplayTopLeftMarker.position.x;
		float mapWorldY = MapDisplayTopLeftMarker.position.y;
		float mapWidth = MapDisplayTopLeftMarker.position.x - MapDisplayBottomRightMarker.position.x;
		float mapHeight = MapDisplayTopLeftMarker.position.y - MapDisplayBottomRightMarker.position.y;

		int x = Mathf.FloorToInt((-pos.x + mapWorldX) / (mapWidth / mapManager.MapWidth));
		int y = mapManager.MapHeight - Mathf.FloorToInt((-pos.y + mapWorldY) / (mapHeight / mapManager.MapHeight)) - 1;

		return mapManager.GetTileAt(x, y);
	}

	void OnBroken(ShipSystem system) {
		BrokenCover.SetActive(true);
	}

	void OnFixed(ShipSystem system) {
		BrokenCover.SetActive(false);
	}

	void OnPowerAvailable(ShipSystem system) {
		NoPowerCover.SetActive(false);
	}

	void OnPowerUnavailable(ShipSystem system) {
		NoPowerCover.SetActive(true);
	}

	void TileChanged(Tile tile) {
		if(tileGameObjectMap.ContainsKey(tile)) {
			GameObject oldGO = tileGameObjectMap[tile];
			Destroy(oldGO);
		}
		GameObject newGO = DrawTile(tile);
		tileGameObjectMap[tile] = newGO;
	}

	void MapChanged() {
		DrawMap();
	}

	void DrawMap() {
		tileGameObjectMap = new Dictionary<Tile, GameObject>();
		foreach(Transform child in MapDisplay.transform) {
			Destroy(child.gameObject);
		}

		for(int x = 0; x < mapManager.MapWidth; x++) {
			for(int y = 0; y < mapManager.MapHeight; y++) {
				Tile tile = mapManager.GetTileAt(x, y);
				GameObject go = DrawTile(tile);
				tileGameObjectMap[tile] = go;
			}
		}
	}

	void RobotMoved(int x, int y) {
		// Uncover map
		RobotTile.SetVisible();
		List<Tile> changedTiles = new List<Tile>();
		changedTiles.Add(RobotTile);
		foreach(Tile neighbour in mapManager.GetNeighbours(RobotTile, true)) {
			mapManager.SetVisible(neighbour);
			changedTiles.Add(neighbour);
		}
		mapManager.TilesChanged(changedTiles);

		// Update drawing
		DrawRobot();
		DrawPath();
	}

	void FinishMoving() {
		robotPath = null;
		currentDestTile = null;
		State = RobotState.Idle;

		if(InventoryState == RobotInventoryState.Empty) {
			if(RobotTile.Type == Tile.TileType.BeaconPart) {
				InventoryState = RobotInventoryState.Part;
				RobotTile.SetTileType(Tile.TileType.Blank);
				mapManager.TileChanged(RobotTile);
			} else if (RobotTile.Type == Tile.TileType.FoodStore) {
				InventoryState = RobotInventoryState.Food;
				RobotTile.SetTileType(Tile.TileType.Blank);
				mapManager.TileChanged(RobotTile);
			}
		} else {
			if(RobotTile.Type == Tile.TileType.Entrance) {
				if(InventoryState == RobotInventoryState.Food) {
					if(OnRobotFoodReturn != null) {
						OnRobotFoodReturn(FoodPerStore);
					}

					shipResources.ChangeFood(FoodPerStore);
				} else if(InventoryState == RobotInventoryState.Part) {
					if(OnRobotPartReturn != null) {
						OnRobotPartReturn();
					}
				}
				InventoryState = RobotInventoryState.Empty;
			}
		}

	}

	GameObject DrawTile(Tile tile) {
		if(tile.Type == Tile.TileType.Blank && tile.Visible) {
			return null;
		}

		GameObject tilePrefab = Tile_Wall;

		if(tile.Visible) {
			switch(tile.Type) {
			case Tile.TileType.Wall:
				tilePrefab = Tile_Wall;
				break;
			case Tile.TileType.Entrance:
				tilePrefab = Tile_Entrance;
				break;
			case Tile.TileType.BeaconPart:
				tilePrefab = Tile_BeaconPart;
				break;
			case Tile.TileType.FoodStore:
				tilePrefab = Tile_FoodStore;
				break;
			}
		} else {
			tilePrefab = Tile_Invisible;
		}

		RectTransform newTileDisplay = Instantiate(tilePrefab).GetComponent<RectTransform>();
		newTileDisplay.SetParent(MapDisplay);
		newTileDisplay.sizeDelta = new Vector2(displayTileWidth, displayTileHeight);
		newTileDisplay.localScale = Vector3.one;
		newTileDisplay.localRotation = Quaternion.identity;
		newTileDisplay.localPosition = Vector3.zero;
		newTileDisplay.anchoredPosition = new Vector2(tile.X * displayTileWidth, tile.Y * displayTileHeight);

		return newTileDisplay.gameObject;
	}

	void DrawRobot() {
		foreach(Transform child in RobotContainer) {
			Destroy(child.gameObject);
		}

		RectTransform robot = Instantiate(Robot).GetComponent<RectTransform>();
		robot.SetParent(RobotContainer);
		robot.sizeDelta = new Vector2(displayTileWidth, displayTileHeight);
		robot.localScale = Vector3.one;
		robot.localRotation = Quaternion.identity;
		robot.localPosition = Vector3.zero;
		robot.anchoredPosition = new Vector2(RobotX * displayTileWidth, RobotY * displayTileHeight);
	}

	void DrawPath() {
		foreach(Transform child in PathContainer) {
			Destroy(child.gameObject);
		}

		if(robotPath != null) {
			foreach(Tile tile in robotPath) {
				RectTransform pathNode = Instantiate(PathNode).GetComponent<RectTransform>();
				pathNode.SetParent(PathContainer);
				pathNode.sizeDelta = new Vector2(displayTileWidth, displayTileHeight);
				pathNode.localScale = Vector3.one;
				pathNode.localRotation = Quaternion.identity;
				pathNode.localPosition = Vector3.zero;
				pathNode.anchoredPosition = new Vector2(tile.X * displayTileWidth, tile.Y * displayTileHeight);
			}
		}
	}

}
