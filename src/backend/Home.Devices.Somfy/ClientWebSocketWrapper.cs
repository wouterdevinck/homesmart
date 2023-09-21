// Adapted from https://github.com/mrbighokage/ClientWebSocketWrapper.DotNet/blob/master/src/ClientWebSocketWrapper/WebSocketWrapper.cs

using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Home.Devices.Somfy {

    internal class ClientWebSocketWrapper : IDisposable {

        private readonly ClientWebSocket _webSocket;

        // private readonly JsonSerializer _serializer;

        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;

        private readonly Uri _addressUri;

        public event MessageArrivedDelegate MessageArrived;

        public event Action ConnectionClosed;
        public event Action<Exception> ConnectionError;

        public bool IsConnected => _webSocket.State == WebSocketState.Open;
        // private WebSocketMessageType WebSocketMessageType => WebSocketMessageType.Text;

        public delegate void MessageArrivedDelegate(string message);

        public ClientWebSocketWrapper(Uri addressUri) {
            _webSocket = new ClientWebSocket();
            _webSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(10);
            //_serializer = new JsonSerializer {
            //    NullValueHandling = NullValueHandling.Ignore
            //};
            _addressUri = addressUri;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        //public Task SendAsync(object command) {
        //    var messageToSend = GetMessageInBytes(command);
        //    return _webSocket.SendAsync(messageToSend, WebSocketMessageType, true, _cancellationToken);
        //}

        public void Dispose() {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        //private ArraySegment<byte> GetMessageInBytes(object command) {
        //    var writer = new StringWriter();
        //    var tokenWriter = new JsonTextWriter(writer) { Formatting = Formatting.None };
        //    _serializer.Serialize(tokenWriter, command);
        //    var formatted = writer.ToString();
        //    var bytes = Encoding.UTF8.GetBytes(formatted);
        //    return new ArraySegment<byte>(bytes);
        //}

        public async Task ConnectAsync() {
            try {
                await _webSocket.ConnectAsync(_addressUri, _cancellationToken).ConfigureAwait(false);
                Task task = Task.Run(this.RunAsync, _cancellationToken);
            } catch (Exception ex) {
                RaiseConnectionError(ex);
                RaiseConnectionClosed();
            }
        }

        private async Task RunAsync() {
            try {
                const int maxMessageSize = 2048;
                var receivedDataBuffer = new ArraySegment<byte>(new byte[maxMessageSize]);
                var memoryStream = new MemoryStream();
                while (IsConnected && !_cancellationToken.IsCancellationRequested) {
                    var webSocketReceiveResult = await ReadMessage(receivedDataBuffer, memoryStream).ConfigureAwait(false);
                    if (webSocketReceiveResult.MessageType != WebSocketMessageType.Close) {
                        memoryStream.Position = 0;
                        OnNewMessage(memoryStream);
                    }
                    memoryStream.Position = 0;
                    memoryStream.SetLength(0);
                }
            } catch (Exception ex) {
                if (!(ex is OperationCanceledException) ||
                    !_cancellationToken.IsCancellationRequested) {
                    RaiseConnectionError(ex);
                }
            }
            if (_webSocket.State != WebSocketState.CloseReceived && _webSocket.State != WebSocketState.Closed) {
                await CloseAsync().ConfigureAwait(false);
            }
            RaiseConnectionClosed();
        }

        public async Task CloseAsync() {
            try {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, String.Empty, CancellationToken.None).ConfigureAwait(false);
            } catch (Exception) {}
        }

        private async Task<WebSocketReceiveResult> ReadMessage(ArraySegment<byte> receivedDataBuffer, MemoryStream memoryStream) {
            WebSocketReceiveResult webSocketReceiveResult;
            do {
                webSocketReceiveResult = await _webSocket.ReceiveAsync(receivedDataBuffer, _cancellationToken).ConfigureAwait(false);
                await memoryStream.WriteAsync(receivedDataBuffer.Array, receivedDataBuffer.Offset, webSocketReceiveResult.Count, _cancellationToken).ConfigureAwait(false);
            }
            while (!webSocketReceiveResult.EndOfMessage);
            return webSocketReceiveResult;
        }

        private void OnNewMessage(MemoryStream payloadData) {
            string message = new StreamReader(payloadData, Encoding.ASCII).ReadToEnd();
            RaiseMessageArrived(message);
        }

        protected virtual void RaiseMessageArrived(string message) {
            MessageArrived?.Invoke(message);
        }

        protected virtual void RaiseConnectionClosed() {
            ConnectionClosed?.Invoke();
        }

        protected virtual void RaiseConnectionError(Exception ex) {
            ConnectionError?.Invoke(ex);
        }

    }

}