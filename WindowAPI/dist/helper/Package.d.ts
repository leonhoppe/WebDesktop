import { PackageActionType, PackageMethod, PackageVariable, WindowEventName } from "./PackageTypes.js";
export interface Package {
    method: PackageMethod;
    variable?: PackageVariable;
    action?: PackageActionType;
    event?: WindowEventName;
    content?: any;
}
