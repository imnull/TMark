using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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
		
        /// <summary>
        /// 模板名称
        /// </summary>
		public string Name
		{
            get
            {
                string name = getter("name");
                if (String.IsNullOrEmpty(name))
                {
                    throw new Exception("The [Name] property is Required.");
                }
                return name;
            }
			set{ json["name"] = value; }
		}
		
        /// <summary>
        /// 查询语句
        /// </summary>
		public string SQL
		{
			get{ return getter("sql"); }
			set{ json["sql"] = value; }
		}
		
        /// <summary>
        /// XSL模板
        /// </summary>
		public string XSL
		{
			get{ return getter("xsl"); }
			set{ json["xsl"] = value; }
		}
		
        /// <summary>
        /// 查询参数
        /// </summary>
		public JSON Parameters
		{
			get{ return json["params"] as JSON; }
		}

        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="s"></param>
        public void Read(string s)
        {
            json.Read(s);
        }

		public override string ToString ()
		{
			return json.ToString();
		}
	}

    /// <summary>
    /// 模板集合
    /// </summary>
    public class TemplateMarkCollection
    {
        public TemplateMarkCollection()
        {
            dic = new Dictionary<string, TemplateMark>();
        }
        private readonly Dictionary<string, TemplateMark> dic;

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            dic.Clear();
        }

        /// <summary>
        /// 添加模板
        /// </summary>
        /// <param name="tm">模板标记对象</param>
        public void Add(TemplateMark tm)
        {
            if (dic.ContainsKey(tm.Name))
            {
                throw new Exception(String.Format("Oops~ We already have a TemplateMark named [{0}]", tm.Name));
            }
            this.dic.Add(tm.Name, tm);
        }

        /// <summary>
        /// 从字符串读取
        /// </summary>
        /// <param name="s">模板字符串</param>
        public void Read(string s)
        {
            Clear();
            MatchCollection matches = Regex.Matches(s, @"<!--\s*(\{[\w\W]*?\})\s*-->");
            foreach (Match match in matches)
            {
                TemplateMark t = new TemplateMark();
                t.Read(match.Value);
                Add(t);
            }
        }

        /// <summary>
        /// 从模板文件读取
        /// </summary>
        /// <param name="filename">模板完整路径</param>
        /// <param name="enc">编码方式</param>
        public void Read(string filename, Encoding enc)
        {
            using (StreamReader sr = new StreamReader(filename, enc))
            {
                Read(sr.ReadToEnd());
            }
        }
    }
}

