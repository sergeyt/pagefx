using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using DataDynamics.PageFX.CodeModel;
using DataDynamics.PageFX.FLI.IL;
using DataDynamics.PageFX.FLI.SWC;
using DataDynamics.PageFX.FLI.SWF;

namespace DataDynamics.PageFX.FLI.ABC
{
    #region enum AbcClassFlags
    [Flags]
    public enum AbcClassFlags
    {
        /// <summary>
        /// The class is sealed: properties can not be dynamically added
        /// to instances of the class.
        /// </summary>
        Sealed = 0x01,

        /// <summary>
        /// The class is final: it cannot be a base class for any other
        /// class.
        /// </summary>
        Final = 0x02,

        /// <summary>
        /// The class is an interface.
        /// </summary>
        Interface = 0x04,

        /// <summary>
        /// The class uses its protected namespace and the
        /// protectedNs field is present in the interface_info
        /// structure.
        /// </summary>
        ProtectedNamespace = 0x08,

        //NOTE: This flag was found in source of flex compiler.
        //We may investigate this, may be AVM already supports some kind of value types
        NonNullable = 0x10,

        SealedProtectedNamespace = Sealed | ProtectedNamespace,
        FinalSealed = Final | Sealed,
    }
    #endregion

    #region class AbcInstance
    /// <summary>
    /// Contains traits for non-static members of user defined type.
    /// </summary>
    public class AbcInstance : ISupportXmlDump, ISwfIndexedAtom, IAbcTraitProvider
    {
        #region Constructors
        public AbcInstance()
        {
            _traits = new AbcTraitCollection(this);
        }

        public AbcInstance(SwfReader reader)
            : this()
        {
            Read(reader);
        }

