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
            string tfile = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "template.html";
            TemplateMarkCollection tc = new TemplateMarkCollection();
            tc.OnGetData += new TemplateDataTableConvertor(tc_OnGetData);
            tc.Read(tfile, Encoding.Default);


            string s = tc.Result();

            Console.Write(s);

            Console.ReadLine();
     
        }

        static DataTable tc_OnGetData(TemplateMark tm)
        {
            return testData01;
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
