using System;
using System.Collections.Generic;
using System.IO;
using DataDynamics.PageFX.CodeModel;

namespace DataDynamics.PageFX
{
    public static class SimpleTestCases
    {
        static bool HasResource(string name)
        {
            var assembly = typeof(TestCase).Assembly;
            var resnames = assembly.GetManifestResourceNames();
            return Array.IndexOf(resnames, name) >= 0;
        }

        static string ReadText(string resName)
        {
            var assembly = typeof(TestCase).Assembly;
            var rs = assembly.GetManifestResourceStream(resName);
            return QA.ReadAllText(rs);
        }

        static List<TestCase> Load(string ext)
        {
            return Load("PageFX.Simple", ext);
        }

        public static List<TestCase> Load(string prefix, string ext)
        {
            var list = new List<TestCase>();
            var assembly = typeof(TestCase).Assembly;
            var resnames = assembly.GetManifestResourceNames();
            foreach (var resName in resnames)
            {
                if (resName.EndsWith(ext, StringComparison.InvariantCultureIgnoreCase)
                    && resName.Contains(prefix))
                {
                    var tc = CreateSimpleTestCase(resName, ext);
                    TestSuite.Register(tc);
                    list.Add(tc);
                }
            }
            return list;
        }

        #region LoadNUnitTests
        static void LoadNUnitTests(ICollection<TestCase> list, string path)
        {
            string fullpath = path + ".avm.dll";
            if (!File.Exists(fullpath)) return;

            if (!QA.LoadNUnitTests) return;

            string name = Path.GetFileNameWithoutExtension(path);
            string tcroot = Path.Combine(QA.Root, name);

            string error = "";
            IAssembly asm;

            bool oldVal = GlobalSettings.EmitDebugInfo;
            try
            {
                GlobalSettings.EmitDebugInfo = false;
                asm = QA.LoadAssembly(fullpath, VM.AVM, tcroot, ref error);
            }
            finally
            {
                GlobalSettings.EmitDebugInfo = oldVal;
            }

            if (asm == null) return;

            foreach (var type in asm.Types)
            {
                if (NUnitHelper.IsTestFixture(type))
                    LoadNUnitTests(list, type, path);
            }
        }

        static void LoadNUnitTests(ICollection<TestCase> list, IType testSuite, string path)
        {
            if (QA.RunSuiteAsOneTest)
            {
                string main = NUnitHelper.GenerateRunnerCode(testSuite, QA.TestRunnerOptions);

                string name = NUnitHelper.GetMonoTestSuiteName(testSuite);

                RegisterNUnitTest(list, name, path, main);
            }
            else
            {
                foreach (var test in NUnitHelper.GetTests(testSuite))
                {
                    string main = NUnitHelper.GenerateRunnerCode(test, QA.TestRunnerOptions);
                    string name = NUnitHelper.GetMonoTestCaseName(test);

                    RegisterNUnitTest(list, name, path, main);
                }
            }
        }

        static void RegisterNUnitTest(ICollection<TestCase> list, string name, string path, string mainCode)
        {
            var tc = new TestCase("PageFX.Simple." + name)
                         {
                             IsNUnit = true,
                             NUnitBasePath = path
                         };

            tc.SourceFiles.Add("Program.cs", mainCode);
            list.Add(tc);

            TestSuite.Register(tc);
        }
        #endregion

        static TestCase CreateSimpleTestCase(string resName, string ext)
        {
            var tc = new TestCase(resName, ReadText(resName));
            
            string configName = resName.Substring(0, resName.Length - ext.Length) + ".xml";
            if (HasResource(configName))
            {
                var assembly = typeof(TestCase).Assembly;
                var rs = assembly.GetManifestResourceStream(configName);
                tc.LoadConfig(rs);
            }
            return tc;
        }

        public static List<TestCase> All
        {
            get
            {
                if (_all == null)
                {
                    _all = new List<TestCase>();
                    _all.AddRange(CSharp);
                    _all.AddRange(VB);
                }
                return _all;
            }
        }

        static List<TestCase> _all;

        public static TestCase Find(string name)
        {
            return Algorithms.Find(All, tc => string.Compare(tc.FullName, name, true) == 0);
        }

        /// <summary>
        /// Gets all CSharp test cases.
        /// </summary>
        public static List<TestCase> CSharp
        {
            get
            {
                if (_cs == null)
                {
                    _cs = Load(".cs");
                    if (QA.LoadNUnitTests)
                    {
                        string dir = QA.NUnitTestsDirectory;
                        LoadNUnitTests(_cs, Path.Combine(dir, "mono"));
                        if (QA.IsNUnitSession)
                            QA.CopyNUnitTests();
                    }
                    if (QA.SortTests)
                        _cs.Sort((x, y) => x.FullName.CompareTo(y.FullName));
                }
                return _cs;
            }
        }
        static List<TestCase> _cs;

        /// <summary>
        /// Gets all VB test cases.
        /// </summary>
        public static List<TestCase> VB
        {
            get
            {
                if (_vb == null)
                    _vb = Load(".vb");
                return _vb;
            }
        }
        static List<TestCase> _vb;
    }
}