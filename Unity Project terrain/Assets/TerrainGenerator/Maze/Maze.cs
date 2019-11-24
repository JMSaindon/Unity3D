using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class Cell
{
    public GameObject floor;
    public GameObject northWall;
    public GameObject westWall;
    public bool north, west;
    public int val, x, y;

    public Cell(int i, int j)
    {
        x = i;
        y = j;
        val = 0;
        north = true;
        west = true;
    }
} 

public class Maze : MonoBehaviour
{
    public GameObject Floor;
    public GameObject NorthWall;
    public GameObject WestWall;
    public float CellSize;
    public float hauteur;
    public int GridSizeX;
    public int GridSizeY;
    List<Cell> doors;
    Cell entry;
    Cell[,] Grid;
    List<Cell> Border;
    public int[] dx;
    public int[] dy;

    void InitMaze() //fini
    {
        Grid = new Cell[GridSizeX, GridSizeY];
        dx = new int[] {0, 1, 0, -1};
        dy = new int[] {-1, 0, 1, 0};

        for(int i=0; i < GridSizeX; i++){
            for(int j=0; j < GridSizeY; j++){
                Grid[i, j] = new Cell(i, j);
            }
        }
    }

    void printVal() //fini
    {
        string alllines = "\n";
        for(int i=0; i < GridSizeX; i++){
            string line = "";
            for(int j=0; j < GridSizeY; j++){
                line = line + Grid[i, j].val;
            }
            alllines = alllines + line + "\n";
        }
        Debug.Log("printVal" + alllines);
    }

    void printGrid2D() //fini
    {
        string alllines = "\n";
        for(int i=0; i < GridSizeX; i++){
            string firstLine = "";
            string secondLine = "";

            for(int j=0; j < GridSizeY; j++){
                if((Grid[i,j].north
                    || Grid[i,j].west) 
                    || (!inside(i-1, j))
                    || (inside(i-1,j) && Grid[i-1,j].west)
                    || (inside(i,j-1) && Grid[i,j-1].north)){
                    firstLine = firstLine + "■";
                }else{
                    firstLine = firstLine + "o";
                }

                if(Grid[i,j].north){
                    firstLine = firstLine + "■";
                }else{
                    firstLine = firstLine + "o";
                }

                if(Grid[i,j].west){
                    secondLine = secondLine + "■";
                }else{
                    secondLine = secondLine + "o";
                }
                secondLine = secondLine + "o";
            }
            firstLine = firstLine + "■";
            secondLine = secondLine + "■";
            alllines = alllines + firstLine + "\n" + secondLine + "\n";
        }
        string lastline = "";
        for (int i = 0; i < 2*GridSizeX +1; i++)
        {
            lastline = lastline + "■";
        }
        alllines = alllines + lastline;
        Debug.Log("print2D" + alllines);

    }

    bool inside(int i, int j) //fini
    {
        return (i>=0 && i<GridSizeX && j>=0 && j<GridSizeY);
    }

    void updateBorder() //fini
    {
        List<Cell> border = new List<Cell>();

        //parcours labyrinthe
        for(int i=0; i < GridSizeX; i++){
            for(int j=0; j < GridSizeY; j++){

                //la case est dans le labyrinthe
                if(Grid[i,j].val == 1){

                    //parcours voisins
                    for(int k=0; k<4; k++){
                        if(inside(i+dx[k], j+dy[k]) && Grid[i+dx[k], j+dy[k]].val == 0){
                            Grid[i+dx[k], j+dy[k]].val = 2;
                        }
                    }
                }
            }
        }

        for(int i=0; i < GridSizeX; i++){
            for(int j=0; j < GridSizeY; j++){
                if(Grid[i,j].val == 2){
                    border.Add(Grid[i, j]);
                }
            }
        }
        Border = border;
    }

