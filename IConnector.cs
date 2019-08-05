using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public interface IConnector : IDisposable
    {
        /// <summary>
        /// 伝文の終端文字
        /// </summary>
        string EndString { get; set; }

        /// <summary>
        /// 送受信用文字列ｴﾝｺｰﾄﾞ
        /// </summary>
        Encoding Encoding { get; set; }

        /// <summary>
        /// 接続文字列
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// ﾀｲﾑｱｳﾄ時間(ﾐﾘ秒)
        /// </summary>
        int Timeout { get; set; }

        /// <summary>
        /// 通信中かどうか
        /// </summary>
        ConnectionStatuses Status { get; }

        /// <summary>
        /// 通信を開始します。
        /// </summary>
        /// <returns></returns>
        Task<bool> ConnectAsync();

        /// <summary>
        /// ﾒｯｾｰｼﾞを送信します。
        /// </summary>
        /// <param name="message">送信するﾒｯｾｰｼﾞ</param>
        /// <returns></returns>
        Task<bool> WriteAsync(string message);

        /// <summary>
        /// ﾒｯｾｰｼﾞを受信します。
        /// </summary>
        /// <returns>受信したﾒｯｾｰｼﾞ</returns>
        Task<string> ReadAsync();

        /// <summary>
        /// 通信を終了します。
        /// </summary>
        /// <returns></returns>
        void DisConnect();

        ///// <summary>
        ///// 接続停止ｲﾍﾞﾝﾄを通知します。
        ///// </summary>
        //event EventHandler Disconnected;

        ///// <summary>
        ///// 接続開始ｲﾍﾞﾝﾄを通知します。
        ///// </summary>
        //event EventHandler Connected;

        /// <summary>
        /// 実行可能になるまで待機します。
        /// </summary>
        /// <returns></returns>
        Task WaitAsync();

        /// <summary>
        /// 処理完了を他ｽﾚｯﾄﾞに通知します。
        /// </summary>
        /// <returns></returns>
        int Release();

    }
}
