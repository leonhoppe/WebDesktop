import { WindowEventName } from "../PackageTypes.js";
import { WindowEvent } from "../EventData.js";
export declare type WindowEventListener = (event: WindowEvent) => void;
export declare abstract class WindowEventEmitter {
    protected _events: {
        [name: string]: WindowEventListener[];
    };
    abstract on(name: WindowEventName, listener: WindowEventListener): void;
    abstract removeListener(name: WindowEventName, listenerToRemove: WindowEventListener): void;
    abstract emit(name: WindowEventName, data: WindowEvent): void;
}
