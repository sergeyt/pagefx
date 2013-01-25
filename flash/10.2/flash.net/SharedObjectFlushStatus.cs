using System;
using System.Runtime.CompilerServices;

namespace flash.net
{
    /// <summary>The SharedObjectFlushStatus class provides values for the code returned from a call to the SharedObject.flush()  method.</summary>
    [PageFX.AbcInstance(255)]
    [PageFX.ABC]
    [PageFX.FP9]
    public partial class SharedObjectFlushStatus : Avm.Object
    {
        /// <summary>Indicates that the flush completed successfully.</summary>
        [PageFX.AbcClassTrait(0)]
        [PageFX.ABC]
        [PageFX.FP9]
        public static Avm.String FLUSHED;

        /// <summary>
        /// Indicates that the user is being prompted to increase disk space for the shared object
        /// before the flush can occur.
        /// </summary>
        [PageFX.AbcClassTrait(1)]
        [PageFX.ABC]
        [PageFX.FP9]
        public static Avm.String PENDING;

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern SharedObjectFlushStatus();
    }
}
