using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class S_Fruit : MonoBehaviour
{
    [Header("Board Variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;
    public GameObject otherCandy;

    public AudioSource matchSfx;

    private FruitMatches fruitMatches;
    private S_JellyTable board;
    private Vector2 firstClickPosition;
    private Vector2 finalClickPosition;
    private Vector2 tempPosition;

    [Header("Swiping Variables")]
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    [Header("PowerUp Variables")]
    public bool isCandyBomb;
    public GameObject CandyBomb;


    // Start is called before the first frame update
    public void Start()
    {
        board = FindObjectOfType<S_JellyTable>();
        fruitMatches = FindObjectOfType<FruitMatches>();

    }

    //Testing
    public void MakeCandyBomb()
    {
        isCandyBomb = true;
        gameObject.tag = null;
        GameObject Candy = Instantiate(CandyBomb, transform.position, Quaternion.identity);
        Candy.transform.parent = this.transform;

    }


    // Update is called once per frame
    void Update()
    {
        

        targetX = column;
        targetY = row;

        //for movement on the X
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            //Move Toward the target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.1f);
            if (board.allCandy[column, row] != this.gameObject)
            {
                board.allCandy[column, row] = this.gameObject;
            }
            fruitMatches.findAllMatches();
        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }

        //For movement on the Y
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            //Move Toward the target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.1f);
            if (board.allCandy[column, row] != this.gameObject)
            {
                board.allCandy[column, row] = this.gameObject;
            }
            fruitMatches.findAllMatches();
        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    public IEnumerator CheckMoveCo()
    {
        if (isCandyBomb)
        {
            //This piece is a candy bomb, and the other piece is the candy to destroy
            fruitMatches.MatchPiecesOfColor(otherCandy.tag);

            //isMatched = true;
        }
        else if (otherCandy.GetComponent<S_Fruit>().isCandyBomb)
        {
            //The other piece is a candy bomb, and this piece has the candy to destroy
            fruitMatches.MatchPiecesOfColor(this.gameObject.tag);
            otherCandy.GetComponent<S_Fruit>().isMatched = true;
        }

        yield return new WaitForSeconds(1.0f);
        if (otherCandy != null)
        {
            if (!isMatched && !otherCandy.GetComponent<S_Fruit>().isMatched)
            {
                otherCandy.GetComponent<S_Fruit>().row = row;
                otherCandy.GetComponent<S_Fruit>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(0.5f);
                board.currentCandy = null;
                board.currentState = GameState.move;

            }
            else
            {

                board.destroyMatches();

            }
            //otherCandy = null;
        }


    }

    private void OnMouseDown()
    {
        if (board.currentState == GameState.move)
        {
            firstClickPosition = Camera.main.WorldToScreenPoint(Input.mousePosition);
        }

        //Debug.Log(firstClickPosition);
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalClickPosition = Camera.main.WorldToScreenPoint(Input.mousePosition);
            //Debug.Log(finalClickPosition);
            CalculateAngle();
            //Debug.Log("Calculate");
        }
    }

    void CalculateAngle()
    {
        if (Mathf.Abs(finalClickPosition.y - firstClickPosition.y) > swipeResist || Mathf.Abs(finalClickPosition.x - firstClickPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalClickPosition.y - firstClickPosition.y, finalClickPosition.x - firstClickPosition.x) * 180 / Mathf.PI;
            //Debug.Log(swipeAngle);       
            movePieces();
            if (otherCandy != null)
            {
                board.currentState = GameState.wait;
                board.currentCandy = this;
            }
        }
        else
        {
            board.currentState = GameState.move;

        }

    }

    void movePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            //Right Swipe
            otherCandy = board.allCandy[column + 1, row];
            previousRow = row;
            previousColumn = column;
            otherCandy.GetComponent<S_Fruit>().column -= 1;
            column += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            //Up Swipe
            otherCandy = board.allCandy[column, row + 1];
            previousRow = row;
            previousColumn = column;
            otherCandy.GetComponent<S_Fruit>().row -= 1;
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            //Left Swipe
            otherCandy = board.allCandy[column - 1, row];
            previousRow = row;
            previousColumn = column;
            otherCandy.GetComponent<S_Fruit>().column += 1;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //Down Swipe
            otherCandy = board.allCandy[column, row - 1];
            previousRow = row;
            previousColumn = column;
            otherCandy.GetComponent<S_Fruit>().row += 1;
            row -= 1;
        }
        StartCoroutine(CheckMoveCo());
    }
}
