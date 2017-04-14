using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    [SerializeField]
    private int maxMoves = 5;

    private int moves;

    [SerializeField]
    private float speed = 5;

    private enum Direction { LEFT, RIGHT };
    private Direction direction = new Direction();

    private Vector2 startPosition;
    private Vector2 nextPosition;
    private Vector2 checkPosition;

    [SerializeField]
    private LayerMask allTilesLayer;

    void Start () {

        moves = maxMoves;
        direction = Direction.LEFT;

        startPosition = transform.position;
        nextPosition = new Vector2();
        checkPosition = new Vector2();
    }
	
	void Update ()
    {
        Vector2 currentPosition = transform.position;

        if(nextPosition == null || currentPosition == nextPosition)
        {
            checkNextPosition();
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime);
        }
    }

    private void checkNextPosition()
    {
        startPosition = transform.position;

        if (moves != 0)
        {
            if (direction == Direction.LEFT)
            {
                checkPosition = new Vector2(Mathf.Round(startPosition.x - 1), Mathf.Round(startPosition.y - 1));
            }
            else if (direction == Direction.RIGHT)
            {
                checkPosition = new Vector2((startPosition.x + 1), (startPosition.y - 1));
            }

            RaycastHit2D rayHit = Physics2D.Raycast(checkPosition, Vector2.zero, Mathf.Infinity, allTilesLayer);

            if (rayHit.collider != null)
            {
                nextPosition = new Vector2(checkPosition.x, checkPosition.y + 1);
                moves--;
            }
            else
            {
                moves = maxMoves;

                if (direction == Direction.LEFT)
                {
                    direction = Direction.RIGHT;
                }
                else if (direction == Direction.RIGHT)
                {
                    direction = Direction.LEFT;
                }
            }
        }
        else if (moves == 0)
        {
            moves = maxMoves;

            if (direction == Direction.LEFT)
            {
                direction = Direction.RIGHT;
            }
            else if (direction == Direction.RIGHT)
            {
                direction = Direction.LEFT;
            }
        }
    }
}
