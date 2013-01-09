﻿using DataDynamics.PageFX.FlashLand.Abc;
using DataDynamics.PageFX.FlashLand.Core.CodeGeneration.Corlib;

namespace DataDynamics.PageFX.FlashLand.Core.CodeGeneration.Builders
{
    internal static class HashCodeImpl
    {
	    public static AbcMethod CalcHashCode(AbcGenerator generator)
        {
            var instance = generator.RuntimeImpl.Instance;

	        return instance.DefineMethod(
		        Sig.@static("get_hash_code", AvmTypeCode.Int32, AvmTypeCode.Object),
		        code =>
			        {
				        var getDic = GetHashCodeDic(generator);

				        const int varKey = 1;
				        const int varDic = 2;
				        const int varHC = 3;

				        code.PushInt(0);
				        code.SetLocal(varHC);

				        code.LoadThis();
				        code.Call(getDic);
				        code.SetLocal(varDic);

				        code.GetLocal(varDic);
				        code.GetLocal(varKey);
				        code.GetNativeArrayItem(); //[]
				        code.CoerceInt32();
				        code.SetLocal(varHC);

				        code.GetLocal(varHC);
				        code.PushInt(0);
				        var br = code.IfNotEquals();

				        code.GetLocal(varDic);
				        code.GetLocal(varKey);
						code.CallStatic(generator.Corlib.GetMethod(ObjectMethodId.NewHashCode));
				        code.SetLocal(varHC);
				        code.GetLocal(varHC);
				        code.SetNativeArrayItem(); //dic[key] = value

				        br.BranchTarget = code.Label();
				        code.GetLocal(varHC);
				        code.ReturnValue();
			        });
        }

        private static AbcMethod GetHashCodeDic(AbcGenerator generator)
        {
            var instance = generator.RuntimeImpl.Instance;

            var dicType = generator.Abc.DefineQName("flash.utils", "Dictionary");
            var dic = instance.DefineStaticSlot("hcdic$", dicType);

	        return instance.DefineMethod(
		        Sig.@static("get_hashcode_dic", dicType),
		        code =>
			        {
				        code.LoadThis();
				        code.GetProperty(dic);
				        var br = code.IfNotNull();

				        code.LoadThis();
				        code.CreateInstance(dicType);
				        code.SetProperty(dic);

				        br.BranchTarget = code.Label();
				        code.LoadThis();
				        code.GetProperty(dic);
				        code.ReturnValue();
			        });
        }
    }
}