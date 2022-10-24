import {WindowEventName} from "./PackageTypes.js";

export interface WindowEvent {
    type: WindowEventName;
    data: any;
}