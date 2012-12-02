using System.Diagnostics;
using DataDynamics.PageFX.CodeModel;
using DataDynamics.PageFX.CodeModel.TypeSystem;

namespace DataDynamics.PageFX.FLI
{
    internal partial class AbcGenerator
    {
        public void CheckEmbedAsset(IField field)
        {
            Debug.Assert(field.HasEmbedAttribute());

            if (!field.IsStatic)
                throw Errors.EmbedAsset.FieldIsNotStatic.CreateException(field.Name);

            if (!IsSwf)
                throw Errors.EmbedAsset.NotFlashRuntime.CreateException();
        }

        int PlayerVersion
        {
            get 
            {
                if (sfc != null)
                    return sfc.PlayerVersion;
                return -1;
            }
        }

        public void CheckApiCompatibility(ITypeMember m)
        {
            if (m == null) return;
            if (!IsSwf) return;

            int v = m.GetPlayerVersion();
            if (v < 0) return;

            if (v > PlayerVersion)
            {
                var method = m as IMethod;
                if (method != null)
                {
                    CompilerReport.Add(Errors.ABC.IncompatibleCall, method.GetFullName(), v);
                    return;
                }

                var f = m as IField;
                if (f != null)
                {
                    CompilerReport.Add(Errors.ABC.IncompatibleField, f.GetFullName(), v);
                    return;
                }
            }
        }
    }
}