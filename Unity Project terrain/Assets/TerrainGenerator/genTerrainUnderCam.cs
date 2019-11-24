using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


public class genTerrainUnderCam : MonoBehaviour
{
    public GameObject Terrain;
    public LinkedList<GameObject> tiles;

    public int SIZE;

    public float altitude;


    // Start is called before the first frame update
    void Start()
    {
        GameObject b;
        Vector3 initePos = transform.position;
        initePos.Set(transform.position.x - SIZE/2, transform.position.y - 50, transform.position.z - SIZE/2);
        altitude = transform.position.y - 50;
        b = Instantiate(Terrain, initePos, Quaternion.identity) as GameObject;
        tiles = new LinkedList<GameObject>();
        tiles.AddLast(b);
    }

    bool tileExists(Vector3 posPlayer, int i, int j){
        foreach (GameObject tile in tiles)
        {
            bool inX = tile.transform.position.x < posPlayer.x + i*SIZE && posPlayer.x  + i*SIZE < tile.transform.position.x + SIZE;
            bool inZ = tile.transform.position.z < posPlayer.z + j*SIZE && posPlayer.z  + j*SIZE < tile.transform.position.z + SIZE;

            if( inX && inZ ){
                return true;
            }
        }
        return false;
    }

    Vector3 posCurrentTile(Vector3 posPlayer){
        foreach (GameObject tile in tiles)
        {
            bool inX = tile.transform.position.x < posPlayer.x && posPlayer.x < tile.transform.position.x + SIZE;
            bool inZ = tile.transform.position.z < posPlayer.z && posPlayer.z < tile.transform.position.z + SIZE;

            if( inX && inZ ){
                return tile.transform.position;
            }
        }
        return Terrain.transform.position;
    }

    void creation(){
        for(int i=-1; i<2; i++){
            for(int j=-1; j<2; j++){
                Vector3 posPlayer = transform.position;
                if (tileExists(posPlayer, i, j) == false)
                {
                    Vector3 posCurrent = posCurrentTile(posPlayer);
                    posCurrent.Set(posCurrent.x + i*SIZE, altitude, posCurrent.z + j*SIZE);
                    tiles.AddLast(Instantiate(Terrain, posCurrent, Quaternion.identity));
                }
            }
        }
    }

    void deletion(){
        List<GameObject> tilesToDestroy = new List<GameObject>();
        foreach (GameObject tile in tiles)
        {
            Vector3 centerOfTile = tile.transform.position;
            centerOfTile.Set(tile.transform.position.x + SIZE/2, tile.transform.position.y, tile.transform.position.z + SIZE/2);
            float distanceCam = Vector3.Distance(transform.position, centerOfTile);
            if(distanceCam > SIZE*3){
                tilesToDestroy.Add(tile);
            }
        }

        foreach (GameObject tile in tilesToDestroy)
        {
            tiles.Remove(tile);
            /*
            if (tile.transform.childCount > 0)
            {
                while (tile.transform.childCount > 0)
                {
                    Transform child = tile.transform.GetChild(0);
                    child.parent = null;
                    Destroy(child.gameObject);
                }
            }*/
            Destroy(tile);
        }
    }

    // Update is called once per frame
    void Update()
    {
        creation();
        deletion();
    }
}
