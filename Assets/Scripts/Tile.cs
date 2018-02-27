using UnityEngine;
using System.Collections;

public class Tile {



	public enum TileType {
		Blank,
		Wall,
		Entrance,
		BeaconPart,
		FoodStore
	}

	public TileType Type { get; protected set; }

	public int X { get; protected set; }
	public int Y { get; protected set; }

	public bool TraversableWhenVisible { get; protected set; }
	public bool Traversable {
		get {
			return TraversableWhenVisible && Visible;
		}
	}
	public int PathingCost { get; protected set; }

	public bool Visible { get; protected set; }

	public float FoodAmount;

	private const int minFood = 100;
	private const int maxFood = 500;

	public Tile(int x, int y, TileType type) {
		X = x;
		Y = y;
		Type = type;

		SetTileType(type);

		PathingCost = 1;
		Visible = false;
	}

	public void SetTileType(TileType type) {
		Type = type;
		if(Type == TileType.Blank || Type == TileType.Entrance || Type == TileType.FoodStore || Type == TileType.BeaconPart) {
			TraversableWhenVisible = true;
		} else {
			TraversableWhenVisible = false;
		}

		if(Type == TileType.FoodStore) {
			FoodAmount = Random.Range(minFood, maxFood);
		}
	}

	public void SetVisible() {
		Visible = true;
	}

}
