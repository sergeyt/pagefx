﻿using System;
using DataDynamics.PageFX.CLI.IL;
using DataDynamics.PageFX.CLI.Metadata;
using DataDynamics.PageFX.CodeModel;

namespace DataDynamics.PageFX.CLI.Tables
{
	internal sealed class MethodTable : MetadataTable<IMethod>
	{
		public MethodTable(AssemblyLoader loader)
			: base(loader)
		{
		}

		public override MdbTableId Id
		{
			get { return MdbTableId.MethodDef; }
		}

		protected override IMethod ParseRow(MdbRow row, int index)
		{
			MdbIndex entryPoint = Mdb.CLIHeader.EntryPointToken;
			bool checkEntryPoint = entryPoint.Table == MdbTableId.MethodDef;

			var implFlags = (MethodImplAttributes)row[MDB.MethodDef.ImplFlags].Value;
			var flags = (MethodAttributes)row[MDB.MethodDef.Flags].Value;

			bool isStatic = (flags & MethodAttributes.Static) != 0;
			var token = MdbIndex.MakeToken(MdbTableId.MethodDef, index + 1);

			var method = new Method
				{
					MetadataToken = token,
					Name = row[MDB.MethodDef.Name].String,
					ImplFlags = implFlags,
					Visibility = ToVisibility(flags),
					IsStatic = isStatic
				};

			// for ref from types
			row.Object = method;

			var genericParams = Loader.GenericParameters.Find(token);
			foreach (var genericParameter in genericParams)
			{
				method.GenericParameters.Add(genericParameter);
				genericParameter.DeclaringMethod = method;
			}

			if (!isStatic)
			{
				method.IsAbstract = (flags & MethodAttributes.Abstract) != 0;
				method.IsFinal = (flags & MethodAttributes.Final) != 0;
				method.IsNewSlot = (flags & MethodAttributes.NewSlot) != 0;
				method.IsVirtual = (flags & MethodAttributes.Virtual) != 0;
			}

			method.IsSpecialName = (flags & MethodAttributes.SpecialName) != 0;
			method.IsRuntimeSpecialName = (flags & MethodAttributes.RTSpecialName) != 0;

			if (checkEntryPoint && entryPoint.Index - 1 == index)
			{
				method.IsEntryPoint = true;
				Loader.Assembly.EntryPoint = method;
			}

			var sigBlob = row[MDB.MethodDef.Signature].Blob;
			var signature = MdbSignature.DecodeMethodSignature(sigBlob);

			method.Meta = new MetaMethod(Loader, method, signature);

			SetParams(method, row, signature);

			uint rva = row[MDB.MethodDef.RVA].Value;
			if (rva != 0) //abstract or extern
			{
				method.Body = new LateMethodBody(Loader, method, rva);
			}

			method.CustomAttributes = new CustomAttributes(Loader, method, token);

			return method;
		}

		private void SetParams(Method method, MdbRow row, MdbMethodSignature signature)
		{
			int from = row[MDB.MethodDef.ParamList].Index - 1;
			
			method.Parameters = new ParamList(Loader, method, from, signature);
		}

		private static Visibility ToVisibility(MethodAttributes f)
		{
			var v = f & MethodAttributes.MemberAccessMask;
			switch (v)
			{
				case MethodAttributes.PrivateScope:
					return Visibility.PrivateScope;
				case MethodAttributes.Private:
					return Visibility.Private;
				case MethodAttributes.FamANDAssem:
				case MethodAttributes.FamORAssem:
					return Visibility.ProtectedInternal;
				case MethodAttributes.Assembly:
					return Visibility.Internal;
				case MethodAttributes.Family:
					return Visibility.Protected;
			}
			return Visibility.Public;
		}

		private sealed class MetaMethod : IMetaMethod
		{
			private readonly AssemblyLoader _loader;
			private readonly Method _method;
			private readonly MdbMethodSignature _signature;
			private IType _type;
			private IType _declType;
			private ITypeMember _association;
			private bool _associationResolved;

			public MetaMethod(AssemblyLoader loader, Method method, MdbMethodSignature signature)
			{
				_loader = loader;
				_method = method;
				_signature = signature;
			}

			public IType Type
			{
				get { return _type ?? (_type = ResolveType()); }
			}

			public IType DeclaringType
			{
				get { return _declType ?? (_declType = ResolveDeclType()); }
			}

			public ITypeMember Association
			{
				get
				{
					if (_associationResolved) return _association;
					_associationResolved = true;
					return (_association = ResolveAssociation());
				}
			}

			private IType ResolveDeclType()
			{
				var type = _loader.ResolveDeclType(_method);
				if (type == null)
					throw new InvalidOperationException();
				return type;
			}

			private IType ResolveType()
			{
				var context = new Context(DeclaringType, _method);

				var type = _loader.ResolveType(_signature.Type, context);
				if (type == null)
					throw new InvalidOperationException();

				return type;
			}

			private ITypeMember ResolveAssociation()
			{
				MdbIndex token = _method.MetadataToken;

				var row = _loader.Mdb.LookupRow(MdbTableId.MethodSemantics, MDB.MethodSemantics.Method, token.Index - 1, true);
				if (row == null) return null;
				
				var sem = (MethodSemanticsAttributes)row[MDB.MethodSemantics.Semantics].Value;

				MdbIndex assoc = row[MDB.MethodSemantics.Association].Value;
				switch (assoc.Table)
				{
					case MdbTableId.Property:
						var property = _loader.Properties[assoc.Index - 1];

						_method.Association = property;
						switch (sem)
						{
							case MethodSemanticsAttributes.Getter:
								property.Getter = _method;
								break;
							case MethodSemanticsAttributes.Setter:
								property.Setter = _method;
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}

						//TODO: lazy resolving
						property.DeclaringType = _method.DeclaringType;

						property.ResolveTypeAndParameters();
						
						return property;

					case MdbTableId.Event:
						var e = _loader.Events[assoc.Index - 1];

						_method.Association = e;
						switch (sem)
						{
							case MethodSemanticsAttributes.AddOn:
								e.Adder = _method;
								break;
							case MethodSemanticsAttributes.RemoveOn:
								e.Remover = _method;
								break;
							case MethodSemanticsAttributes.Fire:
								e.Raiser = _method;
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}

						//TODO: lazy resolving
						e.DeclaringType = _method.DeclaringType;

						e.ResolveType();

						return e;

					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}
