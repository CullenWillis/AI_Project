using UnityEngine;
using System.Collections;

public class BlockSelector : MonoBehaviour {

	[SerializeField]
	private GameObject[] Blocks;

    private GameObject blockToInitialize;
    private GameObject selectedBlock;

	private int currentBlock;

	void Start () 
	{
        currentBlock = 0;
        blockToInitialize = Blocks[currentBlock];

        selectedBlock = Instantiate(blockToInitialize, Vector2.zero, Quaternion.identity) as GameObject;
        selectedBlock.name = "SelectedBlock";
    }
	
	void Update ()
    {
        if (Input.GetKeyDown("space"))
        {
            if (currentBlock < Blocks.Length - 1)
            {
                currentBlock++;
            }
            else if (currentBlock == Blocks.Length - 1)
            {
                currentBlock = 0;
            }

            // Destory Selected block
            Destroy(selectedBlock);

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = new Vector2(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y));

            // Create new block with new sprite
            blockToInitialize = Blocks[currentBlock];
            selectedBlock = Instantiate(blockToInitialize, mousePos, Quaternion.identity) as GameObject;
            selectedBlock.name = "SelectedBlock";
        }
    }
}
