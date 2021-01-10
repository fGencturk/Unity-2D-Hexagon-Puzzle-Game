using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class CameraResizer : MonoBehaviour
{
    [SerializeField] private Board board;
    
    [SerializeField] [Tooltip("Horizontal margin to game board in %.")] private float horizontalMargin = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        float screenRatio = Screen.width / (float)Screen.height;
        Vector2 boardSize = board.GetBoardSizeInUnits();
        float widthMultiplier = 1 + horizontalMargin * 2;
        float targetRatio = boardSize.x * widthMultiplier / boardSize.y;
        if (screenRatio >= targetRatio)
        {
            Camera.main.orthographicSize = boardSize.y / 2;
        }
        else
        {
            Camera.main.orthographicSize = boardSize.y / 2 * (targetRatio / screenRatio);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
