using DataDynamics.PageFX.CodeModel;
using DataDynamics.PageFX.CodeModel.TypeSystem;

namespace DataDynamics.PageFX.FLI
{
    internal class InternalType : ISpecialType
    {
        public InternalType(IType type)
        {
            _type = type;
            type.Tag = this;
        }

        public IType Type
        {
            get { return _type; }
        }
        private readonly IType _type;

        public override string ToString()
        {
            return _type.ToString();
        }
    }
}