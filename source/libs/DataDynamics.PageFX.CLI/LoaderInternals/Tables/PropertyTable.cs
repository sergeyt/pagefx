﻿using DataDynamics.PageFX.Common.Metadata;
using DataDynamics.PageFX.Common.TypeSystem;
using DataDynamics.PageFX.Ecma335.LoaderInternals.Collections;
using DataDynamics.PageFX.Ecma335.Metadata;

namespace DataDynamics.PageFX.Ecma335.LoaderInternals.Tables
{
	internal sealed class PropertyTable : MetadataTable<IProperty>
	{
		public PropertyTable(AssemblyLoader loader)
			: base(loader)
		{
		}

		public override TableId Id
		{
			get { return TableId.Property; }
		}

		protected override IProperty ParseRow(MetadataRow row, int index)
		{
			var flags = (PropertyAttributes)row[Schema.Property.Flags].Value;
			var name = row[Schema.Property.Name].String;
			var token = SimpleIndex.MakeToken(TableId.Property, index + 1);
			var value = Loader.Const[token];

			var property = new Property
				{
					MetadataToken = token,
					Name = name,
					IsSpecialName = ((flags & PropertyAttributes.SpecialName) != 0),
					IsRuntimeSpecialName = ((flags & PropertyAttributes.RTSpecialName) != 0),
					HasDefault = ((flags & PropertyAttributes.HasDefault) != 0),
					Value = value
				};

			property.CustomAttributes = new CustomAttributes(Loader, property);

			return property;
		}
	}
}