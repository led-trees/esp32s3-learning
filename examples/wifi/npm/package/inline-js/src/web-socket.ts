export class TestWebSocket {
    private __socket: WebSocket;

    constructor(url: string = "ws://localhost:8999", onConnect?: () => void) {
        this.__socket = new WebSocket(url);
        if (onConnect) this.__socket.onopen = onConnect;

        this.__socket.onclose = function(event) {
            if (event.wasClean) {
              console.log(`[close] Соединение закрыто чисто, код=${event.code} причина=${event.reason}`);
            } else {
              // например, сервер убил процесс или сеть недоступна
              // обычно в этом случае event.code 1006
              throw new Error('[close] Соединение прервано');
            }
          };
          
          this.__socket.onerror = function(error) {
            alert(`[error]`);
          };
    }

    send(message: string) {
        this.__socket.send(message);
    }

    close() {
        this.__socket.close();
    }

    onMessage(handler: (message: string) => void) {
         this.__socket.onmessage = (e) => {
            handler(e.data);
        }
    }
}