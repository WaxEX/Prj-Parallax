using UnityEngine;
using System.Collections;

public class SimpleMove : MonoBehaviour {
    private Animator animator;
    private int isWalkId;
    private int isCrashId;
    public float RATIO = 1.0f;

    private bool isFront = false;
    private int CrashTimer = 0;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        isWalkId = Animator.StringToHash("isWalk");
        isCrashId = Animator.StringToHash("isCrash");
    }
	
	// Update is called once per frame
	void Update () {

        if(CrashTimer > 0){
            CrashTimer--;
            return;
        }

        if (CrashTimer == 0){
            animator.SetBool(isCrashId, false);
        }

        float z = RATIO * Input.GetAxis("Vertical");

        if (z != 0){
            transform.Translate(0, 0, z);

            if (!isFront && transform.position.z > 1.0f) {
                CrashTimer = 15;
                animator.SetBool(isCrashId, true);
            }
            else{
                animator.SetBool(isWalkId, true);
            }

            // この辺ハードコーディング…
            isFront = transform.position.z > 1.0f;
        }
        else{
            animator.SetBool(isWalkId, false);
        }
	}
    
    // シーン切替もここでやっちゃう
    void FixedUpdate(){

        if (Input.GetButton("Jump")){

            if(Application.loadedLevelName == "Main"){
                Application.LoadLevel("FarScene");
            }
            else{
                Application.LoadLevel("Main");
            }
        }
    }
}
