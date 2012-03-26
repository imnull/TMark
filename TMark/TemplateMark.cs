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
        /// ģ������
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
        /// ��ѯ���
        /// </summary>
		public string SQL
		{
			get{ return getter("sql"); }
			set{ json["sql"] = value; }
		}
		
        /// <summary>
        /// XSLģ��
        /// </summary>
		public string XSL
		{
			get{ return getter("xsl"); }
			set{ json["xsl"] = value; }
		}
		
        /// <summary>
        /// ��ѯ����
        /// </summary>
		public JSON Parameters
		{
			get{ return json["params"] as JSON; }
		}

        /// <summary>
        /// ��ȡ�ַ���
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
    /// ģ�弯��
    /// </summary>
    public class TemplateMarkCollection
    {
        public TemplateMarkCollection()
        {
            dic = new Dictionary<string, TemplateMark>();
        }
        private readonly Dictionary<string, TemplateMark> dic;

        /// <summary>
        /// ���
        /// </summary>
        public void Clear()
        {
            dic.Clear();
        }

        /// <summary>
        /// ���ģ��
        /// </summary>
        /// <param name="tm">ģ���Ƕ���</param>
        public void Add(TemplateMark tm)
        {
            if (dic.ContainsKey(tm.Name))
            {
                throw new Exception(String.Format("Oops~ We already have a TemplateMark named [{0}]", tm.Name));
            }
            this.dic.Add(tm.Name, tm);
        }

        /// <summary>
        /// ���ַ�����ȡ
        /// </summary>
        /// <param name="s">ģ���ַ���</param>
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
        /// ��ģ���ļ���ȡ
        /// </summary>
        /// <param name="filename">ģ������·��</param>
        /// <param name="enc">���뷽ʽ</param>
        public void Read(string filename, Encoding enc)
        {
            using (StreamReader sr = new StreamReader(filename, enc))
            {
                Read(sr.ReadToEnd());
            }
        }
    }
}

