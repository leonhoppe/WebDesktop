import {ChangeDetectorRef, Component, OnInit, ViewChild, ViewContainerRef} from '@angular/core';
import {TaskbarIcon} from "./taskbar-icon/taskbar-icon.component";
import {WindowWrapper} from "../../components/window-wrapper/window-wrapper.component";
import {Notification} from "webdesktop_windowapi/dist/helper/Notification";

export interface IconType {
  uuid: number;
  icon: string;

  program: ProgramArgs;
}

export interface ProgramArgs {
  identifier: string;
  name: string;
  handlerUrl: string;

  permission?: string[];
  args?: string[];
  openFiles?: string[];
}

export const programs: {[programUUID: string]: ProgramArgs} = {
  ['tester']: {
    identifier: 'tester',
    name: 'WindowAPI Tester',
    handlerUrl: 'http://localhost:8080/',
  }
}

@Component({
  selector: 'app-desktop',
  templateUrl: './desktop.component.html',
  styleUrls: ['./desktop.component.scss']
})
export class DesktopComponent implements OnInit {
  @ViewChild('windows', {read: ViewContainerRef}) windowsRef: ViewContainerRef;
  @ViewChild('taskbarIcons', {read: ViewContainerRef}) taskbarIconsRef: ViewContainerRef;
  public static instance: DesktopComponent;
  public static focusedWindow: WindowWrapper;

  time: string;
  date: string;

  private taskbarIcons: {icon: TaskbarIcon, removeOnClose: boolean}[] = [];

  constructor(public cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    DesktopComponent.instance = this;
    setInterval(() => {
      const dt = new Date();
      this.time = dt.toLocaleTimeString();
      this.date = dt.toLocaleDateString();
    }, 200);

    document.addEventListener('mousemove', this.mouseMove);

    setTimeout(() => {
      this.addTaskbarIcon(programs['tester']);
    });
  }

  public openProgram(programUUID: string, args?: string[], asPopup?: boolean): number {
    const program = programs[programUUID];
    const exists = this.getTaskbarIcon(programUUID) != undefined;

    if (!exists)
      this.addTaskbarIcon(program, true);

    return this.getTaskbarIcon(programUUID).icon.openProgram(args, asPopup);
  }

  public addTaskbarIcon(program: ProgramArgs, removeOnClose: boolean = false, index?: number) {
    const type: IconType = {
      uuid: 1,
      icon: program.handlerUrl + 'favicon.ico',

      program: program
    };

    const icon = this.taskbarIconsRef.createComponent(TaskbarIcon, {index});
    icon.instance.initialize(type);
    this.taskbarIcons.push({icon: icon.instance, removeOnClose});
  }

  public removeTaskbarIcon(programUUID: string) {
    const icon = this.getTaskbarIcon(programUUID)?.icon;
    if (icon == undefined) return;
    if (icon.windows.length > 0) return;
    icon.object.nativeElement.parentElement.removeChild(icon.object.nativeElement);
    this.taskbarIcons.slice(this.taskbarIcons.indexOf(this.getTaskbarIcon(programUUID)), 1);
  }

  public getTaskbarIcon(programUUID: string): {icon: TaskbarIcon, removeOnClose: boolean} {
    for (let icon of this.taskbarIcons) {
      if (icon.icon.type.program.identifier == programUUID)
        return icon;
    }
    return undefined;
  }

  public onAllWindowsClosed(programUUID: string) {
    const icon = this.getTaskbarIcon(programUUID);
    if (icon?.removeOnClose)
      this.removeTaskbarIcon(programUUID);
  }

  public unfocusAll(caller?: WindowWrapper) {
    for (let icon of this.taskbarIcons) {
      for (let window of icon.icon.windows) {
        if (window == caller || window == undefined) continue;
        window.unfocus();
      }
    }
  }

  public getWindow(uuid: number): WindowWrapper {
    for (let icon of this.taskbarIcons) {
      for (let window of icon.icon.windows) {
        if (window.uuid == uuid) return window;
      }
    }

    return undefined;
  }

  public generateWindowUUID(): number {
    let uuid = 0;
    while (this.getWindow(uuid) != undefined) {
      uuid++;
    }
    return uuid;
  }

  public sendNotification(notification: Notification) {
    console.log(notification);
  }

  public mouseMove(event: MouseEvent) {
    DesktopComponent.focusedWindow?.onMove(event);
  }

  public static get windowContainer(): ViewContainerRef {
    return this.instance.windowsRef;
  }
}
