using DataDynamics.PageFX.Common.CodeModel;
using DataDynamics.PageFX.Common.TypeSystem;
using DataDynamics.PageFX.FlashLand.IL;

namespace DataDynamics.PageFX.FlashLand.Core.CodeProvider
{
    internal partial class CodeProviderImpl
    {
        public IInstruction[] TypeOf(IType type)
        {
            var code = new AbcCode(_abc);
            code.TypeOf(type);
            return code.ToArray();
        }

        public IInstruction[] SizeOf(IType type)
        {
            EnsureType(type);
            var code = new AbcCode(_abc);
            //TODO:
            return code.ToArray();
        }
    }
}