    bool mazeNotFinished() //fini
    {
        for(int i=0; i < GridSizeX; i++){
            for(int j=0; j < GridSizeY; j++){
                if(Grid[i,j].val != 1){
                    return true;
                }
            }
        }
        return false;
    }

    void DestroyWalls() //fini
    {
        int pos = Random.Range(0, 9);
        Cell firstCase = Grid[0,pos];

        firstCase.val = 1;
        entry = firstCase;

        while(mazeNotFinished()){
            updateBorder();

            //choix case dans la frontière
            int index = Random.Range(0,Border.Count);
            Cell c = new Cell(0,0);
            int count = 0;
            foreach (var item in Border)
            {
                if(count == index){
                    c = item;
                }
                count++;
            }

            //case intègre le labyrinthe
            c.val = 1;

            int i = c.x;
            int j = c.y;

            int xvois = i;
            int yvois = j;

            //parcours voisins
            for(int k=0; k<4; k++){
                if(inside(i+dx[k], j+dy[k]) && Grid[i+dx[k], j+dy[k]].val == 1){
                    xvois = i+dx[k];
                    yvois = j+dy[k];
                }
            }

            bool notBroken = true;
            if(notBroken && xvois > i){
                Grid[xvois, yvois].north = false;
                notBroken = false;
            }
            if(notBroken && xvois < i){
                c.north = false;
                notBroken = false;
            }
            if(notBroken && yvois > j){
                Grid[xvois, yvois].west = false;
                notBroken = false;
            }
                
            if(notBroken && yvois < j){
                c.west = false;
                notBroken = false;
            }
        }
    }

    void replaceDoors(int before, int after)
    {
        for (int i = 0; i < 10; i++){
            for (int j = 0; j < 10; j++){
                if(Grid[i,j].val == before){
                    Grid[i,j].val = after; 
                }
            }
        }
    }

    void AddDoors()
    {
        doors = new List<Cell>();
        int nbDoors = Random.Range(5, 8);
        int doorsAdded = 0;
        while(doorsAdded < nbDoors){
            int xpos = Random.Range(0,9);
            int ypos = Random.Range(0,9);

            Cell c = Grid[xpos, ypos];

            if(c.north && !c.west){
                c.west = true;
                Cell csave = new Cell(xpos, ypos);
                csave.north = false;
                doors.Add(csave);
                doorsAdded++;
            }else{
                if(!c.north && c.west){
                    c.north = true;
                    Cell csave = new Cell(xpos, ypos);
                    csave.west = false;
                    doors.Add(csave);
                    doorsAdded++;
                }
            }
        }
    }

    void RemoveDoors()
    {
        foreach (var door in doors)
        {
            Cell coriginal = Grid[door.x, door.y];
            if(door.north){
                coriginal.north = false;
            }
            if(door.west){
                coriginal.west = false;
            }
        }
    }

    void BreakInsideWalls(){
        int number = 0;
        for (int i = 0; i < 10; i++){
            for (int j = 0; j < 10; j++){
                if(inside(i-1, j) && Grid[i,j].north == false && inside(i, j-1) && Grid[i,j].west == false){
                    if(Grid[i-1,j].val != Grid[i,j-1].val){
                        Grid[i,j].val = Grid[i-1,j].val;
                        replaceDoors(Grid[i,j-1].val, Grid[i-1,j].val);
                    }
                }else{
                    if(inside(i-1, j) && Grid[i,j].north == false){
                        Grid[i,j].val = Grid[i-1,j].val;
                    }else{
                        if(inside(i, j-1) && Grid[i,j].west == false){
                            Grid[i,j].val = Grid[i,j-1].val;
                        }else{
                            Grid[i,j].val = number;
                            number++;
                        }
                    }
                }
                
            }
        }
        printVal();

        for (int i = 9; i >= 0; i--){
            for (int j = 9; j >= 0; j--){
                if(inside(i+1, j) && Grid[i+1,j].north == false && inside(i, j+1) && Grid[i,j+1].west == false){
                    if(Grid[i+1,j].val != Grid[i,j+1].val){
                        Grid[i,j].val = Grid[i,j+1].val;
                        replaceDoors(Grid[i,j+1].val, Grid[i+1,j].val);
                    }
                }else{
                    if(inside(i+1, j) && Grid[i+1,j].north == false){
                        if(Grid[i,j].val != Grid[i+1,j].val){
                            replaceDoors(Grid[i,j].val, Grid[i+1,j].val);
                        }
                    }else{
                        if(inside(i, j+1) && Grid[i,j+1].west == false){
                            if(Grid[i,j].val != Grid[i,j+1].val){
                                replaceDoors(Grid[i,j].val, Grid[i,j+1].val);
                            }
                        }
                    }
                }
                
            }
        }
        printVal();

        for (int i = 0; i < 10; i++){
            for (int j = 0; j < 10; j++){
                if(inside(i-1, j) && Grid[i,j].val == Grid[i-1,j].val){
                    Grid[i,j].north = false;
                }

                if(inside(i, j-1) && Grid[i,j].val == Grid[i,j-1].val){
                    Grid[i,j].west = false;
                }
            }
        }
    }

