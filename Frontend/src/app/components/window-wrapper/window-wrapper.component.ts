import {Component, ElementRef, ViewChild} from '@angular/core';
import {DesktopComponent, ProgramArgs} from "../../sites/desktop/desktop.component";
import {TaskbarIcon} from "../../sites/desktop/taskbar-icon/taskbar-icon.component";

@Component({
  selector: 'app-window-wrapper',
  templateUrl: './window-wrapper.component.html',
  styleUrls: ['./window-wrapper.component.scss']
})
export class WindowWrapper {
  @ViewChild('wrapper') wrapper: ElementRef;
  @ViewChild('content') content: ElementRef;
  public program: ProgramArgs;
  public uuid: number;
  public taskbar: TaskbarIcon;
  public dragHandler: DragHandler;
  public resizeHandler: ResizeHandler;
  public title: string;
  public focused: boolean;
  public contentLoaded: boolean = false;

  public resizeIcon: string = "fullscreen";
  public maximized: boolean = false;
  private lastPos: { left: string, top: string, width: string, height: string };

  constructor(private object: ElementRef) {
    this.uuid = DesktopComponent.instance.generateWindowUUID();
  }

  public initialize(taskbar: TaskbarIcon) {
    this.title = this.program.name;
    this.dragHandler = new DragHandler(this.wrapper.nativeElement, this);
    this.resizeHandler = new ResizeHandler(this.wrapper.nativeElement, this);
    this.taskbar = taskbar;
    this.content.nativeElement.src = this.program.handlerUrl;
    this.focus();
  }

  public close() {
    this.object.nativeElement.parentElement.removeChild(this.object.nativeElement);
    this.taskbar.onClose(this);
  }

  public toggleMinimized() {
    const minimized = this.object.nativeElement.style.display == 'none';

    if (minimized) {
      this.object.nativeElement.style.display = 'block';
      this.taskbar.setIndicator('wide');
    } else {
      this.object.nativeElement.style.display = 'none';
      this.taskbar.setIndicator('dot');
    }
  }

  public toggleMaximized() {
    const wrapper = this.object.nativeElement.children.item(0) as HTMLElement;

    if (this.lastPos == undefined) {
      this.lastPos = {
        width: wrapper.style.width,
        height: wrapper.style.height,
        left: wrapper.style.left,
        top: wrapper.style.top
      };

      wrapper.style.width = '100%';
      wrapper.style.height = '100%';
      wrapper.style.left = '0';
      wrapper.style.top = '0';

      this.resizeIcon = "fullscreen_exit";
      this.maximized = true;
    } else {
      wrapper.style.width = this.lastPos.width;
      wrapper.style.height = this.lastPos.height;
      wrapper.style.left = this.lastPos.left;
      wrapper.style.top = this.lastPos.top;
      delete this.lastPos;

      this.resizeIcon = "fullscreen";
      this.maximized = false;
    }
  }

  public focus() {
    if (this.focused) return;
    DesktopComponent.instance.unfocusAll(this);

    this.object.nativeElement.style.display = 'block';
    this.taskbar.setIndicator('wide');

    this.wrapper.nativeElement.style.zIndex = '7';

    DesktopComponent.focusedWindow = this;
    this.focused = true;
  }

  public unfocus() {
    this.taskbar.setIndicator('dot');

    this.wrapper.nativeElement.style.zIndex = '5';

    this.focused = false;
  }

  public applyContentListeners(content: HTMLIFrameElement) {
    content.contentDocument.addEventListener('mousemove', this.onMove.bind(this));
    content.contentDocument.addEventListener('mousedown', this.focus.bind(this));
    content.contentDocument.addEventListener('mouseup', this.resizeHandler?.windowResizeStop.bind(this.resizeHandler));

    this.contentLoaded = true;
  }

  public onMove(event: MouseEvent) {
    this.dragHandler?.windowDrag(event);
    this.resizeHandler?.windowResize(event);
  }

}

class DragHandler {
  private offsetX: number;
  private offsetY: number;
  public dragging: boolean;
  private origTransitions: string;

  public constructor(private object: HTMLElement, private wrapper: WindowWrapper) {
  }

  public windowDrag(event: MouseEvent): void {
    if (!this.dragging) return;
    if (!this.wrapper.contentLoaded) return;
    const x = event.clientX - this.offsetX;
    const y = event.clientY - this.offsetY;
    this.object.style.left = x + 'px';
    this.object.style.top = y + 'px';
  }

