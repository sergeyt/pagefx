﻿using System;
using DataDynamics.PageFX.Common.TypeSystem;
using DataDynamics.PageFX.Ecma335.Metadata;

namespace DataDynamics.PageFX.Ecma335.LoaderInternals
{
	internal sealed class LazyType
	{
		private readonly AssemblyLoader _loader;
		private readonly TypeSignature _signature;
		private readonly Context _context;

		public LazyType(AssemblyLoader loader, TypeSignature signature, Context context)
		{
			_loader = loader;
			_signature = signature;
			_context = context;
		}

		public IType ResolveType()
		{
			var type = _loader.ResolveType(_signature, _context);
			if (type == null)
				throw new InvalidOperationException();
			return type;
		}
	}
}