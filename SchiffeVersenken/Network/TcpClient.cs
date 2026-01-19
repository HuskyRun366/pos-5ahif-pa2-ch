using System.Net.Sockets;
using System.Text;

namespace SchiffeVersenken.Network
{
    /// <summary>
    /// TCP-Client für Netzwerkspiele.
    /// </summary>
    public class NetzwerkClient : IDisposable
    {
        private TcpClient? _client;
        private NetworkStream? _stream;
        private CancellationTokenSource? _cts;
        private bool _disposed;

        /// <summary>Callback für empfangene Nachrichten</summary>
        public Action<NetzwerkNachricht>? OnNachrichtEmpfangen { get; set; }

        /// <summary>Callback bei Verbindungsverlust</summary>
        public Action? OnVerbindungGetrennt { get; set; }

        /// <summary>True wenn verbunden</summary>
        public bool IstVerbunden => _client?.Connected ?? false;

        /// <summary>
        /// Verbindet zu einem Server.
        /// </summary>
        public async Task VerbindenAsync(string host, int port)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(host, port);
            _stream = _client.GetStream();

            // Starte Empfangs-Loop
            _cts = new CancellationTokenSource();
            _ = EmpfangsLoopAsync(_cts.Token);
        }

        /// <summary>
        /// Sendet eine Nachricht an den Server.
        /// </summary>
        public async Task SendenAsync(NetzwerkNachricht nachricht)
        {
            if (_stream == null || !IstVerbunden) return;

            string json = nachricht.ToJson() + "\n";
            byte[] data = Encoding.UTF8.GetBytes(json);

            try
            {
                await _stream.WriteAsync(data);
                await _stream.FlushAsync();
            }
            catch
            {
                OnVerbindungGetrennt?.Invoke();
            }
        }

        /// <summary>
        /// Empfangs-Loop: Identisch zur Server-Implementierung.
        /// </summary>
        private async Task EmpfangsLoopAsync(CancellationToken token)
        {
            var buffer = new byte[4096];
            var messageBuffer = new StringBuilder();

            try
            {
                while (!token.IsCancellationRequested && _stream != null)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, token);

                    if (bytesRead == 0)
                    {
                        OnVerbindungGetrennt?.Invoke();
                        break;
                    }

                    string received = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    messageBuffer.Append(received);

                    string content = messageBuffer.ToString();
                    int newlineIndex;

                    while ((newlineIndex = content.IndexOf('\n')) >= 0)
                    {
                        string jsonMessage = content.Substring(0, newlineIndex);
                        content = content.Substring(newlineIndex + 1);

                        var nachricht = NetzwerkNachricht.FromJson(jsonMessage);
                        if (nachricht != null)
                        {
                            OnNachrichtEmpfangen?.Invoke(nachricht);
                        }
                    }

                    messageBuffer.Clear();
                    messageBuffer.Append(content);
                }
            }
            catch (OperationCanceledException)
            {
                // Normales Beenden
            }
            catch
            {
                OnVerbindungGetrennt?.Invoke();
            }
        }

        /// <summary>
        /// Trennt die Verbindung.
        /// </summary>
        public void Trennen()
        {
            _cts?.Cancel();
            _stream?.Close();
            _client?.Close();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            Trennen();
            _cts?.Dispose();
            _stream?.Dispose();
            _client?.Dispose();
        }
    }
}
