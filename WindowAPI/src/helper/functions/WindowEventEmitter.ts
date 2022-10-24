import {WindowEventName} from "../PackageTypes.js";
import {WindowEvent} from "../EventData.js";

export type WindowEventListener = (event: WindowEvent) => void;

export abstract class WindowEventEmitter {
    
    protected _events: {[name: string]: WindowEventListener[]} = {};
    
    public abstract on(name: WindowEventName, listener: WindowEventListener): void;
    public abstract removeListener(name: WindowEventName, listenerToRemove: WindowEventListener): void;
    public abstract emit(name: WindowEventName, data: WindowEvent): void;
    
}