using System.Collections.Generic;

namespace SurfaceManagement
{
    public class DataStream
    {
        public Queue<Touch> TouchDown;
        public Dictionary<int, Touch> TouchMove;
        public Queue<Touch> TouchUp;
        public List<int> IDlist;
        public double maxUpdateTime;

        public DataStream()
        {
            TouchMove = new Dictionary<int, Touch>();
            TouchDown = new Queue<Touch>();
            TouchUp = new Queue<Touch>();
            IDlist = new List<int>();
            maxUpdateTime = 1; // seconds
        }
    }
}