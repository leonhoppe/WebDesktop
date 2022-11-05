import {Component, ElementRef, ViewChild} from '@angular/core';
import {DesktopComponent, IconType} from "../desktop.component";
import {WindowWrapper} from "../../../components/window-wrapper/window-wrapper.component";

@Component({
  selector: 'app-taskbar-icon',
  templateUrl: './taskbar-icon.component.html',
  styleUrls: ['./taskbar-icon.component.scss']
})
export class TaskbarIcon {
  @ViewChild('indicator') indicator: ElementRef;
  @ViewChild('instances') instances: ElementRef;

  public type: IconType;
  public windows: WindowWrapper[] = [];
  public instancesOpen: boolean = false;

  constructor(public object: ElementRef) { }

  public initialize(type: IconType) {
    this.type = type;
  }

  public openProgram(args?: string[], asPopup: boolean = false): number {
    const window = DesktopComponent.windowContainer.createComponent(WindowWrapper);
    window.instance.program = this.type.program;
    DesktopComponent.instance.cdr.detectChanges();
    window.instance.initialize(this, args || [], asPopup);

    this.windows.push(window.instance);
    this.setIndicator('wide');
    return window.instance.uuid;
  }

  public onTaskbarClick(event: MouseEvent) {
    if (this.instancesOpen) return;

    if (this.windows.length == 0 || event.shiftKey) {
      this.openProgram();
      return;
    }

    if (this.windows.length == 1) {
      this.windows[0].toggleMinimized();
      return;
    }

    this.instances.nativeElement.style.visibility = 'visible';
    this.instances.nativeElement.style.opacity = '1';
    this.instancesOpen = true;
  }

  public updateInstances() {
    if (this.windows.length <= 1) {
      this.instances.nativeElement.style.visibility = 'hidden';
      this.instances.nativeElement.style.opacity = '0';
      this.instancesOpen = false;
    }
  }

  public setIndicator(mode: 'wide' | 'dot' | 'hidden') {
    switch (mode) {
      case "hidden":
        this.indicator.nativeElement.style.opacity = '0';
        this.indicator.nativeElement.style.width = '0';
        break;

      case "dot":
        this.indicator.nativeElement.style.opacity = '1';
        this.indicator.nativeElement.style.width = 'var(--dot)';
        break;

      case "wide":
        this.indicator.nativeElement.style.opacity = '1';
        this.indicator.nativeElement.style.width = 'var(--wide)';
        break;
    }
  }

  public onClose(window: WindowWrapper) {
    this.windows.splice(this.windows.indexOf(window), 1);

    if (this.windows.length > 0)
      this.setIndicator('dot');
    else
      this.setIndicator('hidden');

    if (this.windows.length <= 0)
      DesktopComponent.instance.onAllWindowsClosed(this.type.program.identifier);
  }

}
