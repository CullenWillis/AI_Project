using UnityEngine;
using System.Collections;

public class GridTemplate : MonoBehaviour
{

    [SerializeField]
    private GameObject finalObject;

    private GameObject parentObject;

    private Grid grid;

    [SerializeField]
    private int gridX = 40;
    [SerializeField]
    private int gridY = 10;

    private Vector2 mousePos;

    [SerializeField]
    private LayerMask allTilesLayer;

    // Update is called once per frame
    void Update()
    {

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 roundedPos = new Vector2(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y));

        //Debug.DrawLine(Camera.main.transform.position, roundedPos, Color.red);

        transform.localPosition = roundedPos;

        if (roundedPos.x < gridX / 2 + 1 && roundedPos.y < gridY / 2 + 1)
        {
            if (roundedPos.x > -gridX / 2 - 1 && roundedPos.y > -gridY / 2 - 1)
            {
                if (Input.GetMouseButton(0))
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit, 100.0f))
                    {
                        if (hit.transform.name != "Wall")
                        {
                            GameObject tempObject = Instantiate(finalObject, transform.position, Quaternion.identity) as GameObject;
                            tempObject.name = "Wall";
                            parentObject = GameObject.Find("Editor");
                            tempObject.transform.parent = parentObject.transform;

                            //Grid.ForceUpdate();
                        }
                    }
                }
                else if (Input.GetMouseButton(1))
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit, 100.0f))
                    {
                        if (hit.transform.name == "Wall")
                            Destroy(hit.transform.gameObject);

                        //Grid.ForceUpdate();
                    }
                }

                
            }
        }
    }
}