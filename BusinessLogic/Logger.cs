using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Configuration;

namespace Commpoint.Utility
{
	public enum LoggingCategory
	{
		Error = 0,
		Overview = 1,
		Detailed = 2,
	}

	public enum ExceptionPolicy
	{
		General = 0
	}

	public class Logger
	{
		public static int Indent;

		/// <summary>
		/// Writes a message to the Detailed category log
		/// </summary>
		/// <param name="message"></param>
		public static void DetailedLog(string message)
		{
			DetailedLog(message, string.Empty);
		}

		/// <summary>
		/// Writes a message to the Detailed category log
		/// </summary>
		/// <param name="message"></param>
		/// <param name="arg"></param>
		public static void DetailedLog(string message, params object[] arg)
		{
			RssFeedWatcherConfig configInfo = (RssFeedWatcherConfig)ConfigurationManager.GetSection("RssFeedWatcherConfig");

			if (configInfo.LogDetails)
			{
				WriteLogEntry(string.Format(WriteStringWithIndent(message), arg), LoggingCategory.Detailed,TraceEventType.Information);
			}
			Console.WriteLine(string.Format(WriteStringWithIndent(message), arg));
		}

		/// <summary>
		/// Writes a message to the Overview category log
		/// </summary>
		/// <param name="message"></param>
		public static void OverviewLog(string message)
		{
			OverviewLog(message, string.Empty);
		}

		/// <summary>
		/// Writes a message to the Overview category log
		/// </summary>
		/// <param name="message"></param>
		/// <param name="arg"></param>
		public static void OverviewLog(string message, params object[] arg)
		{
			WriteLogEntry(string.Format(WriteStringWithIndent(message), arg), LoggingCategory.Overview, TraceEventType.Information);
		}

		/// <summary>
		/// Writes a message to the Overview category log
		/// </summary>
		/// <param name="message"></param>
		public static void OverviewAndDetailedLog(string message)
		{
			DetailedLog(message);
			OverviewLog(message);
		}

		/// <summary>
		/// Writes a message to the Overview category log
		/// </summary>
		/// <param name="message"></param>
		/// <param name="arg"></param>
		public static void OverviewAndDetailedLog(string message, params object[] arg)
		{
			DetailedLog(message, arg);
			OverviewLog(message, arg);
		}

		private static string WriteStringWithIndent(string text)
		{
			if (Indent <= 0)
				return text;

			const string indentSpace = "  ";
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < Indent; i++)
			{
				sb.Append(indentSpace);
			}
			sb.Append(text);
			return sb.ToString();

		}

		private static void WriteLogEntry(string message, LoggingCategory category, TraceEventType traceEventType)
		{
			LogEntry entry = new LogEntry(message, category.ToString(), 0, 0, traceEventType, string.Empty, null);
			
			Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(entry);
		}

	}
}
