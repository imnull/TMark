using System;

namespace TMark
{
	public class TemplateMark
	{
		public TemplateMark ()
		{
			json = new JSON();
			
			json["name"] = new QuoteString(null, '\'');
			json["sql"] = new QuoteString(null, '\'');
			json["xsl"] = new QuoteString(null, '\'');
			json["params"] = new JSON();
		}
		
		private readonly JSON json;
		
		private string getter(string key)
		{
			object o = json[key];
			if(o == null) return String.Empty;
			return o.ToString();
		}
		
		public string Name
		{
			get{ return getter("name"); }
			set{ json["name"] = value; }
		}
		
		public string SQL
		{
			get{ return getter("sql"); }
			set{ json["sql"] = value; }
		}
		
		public string XSL
		{
			get{ return getter("xsl"); }
			set{ json["xsl"] = value; }
		}
		
		public JSON Parameters
		{
			get{ return json["params"] as JSON; }
		}
		
		public override string ToString ()
		{
			return json.ToString();
		}
	}
}

