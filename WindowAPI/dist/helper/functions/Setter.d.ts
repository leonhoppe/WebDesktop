export interface Setters {
    title(value: string): void;
    size(value: {
        width: number;
        height: number;
    }): void;
    position(value: {
        x: number;
        y: number;
    }): void;
    maximized(value: boolean): void;
    minimized(value: boolean): void;
    draggable(value: boolean): void;
    resizable(value: boolean): void;
}