        public AbcInstance(bool createClass)
            : this()
        {
            if (createClass)
                Class = new AbcClass();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets index of the <see cref="AbcInstance"/> within collection of <see cref="AbcInstance"/> in ABC file.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets type QName.
        /// </summary>
        public AbcMultiname Name { get; set; }

        /// <summary>
        /// Gets namespace name
        /// </summary>
        public string NamespaceString
        {
            get
            {
                if (Name != null)
                    return Name.NamespaceString;
                return "";
            }
        }

        /// <summary>
        /// Gets type name
        /// </summary>
        public string NameString
        {
            get
            {
                if (Name != null)
                    return Name.NameString;
                return "";
            }
        }

        public string FullName
        {
            get
            {
                if (Name != null)
                    return Name.FullName;
                return "";
            }
        }

        public string ID
        {
            get
            {
                string ns = NamespaceString;
                if (string.IsNullOrEmpty(ns))
                    return NameString;
                return ns + ":" + NameString;
            }
        }

        public Visibility Visibility
        {
            get
            {
                if (Name == null) return Visibility.Private;
                return Name.Visibility;
            }
        }

        /// <summary>
        /// Gets or sets the name of base type.
        /// </summary>
        public AbcMultiname SuperName { get; set; }

        internal AbcInstance SuperType
        {
            get
            {
                if (_superType == null)
                {
                    if (IsError)
                        return _superType = AssemblyTag.AvmGlobalTypes.Object;

                    var type = Type;
                    if (type != null)
                    {
                        var super = type.BaseType;
                        if (super != null)
                        {
                            var instance = super.Tag as AbcInstance;
                            if (instance != null)
                                return instance;
                        }
                    }
                }
                return _superType;
            }
            set 
            {
                if (_superType != null)
                {
                    int i = Algorithms.IndexOf(_superType.Subclasses, this);
                    if (i >= 0)
                        _superType.Subclasses.RemoveAt(i);
                }
                _superType = value;
                if (value != null)
                    _superType.Subclasses.Add(this);
            }
        }
        AbcInstance _superType;

        /// <summary>
        /// Gets or sets type assotiated with this <see cref="AbcInstance"/>.
        /// </summary>
        public IType Type { get; set; }

        /// <summary>
        /// Gets or sets class flags.
        /// </summary>
        public AbcClassFlags Flags { get; set; }

        public bool IsInterface
        {
            get { return (Flags & AbcClassFlags.Interface) != 0; }
            set
            {
                if (value) Flags |= AbcClassFlags.Interface;
                else Flags &= ~AbcClassFlags.Interface;
            }
        }

        public bool IsNative { get; set; }

        internal AbcInstance ImportedFrom { get; set; }

        /// <summary>
        /// Imported <see cref="AbcInstance"/>.
        /// </summary>
        internal AbcInstance ImportedInstance { get; set; }

        /// <summary>
        /// Returns true if this <see cref="AbcInstance"/> was imported from other <see cref="AbcInstance"/>.
        /// </summary>
        public bool IsImported
        {
            get { return ImportedFrom != null; }
        }

        public bool IsForeign
        {
            get { return IsNative || IsImported; }
        }
        
        public bool HasGlobalName(string name)
        {
            if (!IsGlobal) return false;
            return NameString == name;
        }

        /// <summary>
        /// Returns true if this instance is defined in global package.
        /// </summary>
        public bool IsGlobal
        {
            get
            {
                var mn = Name;
                if (mn == null) return false;
                var ns = mn.Namespace;
                if (ns == null) return false;
                return ns.IsGlobalPackage;
            }
        }

        public bool IsObject
        {
            get { return HasGlobalName(Const.AvmGlobalTypes.Object); }
        }

        public bool IsError
        {
            get { return HasGlobalName(Const.AvmGlobalTypes.Error); }
        }

        public AbcFile ABC
        {
            get { return _abc; }
            set { _abc = value; }
        }
        AbcFile _abc;

        public AbcNamespace ProtectedNamespace
        {
            get { return _protectedNamespace; }
            set { _protectedNamespace = value; }
        }
        AbcNamespace _protectedNamespace;

        public bool HasProtectedNamespace
        {
            get { return (Flags & AbcClassFlags.ProtectedNamespace) != 0; }
            set
            {
                if (value) Flags |= AbcClassFlags.ProtectedNamespace;
                else Flags &= ~AbcClassFlags.ProtectedNamespace;
            }
        }

        public AbcMethod Initializer
        {
            get { return _initializer; }
            set
            {
                if (value != _initializer)
                {
                    _initializer = value;
                    if (value != null)
                    {
                        value.IsInitializer = true;
                        value.Instance = this;
                    }
                }
            }
        }
        AbcMethod _initializer;

        internal AbcMethod StaticCtor;
        internal AbcTrait StaticCtorFlag;
        internal AbcMethod StaticCtorCaller;
        
        public List<AbcMultiname> Interfaces
        {
            get { return _interfaces; }
        }
        readonly List<AbcMultiname> _interfaces = new List<AbcMultiname>();

        public bool HasInterface(AbcInstance iface)
        {
            var ifaceName = iface.Name;
        	return Interfaces.Any(mn => mn == ifaceName);
        }

        public AbcTraitCollection Traits
        {
            get { return _traits; }
        }

        private readonly AbcTraitCollection _traits;

        public IEnumerable<AbcTrait> GetAllTraits()
        {
            foreach (var t in _traits)
                yield return t;
            if (_class != null)
            {
                foreach (var t in _class.Traits)
                    yield return t;
            }
        }

        public AbcTraitCollection GetTraits(bool isStatic)
        {
            if (isStatic)
                return Class.Traits;
            return Traits;
        }

        public AbcClass Class
        {
            get { return _class; }
            set
            {
                if (value != _class)
                {
                    if (_class != null)
                        _class.Instance = null;
                    _class = value;
                    if (_class != null)
                        _class.Instance = this;
                }
            }
        }
        private AbcClass _class;

        internal bool InSwc
        {
            get
            {
                if (_abc != null)
                    return _abc.InSwc;
                return false;
            }
        }

        internal SwcFile SWC
        {
            get
            {
                if (_abc != null)
                    return _abc.SWC;
                return null;
            }
        }

        internal bool UseExternalLinking
        {
            get
            {
                if (_abc != null)
                    return _abc.UseExternalLinking;
                return false;
            }
        }

        internal bool IsLinkedExternally { get; set; }

        internal Embed Embed
        {
            get { return _embed; }
            set { _embed = value; }
        }
        internal Embed _embed;

        internal bool IsEmbeddedAsset
        {
            get { return Embed != null; }
        }

        internal string Locale { get; set; }

        internal string ResourceBundleName { get; set; }

        internal bool IsResourceBundle
        {
            get { return !string.IsNullOrEmpty(ResourceBundleName); }
        }

        internal bool IsMixin { get; set; }

        internal bool IsStyleMixin { get; set; }

        internal bool IsFlexInitMixin { get; set; }

        internal bool HasStyles { get; set; }

        internal bool IsApp { get; set; }

        internal float FlashVersion
        {
            get { return _flashVersion; }
            set { _flashVersion = value; }
        }
        private float _flashVersion = -1;

        internal bool Ordered;
        #endregion

        #region Methods
        /// <summary>
        /// Adds trait to instance or class trait collection.
        /// </summary>
        /// <param name="trait">trait to add</param>
        /// <param name="isStatic">true to add to class trais; false - to instance traits</param>
        public void AddTrait(AbcTrait trait, bool isStatic)
        {
            if (trait == null)
                throw new ArgumentNullException("trait");
            //Note: traits for static members are contained within ABC file in array of AbcClass
            if (isStatic)
                _class.Traits.Add(trait);
            else
                _traits.Add(trait);
        }

        public AbcNamespace GetPrivateNamespace()
        {
            return _abc.DefinePrivateNamespace(this);
        }

        public AbcMultiname DefinePrivateName(string name)
        {
            var ns = GetPrivateNamespace();
            return _abc.DefineQName(ns, name);
        }

        //public AbcTrait GetSlot(string name)
        //{
        //    return _traits.Find(name);
        //}

        public AbcTrait GetStaticSlot(string name)
        {
            return _class.Traits.Find(name);
        }

        #region CreateSlot
        public AbcTrait CreateSlot(object name, object type, bool isStatic)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (type == null)
                throw new ArgumentNullException("type");

            var traitName = _abc.DefineName(name);
            var typeName = _abc.DefineTypeNameStrict(type);

            var trait = AbcTrait.CreateSlot(typeName, traitName);

            AddTrait(trait, isStatic);
            
            return trait;
        }

