import { Getters } from "./helper/functions/Getter.js";
import { Setters } from "./helper/functions/Setter.js";
import { WindowEventEmitter } from "./helper/functions/WindowEventEmitter.js";
import { Actions } from "./helper/functions/Actions.js";
export declare class WindowAPI {
    private currentEventListener;
    constructor();
    private sendRequest;
    private getMethod;
    private setMethod;
    get: Getters;
    set: Setters;
    action: Actions;
    event: WindowEventEmitter;
    private openWindow;
    private closeWindow;
    private sendDataToWindow;
    private close;
    private focus;
    private notification;
    private openPopup;
}
