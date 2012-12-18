using DataDynamics.PageFX.Common.TypeSystem;

namespace DataDynamics.PageFX.FlashLand.Core
{
	internal static class FieldExtensions
	{
		public static bool HasEmbedAttribute(this IField field)
		{
			var attr = field.FindAttribute(Attrs.Embed);
			return attr != null;
		}

		/// <summary>
		/// For enums we will use m_value name for internal value.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static void Rename(this IField field)
		{
			var dt = field.DeclaringType;
			if (!dt.IsEnum) return;
			if (field.IsStatic) return;
			field.Name = Const.Boxing.Value;
		}
	}
}