import {Package} from "./helper/Package.js";
import {WindowEventName, PackageVariable} from "./helper/PackageTypes.js";
import {WindowEvent} from "./helper/EventData.js";
import {Getters} from "./helper/functions/Getter.js";
import {Setters} from "./helper/functions/Setter.js";
import {WindowEventEmitter, WindowEventListener} from "./helper/functions/WindowEventEmitter.js";
import {Actions} from "./helper/functions/Actions.js";
import {Notification} from "./helper/Notification.js";
import {PopupData} from "./helper/PopupData.js";

export class WindowAPI {
    private currentEventListener: (data: Package) => void;
    
    public constructor() {
        window.addEventListener('message', (event: MessageEvent) => {
            const data = event.data as Package;
            
            if (this.currentEventListener !== undefined && data.method !== "event") {
                this.currentEventListener(data);
                delete this.currentEventListener;
            }
            
            if (data.method === 'event') {
                const eventData = data.content as WindowEvent;
                this.event.emit(data.event, eventData);
            }
        }, false);
    }
    
    private sendRequest(data: Package): void {
        parent.postMessage(data, '*');
    }
    
    private getMethod<T>(variable: PackageVariable): () => Promise<T> {
        return () => new Promise<T>(resolve => {
            this.currentEventListener = (data) => {
                resolve(data.content as T);
            }
            
            this.sendRequest({method: 'get', variable});
        });
    }
    
    private setMethod<T>(variable: PackageVariable): (content: T) => void {
        return (content: T) => {
            this.sendRequest({method: "set", variable, content});
        }
    }
    
    public get: Getters = {
        title: this.getMethod<string>('title'),
        size: this.getMethod<{width: number, height: number}>('size'),
        position: this.getMethod<{x: number, y: number}>('size'),
        maximized: this.getMethod<boolean>('maximized'),
        minimized: this.getMethod<boolean>('minimized'),
        draggable: this.getMethod<boolean>('draggable'),
        resizable: this.getMethod<boolean>('resizable'),
    };
    
    public set: Setters = {
        title: this.setMethod<string>('title'),
        size: this.setMethod<{width: number, height: number}>('size'),
        position: this.setMethod<{x: number, y: number}>('position'),
        maximized: this.setMethod<boolean>('maximized'),
        minimized: this.setMethod<boolean>('minimized'),
        draggable: this.setMethod<boolean>('draggable'),
        resizable: this.setMethod<boolean>('resizable'),
    }

    public action: Actions = {
        openWindow: this.openWindow.bind(this),
        closeWindow: this.closeWindow.bind(this),
        sendDataToWindow: this.sendDataToWindow.bind(this),
        close: this.close.bind(this),
        focus: this.focus.bind(this),
        notification: this.notification.bind(this),
        openPopup: this.openPopup.bind(this)
    }
    
    public event: WindowEventEmitter = new class extends WindowEventEmitter {
        emit(name: WindowEventName, data: WindowEvent): void {
            if (!this._events[name]) {
                throw new Error(`Can't emit an event. Event "${name}" doesn't exits.`);
            }

            const fire = (callback) => callback(data);

            this._events[name].forEach(fire);
        }

        on(name: WindowEventName, listener: WindowEventListener): void {
            if (!this._events[name]) {
                this._events[name] = [];
            }

            this._events[name].push(listener);
        }

        removeListener(name: WindowEventName, listenerToRemove: WindowEventListener): void {
            if (!this._events[name]) {
                throw new Error(`Can't remove a listener. Event "${name}" doesn't exits.`);
            }
            
            const filter = (listener) => listener !== listenerToRemove;
            
            this._events[name] = this._events[name].filter(filter);
        }
    }

    private openWindow(identifier: string, args?: string[], asPopup?: boolean): Promise<number> {
        return new Promise<number>(resolve => {
            this.currentEventListener = (data) => {
                resolve(data.content as number);
            }
            
            this.sendRequest({method: 'action', action: 'openWindow', content: {identifier, args, asPopup}});
        });
    }

    private closeWindow(uuid: number): Promise<boolean> {
        return new Promise<boolean>(resolve => {
            this.currentEventListener = (data) => {
                resolve(data.content as boolean);
            }

            this.sendRequest({method: 'action', action: 'closeWindow', content: {uuid}});
        });
    }

    private sendDataToWindow(uuid: number, data: any): Promise<boolean> {
        return new Promise<boolean>(resolve => {
            this.currentEventListener = (data) => {
                resolve(data.content as boolean);
            }

            this.sendRequest({method: 'action', action: 'messageWindow', content: {uuid, data}});
        });
    }

    private close(): Promise<void> {
        return new Promise<void>(resolve => {
            this.currentEventListener = () => resolve();
            
            this.sendRequest({method: 'action', action: 'closeSelf'});
        });
    }

    private focus(): Promise<void> {
        return new Promise<void>(resolve => {
            this.currentEventListener = () => resolve();

            this.sendRequest({method: 'action', action: 'focus'});
        });
    }

    private notification(notification: Notification): Promise<void> {
        return new Promise<void>(resolve => {
            this.currentEventListener = () => resolve();

            this.sendRequest({method: 'action', action: 'notification', content: notification});
        });
    }

    private openPopup<T>(data: PopupData): Promise<T> {
        return new Promise<T>(resolve => {
            this.currentEventListener = (data) => {
                resolve(data.content as T);
            }

            this.sendRequest({method: 'action', action: 'popup', content: data});
        });
    }
    
}