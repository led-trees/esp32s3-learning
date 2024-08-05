import testChunk from "./chunk";
import { DOM } from "@brandup/ui-dom";
import "./styles.less";

const testElem = DOM.tag("h1", null, "Hello from @brandup/ui-dom");
document.body.appendChild(testElem);

console.log("Hello from JS");
testChunk();

export function Test() { console.log("Hello from inline-js") }