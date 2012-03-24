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
            string test2 = "[11,2,3,4,5,6,{a:{b:1}}]";

            object o = Convert.ToInt32("0xff", 16);
            JSON json = new JSON();
            json.Read(test);

            object val = json["aaa"];

            string jsonString = json.ToString();
            Console.WriteLine(jsonString);

            Console.WriteLine("------------------------- I'm splitter -------------------------");

            /*
             * result:
             * {test01:0xf1f1f1,aaa:[12,34,56],'test02':'Using single-quote',"firstName":"John","lastName":"Smith","male":true,"age":25,"address":{"streetAddress":"21 2nd Street","city":"New York","state":"NY","postalCode":"10021"},"phoneNumber":[{"type":"home","number":"212 555-1234"},{"type":"fax","number":"646 555-4567"}]}
             */

            json.Read(jsonString);
            jsonString = json.ToString();
            Console.WriteLine(jsonString);

            Console.WriteLine("------------------------- I'm splitter -------------------------");

            json.Read(jsonString);
            jsonString = json.ToString();
            Console.WriteLine(jsonString);

            Console.WriteLine("------------------------- I'm splitter -------------------------");

            json.Read(jsonString);
            jsonString = json.ToString();
            Console.WriteLine(jsonString);

            Console.ReadLine();

        }
    }
}
