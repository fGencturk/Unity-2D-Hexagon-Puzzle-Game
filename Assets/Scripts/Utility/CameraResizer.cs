using Hexagon;
using UnityEngine;

namespace Utility
{
    public class CameraResizer : MonoBehaviour
    {
        [SerializeField] private Board board;
    
        [SerializeField] [Tooltip("Horizontal margin to game board in %.")] private float horizontalMargin = 0.1f;

        // Start is called before the first frame update
        void Start()
        {
            float screenRatio = Screen.width / (float)Screen.height;
            Vector2 boardSize = GameManager.instance.positionCalculator.GetBoardSizeInUnits();
            float targetRatio = boardSize.x / boardSize.y;
        
            float divider = 2 - horizontalMargin * 2;
        
            if (screenRatio >= targetRatio)
            {
                Camera.main.orthographicSize = boardSize.y / divider;
            }
            else
            {
                Camera.main.orthographicSize = boardSize.y / divider * (targetRatio / screenRatio);
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
