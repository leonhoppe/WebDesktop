import { WindowEventEmitter } from "./helper/functions/WindowEventEmitter.js";
export class WindowAPI {
    constructor() {
        this.get = {
            title: this.getMethod('title'),
            size: this.getMethod('size'),
            position: this.getMethod('size'),
            maximized: this.getMethod('maximized'),
            minimized: this.getMethod('minimized'),
            draggable: this.getMethod('draggable'),
            resizable: this.getMethod('resizable'),
        };
        this.set = {
            title: this.setMethod('title'),
            size: this.setMethod('size'),
            position: this.setMethod('position'),
            maximized: this.setMethod('maximized'),
            minimized: this.setMethod('minimized'),
            draggable: this.setMethod('draggable'),
            resizable: this.setMethod('resizable'),
        };
        this.action = {
            openWindow: this.openWindow.bind(this),
            closeWindow: this.closeWindow.bind(this),
            sendDataToWindow: this.sendDataToWindow.bind(this),
            close: this.close.bind(this),
            focus: this.focus.bind(this),
            notification: this.notification.bind(this),
            openPopup: this.openPopup.bind(this)
        };
        this.event = new class extends WindowEventEmitter {
            emit(name, data) {
                if (!this._events[name]) {
                    throw new Error(`Can't emit an event. Event "${name}" doesn't exits.`);
                }
                const fire = (callback) => callback(data);
                this._events[name].forEach(fire);
            }
            on(name, listener) {
                if (!this._events[name]) {
                    this._events[name] = [];
                }
                this._events[name].push(listener);
            }
            removeListener(name, listenerToRemove) {
                if (!this._events[name]) {
                    throw new Error(`Can't remove a listener. Event "${name}" doesn't exits.`);
                }
                const filter = (listener) => listener !== listenerToRemove;
                this._events[name] = this._events[name].filter(filter);
            }
        };
        window.addEventListener('message', (event) => {
            const data = event.data;
            if (this.currentEventListener !== undefined && data.method !== "event") {
                this.currentEventListener(data);
                delete this.currentEventListener;
            }
            if (data.method === 'event') {
                const eventData = data.content;
                this.event.emit(data.event, eventData);
            }
        }, false);
    }
    sendRequest(data) {
        parent.postMessage(data, '*');
    }
    getMethod(variable) {
        return () => new Promise(resolve => {
            this.currentEventListener = (data) => {
                resolve(data.content);
            };
            this.sendRequest({ method: 'get', variable });
        });
    }
    setMethod(variable) {
        return (content) => {
            this.sendRequest({ method: "set", variable, content });
        };
    }
    openWindow(identifier, args, asPopup) {
        return new Promise(resolve => {
            this.currentEventListener = (data) => {
                resolve(data.content);
            };
            this.sendRequest({ method: 'action', action: 'openWindow', content: { identifier, args, asPopup } });
        });
    }
    closeWindow(uuid) {
        return new Promise(resolve => {
            this.currentEventListener = (data) => {
                resolve(data.content);
            };
            this.sendRequest({ method: 'action', action: 'closeWindow', content: { uuid } });
        });
    }
    sendDataToWindow(uuid, data) {
        return new Promise(resolve => {
            this.currentEventListener = (data) => {
                resolve(data.content);
            };
            this.sendRequest({ method: 'action', action: 'messageWindow', content: { uuid, data } });
        });
    }
    close() {
        return new Promise(resolve => {
            this.currentEventListener = () => resolve();
            this.sendRequest({ method: 'action', action: 'closeSelf' });
        });
    }
    focus() {
        return new Promise(resolve => {
            this.currentEventListener = () => resolve();
            this.sendRequest({ method: 'action', action: 'focus' });
        });
    }
    notification(notification) {
        return new Promise(resolve => {
            this.currentEventListener = () => resolve();
            this.sendRequest({ method: 'action', action: 'notification', content: notification });
        });
    }
    openPopup(data) {
        return new Promise(resolve => {
            this.currentEventListener = (data) => {
                resolve(data.content);
            };
            this.sendRequest({ method: 'action', action: 'popup', content: data });
        });
    }
}
//# sourceMappingURL=WindowAPI.js.map