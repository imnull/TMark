using System;
using System.Collections.Generic;
using System.Text;

namespace TMark
{
    class Program
    {
        static void Main(string[] args)
        {
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

			
			
			
            Console.ReadLine();

        }
    }
}
