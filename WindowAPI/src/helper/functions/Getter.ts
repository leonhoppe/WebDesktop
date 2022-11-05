export interface Getters {
    title(): Promise<string>;
    size(): Promise<{width: number, height: number}>;
    position(): Promise<{x: number, y: number}>;
    maximized(): Promise<boolean>;
    minimized(): Promise<boolean>;
    draggable(): Promise<boolean>;
    resizable(): Promise<boolean>;
    uuid(): Promise<number>;
}