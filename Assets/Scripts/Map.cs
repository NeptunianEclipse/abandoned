using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map {

	public int Width { get; private set; }
	public int Height { get; private set; }

	public int EntranceX { get; private set; }
	public int EntranceY { get; private set; }

	private Tile[,] tiles;


	private int[,] testMap = {
		{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
		{1,0,0,0,0,0,1,4,0,0,0,0,1,4,1},
		{1,0,1,1,1,0,1,1,3,1,1,0,1,0,1},
		{1,0,1,0,0,0,0,1,1,1,0,0,1,0,1},
		{1,3,1,0,1,1,0,0,0,1,0,1,1,0,1},
		{1,0,0,0,0,1,1,1,0,0,0,1,0,0,1},
		{1,1,1,0,1,1,4,1,1,1,0,1,0,1,1},
		{1,0,0,0,0,1,0,1,0,1,0,0,0,0,2},
		{1,0,0,0,4,1,0,1,0,1,0,1,1,1,1},
		{1,0,1,1,1,1,0,0,0,1,0,0,0,0,1},
		{1,0,1,0,0,0,0,1,1,1,1,1,1,0,1},
		{1,0,1,0,1,0,0,0,1,3,1,0,0,0,1},
		{1,0,1,0,0,0,1,0,1,0,1,0,1,0,1},
		{1,0,0,0,3,0,1,0,0,0,0,0,1,4,1},
		{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
	};


	public Map(int width, int height) {
		Width = width;
		Height = height;

		LoadMapFromIntArray(testMap);
	}

	public Tile GetTileAt(int x, int y) {
		if(x >= 0 && x < Width && y >= 0 && y < Height) {
			return tiles[x, y];
		} else {
			return null;
		}

	}

	public List<Tile> GetNeighbours(Tile tile, bool includeDiagonals) {
		List<Tile> neighbours = new List<Tile>();

		if(tile.X > 0)
			neighbours.Add(GetTileAt(tile.X - 1, tile.Y));

		if(tile.X < Width - 1)
			neighbours.Add(GetTileAt(tile.X + 1, tile.Y));

		if(tile.Y > 0)
			neighbours.Add(GetTileAt(tile.X, tile.Y - 1));

		if(tile.Y < Height - 1)
			neighbours.Add(GetTileAt(tile.X, tile.Y + 1));

		if(includeDiagonals) {
			if(tile.X > 0 && tile.Y > 0)
				neighbours.Add(GetTileAt(tile.X - 1, tile.Y - 1));

			if(tile.X > 0 && tile.Y < Height - 1)
				neighbours.Add(GetTileAt(tile.X - 1, tile.Y + 1));

			if(tile.X < Width - 1 && tile.Y > 0)
				neighbours.Add(GetTileAt(tile.X + 1, tile.Y - 1));

			if(tile.X < Width - 1 && tile.Y < Height - 1)
				neighbours.Add(GetTileAt(tile.X + 1, tile.Y + 1));
		}

		return neighbours;
	}

	protected Tile[,] CreateBlankMap(int width, int height) {
		Tile[,] newMap = new Tile[width, height];
		for(int x = 0; x < width; x++) {
			for(int y = 0; y < height; y++) {
				Tile newTile = new Tile(x, y, Tile.TileType.Blank);
				if(Random.Range(0, 2) == 1) {
					newTile.SetTileType(Tile.TileType.Wall);
				}

				newMap[x, y] = newTile;
			}
		}
		return newMap;
	}

	protected void LoadMapFromIntArray(int[,] array) {
		Tile[,] newMap = new Tile[array.GetLength(0), array.GetLength(1)];
		int entranceX = 0;
		int entranceY = 0;

		for(int x = 0; x < array.GetLength(0); x++) {
			for(int y = 0; y < array.GetLength(1); y++) {
				Tile newTile = new Tile(x, y, Tile.TileType.Blank);

				switch(array[x, y]) {
				case 1:
					newTile.SetTileType(Tile.TileType.Wall);
					break;
				case 2:
					newTile.SetTileType(Tile.TileType.Entrance);
					entranceX = x;
					entranceY = y;
					break;
				case 3:
					newTile.SetTileType(Tile.TileType.BeaconPart);
					break;
				case 4:
					newTile.SetTileType(Tile.TileType.FoodStore);
					break;
				case 5:
				default:
					break;
				}

				newMap[x, y] = newTile;
			}
		}

		EntranceX = entranceX;
		EntranceY = entranceY;
		tiles = newMap;
	}

}
