using UnityEngine;
using System.Collections;

public class CameraScaler : MonoBehaviour {

    const float CAMERA_SIZE = 5;
    const float ASPECT = 1.777777f;

    void Start() {
        // Scales camera for an expected 16:9 resolution
        Camera camera = GetComponent<Camera>();
        camera.projectionMatrix = Matrix4x4.Ortho(
            -CAMERA_SIZE * ASPECT, CAMERA_SIZE * ASPECT, -CAMERA_SIZE,
            CAMERA_SIZE, camera.nearClipPlane, camera.farClipPlane);
    }
}
