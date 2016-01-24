using UnityEngine;
using System.Collections;

public class TemporaryAnimationHandler : MonoBehaviour {

    void Start() {
        StartCoroutine("Destroy");
    }

    private IEnumerator Destroy(){
        Animator animator = GetComponent<Animator>();
        AnimatorClipInfo[] test = animator.GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(test[0].clip.length);
        Destroy(gameObject);
    }
}
