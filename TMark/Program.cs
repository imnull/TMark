using System;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace TMark
{
    class Program
    {
        static void Main(string[] args)
        {
            //string html = "<!--{name : 't1', a:1}--> <!--{name : 't2', b:2}--> ";

            string tfile = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "template.html";
            TemplateMarkCollection tc = new TemplateMarkCollection();
            tc.Read(tfile, Encoding.Default);
			
			TemplateMark tm = tc["test01"];
			
			System.Xml.Xsl.XslTransform xt = new System.Xml.Xsl.XslTransform();
			XmlReader xslReader = XmlReader.Create(new StringReader(tm.XSL));
			DataTable dt = testData01;
			
			MemoryStream ms = new MemoryStream();
			dt.WriteXml(ms);
			ms.Seek(0, SeekOrigin.Begin);
			XmlReader xmlReader = XmlReader.Create(ms);
			
			
			XslCompiledTransform xct = new XslCompiledTransform(false);
			xct.Load(xslReader);
			
			
			
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.IndentChars = "\t";
			
			StringBuilder result = new StringBuilder();
			XmlWriter xw = XmlWriter.Create(result, settings);
			
			
			xct.Transform(xmlReader, xw);
			xw.Flush();
			xw.Close();
			
			string r = result.ToString();
			
			Console.Write(r);
			
            Console.ReadLine();
			
			

            /*
            string test =
@"{
    test01 : 0xf1f1f1,
    'test02' : 'Using single-quote',
aaa:[12,34,56],
     ""firstName"": ""John"",
     ""lastName"": ""Smith"",
     ""male"": true,
     ""age"": 25,
     ""address"": 
     {
         ""streetAddress"": ""21 2nd Street"",
         ""city"": ""New York"",
         ""state"": ""NY"",
         ""postalCode"": ""10021""
     },
     ""phoneNumber"": 
     [
         {
           ""type"": ""home"",
           ""number"": ""212 555-1234""
         },
         {
           ""type"": ""fax"",
           ""number"": ""646 555-4567""
         }
     ]
 }
";
            //string test2 = "[11,2,3,4,5,6,{a:{b:1}}]";

            JSON json = new JSON();
            json.Read(test);

            //object val = json["aaa"];

            string jsonString = json.ToString();
            Console.WriteLine(jsonString);

            Console.WriteLine("------------------------- I'm splitter -------------------------");

			
			TemplateMark tm=new TemplateMark();
			tm.Name = "Testing...";
			tm.XSL = "<xsl:template>'a\r\n'</xsl:template>";
			tm.SQL = "SELECT TOP (10) * FROM TB WHERE state = @st";
			tm.Parameters["@st"] = 1;
			
			Console.WriteLine(tm.ToString());
            */
        }
		
		private static DataTable testData01
		{
			get
			{
				DataTable dt = new DataTable("article");
				dt.Columns.Add("id", typeof(int));
				dt.Columns.Add("title", typeof(string));
				
				Random r = new Random();
				
				int len = r.Next(59, 137);
				
				for(int i = 0; i < len; i++)
				{
					DataRow row = dt.NewRow();
					row[0] = i;
					row[1] = String.Format("article {0:000}", i);
					dt.Rows.Add(row);
				}
				
				return dt;
			}
		}
    }
}
