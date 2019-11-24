using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    public float[,] map;
    public float altmoy;
    public int SIZE;
    public int octaves;
    public float STRIDE;
    public float AMPLITUDE;
    public Vector3[] vertices;
    public Vector2[] uv;
    public int[] triangles;
    public GameObject maze;
    public bool mazeOrNot;
    public GameObject tree;
    public GameObject vegeta;

	// Use this for initialization
	void Start () {

        mazeOrNot = false;
        float prob = Random.Range(0, 1f);
        if (prob < 0.3)
        {
            mazeOrNot = true;
        }

        map = new float[SIZE, SIZE];
        for (int i = 0; i < SIZE; i++){
            for (int j = 0; j < SIZE; j++){
                map[i, j] = 0;
            }
        }

        float som = 0;
        for(int oct=1; oct<octaves+1; oct++){
            for (int i = 0; i < SIZE; i++){
                for (int j = 0; j < SIZE; j++){
                    map[i, j] = map[i, j] + AMPLITUDE * Mathf.PerlinNoise((transform.position.x + i) * STRIDE * oct + 2000f, (transform.position.z + j) * STRIDE * oct + 2000f) / (oct*oct*oct);
                }
            }
        }

        for (int i = 0; i < SIZE; i++){
            for (int j = 0; j < SIZE; j++){
                som = som + map[i, j];
            }
        }
        altmoy = som / (SIZE*SIZE);

        if (mazeOrNot)
        {
            for (int i = 0; i < SIZE; i++){
                for (int j = 0; j < SIZE; j++){
                    if(i>=2 && i<27 && j>=2 && j<27){
                        map[i, j] = altmoy;
                    }
                    
                }
            }
        }

        vertices = new Vector3[SIZE * SIZE];
        uv = new Vector2[SIZE * SIZE];
        triangles = new int[(SIZE - 1) * (SIZE - 1) * 2 * 3];

        for (int i = 0; i < SIZE; i++)
        {
            for (int j = 0; j < SIZE; j++)
            {
                vertices[i+ SIZE * j] = new Vector3(i, map[i, j], j);
                uv[i + SIZE * j] = new Vector2( i * 1f / (SIZE -1), j * 1f / (SIZE -1));
            }
        }


        int pos = 0;
        for (int j = 0; j < (SIZE - 1); j++){
            for (int i = 0; i < (SIZE - 1); i++){
                triangles[pos] = i + j * SIZE;
                triangles[pos + 1] = i + j * SIZE + SIZE;
                triangles[pos + 2] = i + j * SIZE + SIZE + 1;
                triangles[pos + 3] = i + j * SIZE;
                triangles[pos + 4] = i + j * SIZE + SIZE + 1;
                triangles[pos + 5] = i + j * SIZE + 1;
                pos += 6;

            }
        }

        Mesh mesh = GetComponent<MeshFilter>().mesh = new Mesh();
        MeshCollider c = GetComponent<MeshCollider>();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        c.sharedMesh = mesh;

        if (mazeOrNot)
        {
            GameObject laby;
            Vector3 labyPos = transform.position;
            labyPos.Set( transform.localPosition.x + 5, transform.localPosition.y + altmoy, transform.localPosition.z + 5);
            laby = Instantiate(maze, labyPos, Quaternion.identity) as GameObject;
            laby.transform.SetParent(this.transform, false);

        }else{

            int numberOfTrees = Random.Range(0,5);
            int numberOfVegeta = Random.Range(0,10);

            for (int i = 0; i < numberOfTrees; i++)
            {
                int x = Random.Range(0, 29);
                int z = Random.Range(0, 29);

                GameObject treeObject;
                Vector3 treePos = transform.position;
                treePos.Set( transform.localPosition.x + x, transform.localPosition.y + map[x, z], transform.localPosition.z + z);
                treeObject = Instantiate(tree, treePos, Quaternion.identity) as GameObject;
                treeObject.transform.SetParent(this.transform);
            }

            for (int i = 0; i < numberOfVegeta; i++)
            {
                int x = Random.Range(0, 29);
                int z = Random.Range(0, 29);

                GameObject vegeObject;
                Vector3 vegePos = transform.position;
                vegePos.Set( transform.localPosition.x + x, transform.localPosition.y + map[x, z], transform.localPosition.z + z);
                vegeObject = Instantiate(vegeta, vegePos, Quaternion.identity) as GameObject;
                vegeObject.transform.SetParent(this.transform);
            }
        }


        
    }
	
	// Update is called once per frame
	void Update () {
	}
}
