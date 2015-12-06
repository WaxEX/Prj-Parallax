using UnityEngine;
using System.Collections;

public class SimpleMove : MonoBehaviour {
    private Animator animator;
    private int isWalkId;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        isWalkId = Animator.StringToHash("isWalk");
    }
	
	// Update is called once per frame
	void Update () {
		float z = 0.5f*Input.GetAxis("Vertical");

        if (z != 0){
            transform.Translate(0, 0, z);
            animator.SetBool(isWalkId, true);
        }
        else
        {
            animator.SetBool(isWalkId, false);
        }
	}
}
