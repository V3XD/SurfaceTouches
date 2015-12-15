using UnityEngine;

public class Movement : MonoBehaviour
{
    private bool invisible = false;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt) && Input.GetKeyUp(KeyCode.F4))
        {
            Application.Quit();
        }

        transform.Translate(Vector3.right * Time.deltaTime);
        if (invisible)
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(0,
                                                                          Screen.height * 0.5f,
                                                                          Camera.main.nearClipPlane));
            position.z = -1;
            transform.position = position;
            invisible = false;
        }
    }

    private void OnBecameInvisible()
    {
        invisible = true;
    }
}