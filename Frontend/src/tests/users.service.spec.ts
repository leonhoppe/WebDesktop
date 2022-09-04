import { TestBed } from '@angular/core/testing';

import {UserApi} from "../app/services/users.service";
import {BackendService} from "../app/services/backend.service";
import {HttpClientModule} from "@angular/common/http";

describe('UserApi', () => {
  let backend: BackendService;
  let service: UserApi;
  let userId: string;

  beforeAll((done) => {
    TestBed.configureTestingModule({imports: [HttpClientModule]});
    backend = TestBed.inject(BackendService);
    service = TestBed.inject(UserApi);

    service.register({
      username: "tester",
      password: "password",
      email: "test@test.com",
      firstName: "test",
      lastName: "test"
    }).then(success => {
      expect(success.success).toBeTrue();
      service.getAuthorizedUser().then(user => {
        expect(user).not.toBeUndefined();
        userId = user.id;
        done();
      })
    })
  }, 5000)

  beforeEach(() => {
    backend.setToken("474a0461-37ec-4b11-aefe-00c423d1511e");
  });

  it('should be create the service', () => {
    expect(service).toBeTruthy();
  });

  it('should show all users', (done) => {
    service.getUsers().then(users => {
      expect(users).not.toBeUndefined();
      done();
    })
  });

  it('should login with the given credentials', (done) => {
    service.login({
      usernameOrEmail: "tester",
      password: "password"
    }).then(result => {
      expect(result.success).toBeTrue();
      done();
    })
  });

  it('should delete all user sessions', (done) => {
    service.login({
      usernameOrEmail: "tester",
      password: "password"
    }).then(result => {
      expect(result.success).toBeTrue();
      service.logout(userId).then(success => {
        expect(success).toBeTrue();
        done();
      })
    })
  });

  it('should show the specified user', (done) => {
    service.getUser(userId).then(user => {
      expect(user).not.toBeUndefined();
      done();
    })
  });

  it('should edit the specified user', (done) => {
    service.editUser(userId, {
      username: "",
      password: "",
      email: "",
      firstName: "Test",
      lastName: "Test"
    }).then(result => {
      expect(result).toBeTrue();
      done();
    })
  });

  it('should show the permissions of the specified user', (done) => {
    service.getUserPermissions(userId).then(perms => {
      expect(perms).not.toBeUndefined();
      done();
    })
  });

  it('should add the permissions to the specified user', (done) => {
    service.addUserPermissions(userId, ["*"]).then(success => {
      expect(success).toBeTrue();
      done();
    })
  });

  it('should remove the permissions to the specified user', (done) => {
    service.removeUserPermissions(userId, ["*"]).then(success => {
      expect(success).toBeTrue();
      done();
    })
  });

  afterAll((done) => {
    service.deleteUser(userId).then(() => done());
  })
});
