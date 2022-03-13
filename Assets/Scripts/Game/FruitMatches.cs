using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitMatches : MonoBehaviour
{
    private S_JellyTable board;
    public List<GameObject> currentMatches = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<S_JellyTable>();
    }

    public void findAllMatches()
    {
        StartCoroutine(findAllMatchesCo());
    }

    private IEnumerator findAllMatchesCo()
    {
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentCandy = board.allCandy[i, j];
                if (currentCandy != null)
                {
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftCandy = board.allCandy[i - 1, j];
                        GameObject rightCandy = board.allCandy[i + 1, j];
                        if (leftCandy != null && rightCandy != null)
                        {
                            if (leftCandy.tag == currentCandy.tag && rightCandy.tag == currentCandy.tag)
                            {
                                if (!currentMatches.Contains(leftCandy))
                                {
                                    currentMatches.Add(leftCandy);
                                }
                                leftCandy.GetComponent<S_Fruit>().isMatched = true;
                                if (!currentMatches.Contains(rightCandy))
                                {
                                    currentMatches.Add(rightCandy);
                                }
                                rightCandy.GetComponent<S_Fruit>().isMatched = true;
                                if (!currentMatches.Contains(currentCandy))
                                {
                                    currentMatches.Add(currentCandy);
                                }
                                currentCandy.GetComponent<S_Fruit>().isMatched = true;

                            }
                        }
                    }

                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upCandy = board.allCandy[i, j + 1];
                        GameObject downCandy = board.allCandy[i, j - 1];
                        if (upCandy != null && downCandy != null)
                        {
                            if (upCandy.tag == currentCandy.tag && downCandy.tag == currentCandy.tag)
                            {
                                if (!currentMatches.Contains(upCandy))
                                {
                                    currentMatches.Add(upCandy);
                                }
                                upCandy.GetComponent<S_Fruit>().isMatched = true;
                                if (!currentMatches.Contains(downCandy))
                                {
                                    currentMatches.Add(downCandy);
                                }
                                downCandy.GetComponent<S_Fruit>().isMatched = true;
                                if (!currentMatches.Contains(currentCandy))
                                {
                                    currentMatches.Add(currentCandy);
                                }
                                currentCandy.GetComponent<S_Fruit>().isMatched = true;

                            }
                        }
                    }
                }
            }
        }
    }

    public void MatchPiecesOfColor(string color)
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                //CHecking if the piece exists
                if (board.allCandy[i, j] != null)
                {
                    //Check the tag of that candy
                    if (board.allCandy[i, j].tag == color)
                    {
                        //Set that candy to be matched
                        board.allCandy[i, j].GetComponent<S_Fruit>().isMatched = true;
                    }
                }
            }
        }
    }
}
