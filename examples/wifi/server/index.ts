const express = require("express");
import * as http from 'http';
import * as WebSocket from 'ws';

const app = express();

//initialize a simple http server
const server = http.createServer(app);

//initialize the WebSocket server instance
const wss = new WebSocket.Server({ server });

wss.on('connection', (ws: WebSocket) => {
    ws.on('message', (message: string) => {
        console.log('received: %s', message);
        // Переотправляем сообщение всем подключенным клиентам
        wss.clients.forEach(client => client.send(`${message}`))
    });

    ws.send('Hi there, I am a WebSocket server');
});

const PORT  = process.env.PORT || 8999;

//start our server
server.listen(PORT, () => {
    console.log(`Server started on port ${PORT}`);
});