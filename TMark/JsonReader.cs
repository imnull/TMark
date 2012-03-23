/** 
 *
 * JsonReader.cs
 * (c) 2011-2012 mk31415926535@gmail.com
 * JsonReader.cs may be freely distributed under the MIT license.
 *
 * */


using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TMark
{
    public class JsonReader
    {
        public JsonReader(string s)
        {
            origin = s;
            Length = s.Length;
            initReader();
        }

        public readonly int Length;
        private readonly string origin;

        private StringReader reader;
        private int position;

        /// <summary>
        /// 初始化
        /// </summary>
        protected void initReader()
        {
            position = 0;
            reader = new StringReader(origin);
        }

        /// <summary>
        /// 当前位置字符
        /// </summary>
        public char Current
        {
            get
            {
                return origin[Math.Min(origin.Length, position) - 1];
            }
        }

        /// <summary>
        /// 读一个字符
        /// </summary>
        /// <returns></returns>
        public int Read()
        {
            position++;
            return reader.Read();
        }

        /// <summary>
        /// 预读一个字符
        /// </summary>
        /// <returns></returns>
        public int Peek()
        {
            return reader.Peek();
        }

        /// <summary>
        /// 读取到一个对象的开始部分
        /// </summary>
        /// <returns>返回int用于判断数据类型</returns>
        public int ReadToObjBegin()
        {
            int ch;
            while ((ch = Read()) > -1 && ch != '{' && ch != '[');

            switch (ch)
            {
                case '{':
                    return 0;
                case '[':
                    return 1;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// 读取连续的空白字符
        /// </summary>
        public void ReadBlank() 
        {
            int ch;
            int p = position;
            while ((ch = Peek()) > -1)
            {
                switch (ch)
                {
                    case '\r':
                    case '\n':
                    case '\t':
                    case ' ':
                        Read();
                        continue;
                    default:
                        return;
                }
            }
            return;
        }

        /// <summary>
        /// 读取到空白字符为止
        /// </summary>
        public void ReadToBlank(out string value)
        {
            int ch;
            int start = position;
            while ((ch = Peek()) > -1)
            {
                switch (ch)
                {
                    case '\r':
                    case '\n':
                    case '\t':
                    case ' ':
                        value = origin.Substring(start, position - start);
                        return;
                    default:
                        Read();
                        break;
                }
            }
            value = null;
        }

        /// <summary>
        /// 读取单双引号内容
        /// </summary>
        /// <param name="charIn">引号值</param>
        /// <param name="value">输出内部内容</param>
        public void ReadQuote(int charIn, out string value)
        {
            int ch;
            int start = position;
            while ((ch = Peek()) > -1)
            {
                if (ch == charIn)
                {
                    if (Read() != '\\') //转义符号处理 若 \\' 则会出错 
                    {
                        value = origin.Substring(start, position - start - 1);
                        ReadBlank();
                        return;
                    }
                }
                Read();
            }
            value = null;
        }

        /// <summary>
        /// 读取属性名称
        /// </summary>
        /// <param name="name">输出属性名称</param>
        public void ReadPropertyName(out string name)
        {
            ReadBlank();

            int ch = Read();
            int start = position;

            if (ch == '\'' || ch == '"')
            {
                QuoteString qs = new QuoteString();
                ReadQuote(ch, out qs.Value);
                qs.Quote = (char)ch;
                name = qs.ToString();
                if (Current != ':')
                {
                    ReadBlank();
                    ch = Read();
                }
                if (ch != ':') throw new Exception("Should be ':'");
            }
            else
            {
                while ((ch = Read()) > -1)
                {
                    switch (ch)
                    {
                        case ':':
                        case ' ':
                        case '\r':
                        case '\n':
                        case '\t':
                            name = origin.Substring(start - 1, position - start);
                            if (Current != ':')
                            {
                                ReadBlank();
                                ch = Read();
                            }
                            if (ch != ':') throw new Exception("Should be ':'");
                            return;
                        default:
                            continue;
                    }
                }
                name = null;
            }
        }

        /// <summary>
        /// 读取属性值
        /// </summary>
        /// <param name="value">输出属性值</param>
        /// <param name="endtype">根据结束字符，判断为结束对象或者进入下一属性）</param>
        public void ReadPorperty(out object value, out int endtype)
        {
            if (Current != ':') throw new Exception("The current char should be ':'");
            ReadBlank();


            int ch = Read();
            int start = position - 1;
            int sub = 0;

            switch (ch)
            {
                case '\'':
                case '"':
                    string _value;
                    ReadQuote(ch, out _value);
                    value = ConvertData(_value, ch);
                    ReadBlank();
                    endtype = Read();

                    return;
                case '{':
                    JSON json = new JSON();
                    ReadToJSON(this, json, out endtype);
                    value = json;
                    return;
                case '[':
                    value = ReadArray(out endtype);
                    return;
                default:
                    while ((ch = Read()) > -1)
                    {
                        switch (ch)
                        {
                            case '}':
                            case ',':
                                sub = position;
                                goto BREAK_READ_PROP;
                            case ' ':
                            case '\r':
                            case '\n':
                            case '\t':
                                sub = position;
                                ReadBlank();
                                ch = Read();
                                goto BREAK_READ_PROP;
                        }
                    }
                    goto NO_RIGHT_END;
                BREAK_READ_PROP:
                    endtype = ch;
                    value = ConvertData(origin.Substring(start, sub - start - 1), -1);
                    return;
            }

        NO_RIGHT_END:
            value = null;
            endtype = -1;
            return;
        }

        /// <summary>
        /// 从当前位置读到指定的字符为止
        /// </summary>
        /// <param name="value">输出值</param>
        /// <param name="ch">字符组</param>
        public void ReadTo(out string value, params int[] ch) 
        {
            int start = position - 1;
            int c;
            while ((c = Read()) > -1 && Array.IndexOf<int>(ch, c) < 0) ;
            value = origin.Substring(start, position - start - 1);
        }

        /// <summary>
        /// 读数组对象
        /// </summary>
        /// <param name="type">输出结束字符</param>
        /// <returns>返回无类型数组</returns>
        public object[] ReadArray(out int type)
        {
            if (Current != '[') throw new Exception("should be '['");

            List<object> list = new List<object>();

            int ch;

            ARRAY_LOOP_READ:

            ReadBlank();

            ch = Read();
            if (ch < 0 || ch == '}' || ch == ',')
            {
                type = ch;
                return list.ToArray();
            }
            string _value;

            switch (ch)
            {
                case '\'':
                case '"':
                    ReadQuote(ch, out _value);
                    list.Add(ConvertData(_value, ch));
                    Read();
                    ReadBlank();


                    switch (Current)
                    {
                        case ']':
                            ReadBlank();
                            type = Read();
                            return list.ToArray();
                        case ',':
                            ReadBlank();
                            goto ARRAY_LOOP_READ;
                        default:
                            throw new Exception("Why?!");
                    }
                case '{':
                    JSON json = new JSON();
                    ReadToJSON(this, json, out type);
                    list.Add(json);
                    goto ARRAY_LOOP_READ;
                case '[':
                    object[] arr = ReadArray(out type);
                    list.Add(arr);
                    goto ARRAY_LOOP_READ;
                default:
                    ReadTo(out _value, ',', ']');
                    list.Add(ConvertData(_value, -1));
                    ReadBlank();
                    switch (Current)
                    {
                        case ']':
                            type = Read();
                            return list.ToArray();
                        case ',':
                            goto ARRAY_LOOP_READ;
                        default:
                            throw new Exception("Why?!");
                    }
            }

        }

        /// <summary>
        /// 从JsonReader的当前位置读一个JSON对象，并存储到参数中
        /// </summary>
        /// <param name="jr">源JR</param>
        /// <param name="json">写入对象</param>
        /// <param name="endtype">结束字符</param>
        public static void ReadToJSON(JsonReader jr, JSON json, out int endtype)
        {
            if (jr.Current != '{') throw new Exception("should be '['");

            string name;
            object value;
            int type;

            while (true)
            {
                jr.ReadPropertyName(out name);
                jr.ReadPorperty(out value, out type);
                json.Add(name, value);

                switch (type)
                {

                    case ',':
                        continue;
                    default:
                    case '}':
                        jr.ReadBlank();
                        type = jr.Read();
                        goto READ_END;
                }
            }

                READ_END:
                endtype = type;
            }

        /// <summary>
        /// 转换数据格式
        /// </summary>
        /// <param name="s">原始字符串</param>
        /// <param name="quote">是否在引号中</param>
        /// <returns>猜测一种类型返回</returns>
        public static object ConvertData(string s, int quote)
        {
            if (quote > 0) return new QuoteString(s, quote);

            if (Regex.IsMatch(s, @"\d+|0[x][\da-f]+", RegexOptions.IgnoreCase))
            {
                if (s.Length > 2 && s.Substring(0, 2).ToLower() == "0x")
                {
                    return Convert.ToInt64(s, 16);
                }
                else 
                {
                    int i;
                    if (int.TryParse(s, out i)) return i;
                    return s;
                }
            }
            else if (s == "null")
            {
                return null;
            }
            else if (s == "true" || s == "false")
            {
                return s == "true";
            }
            else
            {
                return new QuoteString(s, -1);
            }
        }
    }

    public struct QuoteString
    {
        public QuoteString(string v, int q)
        {
            Value = v;
            Quote = (char)q;
        }
        public string Value;
        public char Quote;

        public override string ToString()
        {
            if (Quote < 0) return Value;
            else return String.Format("{0}{1}{0}", Quote, Value);
        }
    }

    /// <summary>
    /// JSON对象
    /// </summary>
    public class JSON
    {
        public JSON() { dic = new Dictionary<string, object>(); }
        private readonly Dictionary<string, object> dic;

        /// <summary>
        /// 添加值对
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="val">值</param>
        public void Add(string key, object val)
        {
            dic[key] = val;
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear() { dic.Clear(); }

        /// <summary>
        /// 读取JSON字符串
        /// </summary>
        /// <param name="s"></param>
        public void Read(string s)
        {
            Clear();

            JsonReader jr = new JsonReader(s);
            jr.ReadToObjBegin();

            int type;

            JsonReader.ReadToJSON(jr, this, out type);
        }

        public override string ToString()
        {
            List<string> list = new List<string>();
            foreach (string key in dic.Keys)
            {

                list.Add(String.Format("{0}:{1}", key, GetValue(dic[key])));
            }

            return String.Format("{{{0}}}", String.Join(",", list.ToArray()));
        }

        private static string GetValue(object value)
        {
            if (value == null) return "null";
            else if (value is QuoteString || value is Int32 || value is JSON)
            {
                return value.ToString();
            }
            else if (value is Int64)
            {
                return String.Format("0x{0:x}", value);
            }
            else if (value is bool)
            {
                return value.ToString().ToLower();
            }
            else if (value is object[])
            {
                object[] vs = (object[])value;
                string[] ss = new string[vs.Length];
                for (int i = 0; i < ss.Length; i++)
                {
                    ss[i] = GetValue(vs[i]);
                }
                return String.Format("[{0}]", String.Join(",", ss));
            }
            else
            {
                throw new Exception("Unknown type");
            }
        }

    }
}
