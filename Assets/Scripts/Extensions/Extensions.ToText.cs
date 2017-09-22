using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Extensions
{
	public static class Extensions
	{
		public static string ToText<T>(this IEnumerable<T> source, string header = null, Func<T, string> renderer = null)
		{
			var num = 0;
			var stringBuilder = new StringBuilder();
			renderer = renderer ?? (@object => @object.ToString());
			if(source == null)
				return (header != null ? string.Format("{0} [rows: {1}]\n{2}", header, num, stringBuilder) : stringBuilder.ToString()).TrimEnd(Environment.NewLine.ToCharArray());
			foreach(var obj in source)
				stringBuilder.AppendLine((object)obj == null ? "null" : string.Format("{0}: {1}", ++num, renderer(obj)));
			return (header != null ? string.Format("{0} [rows: {1}]\n{2}", header, num, stringBuilder) : stringBuilder.ToString()).TrimEnd(Environment.NewLine.ToCharArray());
		}

		public static string ToText(this Exception source)
		{
			if(source == null)
				return "exception is null";
			var exception = source;
			var builder = new StringBuilder();
			var counter = 0;
			while(exception != null)
			{
				builder.AppendLine("-- ." + ++counter);
				builder.Append("Exception: ");
				builder.AppendLine(exception.GetType().Name);
				builder.Append("Message: ");
				builder.AppendLine(exception.Message.TrimEnd('\n'));
				builder.AppendLine("Trace: ");
				builder.AppendLine(exception.StackTrace);
				exception = exception.InnerException;
			}
			builder.Append("-- .end exceptions trace");
			return string.Format("Roll out totals: {0}\n", builder);
		}
	}
}
