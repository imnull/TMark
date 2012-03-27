/** 
 *
 * TemplateMark.cs
 * (c) 2011-2012 mk31415926535@gmail.com
 * TemplateMark.cs may be freely distributed under the MIT license.
 *
 * */

using System;
using System.Data;
using System.Xml;
using System.Xml.Xsl;
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
            json["node"] = new QuoteString("div", '\'');
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
        /// ���ڵ�����
        /// </summary>
        public object Node
        {
            get 
            {
                return json["node"];
            }
            set { json["node"] = value; }
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

        public string OriginString
        {
            get { return json.OriginString; }
        }

        /// <summary>
        /// ����ģ���ĵ���
        /// </summary>
        public MemoryStream XSLT
        {
            get
            {
                XmlDocument xslDoc = new XmlDocument();
                XmlAttribute xslBaseUri = xslDoc.CreateAttribute("xmlns", "xsl", "http://www.w3.org/2000/xmlns/");
                xslBaseUri.Value = "http://www.w3.org/1999/XSL/Transform";
                XmlAttribute xslVersion = xslDoc.CreateAttribute("xsl", "version", "http://www.w3.org/1999/XSL/Transform");
                xslVersion.Value = "1.0";

                string nodeName = null;
                JSON node = null;
                if (Node is JSON)
                {
                    node = Node as JSON;
                    if (node["name"] is string)
                    {
                        nodeName = (string)node["name"];
                    }
                }
                if (nodeName == null || (nodeName = nodeName.Trim().ToLower()).Length < 1)
                {
                    nodeName = "div";
                }
                XmlElement xslRoot = xslDoc.CreateElement(nodeName);
                if (node != null)
                {
                    foreach (string key in node.Keys)
                    {
                        if (key == "name") continue;
                        object o = node[key];
                        if (o is string)
                        {
                            xslRoot.SetAttribute(key, (string)o);
                        }
                    }
                }

                xslRoot.Attributes.Append(xslBaseUri);
                xslRoot.Attributes.Append(xslVersion);

                xslRoot.InnerXml = XSL;

                xslDoc.AppendChild(xslRoot);

                MemoryStream xslStream = new MemoryStream();
                xslDoc.Save(xslStream);
                xslStream.Seek(0, SeekOrigin.Begin);

                return xslStream;
            }
        }

        /// <summary>
        /// ת��XML�ĵ�
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public void Transform(XmlReader reader, XmlWriter output)
        {
            XslCompiledTransform xct = new XslCompiledTransform(false);
            using (MemoryStream xslStream = XSLT)
            {
                xct.Load(XmlReader.Create(xslStream));
            }
            xct.Transform(reader, output);
        }

        /// <summary>
        /// ��ת������������
        /// </summary>
        /// <param name="reader">����</param>
        /// <param name="output">�����</param>
        public void Transform(XmlReader reader, Stream output)
        {
            XslCompiledTransform xct = new XslCompiledTransform(false);
            using (MemoryStream xslStream = XSLT)
            {
                xct.Load(XmlReader.Create(xslStream));
            }
            xct.Transform(reader, null, output);
        }

        /// <summary>
        /// ��ת������������
        /// </summary>
        /// <param name="reader">����</param>
        /// <param name="output">�����</param>
        public void Transform(XmlReader reader, TextWriter output)
        {
            XslCompiledTransform xct = new XslCompiledTransform(false);
            using (MemoryStream xslStream = XSLT)
            {
                xct.Load(XmlReader.Create(xslStream));
            }
            xct.Transform(reader, null, output);
        }

        /// <summary>
        /// ת��Datatable
        /// </summary>
        /// <param name="dt">����</param>
        /// <param name="output">���XML</param>
        public void Transform(DataTable dt, XmlWriter output)
        {
            if (dt == null || dt.Rows.Count < 1) return;
            using (MemoryStream ms = new MemoryStream())
            {
                dt.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                XmlReader reader = XmlReader.Create(ms);
                Transform(reader, output);
            }
        }

        /// <summary>
        /// ��ת������������
        /// </summary>
        /// <param name="reader">����</param>
        /// <param name="output">�����</param>
        public void Transform(DataTable dt, TextWriter output)
        {
            if (dt == null || dt.Rows.Count < 1) return;
            using (MemoryStream ms = new MemoryStream())
            {
                dt.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                XmlReader reader = XmlReader.Create(ms);

                XslCompiledTransform xct = new XslCompiledTransform(false);
                using (MemoryStream xslStream = XSLT)
                {
                    xct.Load(XmlReader.Create(xslStream));
                }
                xct.Transform(reader, null, output);
            }
        }

        /// <summary>
        /// ��ת������������
        /// </summary>
        /// <param name="reader">����</param>
        /// <param name="output">�����</param>
        public void Transform(DataTable dt, Stream output)
        {
            if (dt == null || dt.Rows.Count < 1) return;
            using (MemoryStream ms = new MemoryStream())
            {
                dt.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                XmlReader reader = XmlReader.Create(ms);

                XslCompiledTransform xct = new XslCompiledTransform(false);
                using (MemoryStream xslStream = XSLT)
                {
                    xct.Load(XmlReader.Create(xslStream));
                }
                xct.Transform(reader, null, output);
            }
        }

        /// <summary>
        /// ת��DatatableΪ�ַ���
        /// </summary>
        /// <param name="dt">����</param>
        /// <returns>�ַ���</returns>
        public string Transform(DataTable dt)
        {
            if (dt == null || dt.Rows.Count < 1) return String.Empty;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineChars = Environment.NewLine;
            settings.IndentChars = "";
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.CloseOutput = true;
            StringBuilder result = new StringBuilder();
            XmlWriter output = XmlWriter.Create(result, settings);
            Transform(dt, output);
            return result.ToString();
        }

		public override string ToString ()
		{
			return json.ToString();
		}
	}

    public delegate DataTable TemplateDataTableConvertor(TemplateMark tm);

    /// <summary>
    /// ģ�弯��
    /// </summary>
    public class TemplateMarkCollection
    {
        public TemplateMarkCollection()
        {
            dic = new Dictionary<string, TemplateMark>();
            str = null;
        }
        private readonly Dictionary<string, TemplateMark> dic;

        private string str;

        public bool HasTemplateContent
        {
            get
            {
                return !String.IsNullOrEmpty(str);
            }
        }

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
            str = s;
            MatchCollection matches = Regex.Matches(str, @"<!--\s*(\{[\w\W]*?\})\s*-->");
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

        public event TemplateDataTableConvertor OnGetData;

        public string Result()
        {
            if (OnGetData == null) throw new Exception("Define the [OnGetData] event method first.");
            if (!HasTemplateContent) throw new Exception("Read a template file first.");
            string s = str;
            foreach (string key in dic.Keys)
            {
                string r;
                try
                {
                    using (DataTable dt = OnGetData(dic[key]))
                    {
                        r = dic[key].Transform(dt);
                    }
                }
                catch (Exception ex)
                {
                    r = String.Format("\r\n<!-- \r\n [����ת��ʧ��] {0} \r\n-->", ex.Message);
                }
                s = s.Replace(dic[key].OriginString, r);
            }
            return s;
        }

		public TemplateMark this[string key]
		{
			get
			{
				if(dic.ContainsKey(key)) return dic[key];
				return null;
			}
		}
    }
}

