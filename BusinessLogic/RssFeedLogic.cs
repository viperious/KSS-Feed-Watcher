#region

using System;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Xml;
using Commpoint.Messaging;
using Commpoint.Utility.SmsService;

#endregion

namespace Commpoint.Utility
{
	public class RssFeedLogic
	{
		private readonly RssFeedWatcherConfig _config;
		public bool IsStopped;

		public RssFeedLogic()
		{
			_config = (RssFeedWatcherConfig)ConfigurationManager.GetSection("RssFeedWatcherConfig");

			//StartFeedWatcher();
		}

		public void StartFeedWatcher()
		{
			Logger.OverviewAndDetailedLog("Processing RssFeeds");
			Logger.Indent++;
			foreach (RssFeedElement rssFeedElement in _config.RssFeeds)
			{
				Thread thrd = new Thread(ProcessFeed);
				thrd.Start(rssFeedElement);
			}
			Logger.Indent--;
		}

		//Shutdown all threads
		public void Stop()
		{
			IsStopped = true;
		}

		public SyndicationFeed ReadFeed(string url)
		{
			try
			{
				XmlReader reader = XmlReader.Create(url);
				SyndicationFeed feed = SyndicationFeed.Load(reader);

				reader.Close();

				return feed;
			}
			catch (Exception ex)
			{
				Logger.DetailedLog(ex.Message);
				Email email = new Email();
				email.To.Add("Viperious@gmail.com");
				email.Subject = string.Format("Error in Processing Feed Ur: '{0}'", url);
				email.Body = ex.Message + "<br /><br/> " + ex.StackTrace;
				email.Send();

				return new SyndicationFeed();
			}

		}

