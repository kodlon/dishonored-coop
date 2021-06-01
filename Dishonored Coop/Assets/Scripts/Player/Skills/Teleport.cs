using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Teleport : MonoBehaviour
{
    [SerializeField] private Transform playerBody;
    [SerializeField] private CharacterController playerController;
    private Camera _camera;
    private MouseLook sensitive;
    private Vector3 sphereBody;
    private RaycastHit hit;
    private bool skillActive = false;

    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            skillActive = true;
            Time.timeScale = 0.1f;
            Vector3 point = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0);
            Ray ray = _camera.ScreenPointToRay(point);

            if (Physics.Raycast(ray, out hit))
            {
                StartCoroutine(SphereIndicator(hit.point));
            }
        }
        if (Input.GetButtonUp("Fire1") && skillActive)
        {
            skillActive = false;
            Time.timeScale = 1f;

            Vector3 point = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0);
            Ray ray = _camera.ScreenPointToRay(point);

            if (Physics.Raycast(ray, out hit))
            {
                playerController.enabled = false;
                playerBody.position = sphereBody;
                playerController.enabled = true;
            }
        }
    }

    private IEnumerator SphereIndicator(Vector3 pos)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = pos + Vector3.up;
        sphereBody = sphere.transform.position;

        yield return new WaitForSeconds(0.001f);

        Destroy(sphere);
    }
}
