using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Commpoint.Messaging.SMS;


namespace Commpoint.Utility
{
	public class RssFeedWatcherConfig : ConfigurationSection
	{

		[ConfigurationProperty("RssFeeds"),
		 ConfigurationCollection(typeof(RssFeedsCollection),
			AddItemName = "RssFeed")]
		public RssFeedsCollection RssFeeds
		{
			get { return (RssFeedsCollection)this["RssFeeds"]; }
			set { this["RssFeeds"] = value; }
		}

		[ConfigurationProperty("LogDetails", IsRequired = false, DefaultValue = true)]
		public bool LogDetails
		{
			get
			{
				return (bool)this["LogDetails"];
			}
			set
			{
				this["LogDetails"] = value;
			}
		}

	}

	#region RssFeeds Collection

	public class RssFeedsCollection : ConfigurationElementCollection
	{
		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}

		public RssFeedElement this[int index]
		{
			get
			{
				return BaseGet(index) as RssFeedElement;
			}
			set
			{
				if (BaseGet(index) != null)
				{
					BaseRemoveAt(index);
				}
				BaseAdd(index, value);
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new RssFeedElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((RssFeedElement)element).Name;
		}

		
	}
	public class RssFeedElement : ConfigurationElement
	{
		[ConfigurationProperty("WaitPeriod", IsRequired = true)]
		public int WaitPeriod
		{
			get
			{
				return (int)this["WaitPeriod"];
			}
			set
			{
				this["WaitPeriod"] = value;
			}
		}

		[ConfigurationProperty("LastChecked", IsRequired = true)]
		public DateTime LastChecked
		{
			get
			{
				DateTime dateTime;
				DateTime.TryParse(this["LastChecked"] == null ? string.Empty : this["LastChecked"].ToString(), out dateTime);
				return dateTime;
			}
			set
			{
				this["LastChecked"] = value;
			}
		}

		[ConfigurationProperty("Url", IsRequired = true)]
		public String Url
		{
			get
			{
				return (String)this["Url"];
			}
			set
			{
				this["Url"] = value;
			}
		}

		[ConfigurationProperty("SearchTerms")]
		public TermsConfigCollection SearchTerms
		{
			get { return (TermsConfigCollection)this["SearchTerms"]; }
			set { this["SearchTerms"] = value; }
		}

		[ConfigurationProperty("PriorityTerms")]
		public TermsConfigCollection PriorityTerms
		{
			get { return (TermsConfigCollection)this["PriorityTerms"]; }
			set { this["PriorityTerms"] = value; }
		}

		[ConfigurationProperty("ExcludedTerms")]
		public TermsConfigCollection ExcludedTerms
		{
			get { return (TermsConfigCollection)this["ExcludedTerms"]; }
			set { this["ExcludedTerms"] = value; }
		}

		[ConfigurationProperty("Name", IsRequired = true)]
		public String Name
		{
			get
			{
				return (String)this["Name"];
			}
			set
			{
				this["Name"] = value;
			}
		}

		[ConfigurationProperty("MaxPrice", IsRequired = false)]
		public int MaxPrice
		{
			get
			{
				return (int)this["MaxPrice"];
			}
			set
			{
				this["MaxPrice"] = value;
			}
		}

		[ConfigurationProperty("Emails", IsRequired = true)]
		public EmailConfigCollection Emails
		{
			get
			{
				return (EmailConfigCollection)this["Emails"];
			}
			set
			{
				this["Emails"] = value;
			}
		}

		[ConfigurationProperty("SMS", IsRequired = false)]
		public SMSConfigCollection SMS
		{
			get
			{
				return (SMSConfigCollection)this["SMS"];
			}
			set
			{
				this["SMS"] = value;
			}
		}

	}
	#endregion

	#region Email Configuration

	public class EmailConfigCollection : ConfigurationElementCollection
	{
		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}

		public EmailConfigElement this[int index]
		{
			get
			{
				return BaseGet(index) as EmailConfigElement;
			}
			set
			{
				if (BaseGet(index) != null)
				{
					BaseRemoveAt(index);
				}
				BaseAdd(index, value);
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new EmailConfigElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((EmailConfigElement)element).Address;
		}

		public List<EmailConfigElement> ToList()
		{
			return this.Cast<EmailConfigElement>().ToList();
		}
	}

	public class EmailConfigElement : ConfigurationElement
	{
		[ConfigurationProperty("Address", IsRequired = true)]
		public String Address
		{
			get
			{
				return (String)this["Address"];
			}
			set
			{
				this["Address"] = value;
			}
		}

		[ConfigurationProperty("DisplayName", IsRequired = false)]
		public String DisplayName
		{
			get
			{
				return (String)this["DisplayName"];
			}
			set
			{
				this["DisplayName"] = value;
			}
		}
	}

	#endregion

	#region SMS Configuration

	public class SMSConfigCollection : ConfigurationElementCollection
	{
		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}

		public SMSConfigElement this[int index]
		{
			get
			{
				return BaseGet(index) as SMSConfigElement;
			}
			set
			{
				if (BaseGet(index) != null)
				{
					BaseRemoveAt(index);
				}
				BaseAdd(index, value);
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new SMSConfigElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((SMSConfigElement)element).Phone;
		}

		public List<SMSConfigElement> ToList()
		{
			return this.Cast<SMSConfigElement>().ToList();
		}

		[ConfigurationProperty("ZeepApiKey", IsRequired = true)]
		public String ZeepApiKey
		{
			get
			{
				return (String)this["ZeepApiKey"];
			}
			set
			{
				this["ZeepApiKey"] = value;
			}
		}

		[ConfigurationProperty("ZeepSecertKey", IsRequired = true)]
		public String ZeepSecertKey
		{
			get
			{
				return (String)this["ZeepSecertKey"];
			}
			set
			{
				this["ZeepSecertKey"] = value;
			}
		}
	}

	public class SMSConfigElement : ConfigurationElement
	{
		[ConfigurationProperty("Phone", IsRequired = true)]
		public String Phone
		{
			get
			{
				return (String)this["Phone"];
			}
			set
			{
				this["Phone"] = value;
			}
		}

		[ConfigurationProperty("ServiceProvider", IsRequired = true)]
		public ServiceProvider ServiceProvider
		{
			get
			{
				return (ServiceProvider)Enum.Parse(typeof(ServiceProvider),this["ServiceProvider"].ToString());
			}
			set
			{
				this["ServiceProvider"] = value;
			}
		}

		
	}

	#endregion

	#region Terms Configuration

	public class TermsConfigCollection : ConfigurationElementCollection
	{
		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}

		public TermConfigElement this[int index]
		{
			get
			{
				return BaseGet(index) as TermConfigElement;
			}
			set
			{
				if (BaseGet(index) != null)
				{
					BaseRemoveAt(index);
				}
				BaseAdd(index, value);
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new TermConfigElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((TermConfigElement)element).Term;
		}

		public List<TermConfigElement> ToList()
		{
			return this.Cast<TermConfigElement>().ToList();
		}
	}

	public class TermConfigElement : ConfigurationElement
	{
		[ConfigurationProperty("Term", IsRequired = true)]
		public String Term
		{
			get
			{
				return (String)this["Term"];
			}
			set
			{
				this["Term"] = value;
			}
		}
	}

	#endregion

}