    void GenerateMaze()
    {
        InitMaze();
        DestroyWalls();
        printGrid2D();
    }

    void GenerateRooms()
    {
        GenerateMaze();
        AddDoors();
        BreakInsideWalls();
        RemoveDoors();
    }

    void Generate3DMaze()
    {
        for(int i=0; i < GridSizeX; i++){
            for(int j=0; j < GridSizeY; j++){
                GameObject floor;
                GameObject northWall;
                GameObject westWall;
                Vector3 floorPos = transform.localPosition;
                floorPos.Set( transform.localPosition.x + i*CellSize, transform.localPosition.y + 0.01f, transform.localPosition.z + j*CellSize);
                floor = Instantiate(Floor, floorPos, Quaternion.identity) as GameObject;
                floor.transform.SetParent(this.transform);
                

                if(Grid[i,j].north){
                    if (!(i == 0 && entry.y == j))
                    {
                        Vector3 northPos = transform.localPosition;
                        northPos.Set( transform.localPosition.x + i*CellSize - 0.5f*CellSize, transform.localPosition.y + hauteur/2, transform.localPosition.z + j*CellSize);
                        northWall = Instantiate(NorthWall, northPos, Quaternion.identity) as GameObject;
                        northWall.transform.SetParent(this.transform);
                    }
                }
                if(Grid[i,j].west){
                    Vector3 westPos = transform.localPosition;
                    westPos.Set( transform.localPosition.x + i*CellSize, transform.localPosition.y + hauteur/2, transform.localPosition.z + j*CellSize - 0.5f*CellSize);
                    westWall = Instantiate(WestWall, westPos, Quaternion.identity) as GameObject;
                    westWall.transform.SetParent(this.transform);
                }
                if(i == 9){
                    GameObject northLast;
                    Vector3 northPosLast = transform.localPosition;
                    northPosLast.Set( transform.localPosition.x + GridSizeX*CellSize - 0.5f*CellSize, transform.localPosition.y + hauteur/2, transform.localPosition.z + j*CellSize);
                    northLast = Instantiate(NorthWall, northPosLast, Quaternion.identity) as GameObject;
                    northLast.transform.SetParent(this.transform);
                }
            }
            GameObject westLast;
            Vector3 westPosLast = transform.localPosition;
            westPosLast.Set( transform.localPosition.x + i*CellSize, transform.localPosition.y + hauteur/2, transform.localPosition.z + GridSizeY*CellSize - 0.5f*CellSize);
            westLast = Instantiate(WestWall, westPosLast, Quaternion.identity) as GameObject;
            westLast.transform.SetParent(this.transform);
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        float choice = Random.Range(0,1f);
        if(choice < 0.5f){
            GenerateMaze();
        }else{
            GenerateRooms();
        }
        
        Generate3DMaze();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}