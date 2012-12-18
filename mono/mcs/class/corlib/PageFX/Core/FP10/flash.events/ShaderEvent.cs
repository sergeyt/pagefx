using System;
using System.Runtime.CompilerServices;

namespace flash.events
{
    [PageFX.ABC]
    [PageFX.FP10]
    public class ShaderEvent : Event
    {
        [PageFX.ABC]
        [PageFX.FP10]
        public static Avm.String COMPLETE;

        public extern virtual flash.utils.ByteArray byteArray
        {
            [PageFX.ABC]
            [PageFX.FP10]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [PageFX.ABC]
            [PageFX.FP10]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
        }

        public extern virtual Avm.Vector<double> vector
        {
            [PageFX.ABC]
            [PageFX.FP10]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [PageFX.ABC]
            [PageFX.FP10]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
        }

        public extern virtual flash.display.BitmapData bitmapData
        {
            [PageFX.ABC]
            [PageFX.FP10]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [PageFX.ABC]
            [PageFX.FP10]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern ShaderEvent(Avm.String arg0, bool arg1, bool arg2, flash.display.BitmapData arg3, flash.utils.ByteArray arg4, Avm.Vector<double> arg5);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern ShaderEvent(Avm.String arg0, bool arg1, bool arg2, flash.display.BitmapData arg3, flash.utils.ByteArray arg4);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern ShaderEvent(Avm.String arg0, bool arg1, bool arg2, flash.display.BitmapData arg3);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern ShaderEvent(Avm.String arg0, bool arg1, bool arg2);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern ShaderEvent(Avm.String arg0, bool arg1);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern ShaderEvent(Avm.String arg0);

        [PageFX.ABC]
        [PageFX.FP10]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern override Avm.String toString();

        [PageFX.ABC]
        [PageFX.FP10]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern override Event clone();
    }
}