		private void ProcessFeed(Object obj)
		{
			try
			{
				RssFeedElement rssFeed = (RssFeedElement)obj;

				DateTime lastChecked = rssFeed.LastChecked;
				Logger.DetailedLog("Logging {0} rssFeed", rssFeed.Name);
				do
				{
					//Get rssFeed
					SyndicationFeed feed = ReadFeed(rssFeed.Url);
					DateTime startTime = DateTime.Now;
					foreach (SyndicationItem item in feed.Items)
					{
						//Only look new items since last checked date.
						bool isNew = item.PublishDate >= lastChecked;

						if (!isNew) continue;

						string[] rawTitle = item.Title.Text.Split(new[] { " : " }, StringSplitOptions.RemoveEmptyEntries);
						float price = 0;
						string searchString;
						//If no price then just return the title.
						if (rawTitle.Length == 1)
							searchString = rawTitle[0] + " " + item.Summary.Text;
						else
						{
							float.TryParse(rawTitle[0].Substring(1), out price);
							searchString = rawTitle[1] + " " + item.Summary.Text;
						}

						//If MaxPrice is set and if the item is greater than the max, just continue on
						if (rssFeed.MaxPrice != 0 && price > rssFeed.MaxPrice)
						{
							Console.WriteLine("Ignoring {0} price = {1} ", item.Title.Text, price);
							continue;
						}

						//Check for keywords
						if (!rssFeed.SearchTerms.Cast<TermConfigElement>().Any(term => searchString.IndexOf(term.Term, StringComparison.OrdinalIgnoreCase) > -1))
						{
							Logger.DetailedLog("Ignoring '{0}' did not contain a search term", item.Title.Text);
							continue;
						}

						//If has excluded item, move on.
						if (rssFeed.ExcludedTerms.Cast<TermConfigElement>().Any(term => searchString.IndexOf(term.Term, StringComparison.OrdinalIgnoreCase) > -1))
						{
							Logger.DetailedLog("Ignoring '{0}' did contains an excluded term", item.Title.Text);
							continue;
						}

						//Check to see if has a priority term
						bool priority = false;
						if (rssFeed.PriorityTerms.Cast<TermConfigElement>().Any(term => searchString.IndexOf(term.Term, StringComparison.OrdinalIgnoreCase) > -1))
						{
							Logger.DetailedLog("'{0}' marked as priority", item.Title.Text);
							priority = true;
						}


						//Email
						if (rssFeed.Emails.Count == 0) continue;
						Logger.DetailedLog("Sending email for '{0}'", item.Title.Text);
						Email email = new Email();
						email.From = new MailAddress("noreply@commpoint.com", rssFeed.Name);
						if (priority)
							email.Priority = MailPriority.High;
						foreach (EmailConfigElement emailItems in rssFeed.Emails)
						{
							email.To.Add(new MailAddress(emailItems.Address, emailItems.DisplayName));
						}
						email.Subject = item.Title.Text;
						email.Body = item.Summary.Text + "<br />" + (item.Links.Count > 0 ? item.Links[0].Uri.AbsoluteUri : string.Empty);
						email.Send();

						//SMS
						if (rssFeed.SMS.Count == 0) continue;
						Logger.DetailedLog("Sending sms for '{0}'", item.Title.Text);
						foreach (SMSConfigElement smsItem in rssFeed.SMS)
						{
							//string connectionString = string.Format("http://s2.freesmsapi.com/messages/send?skey={0}&message={1}&senderid={2}&recipient={3}",
							//    "ae8bfac2934b977127dc9630999a4d9e",
							//    (item.Title.Text + " " + item.Links[0].Uri + " " + item.Summary.Text).Substring(0, 160),
							//    string.Empty,
							//    smsItem.Phone
							//    );
							//try
							//{
							//    System.IO.Stream SourceStream = null;
							//    System.Net.WebRequest myRequest = WebRequest.Create(connectionString);
							//    myRequest.Credentials = CredentialCache.DefaultCredentials;
							//    WebResponse webResponse = myRequest.GetResponse();
							//    SourceStream = webResponse.GetResponseStream();
							//    StreamReader reader = new StreamReader(webResponse.GetResponseStream());
							//    string str = reader.ReadToEnd();
							//}
							//catch (Exception ex)
							//{
							//}


							//TextMessage.SendText(smsItem.Phone, smsItem.ServiceProvider, message.Length > 160 ? message.Substring(0, 160) : message);

							using (SmsService.SmsServiceClient client = new SmsServiceClient())
							{
								string message = item.Title.Text + " " + (item.Links.Count > 0 ? item.Links[0].Uri.AbsoluteUri : string.Empty) + " " + item.Summary.Text;
								string result = client.SendSms(rssFeed.SMS.ZeepApiKey, rssFeed.SMS.ZeepSecertKey, message.Length > 112 ? message.Substring(0, 112) : message, string.Empty);
								//ZeepSms.SendSMS(rssFeed.SMS.ZeepApiKey, rssFeed.SMS.ZeepSecertKey, message.Length > 112 ? message.Substring(0, 112) : message, string.Empty);
							}

						}
					}

					Logger.DetailedLog("Completed processing {0} rssFeed, sleeping. LastChecked:{1} StartTime:{2} ", rssFeed.Name, lastChecked, startTime);

					lastChecked = startTime;

					WriteLastUpdatedValue(rssFeed.Name, lastChecked);
					
					Thread.Sleep(rssFeed.WaitPeriod * 1000);
				} while (!IsStopped);
				Logger.DetailedLog("{0} rssFeed has stopped, sleeping.", rssFeed.Name);

			}
			catch (Exception ex)
			{
				Logger.DetailedLog(ex.Message);
				Email email = new Email();
				email.To.Add("Viperious@gmail.com");
				email.Subject = string.Format("Error in Processing '{0}'", obj.GetType());
				email.Body = ex.Message + "<br /><br/> " + ex.StackTrace + "<br/><br/>";
				if (ex.InnerException != null)
					email.Body += ex.InnerException.Message + "<br /><br/> " + ex.InnerException.StackTrace + "<br/><br/>";
				email.Send();

				throw;
			}
		}

		private void WriteLastUpdatedValue(string rssFeedName, DateTime lastChecked)
		{
			lock(this)
			{
				//Write updated time
				Configuration writeConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				RssFeedWatcherConfig config = (RssFeedWatcherConfig)writeConfig.GetSection("RssFeedWatcherConfig");
				foreach (RssFeedElement feedElement in config.RssFeeds)
				{
					if (feedElement.Name == rssFeedName)
						feedElement.LastChecked = lastChecked;
				}
				writeConfig.Save();
			}
		}
	}
}