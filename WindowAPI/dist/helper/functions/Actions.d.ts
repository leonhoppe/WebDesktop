import { Notification } from "../Notification.js";
import { PopupData } from "../PopupData.js";
export interface Actions {
    openWindow(identifier: string, args?: string[], asPopup?: boolean): Promise<number>;
    closeWindow(uuid: number): Promise<boolean>;
    sendDataToWindow(uuid: number, data: any): Promise<boolean>;
    close(): Promise<void>;
    focus(): Promise<void>;
    notification(notification: Notification): Promise<void>;
    openPopup<T>(data: PopupData): Promise<T>;
}
