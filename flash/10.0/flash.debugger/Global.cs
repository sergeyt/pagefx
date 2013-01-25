using System;
using System.Runtime.CompilerServices;

namespace flash.debugger
{
    [PageFX.GlobalFunctions]
    public class Global
    {
        [PageFX.ABC]
        [PageFX.QName("enterDebugger", "flash.debugger", "package")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void enterDebugger();
    }
}