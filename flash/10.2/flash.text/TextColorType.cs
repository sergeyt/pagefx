using System;
using System.Runtime.CompilerServices;

namespace flash.text
{
    /// <summary>The TextColorType class provides color values for the flash.text.TextRenderer class.</summary>
    [PageFX.AbcInstance(250)]
    [PageFX.ABC]
    [PageFX.FP9]
    public partial class TextColorType : Avm.Object
    {
        /// <summary>
        /// Used in the colorType parameter in the setAdvancedAntiAliasingTable() method.
        /// Use the syntax TextColorType.DARK_COLOR.
        /// </summary>
        [PageFX.AbcClassTrait(0)]
        [PageFX.ABC]
        [PageFX.FP9]
        public static Avm.String DARK_COLOR;

        /// <summary>
        /// Used in the colorType parameter in the setAdvancedAntiAliasingTable() method.
        /// Use the syntax TextColorType.LIGHT_COLOR.
        /// </summary>
        [PageFX.AbcClassTrait(1)]
        [PageFX.ABC]
        [PageFX.FP9]
        public static Avm.String LIGHT_COLOR;

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern TextColorType();
    }
}
