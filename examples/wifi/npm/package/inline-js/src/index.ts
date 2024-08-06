import { TestWebSocket } from "./web-socket";
import { DOM } from "@brandup/ui-dom";
import "./styles.less";

console.log("Hello from JS");
export function Test() { console.log("Hello from inline-js") }

//--WEBSOCKETS--//
const socket = new TestWebSocket();
socket.onMessage((message) => console.log("Server say: " + message));

const sayHelloBtn = DOM.tag("button", null, "Say hello to server");
document.body.appendChild(sayHelloBtn);
sayHelloBtn.addEventListener('click', () => {
    socket.send("Hello from client")
})