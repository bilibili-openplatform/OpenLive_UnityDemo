namespace OpenBLive.Runtime.Data
{
    /// <summary>
    /// 服务器返回的Websocket长链接信息 https://open-live.bilibili.com/doc/2/1
    /// </summary>
    public struct WebsocketInfo
    {
        public int code;
        public string message;
        public WebsocketInfoData data;
    }
}