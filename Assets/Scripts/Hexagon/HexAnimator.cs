using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace Hexagon
{
    public class HexAnimator : MonoBehaviour
    {
        public Hex attachedHex;
        
        public void StartMoveAnimation(int lastExplodedHex)
        {
            StartCoroutine(MoveToPosition(lastExplodedHex));
        }
        
        private IEnumerator MoveToPosition(int lastExplodedHex)
        {
            int countFromBottomHexagon = lastExplodedHex - attachedHex.index.y;
            if (countFromBottomHexagon > 0)
            {
                yield return new WaitForSeconds(AnimationData.hexMoveDelayForEachRow / 2f * countFromBottomHexagon);
            }
            float time = 0;
            Vector2 startPosition = transform.position;
            
            Vector2 newPosition = GameManager.instance.positionCalculator.GetPosition(attachedHex.index);
            Vector2 difference = newPosition - startPosition;
            float distance = difference.magnitude;
            Vector3 direction = difference.normalized;
            float speed = AnimationData.hexMoveSpeed;
            while (time * speed <= distance)
            {
                time += Time.deltaTime;
                transform.position = transform.position + direction * (speed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            transform.position = newPosition;
        }

        public void StartRotateAnimation(bool clockwise)
        {
            StartCoroutine(RotateAroundSelection(clockwise));
        }
        private IEnumerator RotateAroundSelection(bool clockwise)
        {
            float time = 0;
            Vector3 direction = clockwise ? Vector3.back : Vector3.forward;
            float speed = AnimationData.rotationSpeed;
            transform.position = transform.position + Vector3.back;
            while (time *  speed <= 120f)
            {
                time += Time.deltaTime;
                transform.RotateAround(GameManager.instance.positionCalculator.GetSelectionPoint(), 
                    direction, speed * Time.deltaTime);
                
                yield return new WaitForEndOfFrame();
            }

            transform.rotation = Quaternion.identity;
            transform.position = GameManager.instance.positionCalculator.GetPosition(attachedHex.index) + Vector3.forward;
        }
    }
}