  public windowDragStart(event: MouseEvent): void {
    if (this.wrapper.maximized) return;
    if (this.wrapper.resizeHandler?.resizing) return;

    if (this.origTransitions == undefined)
      this.origTransitions = this.object.style.transition;
    this.object.style.transition = 'none';

    this.offsetX = event.clientX - this.object.offsetLeft;
    this.offsetY = event.clientY - this.object.offsetTop;
    this.dragging = true;
  }

  public windowDragStop(): void {
    if (!this.dragging) return;
    this.object.style.transition = this.origTransitions;
    delete this.origTransitions;
    this.dragging = false;
  }

  public get isDragging(): boolean {
    return this.dragging;
  }
}

class ResizeHandler {
  public minSize: { width: number, height: number } = {width: 800, height: 600};
  private readonly resizingArea: number = 10;
  public resizing: boolean = false;
  private lastResizeManager: string;

  public constructor(private object: HTMLElement, private wrapper: WindowWrapper) {
  }

  public windowResizeStart(event: MouseEvent): void {
    if (this.wrapper.maximized) return;
    if (!this.wrapper.contentLoaded) return;
    if (this.wrapper.dragHandler?.dragging) return;
    if (!this.isHoverBorder(event)) return;

    this.object.classList.add("unselectable");
    this.lastResizeManager = this.isHoverBorder(event);

    this.resizing = true;
  }

  public windowResizeStop(): void {
    if (!this.resizing) return;
    this.resizing = false;
    delete this.lastResizeManager;
    this.object.classList.remove("unselectable");
  }

  public windowResize(event: MouseEvent) {
    if (!this.wrapper.focused) return;
    if (this.wrapper.maximized) return;

    if (!this.resizing) {
      const c = this.isHoverBorder(event);

      if (c) {
        this.object.style.cursor = c + "-resize";
        this.lastResizeManager = c;
      } else
        this.object.style.cursor = "auto";
    }

    if (this.resizing) this.handleResizing(event);
  }

  private handleResizing(event: MouseEvent): void {
    let newDimensions: {x: number, y: number, width: number, height: number} = {
      x: undefined,
      y: undefined,
      width: undefined,
      height: undefined
    };

    if (this.lastResizeManager.includes("n")) { //TOP
      newDimensions.y = this.object.offsetTop + event.movementY;
      newDimensions.height = this.object.offsetHeight - event.movementY;
    } else if (this.lastResizeManager.includes("s")) { //BOTTOM
      //this.object.style.height = (this.resizeOrigin.height + event.movementY) + "px";
      newDimensions.height = this.object.offsetHeight + event.movementY;
    }
    if (this.lastResizeManager.includes("w")) { //LEFT
      /*this.object.style.left = (this.resizeOrigin.x + event.movementX) + "px";
      this.object.style.width = (this.resizeOrigin.width - event.movementX) + "px";*/
      newDimensions.x = this.object.offsetLeft + event.movementX;
      newDimensions.width = this.object.offsetWidth - event.movementX;
    } else if (this.lastResizeManager.includes("e")) { //RIGHT
      //this.object.style.width = (this.resizeOrigin.width + event.movementX) + "px";
      newDimensions.width = this.object.offsetWidth + event.movementX;
    }

    if (newDimensions.width < this.minSize.width) {
      newDimensions.width = this.minSize.width;
      this.windowResizeStop();
    }
    if (newDimensions.height < this.minSize.height) {
      newDimensions.height = this.minSize.height;
      this.windowResizeStop();
    }

    if (newDimensions.x) this.object.style.left = newDimensions.x + 'px';
    if (newDimensions.y) this.object.style.top = newDimensions.y + 'px';
    if (newDimensions.width) this.object.style.width = newDimensions.width + 'px';
    if (newDimensions.height) this.object.style.height = newDimensions.height + 'px';
  }

  private isHoverBorder(event: MouseEvent): string {
    const delta = this.resizingArea;                      // the thickness of the hovered border area

    const rect = this.object.getBoundingClientRect();
    const x = event.clientX - rect.left,   // the relative mouse position to the element
      y = event.clientY - rect.top,        // ...
      w = rect.right - rect.left,          // width of the element
      h = rect.bottom - rect.top;          // height of the element

    let c = "";                            // which cursor to use
    if (y < delta) c += "n";                // north
    else if (y > h - delta) c += "s";      // south
    if (x < delta) c += "w";                // west
    else if (x > w - delta) c += "e";       // east

    return c;
  }
}
