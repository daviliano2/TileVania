using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapMovement : MonoBehaviour
{
    const string IS_ACTIVE = "isActive";


    // [Tooltip("Game units per second")]
    // [SerializeField] float moveRate = 0.2f;
    // [SerializeField] float transitionTime = 2f;

    BoxCollider2D trapTrigger = null;
    GameObject trapGrid = null;
    // Vector2 triggerOriginalSize = new Vector2(0f, 0f);

    private void Start()
    {
        trapGrid = gameObject.transform.parent.gameObject;
        trapTrigger = GetComponent<BoxCollider2D>();
        // triggerOriginalSize = trapTrigger.size;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GetComponent<AudioSource>().Play();
        trapGrid.GetComponent<Animator>().SetBool(IS_ACTIVE, true);
        
        // Leaving the coroutine for reference
        // StartCoroutine(MoveTrap());
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        trapGrid.GetComponent<Animator>().SetBool(IS_ACTIVE, false);
    }

    // This is another way of moving the trap only with code
    // way more complicated that just doing animations.
    // IEnumerator MoveTrap()
    // {
    //     GameObject trapGrid = gameObject.transform.parent.gameObject;
    //     float xPos = trapGrid.transform.position.x;
    //     float zPos = trapGrid.transform.position.z;
    //     float movingTime = 0f;
    //     var originalPosition = trapGrid.transform.position;

    //     trapTrigger.size = new Vector2(trapTrigger.size.x, 0.75f);

    //     while (movingTime < 0.75f)
    //     {
    //         movingTime += Time.deltaTime / moveRate;
    //         trapGrid.transform.position = new Vector3(xPos, -movingTime, zPos);
    //         yield return null;
    //     }

    //     yield return new WaitForSeconds(1f);

    //     movingTime *= -1;
    //     while (movingTime < 0f)
    //     {
    //         movingTime += Time.deltaTime / (moveRate + 0.4f);
    //         trapGrid.transform.position = new Vector3(xPos, movingTime, zPos);
    //         yield return null;
    //     }

    //     trapTrigger.size = triggerOriginalSize;
    // }
}
