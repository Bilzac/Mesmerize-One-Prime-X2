using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mes
{
    /*============================================================================
    * Video Monitor Class - Implements the default constructor of a video monitor
    * ===========================================================================
    */
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