        public AbcTrait CreateSlot(object name, object type)
        {
            return CreateSlot(name, type, false);
        }

        public AbcTrait CreateStaticSlot(object name, object type)
        {
            return CreateSlot(name, type, true);
        }

        public AbcTrait CreatePrivateSlot(string name, object type)
        {
            var mn = DefinePrivateName(name);
            return CreateSlot(mn, type);
        }

        public AbcTrait CreatePrivateStaticSlot(string name, object type)
        {
            var mn = DefinePrivateName(name);
            return CreateStaticSlot(mn, type);
        }
        #endregion

        #region DefineSlot
        public AbcTrait DefineSlot(object name, object type, bool isStatic)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (type == null)
                throw new ArgumentNullException("type");

            var traitName = _abc.DefineName(name);

            var traits = isStatic ? Class.Traits : Traits;
            var trait = traits.Find(traitName, AbcTraitKind.Slot);
            if (trait != null) return trait;

            var typeName = _abc.DefineTypeNameStrict(type);

            trait = AbcTrait.CreateSlot(typeName, traitName);
            traits.Add(trait);

            return trait;
        }

        public AbcTrait DefineSlot(object name, object type)
        {
            return DefineSlot(name, type, false);
        }

        public AbcTrait DefineStaticSlot(object name, object type)
        {
            return DefineSlot(name, type, true);
        }

        public AbcTrait DefinePrivateSlot(string name, object type)
        {
            var mn = DefinePrivateName(name);
            return DefineSlot(mn, type);
        }

