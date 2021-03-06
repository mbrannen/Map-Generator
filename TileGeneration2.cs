using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGeneration2 : MonoBehaviour 
{

	public GameObject GameState;
	public GameObject MapLayer;
	public GameObject PollutionLayer;
	Master db;
	//adds public vars to be editable from the unity editor when experimenting with map sizes
	//Public vars are attached to MapGenerator in Hierarchy
	public int mapDimX; //map dimensions x axis
	public int mapDimY; //map dimensions y axis
	[Range(0f,30f)]
	public float sensitivity; //noise generation attenuator with a range slider of 0-30
	[Range(0f,200f)]
	public int seed;  //seed offset for generating different maps at specific sensitivities with a range slider of 0-200
	public GameObject[] tiles; // array to put different tile prefabs for placing
	public GameObject[] pollutionTiles;
	int[,] mapGrid; //initites 2D array to hold the map's grid information

	void onEnable()
	{
		
	}
	void Start () 
	{
		db = GameState.GetComponent<Master>();
		GenerateMap(mapDimX, mapDimY); //runs the map generation at the start (after play is pressed)
	}

	//algorithm to generate map
	void GenerateMap(int mapDimX, int mapDimY)
	{
		//generates a noise map for placing the tiles and stores in 2D array mapGrid
		mapGrid = new int[mapDimX,mapDimY];
		for(int i = 0; i < mapDimY; i++)
		{
			for(int j = 0; j < mapDimX; j++)
			{
				//uses Perlin Noise to populate the 2D array mapGrid with integers from 0 - 10
				mapGrid[j,i] = (int) (Mathf.PerlinNoise(j/sensitivity + seed, i/sensitivity + seed)*10);

			}
		}

		//generates streets along y axis
		//loops through rows at random distances and only places roads on population tiles
		//ignores grass and water
		int randomStart = Random.Range(0,10);
		for(int n = 0; n < 50; n++)
		{
			for(int j = 0; j < mapDimY; j++)
			{
				//looks to see if tile is a bulding/population tile
				if(mapGrid[randomStart, j] >= 4 && mapGrid[randomStart, j] <= 10)
				{
					//converts all population tiles in selected row to a road 
					mapGrid[randomStart, j] = -1;
				}
			}
			//adds a random interval to go to the next row to place roads
			randomStart += Random.Range(3,10);

			//once it reaches the map end then stop
			if( randomStart >= mapDimX)break;
		}

		//generates streets along x axis
		//loops through rows at random distances and only places roads on population tiles
		//ignores grass and water
		int randomStart2 = Random.Range(0,10);

		//will place max of 50 rows, just a large amount that most likely will be reached with enormous map sizes
		for(int n = 0; n < 50; n++)
		{
			for(int j = 0; j < mapDimX; j++)
			{
				//looks to see if tile is a bulding/population tile
				if(mapGrid[j,randomStart2] >= 4 && mapGrid[randomStart2, j] <= 10)
				{
					//converts all population tiles in selected row to a road
					mapGrid[j,randomStart2] = -1;
				}
			}
			//adds a random interval to go to the next row to place roads
			randomStart2 += Random.Range(3,10);

			//once it reaches the map end then stop
			if( randomStart2 >= mapDimY)break; 
		}

		//places tiles based upon noise map stored in mapGrid
		for(int i = 0; i < mapDimY; i++)
		{
			
			for(int j = 0; j < mapDimX; j++)
			{
				
				int tileType = mapGrid[j,i];
				Vector3  position= new Vector3(j*10, 0, i*10);
				Vector3 pollPosition = new Vector3(j*10, 4, i*10);
				if(tileType < 0)
				{
					//instantiate road tile
					GameObject tile = Instantiate(tiles[5], position,Quaternion.identity, MapLayer.transform);
					GameObject pollution = Instantiate(pollutionTiles[2], pollPosition,Quaternion.identity, PollutionLayer.transform);
					tile.name = "RoadTile" + " X" + j + "Y" + i;
				}
				else if(tileType < 2)
				{
					//instantiate a tile and name it according to its position and type
					GameObject tile = Instantiate(tiles[0], position,Quaternion.identity, MapLayer.transform);
					GameObject pollution = Instantiate(pollutionTiles[0], pollPosition,Quaternion.identity, PollutionLayer.transform);
					tile.name = "WaterTile" + " X" + j + "Y" + i;
					db.waterCount++;
				}
				else if( tileType < 4)
				{
					//instantiate a tile and name it according to its position and type
					GameObject tile = Instantiate(tiles[1], position,Quaternion.identity, MapLayer.transform);
					GameObject pollution = Instantiate(pollutionTiles[0], pollPosition,Quaternion.identity, PollutionLayer.transform);
					tile.name = "GrassTile" + " X" + j + "Y" + i;
					db.grassCount++;
				}
				else if( tileType < 6)
				{
					//instantiate a tile and name it according to its position and type
					GameObject tile = Instantiate(tiles[2], position,Quaternion.identity, MapLayer.transform);
					GameObject pollution = Instantiate(pollutionTiles[1], pollPosition,Quaternion.identity, PollutionLayer.transform);
					tile.name = "SuburbTile" + " X" + j + "Y" + i;
					db.suburbCount++;
				}
				else if( tileType < 8)
				{
					//instantiate a tile and name it according to its position and type
					GameObject tile = Instantiate(tiles[3], position,Quaternion.identity, MapLayer.transform);
					GameObject pollution = Instantiate(pollutionTiles[2], pollPosition,Quaternion.identity, PollutionLayer.transform);
					tile.name = "UrbanTile" + " X" + j + "Y" + i;
					db.urbanCount++;
				}
				else if( tileType <= 10)
				{
					//instantiate a tile and name it according to its position and type
					GameObject tile = Instantiate(tiles[4], position,Quaternion.identity, MapLayer.transform);
					GameObject pollution = Instantiate(pollutionTiles[3], pollPosition,Quaternion.identity, PollutionLayer.transform);
					tile.name = "BusinessTile" + " X" + j + "Y" + i;
					db.metroCount++;
				}
			}
		}

		db.pollutionToggle(false);
		return; //end map generation
	}
	
}
