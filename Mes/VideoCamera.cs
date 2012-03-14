using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mes
{
    class VideoCamera : Monitor
    {
        public VideoCamera()
        {
            isEnabled = false;
            isTriggered = false;
            location = null;
            sensorType = "video";
            monitorId = 0;
        }
    }
}
