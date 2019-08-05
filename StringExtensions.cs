using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public static class StringExtensions
    {
        /// <summary>
        /// 左辺から指定した長さの文字を取得します。
        /// </summary>
        /// <param name="s">対象文字</param>
        /// <param name="length">長さ</param>
        /// <param name="padding">長さが足りない場合に埋める文字(ﾃﾞﾌｫﾙﾄ空白)</param>
        /// <returns></returns>
        public static string Left(this string s, int length, char padding = ' ')
        {
            return s.Mid(0, length, padding);
        }

        /// <summary>
        /// 取得する位置と長さを指定して文字を取得します。
        /// </summary>
        /// <param name="s">対象文字</param>
        /// <param name="start">取得する開始位置</param>
        /// <param name="length">取得する文字の長さ</param>
        /// <param name="padding">長さが足りない場合に埋める文字(ﾃﾞﾌｫﾙﾄ空白)</param>
        /// <returns></returns>
        public static string Mid(this string s, int start, int length, char padding = ' ')
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            else if (s.Length < (start + length))
            {
                return s.PadRight(start + length, padding).Mid(start, length, padding);
            }
            else
            {
                return s.Substring(start, length);
            }
        }

        /// <summary>
        /// 右辺から指定した長さの文字を取得します。
        /// </summary>
        /// <param name="s">対象文字</param>
        /// <param name="length">長さ</param>
        /// <param name="padding">長さが足りない場合に埋める文字(ﾃﾞﾌｫﾙﾄ空白)</param>
        /// <returns></returns>
        public static string Right(this string s, int length, char padding = ' ')
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            else if (s.Length < (length))
            {
                return s.PadLeft(length, padding).Right(length, padding);
            }
            else
            {
                return s.Substring(s.Length - length, length);
            }
        }

        /// <summary>
        /// 文字をﾊﾞｲﾄ配列に変換します。
        /// </summary>
        /// <param name="s">対象文字</param>
        /// <param name="encoding">ﾊﾞｲﾄ配列に変換する際に使用するｴﾝｺｰﾃﾞｨﾝｸﾞ</param>
        /// <returns></returns>
        public static byte[] ToBytes(this string s, Encoding encoding)
        {
            return encoding.GetBytes(s);
        }

        /// <summary>
        /// 文字を16進数文字に変換します。
        /// </summary>
        /// <param name="s">対象文字</param>
        /// <param name="encoding">16進数文字に変換する際に使用するｴﾝｺｰﾃﾞｨﾝｸﾞ</param>
        /// <param name="length">16進数文字の長さ(ﾃﾞﾌｫﾙﾄ4)</param>
        /// <returns></returns>
        public static string ToHex(this string s, Encoding encoding, int length = 4)
        {
            return BitConverter.ToString(s.ToBytes(encoding))
                .Replace("-", "")
                .PadRight(length, '0');
        }

        /// <summary>
        /// 16進数文字を数値に変換します。
        /// </summary>
        /// <param name="s">対象の16進数文字</param>
        /// <returns></returns>
        public static long Hex2Long(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0;

            switch (s.Length)
            {
                case 2:
                    return Convert.ToInt16(s, 16);
                case 4:
                    return Convert.ToInt16(s, 16);
                case 8:
                    return Convert.ToInt32(s, 16);
                default:
                    return Convert.ToInt64(s, 16);
            }
        }

        /// <summary>
        /// 16進数文字を2進数文字に変換します。
        /// </summary>
        /// <param name="s">16進数文字</param>
        /// <param name="length">変換後の文字長さ</param>
        /// <returns></returns>
        public static string Hex2Bin(this string s, int length = 16)
        {
            return Convert.ToString(s.Hex2Long(), 2).PadLeft(length, '0');
        }

        /// <summary>
        /// 16進数文字から文字列に変換します。
        /// </summary>
        /// <param name="s">16進数文字</param>
        /// <param name="encoding">ｴﾝｺｰﾃﾞｨﾝｸﾞ</param>
        /// <returns></returns>
        public static string FromHex(this string s, Encoding encoding)
        {
            return encoding.GetString(
                Enumerable.Range(0, s.Length / 2)
                    .Select(i => Convert.ToByte(s.Mid(i * 2, 2), 16))
                    .ToArray()
            ).TrimEnd(new[] { (char)0, ' ' });
        }

        /// <summary>
        /// 2進数文字を16進数文字に変換します。
        /// </summary>
        /// <param name="s">2進数文字</param>
        /// <param name="length">16進数文字変換後の文字長さ</param>
        /// <returns></returns>
        //public static string Bin2Hex(this string s, int length = 4)
        //{
        //    return Convert.ToInt32(s, 2).ToHex(length);
        //}

        public static TimeSpan? ToTimeSpan(this string s)
        {
            const string dd = "(?<day>[\\d]+)";
            const string hh = "(?<hour>[\\d]+)";
            const string mm = "(?<minute>[\\d]+)";
            const string ss = "(?<second>[\\d]+)";

            string[] formats = new[]
            {
                $"^{dd}\\.{hh}:{mm}:{ss}$",
                $"^{dd}\\.{hh}:{mm}$",
                $"^{dd}\\.{hh}$",

                $"^{dd}日{hh}:{mm}:{ss}$",
                $"^{dd}日{hh}:{mm}$",
                $"^{dd}日{hh}$",

                $"^{hh}:{mm}:{ss}$",
                $"^{hh}:{mm}$",
                $"^{hh}$",
            };

            foreach (var expr in formats)
            {
                // 正規表現で検索
                var regex = new Regex(expr, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var match = regex.Match(s);

                if (!match.Success)
                {
                    // 検索できない場合は次の正規表現へ
                    continue;
                }

                // 年月日時分を入れるための変数を用意
                var result = TimeSpan.Zero;

                foreach (var groupname in regex.GetGroupNames())
                {
                    switch (groupname)
                    {
                        case "day":
                        case "hour":
                        case "minute":
                        case "second":
                            break;
                        default:
                            continue;
                    }

                    var value = double.Parse(match.Groups[groupname].Value);

                    switch (groupname)
                    {
                        case "day":
                            result += TimeSpan.FromDays(value);
                            break;
                        case "hour":
                            result += TimeSpan.FromHours(value);
                            break;
                        case "minute":
                            result += TimeSpan.FromMinutes(value);
                            break;
                        case "second":
                            result += TimeSpan.FromSeconds(value);
                            break;
                    }
                }

                // 取得できたら日付に変換して返却する
                return result;
            }

            return null;
        }

        /// <summary>
        /// ﾐﾘ秒以下を切り捨てた現在日付を取得します。
        /// </summary>
        public static TextInfo TextInfo
        {
            get { return _TextInfo = _TextInfo ?? CultureInfo.CurrentCulture.TextInfo; }
        }
        private static TextInfo _TextInfo;

        public static string ToTitleCase(this string s)
        {
            return TextInfo.ToTitleCase(s);
        }

        /// <summary>
        /// ﾊﾞｲﾄ配列を文字に変換します。
        /// </summary>
        /// <param name="bytes">対象ﾊﾞｲﾄ配列</param>
        /// <param name="encoding">文字に変換する際に使用するｴﾝｺｰﾃﾞｨﾝｸﾞ</param>
        /// <returns></returns>
        public static string ToString(this byte[] bytes, Encoding encoding)
        {
            return encoding.GetString(bytes);
        }

        public static string ToHex(this int i, int length = 2)
        {
            return Convert.ToString(i, 16).Right(length, '0');
        }
        /// <summary>
        /// 2進数文字を16進数文字に変換します。
        /// </summary>
        /// <param name="s">2進数文字</param>
        /// <param name="length">16進数文字変換後の文字長さ</param>
        /// <returns></returns>
        public static string Bin2Hex(this string s, int length = 4)
        {
            return Convert.ToInt32(s, 2).ToHex(length);
        }
    }
}
