using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    /// <summary>
    /// Azbil F4Hの制御ON/OFFを変更するためのコンソールアプリ。
    /// アプリケーション情報をコマンドで受け取って、ヘッダとフッタをプログラム上で作成し、F4Hへ送信。
    /// 受信した内容を編集せずコンソールに表示する。
    /// 制御ON/OFFを変更するために作成したが、コマンドは自由に変更可能なはず。
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Sub();
        }

        static void Sub()
        {
            const char STX = (char)0x02;
            const char ETX = (char)0x03;
            const char CR = (char)0x0D;
            const char LF = (char)0x0A;

            try
            {
                using (var conn = new SERIALConnector("COM1:19200^8^1^N"))
                {
                    conn.Connect();
                    Console.WriteLine("WS,1204W,%u: %u=1/control, 0/close");

                    string read = "";

                    while ((read = Console.ReadLine()) != "")
                    {
                        // WS,1204W,%u%c
                        var tmp = $"{STX}0100X{read}{ETX}";
                        var chk1 = tmp.ToBytes(conn.Encoding).Sum(c => (int)c);
                        var chk2 = chk1.ToHex(2);
                        var chk3 = chk2.Hex2Bin(16);
                        var chk4 = (int)(string.Join("", chk3.Select(c => 1 - int.Parse($"{c}")).ToArray()).Bin2Hex().Hex2Long() + 1);
                        var chk5 = chk4.ToHex(2).ToUpper();
                        Console.WriteLine($"{chk1}:{chk2}:{chk3}:{chk4}:{chk5}");
                        conn.Write($"{tmp}{chk5}{CR}{LF}");

                        Console.WriteLine(conn.ReadAsync());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


            Console.WriteLine("おわり！");
            Console.ReadLine();
        }
    }
}
