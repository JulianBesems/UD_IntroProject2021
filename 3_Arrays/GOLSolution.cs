using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    [SerializeField]
    private GameObject whiteBlock;

    [SerializeField]
    private GameObject blackBlock;

    class GolCell {
        public bool Alive = false;
        public GameObject Block;
        public int X;
        public int Y;
    }

    private static int width = 70;
    private static int height = 40;

    private GolCell[,] board = new GolCell[width, height];

    void emptyBoard(GolCell[,] board) {
        for(int i =0; i < width; i++) {
            for(int j = 0; j < height; j++) {
                GolCell cell = new GolCell();
                cell.X = i;
                cell.Y = j;
                Vector2 cellPos = new Vector2(cell.X, cell.Y);
                cell.Block = Instantiate(blackBlock, cellPos, Quaternion.identity) as GameObject;
                board[i, j] = cell;
            }
        }
    }

    void turnOnCell(GolCell cell) {
        if (!cell.Alive) {
            Destroy(cell.Block);
            Vector2 cellPos = new Vector2(cell.X, cell.Y);
            cell.Block = Instantiate(whiteBlock, cellPos, Quaternion.identity) as GameObject;
            cell.Alive = true;
        }
    }

    void turnOffCell(GolCell cell) {
        if (cell.Alive) {
            Destroy(cell.Block);
            Vector2 cellPos = new Vector2(cell.X, cell.Y);
            cell.Block = Instantiate(blackBlock, cellPos, Quaternion.identity) as GameObject;
            cell.Alive = false;
        }
    }

    int remapToGridX(int x) {
        if (x < 0) {
            x += width;
        }
        else if (x >= width) {
            x -= width;
        }
        return x;
    }

    int remapToGridY(int y) {
        if (y < 0) {
            y += height;
        }
        else if (y >= height) {
            y -= height;
        }
        return y;
    }

    void drawOscillator1(GolCell[,] board, int xs, int ys) {
        turnOnCell(board[remapToGridX(xs), remapToGridY(ys)]);
        turnOnCell(board[remapToGridX(xs+1), remapToGridY(ys)]);
        turnOnCell(board[remapToGridX(xs+2), remapToGridY(ys)]);
    }

    void drawGlider1(GolCell[,] board, int xs, int ys) {
        turnOnCell(board[remapToGridX(xs+1), remapToGridY(ys)]);
        turnOnCell(board[remapToGridX(xs+2), remapToGridY(ys+1)]);
        turnOnCell(board[remapToGridX(xs), remapToGridY(ys+2)]);
        turnOnCell(board[remapToGridX(xs+1), remapToGridY(ys+2)]);
        turnOnCell(board[remapToGridX(xs+2), remapToGridY(ys+2)]);
    }

    void fillRandom(GolCell[,] board) {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                int v = (int)Random.Range(0,2);
                Debug.Log(v);
                if (v == 1) {
                    turnOnCell(board[i, j]);
                }
            }
        }
    }

    // -------------------- Define your own function----------------
    // read a figure from a txt file, and draw it

    void drawTextFile(GolCell[,] board, int xs, int ys, string txtFile) {
        string[] lines = System.IO.File.ReadAllLines("/Users/julianbesems/Desktop/GameOfLifePatterns/" + txtFile + ".txt");
        for(int i = 0; i < lines.Length; i++) {
            for(int j = 0; j < lines[i].Length; j++) {
                if(lines[i][j] == 'x') {
                    turnOnCell(board[remapToGridX(xs + j), remapToGridY(ys + i)]);
                }
            }
        }
    }

    int getLivesNeighbours(GolCell cell) {
        int lives = 0;
        for (int i = -1; i < 2; i++) {
            for (int j = -1; j < 2; j++) {
                // increment the lives variable if any of the cells around the middle
                // found at coordinate cell.X and cell.Y are alive
                // Watch out if cell.X and or cell.Y are on the edge of the board
                // Then you take the other side into account
                // for instance: if cell.X == width - 1, then you check the adjacent
                // cells in column 0.

                int xc = remapToGridX(cell.X + i);
                int yc = remapToGridY(cell.Y + j);
                if (!(i == 0 && j == 0)) {
                    if (board[xc, yc].Alive) {
                        lives++;
                    }
                }
            }
        }
        return lives;
    }

    void updateCells(GolCell[,] board) {
        List<GolCell> toKill = new List<GolCell>();
        List<GolCell> toRevive = new List<GolCell>();
        /*
        Iterate over the entire board (double for loop!!)
        Get the number of live neighbors for each cell
        (this is what happens within the loop)
        check if the cell is alive or dead
        Depending on live neighbors you add the cell to toKill list or to Revive list
        */
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                GolCell cell = board[i, j];
                int lives = getLivesNeighbours(cell);
                if (cell.Alive) {
                    if (!(lives == 2 || lives == 3)) {
                        toKill.Add(cell);
                    }
                }
                else {
                    if (lives == 3) {
                        toRevive.Add(cell);
                    }
                }
            }
        }
        foreach(GolCell death in toKill) {
            turnOffCell(death);
        }
        foreach (GolCell life in toRevive) {
            turnOnCell(life);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        emptyBoard(board);
        //drawOscillator1(board, 35, 20);
        //drawGlider1(board, 20, 20);
        //drawTextFile(board, 20, 20, "pulsar");
        fillRandom(board);
    }

    int speed = 10;
    // Update is called once per frame
    void Update()
    {
        Application.targetFrameRate = speed;
        updateCells(board);

    }
}
