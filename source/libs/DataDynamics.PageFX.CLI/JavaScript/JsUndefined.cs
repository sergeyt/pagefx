namespace DataDynamics.PageFX.CLI.JavaScript
{
	internal sealed class JsUndefined : JsNode
	{
		public static readonly JsNode Value = new JsUndefined();

		private JsUndefined()
		{
		}

		public override void Write(JsWriter writer)
		{
			writer.Write("undefined");
		}
	}
}