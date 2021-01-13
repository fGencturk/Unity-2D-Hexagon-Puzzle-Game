using System;
using Hexagon;
using UnityEngine;

namespace Controller
{
    using System.Collections;
 
    public class TouchDetector : MonoBehaviour {
 
 
        public static event Action<SwipeDirection, Vector2, Vector2> Swipe;
        public static event Action OnClick;
        private Vector2 _lastPosition,
            _hexagonSize;
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
            _hexagonSize = GameManager.instance.positionCalculator.hexagonSize;
        }

        void Update () {
            if (Input.GetMouseButtonDown(0))
            {
                _lastPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector2 secondPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                if (Vector2.Distance(secondPosition, _lastPosition) < _hexagonSize.x)
                {
                    OnClick();
                    return;
                }
                Vector2 direction = secondPosition - _lastPosition;
 
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)){
                    if (direction.x > 0) 
                        Swipe(SwipeDirection.Right, _lastPosition, secondPosition);
                    else
                        Swipe(SwipeDirection.Left, _lastPosition, secondPosition);
                }
                else{
                    if (direction.y > 0)
                        Swipe(SwipeDirection.Up, _lastPosition, secondPosition);
                    else
                        Swipe(SwipeDirection.Down, _lastPosition, secondPosition);
                }
            }
        }
    }
}