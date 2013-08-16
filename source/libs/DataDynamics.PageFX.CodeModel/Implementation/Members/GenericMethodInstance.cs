using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using DataDynamics.PageFX.CodeModel.Syntax;

namespace DataDynamics.PageFX.CodeModel
{
    public class GenericMethodInstance : IMethod
    {
        private readonly IType _retType;
        private readonly IMethod _method;
        private readonly IType[] _args;
        private readonly ParameterCollection _params = new ParameterCollection();

        public GenericMethodInstance(IType declType, IMethod method, IType[] args)
        {
            if (declType == null)
                declType = method.DeclaringType;

            if (method == null)
                throw new ArgumentNullException("method");

            method = Unwrap(method);

            _method = method;
            _args = args;
            DeclaringType = declType;

            _retType = GenericType.Resolve(declType, this, _method.Type);
            if (_retType != _method.Type)
                _signatureChanged = true;

            foreach (var p in method.Parameters)
            {
                var ptype = GenericType.Resolve(declType, this, p.Type);
                var p2 = new Parameter(ptype, p.Name, p.Index);
                _params.Add(p2);

                if (ptype != p.Type)
                {
                    _signatureChanged = true;
                    p2.HasResolvedType = true;
                }
            }
        }

        /// <summary>
        /// Returns true if signature was changed during resolving.
        /// </summary>
        public bool SignatureChanged
        {
            get { return _signatureChanged; }
        }
        private readonly bool _signatureChanged;

        //public IType ContextType { get; set; }

        //public IMethod ContextMethod { get; set; }

        public static IMethod Unwrap(IMethod method)
        {
            while (method.ProxyOf != null)
                method = method.ProxyOf;

            while (method.IsGenericInstance)
            {
                method = method.InstanceOf;
                if (method == null)
                    throw new InvalidOperationException();
            }

            if (!method.IsGeneric)
                throw new InvalidOperationException(
                    string.Format("Method '{0}' has no generic parameters", method.FullName));

            return method;
        }

        #region IMethod Members
        public bool IsEntryPoint
        {
            get { return _method.IsEntryPoint; }
        }

        public bool IsConstructor
        {
            get { return _method.IsConstructor; }
        }

        public bool IsAbstract
        {
            get { return _method.IsAbstract; }
            set { throw new NotSupportedException(); }
        }

        public bool IsFinal
        {
            get { return _method.IsFinal; }
            set { throw new NotSupportedException(); }
        }

        public bool IsNewSlot
        {
            get { return _method.IsNewSlot; }
            set { throw new NotSupportedException(); }
        }