        public AbcTrait DefinePrivateStaticSlot(string name, object type)
        {
            var mn = DefinePrivateName(name);
            return DefineStaticSlot(mn, type);
        }
        #endregion

        #region DefineMethod
        #region DefineMethod - Main Impl
        internal AbcMethod DefineMethod(object name, object returnType,
                                        AbcTraitKind kind, AbcMethodSemantics sem,
                                        AbcCoder coder, params object[] args)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            var klass = Class;
            if (klass == null)
                throw new InvalidOperationException(string.Format("Class is not defined yet for Instance {0}", FullName));

            var traitName = _abc.DefineName(name);

            bool isStatic = (sem & AbcMethodSemantics.Static) != 0;

            var traits = isStatic ? klass.Traits : Traits;
            var trait = traits.Find(traitName, kind);
            if (trait != null)
                return trait.Method;

            var retType = _abc.DefineTypeNameStrict(returnType);

            var method = new AbcMethod 
            {
                ReturnType = retType
            };

            trait = AbcTrait.CreateMethod(method, traitName);
            trait.Kind = kind;
            if (!isStatic)
            {
                trait.IsVirtual = (sem & AbcMethodSemantics.Virtual) != 0;
                trait.IsOverride = (sem & AbcMethodSemantics.Override) != 0;
            }
            traits.Add(trait);

            if (args != null)
            {
                if (args.Length == 1 && args[0] is IMethod)
                {
                    var m = (IMethod)args[0];
                    _abc.generator.DefineParameters(method, m);
                }
                else
                {
                    _abc.DefineParams(method.Parameters, args);
                }
            }

            var body = new AbcMethodBody(method);
            _abc.AddMethod(method);

            if (coder != null)
            {
                var code = new AbcCode(_abc);
                coder(code);
                body.Finish(code);
            }

