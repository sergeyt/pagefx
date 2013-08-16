#if DEBUG
using System;
using System.Collections.Generic;
using System.IO;
using DataDynamics.PageFX.CLI.CFG;
using DataDynamics.PageFX.CodeModel;

namespace DataDynamics.PageFX.CLI.IL
{
    using Code = List<IInstruction>;

    internal partial class ILTranslator
    {
        static readonly string separator = new string('-', 200);
        int _beginSize;

        public void DumpILMap(string format, string filename)
        {
            CLIDebug.LogInfo("DumpILMap started. Format = {0}. FileName = {1}.", format, filename);

            string dir = DirHelper.GetDirectory(_body);
            Directory.CreateDirectory(dir);
            using (var writer = new StreamWriter(Path.Combine(dir, filename)))
            {
                DumpService.DumpLocalVariables(writer, _body);
                writer.WriteLine(separator);

                writer.WriteLine("#BEGIN CODE");
                writer.WriteLine(separator);
                for (int i = 0; i < _beginSize; ++i)
                {
                    writer.WriteLine(_outcode[i].ToString(format, null));
                }
                writer.WriteLine(separator);

                foreach (var bb in Blocks)
                {
                    CLIDebug.DoCancel();
                    writer.WriteLine("#BASIC BLOCK {0}", bb.Index);
                    DumpStackState(writer, bb);
                    writer.WriteLine(separator);

                    writer.WriteLine("#ORIGINAL CODE");
                    for (int i = 0; i < bb.Code.Count; ++i)
                    {
                        writer.WriteLine(bb.Code[i].ToString(format, null));
                    }
                    writer.WriteLine();

                    var code = bb.TranslatedCode;
                    writer.WriteLine("#TRANSLATED CODE");
                    for (int i = 0; i < code.Count; ++i)
                    {
                        writer.WriteLine(code[i].ToString(format, null));
                    }
                    writer.WriteLine(separator);
                }

                if (_endCode != null && _endCode.Length > 0)
                {
                    writer.WriteLine("#END CODE");
                    writer.WriteLine(separator);
                    for (int i = 0; i < _endCode.Length; ++i)
                        writer.WriteLine(_endCode[i].ToString(format, null));
                }
            }

            CLIDebug.LogInfo("DumpILMap succeded");
        }

        static void DumpStackState(TextWriter writer, Node bb)
        {
            if (!bb.IsTranslated)
                throw new InvalidOperationException();

            writer.Write("Stack Before: ");
            var arr = bb.StackBefore.ToArray();
            for (int i = 0; i < arr.Length; ++i)
            {
                if (i > 0) writer.Write(", ");
                writer.Write(arr[i].value.ToString());
            }
            writer.WriteLine();

            if (bb.Stack != null)
            {
                writer.Write("Stack After: ");
                arr = bb.Stack.ToArray();
                for (int i = 0; i < arr.Length; ++i)
                {
                    if (i > 0) writer.Write(", ");
                    writer.Write(arr[i].value.ToString());
                }
                writer.WriteLine();
            }
        }
    }
}
#endif