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
            this.Disable();
            this.Untrigger();
            this.Type = "video";
        }
    }
}