            return method;
        }

        internal AbcMethod DefineMethod(AbcMethod prototype, AbcCoder coder, bool resetOverrideFlag)
        {
            var t = prototype.Trait;
            if (t == null)
                throw new InvalidOperationException();
            var m = DefineMethod(t.Name, prototype.ReturnType, t.Kind, t.MethodSemantics, coder, prototype);
            if (resetOverrideFlag)
                m.Trait.IsOverride = false;
            return m;
        }

        internal AbcMethod DefineMethod(AbcMethod prototype, AbcCoder coder)
        {
            return DefineMethod(prototype, coder, false);
        }
        #endregion

        #region DefineMethod
        internal AbcMethod DefineMethod(object name, object returnType, AbcMethodSemantics sem,
            AbcCoder coder, params object[] args)
        {
            return DefineMethod(name, returnType, AbcTraitKind.Method, sem, coder, args);
        }

        internal AbcMethod DefineInstanceMethod(object name, object returnType,
                                                AbcCoder coder, params object[] args)
        {
            return DefineMethod(name, returnType, AbcMethodSemantics.Default, coder, args);
        }

        internal AbcMethod DefineVirtualMethod(object name, object returnType,
                                               AbcCoder coder, params object[] args)
        {
            return DefineMethod(name, returnType, AbcMethodSemantics.Virtual, coder, args);
        }

        internal AbcMethod DefineOverrideMethod(object name, object returnType,
                                                AbcCoder coder, params object[] args)
        {
            return DefineMethod(name, returnType, AbcMethodSemantics.Override, coder, args);
        }

        internal AbcMethod DefineVirtualOverrideMethod(object name, object returnType,
                                                       AbcCoder coder, params object[] args)
        {
            return DefineMethod(name, returnType, AbcMethodSemantics.VirtualOverride, coder, args);
        }

        internal AbcMethod DefineStaticMethod(object name, object returnType,
                                              AbcCoder coder, params object[] args)
        {
            return DefineMethod(name, returnType, AbcMethodSemantics.Static, coder, args);
        }
        #endregion

        #region DefineGetter, DefineSetter
        internal AbcMethod DefineGetter(object name, object returnType, AbcMethodSemantics sem, AbcCoder coder)
        {
            return DefineMethod(name, returnType, AbcTraitKind.Getter, sem, coder);
        }

        internal AbcMethod DefineSetter(object name, object valueType, AbcMethodSemantics sem, AbcCoder coder)
        {
            return DefineMethod(name, AvmTypeCode.Void, AbcTraitKind.Setter, sem, coder, valueType, "value");
        }

        internal AbcMethod DefineInstanceGetter(object name, object returnType, AbcCoder coder)
        {
            return DefineGetter(name, returnType, AbcMethodSemantics.Default, coder);
        }

        internal AbcMethod DefineInstanceSetter(object name, object valueType, AbcCoder coder)
        {
            return DefineSetter(name, valueType, AbcMethodSemantics.Default, coder);
        }

        internal AbcMethod DefineStaticGetter(object name, object returnType, AbcCoder coder)
        {
            return DefineGetter(name, returnType, AbcMethodSemantics.Static, coder);
        }

        internal AbcMethod DefineStaticSetter(object name, object valueType, AbcCoder coder)
        {
            return DefineSetter(name, valueType, AbcMethodSemantics.Static, coder);
        }

        internal AbcMethod DefinePtrGetter(AbcCoder coder)
        {
            return DefineInstanceGetter(_abc.PtrValueName, AvmTypeCode.Object, coder);
        }

        internal AbcMethod DefinePtrSetter(AbcCoder coder)
        {
            return DefineInstanceSetter(_abc.PtrValueName, AvmTypeCode.Object, coder);
        }
        #endregion
        #endregion
        #endregion

        #region IO
        public void Read(SwfReader reader)
        {
            Name = AbcIO.ReadMultiname(reader);
            SuperName = AbcIO.ReadMultiname(reader);
            Flags = (AbcClassFlags)reader.ReadUInt8();

            if ((Flags & AbcClassFlags.ProtectedNamespace) != 0)
            {
                _protectedNamespace = AbcIO.ReadNamespace(reader);
            }

            int intrf_count = (int)reader.ReadUIntEncoded();
            for (int i = 0; i < intrf_count; ++i)
            {
                var iface = AbcIO.ReadMultiname(reader);
                _interfaces.Add(iface);
            }

            Initializer = AbcIO.ReadMethod(reader);

            _traits.Read(reader);
        }

        public void Write(SwfWriter writer)
        {
            writer.WriteUIntEncoded((uint)Name.Index);

            if (SuperName == null) writer.WriteUInt8(0);
            else writer.WriteUIntEncoded((uint)SuperName.Index);

            writer.WriteUInt8((byte)Flags);

            if ((Flags & AbcClassFlags.ProtectedNamespace) != 0)
            {
                writer.WriteUIntEncoded((uint)_protectedNamespace.Index);
            }

            int n = _interfaces.Count;
            writer.WriteUIntEncoded((uint)n);
            for (int i = 0; i < n; ++i)
            {
                var iface = _interfaces[i];
                writer.WriteUIntEncoded((uint)iface.Index);
            }

            writer.WriteUIntEncoded((uint)_initializer.Index);
            _traits.Write(writer);
        }
        #endregion

        #region Dump
        #region Xml Format
        internal bool IsDumped;

        public void DumpXml(XmlWriter writer)
        {
            if (AbcDumpService.FilterClass(this)) return;
            IsDumped = true;

            writer.WriteStartElement("instance");
            writer.WriteAttributeString("index", Index.ToString());

            if (Name != null)
                writer.WriteAttributeString("name", Name.FullName);

            //if (_superName != null)
            //    writer.WriteAttributeString("supername", _superName.FullName);
            //if (_name != null)
            //    writer.WriteElementString("name", _name.ToString());

            if (SuperName != null)
                writer.WriteElementString("super", SuperName.ToString());

            writer.WriteElementString("flags", Flags.ToString());

            if ((Flags & AbcClassFlags.ProtectedNamespace) != 0 && _protectedNamespace != null)
            {
                writer.WriteStartElement("protectedNamespace");
                writer.WriteAttributeString("name", _protectedNamespace.Name.Value);
                writer.WriteAttributeString("kind", _protectedNamespace.Kind.ToString());
                writer.WriteEndElement();
            }

            if (_interfaces.Count > 0)
            {
                writer.WriteStartElement("interfaces");
                foreach (var i in _interfaces)
                {
                    writer.WriteStartElement("interface");
                    writer.WriteAttributeString("name", i.ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            if (_initializer != null)
            {
                if (AbcDumpService.DumpInitializerCode)
                {
                    bool old = AbcDumpService.DumpCode;
                    AbcDumpService.DumpCode = true;
                    _initializer.DumpXml(writer, "iinit");
                    AbcDumpService.DumpCode = old;
                }
                else
                {
                    writer.WriteElementString("iinit", _initializer.ToString());
                }
            }

            _traits.DumpXml(writer);

            if (_class != null)
                _class.DumpXml(writer);

            writer.WriteEndElement();
        }
        #endregion

        #region Text Format
        public void DumpDirectory(string dir)
        {
            string name = NameString;
            string ns = NamespaceString;
            if (!string.IsNullOrEmpty(ns))
            {
                dir = Path.Combine(dir, ns);
                Directory.CreateDirectory(dir);
            }
            string path = Path.Combine(dir, name + ".cs");
            using (var writer = new StreamWriter(path))
                Dump(writer, true);
        }

        private static void DumpMembers(TextWriter writer, AbcTraitCollection traits, string tab, bool isStatic)
        {
            string prefix = isStatic ? "Static" : "Instance";

            bool eol = false;
            var fields = traits.GetFields();
            if (fields != null && fields.Length > 0)
            {
                writer.WriteLine("{0}#region {1} Fields", tab, prefix);
                for (int i = 0; i < fields.Length; ++i)
                {
                    if (i > 0) writer.WriteLine();
                    fields[i].DumpField(writer, tab, isStatic);
                }
                writer.WriteLine("{0}#endregion", tab);
                eol = true;
            }

            var props = traits.GetProperties();
            if (props.Count > 0)
            {
                if (eol) writer.WriteLine();
                writer.WriteLine("{0}#region {1} Properties", tab, prefix);
                props.Dump(writer, tab, isStatic);
                writer.WriteLine("{0}#endregion", tab);
                eol = true;
            }

            var methods = traits.GetMethods();
            if (methods != null && methods.Length > 0)
            {
                if (eol) writer.WriteLine();
                writer.WriteLine("{0}#region {1} Methods", tab, prefix);
                int n = methods.Length;
                for (int i = 0; i < n; ++i)
                {
                    if (i > 0) writer.WriteLine();
                    methods[i].Dump(writer, tab, isStatic);
                }
                writer.WriteLine("{0}#endregion", tab);
            }
        }

        public void Dump(TextWriter writer, bool withNamespace)
        {
            var tab = new Indent();
            if (withNamespace)
            {
                writer.WriteLine("namespace {0}", NamespaceString);
                writer.WriteLine("{");
                tab++;
            }

            #region def
            writer.Write(tab.Value);
            writer.Write("public ");
            if ((Flags & AbcClassFlags.Final) != 0)
                writer.Write("final ");
            if ((Flags & AbcClassFlags.Sealed) != 0)
                writer.Write("sealed ");

            writer.Write("{0} {1}", IsInterface ? "interface" : "class", NameString);

            int ifaceNum = _interfaces.Count;
            if (SuperName != null || ifaceNum > 0)
                writer.Write(" : ");

            if (SuperName != null)
            {
                writer.Write(SuperName.NameString);
                if (ifaceNum > 0)
                    writer.Write(", ");
            }
            for (int i = 0; i < ifaceNum; ++i)
            {
                if (i > 0) writer.Write(", ");
                writer.Write(_interfaces[i].NameString);
            }
            writer.WriteLine();
            #endregion

            writer.WriteLine("{0}{{", tab.Value);

            tab++;
            DumpMembers(writer, _traits, tab, false);
            if (_class != null)
                DumpMembers(writer, _class.Traits, tab, true);
            --tab;

            writer.WriteLine("{0}}}", tab);

            if (withNamespace)
                writer.WriteLine("}");
        }
        #endregion
        #endregion

        #region Object Override Methods
        public override string ToString()
        {
            if (Name != null)
                return Name.ToString();
            return base.ToString();
        }
        #endregion

        #region Internal Properties
        internal List<AbcInstance> Implementations
        {
            get { return _impls ?? (_impls = new List<AbcInstance>()); }
        }
        private List<AbcInstance> _impls;

        internal List<AbcInstance> Implements
        {
            get { return _ifaces ?? (_ifaces = new List<AbcInstance>()); }
        }
        private List<AbcInstance> _ifaces;

        internal List<AbcInstance> Subclasses
        {
            get { return _subclasses ?? (_subclasses = new List<AbcInstance>()); }
        }
        private List<AbcInstance> _subclasses;

        internal List<AbcFile> ImportAbcFiles
        {
            get { return _importAbcFiles ?? (_importAbcFiles = new List<AbcFile>()); }
        }
        private List<AbcFile> _importAbcFiles;

        public AbcScript Script
        {
            get
            {
                if (_class != null)
                {
                    var t = _class.Trait;
                    if (t != null)
                        return t.Owner as AbcScript;
                }
                return null;
            }
        }
        #endregion

        #region Trait Cache
        void CacheTraits()
        {
            if (_traitCache != null) return;
            _traitCache = new AbcTraitCache();
            _traitCache.Add(GetAllTraits());
        }
        AbcTraitCache _traitCache;

        internal AbcTrait FindTrait(ITypeMember member)
        {
            CacheTraits();
            return _traitCache.Find(member);
        }
        #endregion

        #region Utils
        internal bool IsInheritedFrom(AbcMultiname typename)
        {
        	return SuperName == typename || _interfaces.Any(iface => iface == typename);
        }

    	internal bool IsTypeUsed(AbcMultiname typename)
        {
            foreach (var t in GetAllTraits())
            {
                switch (t.Kind)
                {
                    case AbcTraitKind.Function:
                    case AbcTraitKind.Getter:
                    case AbcTraitKind.Setter:
                    case AbcTraitKind.Method:
                        {
                            var m = t.Method;
                            if (m.IsTypeUsed(typename))
                                return true;
                        }
                        break;

                    case AbcTraitKind.Const:
                    case AbcTraitKind.Slot:
                        {
                            if (t.SlotType == typename)
                                return true;
                        }
                        break;
                }
            }
            return false;
        }

        public AbcTrait FindSuperTrait(AbcMultiname name, AbcTraitKind kind)
        {
            var st = SuperType;
            while (st != null)
            {
                var t = st.Traits.Find(name, kind);
                if (t != null) return t;
                st = st.SuperType;
            }
            return null;
        }
        #endregion
    }
    #endregion

    #region class AbcInstanceCollection
    public class AbcInstanceCollection : List<AbcInstance>, ISupportXmlDump
    {
        readonly AbcFile _abc;
        readonly Hashtable _cache = new Hashtable();

        public AbcInstanceCollection(AbcFile abc)
        {
            _abc = abc;
        }

        #region Public Members
        internal void AddInternal(AbcInstance instance)
        {
            base.Add(instance);
        }

        public new void Add(AbcInstance instance)
        {
            //#if DEBUG
            //            var other = Find(instance.Name);
            //            if (other != null)
            //                throw new InvalidOperationException();
            //            if (IsDefined(instance))
            //                throw new InvalidOperationException();
            //#endif
            instance.Index = Count;
            instance.ABC = _abc;
            _cache[instance.FullName] = instance;
            base.Add(instance);
        }

        public bool IsDefined(AbcInstance instance)
        {
            if (instance == null) return false;
            int index = instance.Index;
            if (index < 0 || index >= Count)
                return false;
            return this[index] == instance;
        }

        AbcInstance Find(string fullname)
        {
            return _cache[fullname] as AbcInstance;
        }

        public AbcInstance Find(AbcMultiname mname)
        {
            if (mname == null)
                throw new ArgumentNullException("mname");
            if (mname.IsRuntime) return null;
            string name = mname.NameString;
            if (string.IsNullOrEmpty(name))
                return null;
            if (mname.NamespaceSet != null)
            {
            	return mname.NamespaceSet
					.Select(ns => NameHelper.MakeFullName(ns.NameString, name))
					.Select(fullname => Find(fullname))
					.FirstOrDefault(instance => instance != null);
            }
        	return Find(mname.FullName);
        }

        public AbcInstance FindStrict(AbcMultiname name)
        {
            return Find(i => i.Name == name);
        }

        public AbcInstance this[AbcMultiname name]
        {
            get { return Find(name); }
        }

        public AbcInstance this[string fullname]
        {
            get { return Find(fullname); }
        }

        public bool Contains(AbcMultiname name)
        {
            return Find(name) != null;
        }
        #endregion

        #region IO
        private int _begin;
        private int _end;

        public void Read(int n, SwfReader reader)
        {
            _begin = (int)reader.Position;
            for (int i = 0; i < n; ++i)
            {
                Add(new AbcInstance(reader));
            }
            _end = (int)reader.Position;
        }

        public void Write(SwfWriter writer)
        {
            int n = Count;
            for (int i = 0; i < n; ++i)
                this[i].Write(writer);
        }

        public string FormatOffset(AbcFile file, int offset)
        {
            return AbcHelper.FormatOffset(file, offset, this, _begin, _end, "AbcInstance Array", false, false);
        }
        #endregion

        #region Dump
        public void DumpXml(XmlWriter writer)
        {
            if (!AbcDumpService.DumpInstances) return;
            writer.WriteStartElement("instances");
            writer.WriteAttributeString("count", Count.ToString());
            foreach (var i in this)
                i.DumpXml(writer);
            writer.WriteEndElement();
        }

        public void DumpDirectory(string dir)
        {
            foreach (var i in this)
                i.DumpDirectory(dir);
        }
        #endregion

        #region DevUtils
        public InstanceNamespaceCollection GroupByNamespace()
        {
            var list = new InstanceNamespaceCollection();
            foreach (var info in this)
            {
                var ns = list[info.NamespaceString];
                if (ns == null)
                {
                    ns = new InstanceNamespace(info.NamespaceString);
                    list.Add(ns);
                }
                ns.Instances.Add(info);
            }
            return list;
        }
        #endregion
    }
    #endregion

    #region class InstanceNamespace
    public class InstanceNamespace
    {
        public InstanceNamespace(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        private readonly string _name;

        public List<AbcInstance> Instances
        {
            get { return _instances; }
        }

        private readonly List<AbcInstance> _instances = new List<AbcInstance>();

        public void Dump(TextWriter writer)
        {
            writer.WriteLine("#region namespace {0}", _name);
            writer.WriteLine("namespace {0}", _name);
            writer.WriteLine("{");

            bool eol = false;
            foreach (var info in _instances)
            {
                if (eol) writer.WriteLine();
                info.Dump(writer, false);
                eol = true;
            }

            writer.WriteLine("}");
            writer.WriteLine("#endregion");
        }
    }
    #endregion

    #region class InstanceNamespaceCollection
    public class InstanceNamespaceCollection : List<InstanceNamespace>
    {
        public InstanceNamespace this[string name]
        {
            get { return Find(ns => ns.Name == name); }
        }

        public void Dump(TextWriter writer)
        {
            bool eol = false;
            foreach (var ns in this)
            {
                if (eol) writer.WriteLine();
                ns.Dump(writer);
                eol = true;
            }
        }
    }
    #endregion
}