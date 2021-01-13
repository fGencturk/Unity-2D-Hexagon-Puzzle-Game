using Hexagon;
using UnityEngine;
using UnityEngine.Serialization;

namespace Utility
{
    public class CameraResizer : MonoBehaviour
    {
        [SerializeField] private Board board;
        [SerializeField] private RectTransform rectTransform;
    
        [SerializeField] [Tooltip("Horizontal margin to game board in %.")] private float horizontalMargin = 0.1f;

        // Start is called before the first frame update
        void Start()
        {
            float screenRatio = Screen.width / (float)(Screen.height - rectTransform.rect.height);
            Vector2 boardSize = GameManager.instance.positionCalculator.GetBoardSizeInUnits();
            float targetRatio = boardSize.x / boardSize.y;
        
            float divider = 2 * (1 - rectTransform.rect.height / Screen.height - horizontalMargin);
        
            if (screenRatio >= targetRatio)
            {
                Camera.main.orthographicSize = boardSize.y / divider;
            }
            else
            {
                Camera.main.orthographicSize = boardSize.y / divider * (targetRatio / screenRatio);
            }
            
            Vector3 cameraPosition = Camera.main.ScreenToWorldPoint (new Vector3 (0, 0, 0));
            
            Vector3 topPanelBottomPosition =
                Camera.main.ScreenToWorldPoint(rectTransform.position +
                                               new Vector3(0, -rectTransform.rect.height / 2, 0));
            float yDifference = -cameraPosition.y - topPanelBottomPosition.y;
            Camera.main.transform.position -= new Vector3(0, -yDifference / 2, 0);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
