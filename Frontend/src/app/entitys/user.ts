export interface User extends UserEditor {
  id: string;
  created: string;
}

export interface UserLogin {
  usernameOrEmail: string;
  password: string;
}

export interface UserEditor {
  firstName: string;
  lastName: string;
  email: string;
  username: string;
  password: string;
}