        public bool IsVirtual
        {
            get { return _method.IsVirtual; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets or sets the flag indicating whether the method overrides implementation of base type.
        /// </summary>
        public bool IsOverride
        {
            get { return _method.IsOverride; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets or sets flag indicating the method implementation is forwarded through PInvoke (Platform Invocation Services).
        /// </summary>
        public bool PInvoke
        {
            get { return _method.PInvoke; }
            set { throw new NotSupportedException(); }
        }

        public MethodCallingConvention CallingConvention
        {
            get { return _method.CallingConvention; }
            set { throw new NotSupportedException(); }
        }

        #region Impl Flags
        /// <summary>
        /// Gets or sets value indicating what kind of implementation is provided for this method.
        /// </summary>
        public MethodCodeType CodeType
        {
            get { return _method.CodeType; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets or sets flag indicating whether the method is managed.
        /// </summary>
        public bool IsManaged
        {
            get { return _method.IsManaged; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets or sets flag indicating that the method is declared, but its implementation is provided elsewhere.
        /// </summary>
        public bool IsForwardRef
        {
            get { return _method.IsForwardRef; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets or sets flag indicating that the method signature is exported exactly as declared.
        /// </summary>
        public bool IsPreserveSig
        {
            get { return _method.IsPreserveSig; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets or sets flag indicating that the method implemented within the common language runtime itself.
        /// </summary>
        public bool IsInternalCall
        {
            get { return _method.IsInternalCall; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets or sets flag indicating that the method can be executed by only one thread at a time.
        /// </summary>
        public bool IsSynchronized
        {
            get { return _method.IsSynchronized; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets or sets flag indicating that the method can not be inlined.
        /// </summary>
        public bool NoInlining
        {
            get { return _method.NoInlining; }
            set { throw new NotSupportedException(); }
        }
        #endregion

        public IGenericParameterCollection GenericParameters
        {
            get { return EmptyGenericParamaterCollection.Instance; }
        }

        public IType[] GenericArguments
        {
            get { return _args; }
        }

        /// <summary>
        /// Returns true if the method is generic.
        /// </summary>
        public bool IsGeneric
        {
            get { return false; }
        }

        public bool IsGenericInstance
        {
            get { return true; }
        }

        public IParameterCollection Parameters
        {
            get { return _params; }
        }

        /// <summary>
        /// Gets collection of custom attributes for return type.
        /// </summary>
        public ICustomAttributeCollection ReturnCustomAttributes
        {
            get { return _method.ReturnCustomAttributes; }
        }

        public ITypeMember Association
        {
            get { return _method.Association; }
            set { throw new NotSupportedException(); }
        }
        public bool IsGetter
        {
            get { return _method.IsGetter; }
        }

        public bool IsSetter
        {
            get { return _method.IsSetter; }
        }

        /// <summary>
        /// Gets or sets boolean flag indicating whether the method is explicit implementation of some interface method.
        /// </summary>
        public bool IsExplicitImplementation
        {
            get
            {
                return _method.IsExplicitImplementation;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets or sets methods implemented by this method
        /// </summary>
        public IMethod[] ImplementedMethods
        {
            get
            {
                if (_implMethods == null)
                {
                    var impl = _method.ImplementedMethods;
                    if (impl != null && impl.Length > 0)
                    {
                        int n = impl.Length;
                        _implMethods = new IMethod[n];
                        for (int i = 0; i < n; ++i)
                        {
                            var m = ResolveInstance(impl[i]);
                            _implMethods[i] = m;
                        }
                    }
                    else
                    {
                        _implMethods = new IMethod[0];
                    }
                }
                return _implMethods;
            }
            set
            {
                throw new NotSupportedException();
            }
        }
        private IMethod[] _implMethods;

        public IMethodBody Body
        {
            get { return _method.Body; }
            set { throw new NotSupportedException(); }
        }

        private IMethod ResolveInstance(IMethod method)
        {
            if (method.IsGeneric)
            {
                var declType = GenericType.Resolve(DeclaringType, this, method.DeclaringType);
                return GenericType.CreateMethodInstance(declType, method, GenericArguments);
            }

            throw new InvalidOperationException();
        }

        public IMethod BaseMethod
        {
            get
            {
                if (_resolveBaseMethod)
                {
                    _resolveBaseMethod = false;
                    var bm = _method.BaseMethod;
                    if (bm != null)
                        _baseMethod = ResolveInstance(bm);
                }
                return _baseMethod;
            }
            set { throw new NotSupportedException(); }
        }
        private IMethod _baseMethod;
        private bool _resolveBaseMethod = true;

        public IMethod ProxyOf
        {
            get { return null; }
        }

        public IMethod InstanceOf
        {
            get { return _method; }
        }
        #endregion

        #region ITypeMember Members
        /// <summary>
        /// Gets the assembly in which the member is declared.
        /// </summary>
        public IAssembly Assembly
        {
            get { return _method.Assembly; }
        }

        /// <summary>
        /// Gets the module in which the member is defined. 
        /// </summary>
        public IModule Module
        {
            get { return _method.Module; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the kind of this member.
        /// </summary>
        public TypeMemberType MemberType
        {
            get { return _method.MemberType; }
        }

        public string Name
        {
            get
            {
                //TODO:
                return _method.Name;
            }
            set { throw new NotSupportedException(); }
        }

        public string FullName
        {
            get { return _method.FullName; }
        }

        public string DisplayName
        {
            get
            {
                return Name;
            }
        }

        public IType DeclaringType { get; set; }

        public IType Type
        {
            get { return _retType; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets visibility of this member.
        /// </summary>
        public Visibility Visibility
        {
            get { return _method.Visibility; }
            set { throw new NotSupportedException(); }
        }

        public bool IsVisible
        {
            get { return _method.IsVisible; }
        }

        public bool IsStatic
        {
            get { return _method.IsStatic; }
            set { throw new NotSupportedException(); }
        }

        public bool IsSpecialName
        {
            get { return _method.IsSpecialName; }
            set { throw new NotSupportedException(); }
        }

        public bool IsRuntimeSpecialName
        {
            get { return _method.IsRuntimeSpecialName; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets or sets value that identifies a metadata element. 
        /// </summary>
        public int MetadataToken { get; set; }
        #endregion

        #region ICustomAttributeProvider Members
        public ICustomAttributeCollection CustomAttributes
        {
            get { return _method.CustomAttributes; }
        }
        #endregion

        #region ICodeNode Members
        public CodeNodeType NodeType
        {
            get { return CodeNodeType.Method; }
        }

        public IEnumerable<ICodeNode> ChildNodes
        {
            get { return null; }
        }

        /// <summary>
        /// Gets or sets user defined data assotiated with this object.
        /// </summary>
        public object Tag { get; set; }
        #endregion

        #region IFormattable Members
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return SyntaxFormatter.Format(this, format, formatProvider);
        }
        #endregion

        #region IDocumentationProvider Members
        /// <summary>
        /// Gets or sets documentation of this member
        /// </summary>
        public string Documentation
        {
            get { return _method.Documentation; }
            set { }
        }

        /// <summary>
        /// Gets or sets documentation for return value.
        /// </summary>
        public string ReturnDocumentation
        {
            get { return _method.ReturnDocumentation; }
            set { throw new NotSupportedException(); }
        }
        #endregion

        public override string ToString()
        {
            return ToString(null, null);
        }
    }
}