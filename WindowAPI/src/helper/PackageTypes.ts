export type PackageMethod = "get" | "set" | "event" | "action";
export type PackageVariable = "title" | "size" | "position" | "maximized" | "minimized" | "draggable" | "resizable" | "uuid";
export type PackageActionType = "openWindow" | "closeWindow" | "closeSelf" | "focus" | "messageWindow" | "notification" | "popup";
export type WindowEventName = "resize" | "move" | "openAsPopup" | "open" | "close";