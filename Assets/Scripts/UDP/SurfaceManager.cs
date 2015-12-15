using SurfaceManagement;
using System;
using System.Diagnostics;
using UnityEngine;

public class SurfaceManager : Singleton<SurfaceManager>
{
    protected SurfaceManager()
    {
    }

    public Touch getTouchDown()
    {
        if (UDPClient.GetDataStream().TouchDown.Count > 0)
        {
            return UDPClient.GetDataStream().TouchDown.Dequeue();
        }
        return null;
    }

    public Vector2 getTouchMove(int ID)
    {
        Touch touch;
        UDPClient.GetDataStream().TouchMove.TryGetValue(ID, out touch);
        return touch.getPosition();
    }

    public int getTouchesCount()
    {
        return UDPClient.GetDataStream().TouchMove.Count;
    }

    public Touch getTouchUp()
    {
        if (UDPClient.GetDataStream().TouchUp.Count > 0)
        {
            Touch touchUp = UDPClient.GetDataStream().TouchUp.Dequeue();
            UDPClient.GetDataStream().TouchMove.Remove(touchUp.getID());
            return touchUp;
        }
        return null;
    }

    public bool isConnected()
    {
        return UDPClient.IsListening();
    }

    protected override void Destroy()
    {
        UnityEngine.Debug.Log("SurfaceManager: Destruct");
        UDPClient.Close();
    }

    private static void RemoveOldTouches()
    {
        for (int i = 0; i < UDPClient.GetDataStream().IDlist.Count; i++)
        {
            int ID = UDPClient.GetDataStream().IDlist[i];
            Touch touch;
            UDPClient.GetDataStream().TouchMove.TryGetValue(ID, out touch);
            UnityEngine.Debug.Log("remove old touches " + touch.getPosition());
            DateTime touchExpiration = touch.getUpdateTime().AddSeconds(UDPClient.GetDataStream().maxUpdateTime);
            if (touchExpiration.CompareTo(DateTime.Now) < 0) // touchExpiration earlier than now
            {
                UDPClient.OnTouchUp(touch);
                UnityEngine.Debug.Log("missed touchUp");
            }
        }
    }

    private void Awake()
    {
        UDPClient.Start();
        Application.runInBackground = true;
    }

    private void Start()
    {
        Process.Start("SurfaceUDP.exe");
    }

    private void Update()
    {
        RemoveOldTouches();
    }
}