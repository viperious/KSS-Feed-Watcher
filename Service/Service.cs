using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using Commpoint.Utility;

namespace Service
{
	public partial class Service : ServiceBase
	{
		private RssFeedLogic _feedLogic;

		public Service()
		{
			InitializeComponent();
			_feedLogic = new RssFeedLogic();
		}

		protected override void OnStart(string[] args)
		{
			Thread thread = new Thread(_feedLogic.StartFeedWatcher);
			thread.Start();
		}

		protected override void OnStop()
		{
			if (_feedLogic != null)
				_feedLogic.Stop();
		}
	}
}
