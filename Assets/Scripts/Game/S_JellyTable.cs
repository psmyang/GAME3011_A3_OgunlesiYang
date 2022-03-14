using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public enum GameState
    {
        wait,
        move
    }

    public class S_JellyTable : MonoBehaviour
    {
        public int maxCandy = 0;
        public GameState currentState = GameState.move;
        public int width;
        public int height;
        public int offset;
        public GameObject tilePrefab;
        public GameObject[] candies;
        private FruitTile[,] allTiles;
        public GameObject[,] allCandy;
        public S_Fruit currentCandy;
        private FruitMatches fruitMatches;

    //Animator anim;




    // Start is called before the first frame update
    void Start()
        {
            fruitMatches = FindObjectOfType<FruitMatches>();
        //anim = GetComponentInParent<Animator>();


        //Creating the width and height of board
        allTiles = new FruitTile[width, height];
            allCandy = new GameObject[width, height];

            //Setup();

        }


        public void Setup()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Vector2 tempPosition = new Vector2(i, j + offset);
                    GameObject backgrounTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                    backgrounTile.transform.parent = this.transform;
                    backgrounTile.name = "( " + i + ", " + j + " )";
                    int candyToUse = Random.Range(0, maxCandy);
                    int maxIterations = 0;

                    while (matchesAt(i, j, candies[candyToUse]) && maxIterations < 100)
                    {
                        candyToUse = Random.Range(0, maxCandy);
                        maxIterations++;
                    }
                    maxIterations = 0;

                    GameObject candy = Instantiate(candies[candyToUse], tempPosition, Quaternion.identity);
                    candy.GetComponent<S_Fruit>().row = j;
                    candy.GetComponent<S_Fruit>().column = i;

                    candy.transform.parent = this.transform;
                    candy.name = "( " + i + ", " + j + " )";

                    allCandy[i, j] = candy;

                }
            }
        }
        //Prevents matches at th beginning of the game
        private bool matchesAt(int column, int row, GameObject piece)
        {
            if (column > 1 && row > 1)
            {
                if (allCandy[column - 1, row].tag == piece.tag && allCandy[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
                if (allCandy[column, row - 1].tag == piece.tag && allCandy[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            else if (column <= 1 || row <= 1)
            {
                if (row > 1)
                {
                    if (allCandy[column, row - 1].tag == piece.tag && allCandy[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
                if (column > 1)
                {
                    if (allCandy[column - 1, row].tag == piece.tag && allCandy[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void CheckTomakeBombs()
        {
            if (fruitMatches.currentMatches.Count >= 5)
            {
                Debug.Log("Make a color bomb");
                if (currentCandy != null)
                {
                    if (currentCandy.isMatched)
                    {
                        if (!currentCandy.isCandyBomb)
                        {
                            currentCandy.isMatched = false;
                            currentCandy.MakeCandyBomb();
                        }
                    }
                    else
                    {
                        if (currentCandy.otherCandy != null)
                        {
                            S_Fruit otherCandy = currentCandy.otherCandy.GetComponent<S_Fruit>();
                            if (otherCandy.isMatched)
                            {
                                if (!otherCandy.isCandyBomb)
                                {
                                    otherCandy.isMatched = false;
                                    otherCandy.MakeCandyBomb();
                                }
                            }
                        }
                    }
                }
            }
        }

        private void destroyMatchesAt(int column, int row)
        {
            if (allCandy[column, row].GetComponent<S_Fruit>().isMatched)
            {
                if (fruitMatches.currentMatches.Count >= 5)
                {
                    CheckTomakeBombs();

                }
            fruitMatches.currentMatches.Remove(allCandy[column, row]);
               FindObjectOfType<S_JellyManager>().AddScore( 25);

            

            FindObjectOfType<S_SoundMana>().PlayPop();


                Destroy(allCandy[column, row]);
                allCandy[column, row] = null;
            }
        }
        //Destroys the candies that were a match
        public void destroyMatches()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (allCandy[i, j] != null)
                    {
                        destroyMatchesAt(i, j);

                    }
                }
            }
            StartCoroutine(decreasedRowCo());
        }
        //makes candies above fall into the empty spaces
        private IEnumerator decreasedRowCo()
        {
            int nullCount = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (allCandy[i, j] == null)
                    {
                        nullCount++;
                    }
                    else if (nullCount > 0)
                    {
                        allCandy[i, j].GetComponent<S_Fruit>().row -= nullCount;
                        allCandy[i, j] = null;
                    }
                }
                nullCount = 0;
            }
            yield return new WaitForSeconds(0.4f);
            StartCoroutine(FillBoardCo());

        }

        private void RefillBoard()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (allCandy[i, j] == null)
                    {
                        Vector2 tempPosition = new Vector2(i, j + offset);
                        int candyToUse = Random.Range(0, maxCandy);
                        GameObject piece = Instantiate(candies[candyToUse], tempPosition, Quaternion.identity);
                        allCandy[i, j] = piece;
                        piece.transform.parent = this.transform;
                        piece.name = "( " + i + ", " + j + " )";
                        piece.GetComponent<S_Fruit>().row = j;
                        piece.GetComponent<S_Fruit>().column = i;
                    }
                }
            }
        }

        private bool matchesOnBoard()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (allCandy[i, j] != null)
                    {
                        if (allCandy[i, j].GetComponent<S_Fruit>().isMatched)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private IEnumerator FillBoardCo()
        {
            RefillBoard();
            yield return new WaitForSeconds(0.5f);

            while (matchesOnBoard())
            {
                yield return new WaitForSeconds(0.5f);
                destroyMatches();
            }
            yield return new WaitForSeconds(0.5f);
            currentState = GameState.move;
        }


    }

