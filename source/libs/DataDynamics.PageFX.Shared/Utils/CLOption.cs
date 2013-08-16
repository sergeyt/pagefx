using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace DataDynamics
{
    public class CLOption
    {
        #region ctors
        public CLOption(string name)
        {
            Name = name;
        }

        public CLOption(string name, string aliases)
        {
            Name = name;
            Aliases = aliases;
        }
        #endregion

        #region Fields
        /// <summary>
        /// Name of this option
        /// </summary>
        public string Name;

        /// <summary>
        /// Format of value
        /// </summary>
        public string Format;

        /// <summary>
        /// Description for this option
        /// </summary>
        public string Description;

        /// <summary>
        /// Default value for this option
        /// </summary>
        public string DefaultValue;

        public string Aliases;

        /// <summary>
        /// Cetegory of this option
        /// </summary>
        public string Category;

        public bool DotNetCompiler;

        public bool PlusMinus;
        #endregion

        #region Properties
        static readonly char[] sep = { ',', ';' };

        public string[] Names
        {
            get
            {
                if (_names == null)
                {
                    var list = new List<string>();

                    AddName(list, Name);
                    
                    if (!string.IsNullOrEmpty(Aliases))
                    {
                        var ss = Aliases.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < ss.Length; ++i)
                            AddName(list, ss[i]);
                    }

                    _names = list.ToArray();
                }
                return _names;
            }
        }

        void AddName(ICollection<string> list, string name)
        {
            list.Add(name);
            if (PlusMinus)
            {
                list.Add(name + '+');
                list.Add(name + '-');
            }
        }

        string[] _names;
        #endregion

        public override string ToString()
        {
            return ToString(DefaultValue);
        }

        public string ToString(string value)
        {
            if (value == null)
                return string.Format("/{0}", Name);
            return string.Format("/{0}:{1}", Name, value);
        }

        public bool CheckName(string name, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return Algorithms.Contains(Names, s => string.Compare(name, s, ignoreCase) == 0);
        }

        public static void Init(Type options)
        {
            var fields = options.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var fi in fields)
            {
                if (fi.FieldType == typeof(CLOption))
                {
                    var opt = fi.GetValue(null) as CLOption;
                    if (opt != null)
                    {
                        var desc = ReflectionHelper.GetAttribute<DescriptionAttribute>(fi, true);
                        if (desc != null)
                            opt.Description = desc.Description;

                        var cat = ReflectionHelper.GetAttribute<CategoryAttribute>(fi, true);
                        if (cat != null)
                            opt.Category = cat.Category;

                        var format = ReflectionHelper.GetAttribute<FormatAttribute>(fi, true);
                        if (format != null)
                            opt.Format = format.Value;

                        var defVal = ReflectionHelper.GetAttribute<DefaultValueAttribute>(fi, true);
                        if (defVal != null)
                            opt.DefaultValue = defVal.Value.ToString();
                    }
                }
            }
        }
    }
}