using UnityEngine;

public class SurfaceMessages : MonoBehaviour
{
    public Touch touchDown;
    public Transform touches;
    public GameObject touchObject;
    public Touch touchUp;
    private SurfaceManager surfaceManager;
    private bool paused = false;
    private bool focused = false;

    private void OnGUI()
    {
        GUI.Box(new Rect(0, 0, 200, 70), "<size=20>" + "paused " + paused + "\nfocused " + focused + "</size>");
    }

    private void Awake()
    {
        surfaceManager = SurfaceManager.Instance;
        Application.runInBackground = true;
    }

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

        //Debug.DrawLine(new Vector3(-54.25f, 0, 2), new Vector3(54.25f, 0, 2), Color.red);
        //Debug.DrawLine(new Vector3(0, -34.95f, 2), new Vector3(0, 34.95f, 2), Color.red);

        if (surfaceManager.isConnected())
        {
            touchDown = surfaceManager.getTouchDown();
            touchUp = surfaceManager.getTouchUp();

            if (touchDown != null)
            {
                Vector3 position = SurfaceToWorldPoint(touchDown.getPosition());
                var clone = (GameObject)Instantiate(touchObject, position, Quaternion.identity);
                clone.transform.name = touchDown.getID().ToString();
                clone.transform.parent = touches;
            }

            if (touchUp != null)
            {
                Transform tmpUp = touches.Find(touchUp.getID().ToString());
                if (tmpUp != null)
                {
                    tmpUp.parent = null;
                    Destroy(tmpUp.gameObject);
                }
            }

            foreach (Transform child in touches)
            {
                int touchID = int.Parse(child.name);
                Vector2 worldPosition = surfaceManager.getTouchMove(touchID);
                child.position = SurfaceToWorldPoint(worldPosition);
            }

            // cleanup
            if((touches.childCount > 0) && (surfaceManager.getTouchesCount() == 0))
            {
                foreach (Transform child in touches)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    // Unity3d bottom left 0,0. Surface top left 0,0
    private Vector3 SurfaceToWorldPoint(Vector2 screenPoints)
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(screenPoints.x,
                                                                      Screen.height - screenPoints.y,
                                                                      Camera.main.nearClipPlane));
        position.z = -1;
        return position;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        paused = pauseStatus;
    }

    private void OnApplicationFocus(bool focusStatus)
    {
        focused = focusStatus;
    }
}