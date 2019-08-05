using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class SERIALConnector : IConnector
    {
        /** ****************************************************************************************************
         ** ｺﾝｽﾄﾗｸﾀ
         ** ****************************************************************************************************/

        /// <summary>
        /// ｺﾝｽﾄﾗｸﾀ
        /// </summary>
        /// <param name="connectionString">接続文字列。これは、「COM:BaudRate^DataBits^StopBits^Parity」の形式である必要があります。</param>
        /// <example>192.168.1.100:9001</example>
        public SERIALConnector(string connectionString)
        {
            ConnectionString = connectionString;

            var buf1 = ConnectionString.Split(':');

            if (buf1.Length != 2)
            {
                throw new FormatException("接続文字列の書式が[COM:BaudRate^DataBits^StopBits^Parity]ではありません。");
            }

            var buf2 = buf1[1].Split('^');

            if (buf2.Length != 4)
            {
                throw new FormatException("接続文字列の書式が[COM:BaudRate^DataBits^StopBits^Parity]ではありません。");
            }

            if (buf1[0].StartsWith("COM"))
            {
                PortName = buf1[0];
            }
            else
            {
                throw new FormatException("COMﾎﾟｰﾄの指定が不正です。");
            }

            int buf3;
            if (int.TryParse(buf2[0], out buf3))
            {
                BaudRate = buf3;
            }
            else
            {
                throw new FormatException("ﾎﾞｰﾚｰﾄの指定が不正です。");
            }

            if (int.TryParse(buf2[1], out buf3))
            {
                DataBits = buf3;
            }
            else
            {
                throw new FormatException("ﾃﾞｰﾀﾋﾞｯﾄの指定が不正です。");
            }

            if (int.TryParse(buf2[2], out buf3) && 0 <= buf3 && buf3 <= 2)
            {
                StopBits = buf3 == 0
                    ? StopBits.None
                    : buf3 == 1
                    ? StopBits.One
                    : StopBits.Two;
            }
            else
            {
                throw new FormatException("ｽﾄｯﾌﾟﾋﾞｯﾄの指定が不正です。");
            }

            if ("N".Equals(buf2[3]))
            {
                Parity = Parity.None;
            }
            else if ("E".Equals(buf2[3]))
            {
                Parity = Parity.Even;
            }
            else if ("O".Equals(buf2[3]))
            {
                Parity = Parity.Odd;
            }
            else
            {
                throw new FormatException("ﾊﾟﾘﾃｨの指定が不正です。");
            }
        }

        /// <summary>
        /// ｺﾝｽﾄﾗｸﾀ
        /// </summary>
        /// <param name="portname">ﾎﾟｰﾄ名(COM1等)</param>
        /// <param name="baudrate">ﾎﾞｰﾚｰﾄ</param>
        /// <param name="databits">ﾃﾞｰﾀﾋﾞｯﾄ</param>
        /// <param name="stopbits">ｽﾄｯﾌﾟﾋﾞｯﾄ(0-2)(</param>
        /// <param name="parity">ﾊﾟﾘﾃｨ(N:None,E:Even,O:Odd)</param>
        public SERIALConnector(string portname, int baudrate, int databits, int stopbits, string parity) : this($"{portname}:{baudrate}^{databits}^{stopbits}^{parity}")
        {

        }

        /** ****************************************************************************************************
         ** ｲﾍﾞﾝﾄ
         ** ****************************************************************************************************/

        ///// <summary>
        ///// 接続停止ｲﾍﾞﾝﾄを通知します。
        ///// </summary>
        //public event EventHandler Disconnected;

        ///// <summary>
        ///// 接続開始ｲﾍﾞﾝﾄを通知します。
        ///// </summary>
        //public event EventHandler Connected;

        /** ****************************************************************************************************
         ** ﾌﾟﾛﾊﾟﾃｨ
         ** ****************************************************************************************************/

        /// <summary>
        /// 伝文の終端文字
        /// </summary>
        public string EndString { get; set; } = "\r\n";

        /// <summary>
        /// 送受信用文字列ｴﾝｺｰﾄﾞ
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.GetEncoding("Shift_JIS");

        /// <summary>
        /// 接続文字列
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// ﾀｲﾑｱｳﾄ時間
        /// </summary>
        public int Timeout { get; set; } = 3000;

        /// <summary>
        /// 接続が確立しているかどうか
        /// </summary>
        public ConnectionStatuses Status
        {
            get
            {
                if (COM == null)
                {
                    return ConnectionStatuses.DisConnect;
                }
                if (!COM.IsOpen)
                {
                    return ConnectionStatuses.DisConnect;
                }

                return ConnectionStatuses.Connect;
            }
        }

        /// <summary>
        /// ﾎﾟｰﾄ名
        /// </summary>
        private string PortName { get; set; }

        /// <summary>
        /// ﾎﾞｰﾚｰﾄ
        /// </summary>
        private int BaudRate { get; set; }

        /// <summary>
        /// ﾃﾞｰﾀﾋﾞｯﾄ
        /// </summary>
        private int DataBits { get; set; }

        /// <summary>
        /// ｽﾄｯﾌﾟﾋﾞｯﾄ
        /// </summary>
        private StopBits StopBits { get; set; }

        /// <summary>
        /// ﾊﾟﾘﾃｨ
        /// </summary>
        private Parity Parity { get; set; }

        /// <summary>
        /// ｼﾘｱﾙﾎﾟｰﾄ
        /// </summary>
        private SerialPort COM { get; set; }

        /** ****************************************************************************************************
         ** 公開ﾒｿｯﾄﾞ
         ** ****************************************************************************************************/

        /// <summary>
        /// 通信を確立します。
        /// </summary>
        public bool Connect()
        {
            if (Status == ConnectionStatuses.Connect || COM != null)
            {
                // 既に通信している場合は再接続
                return true;
            }
            else if (COM != null)
            {
                // ｺﾞﾐ掃除
                DisConnect();
            }

            // ｲﾝｽﾀﾝｽ生成
            COM = new SerialPort();
            COM.PortName = PortName;
            COM.NewLine = EndString.Last().ToString();
            COM.BaudRate = BaudRate;
            COM.DataBits = DataBits;
            COM.StopBits = StopBits;
            COM.Parity = Parity;
            COM.ReadTimeout = Timeout;
            COM.WriteTimeout = Timeout;

            // 通信開始
            COM.Open();

            // 通信開始ｲﾍﾞﾝﾄ
            //Connected(this, new EventArgs());

            return true;
        }

        public async Task<bool> ConnectAsync()
        {
            await Task.Delay(1);
            return Connect();
        }
        /// <summary>
        /// 通信を切断します。
        /// </summary>
        public void DisConnect()
        {
            if (COM != null)
            {
                COM.Close();
                COM.Dispose();
                COM = null;

                // 通信開始ｲﾍﾞﾝﾄ
                //Disconnected(this, new EventArgs());
            }
        }

        /// <summary>
        /// 非同期受信を開始して、結果を返却します。
        /// </summary>
        public string ReadAsync()
        {
            try
            {
                if (Status != ConnectionStatuses.Connect)
                {
                    return string.Empty;
                }

                using (var buffer = new MemoryStream())
                {
                    var bytes = new byte[2048];
                    var size = 0;
                    var endstring = EndString.Left(EndString.Length - 1);

                    // 読取ﾃﾞｰﾀがある間繰り返す。
                    while ((size = COM.Read(bytes, 0, bytes.Length)) != 0)
                    {

                        buffer.Write(bytes, 0, size);

                        if (buffer.Length < endstring.Length)
                        {
                            // 終端文字の数だけ読み取っていない場合は次ﾙｰﾌﾟへ
                            continue;
                        }

                        // 終端文字の数だけｼｰｸ位置を戻す。
                        buffer.Seek(endstring.Length * -1, SeekOrigin.End);

                        if (buffer.ToArray().ToString(Encoding).Contains(EndString))
                        {
                            // 読み取った最後の文字がEndStringと同値なら終了
                            break;
                        }
                        else
                        {
                            // ｼｰｸ位置を戻す。
                            buffer.Seek(0, SeekOrigin.End);
                        }
                    }

                    // ﾒｯｾｰｼﾞ取得
                    var message = buffer.ToArray().ToString(Encoding).ToUpper();

                    Console.WriteLine("Read終了");

                    // 終端文字を除外して返却
                    return message.Left(message.Length - endstring.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                // 例外発生時は切断
                DisConnect();

                // 空文字を返却
                return string.Empty;
            }
        }

        /// <summary>
        /// 非同期送信を実行します。
        /// </summary>
        public bool Write(string message)
        {
            try
            {
                if (Status != ConnectionStatuses.Connect)
                {
                    return false;
                }

                if (message.EndsWith(EndString))
                {
                    // 送信文字作成+ﾊﾞｲﾄ変換
                    var messageBytes = message.ToUpper().ToBytes(Encoding);

                    // 送信処理
                    COM.Write(messageBytes, 0, messageBytes.Length);

                    Console.WriteLine($"Write終了:{message}");

                    return true;
                }
                else
                {
                    // 終端文字を追加して再帰
                    return Write($"{message}{EndString}");
                }
            }
            catch
            {
                // 例外発生時は切断
                DisConnect();

                // 失敗を返却
                return false;
            }
        }

        /// <summary>
        /// 実行可能になるまで待機します。
        /// </summary>
        /// <returns></returns>
        public async Task WaitAsync()
        {
            // COMﾎﾟｰﾄ単位で1つ
            //await SemaphoreManager.WaitAsync(PortName);
            await Task.Delay(1);
        }

        /// <summary>
        /// 処理完了を他ｽﾚｯﾄﾞに通知します。
        /// </summary>
        /// <returns></returns>
        public int Release()
        {
            // COMﾎﾟｰﾄ単位で1つ
            //return SemaphoreManager.Release(PortName);
            return 0;
        }

        /** ****************************************************************************************************
         ** Disposable Support
         ** ****************************************************************************************************/

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)。
                    DisConnect();
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~TCPIP4Connector() {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }

        public Task<bool> WriteAsync(string message)
        {
            throw new NotImplementedException();
        }

        Task<string> IConnector.ReadAsync()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
