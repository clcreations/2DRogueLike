using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    const float CAMERA_SIZE = 5;
    const float ASPECT = 1.777777f;
    float duration;
    float magnitude;

    void Start() {
        // Scales camera for an expected 16:9 resolution
        Camera camera = GetComponent<Camera>();
        camera.projectionMatrix = Matrix4x4.Ortho(
            -CAMERA_SIZE * ASPECT, CAMERA_SIZE * ASPECT, -CAMERA_SIZE,
            CAMERA_SIZE, camera.nearClipPlane, camera.farClipPlane);
    }

    public void ShakeScreen(float dur, float mag){
        duration = dur;
        magnitude = mag;
        StopAllCoroutines();
        StartCoroutine("Shake");
    }

    // http://unitytipsandtricks.blogspot.com/2013/05/camera-shake.html
    IEnumerator Shake() {

        float elapsed = 0.0f;

        Vector3 originalCamPos = transform.position;

        while (elapsed < duration) {

            elapsed += Time.deltaTime;

            float percentComplete = elapsed / duration;
            float damper =
                1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

            float x = Random.value * 2.0f - 1.0f;
            float y = Random.value * 2.0f - 1.0f;
            x *= magnitude * damper;
            y *= magnitude * damper;

            transform.position = new Vector3(
                originalCamPos.x + x, originalCamPos.y + y, originalCamPos.z);

            yield return null;
        }

        Camera.main.transform.position = originalCamPos;
    }
}
