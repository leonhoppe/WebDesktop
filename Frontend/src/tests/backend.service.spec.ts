import {BackendService, RequestTypes} from "../app/services/backend.service";
import {TestBed} from "@angular/core/testing";
import {HttpClientModule} from "@angular/common/http";

describe('BackendService', () => {
  let service : BackendService;

  beforeEach(() => {
    TestBed.configureTestingModule({imports: [HttpClientModule]})
    service = TestBed.inject(BackendService);
  })

  it('should connect to the backend', function (done) {
    service.testConnection().then(result => {
      expect(result).toBeTrue();
      done();
    })
  });
})
