namespace Backend.LogicResults {
    internal class LogicResult : ILogicResult {
        public LogicResultState State { get; set; }

        public string Message { get; set; }

        public bool IsSuccessful => State == LogicResultState.Ok;

        public static LogicResult Ok() {
            return new LogicResult() {
                State = LogicResultState.Ok
            };
        }

        public static LogicResult BadRequest() {
            return new LogicResult() {
                State = LogicResultState.BadRequest
            };
        }

        public static LogicResult BadRequest(string message) {
            return new LogicResult() {
                State = LogicResultState.BadRequest,
                Message = message
            };
        }

        public static LogicResult Forbidden() {
            return new LogicResult() {
                State = LogicResultState.Forbidden
            };
        }

        public static LogicResult Forbidden(string message) {
            return new LogicResult() {
                State = LogicResultState.Forbidden,
                Message = message
            };
        }

        public static LogicResult NotFound() {
            return new LogicResult() {
                State = LogicResultState.NotFound
            };
        }

        public static LogicResult NotFound(string message) {
            return new LogicResult() {
                State = LogicResultState.NotFound,
                Message = message
            };
        }

        public static LogicResult Conflict() {
            return new LogicResult() {
                State = LogicResultState.Conflict
            };
        }

        public static LogicResult Conflict(string message) {
            return new LogicResult() {
                State = LogicResultState.Conflict,
                Message = message
            };
        }

        public static LogicResult Forward(LogicResult result) {
            return new LogicResult() {
                State = result.State,
                Message = result.Message
            };
        }

        public static LogicResult Forward<T>(ILogicResult<T> result) {
            return new LogicResult() {
                State = result.State,
                Message = result.Message
            };
        }
    }

    internal class LogicResult<T> : ILogicResult<T> {
        public LogicResultState State { get; set; }

        public T Data { get; set; }

        public string Message { get; set; }

        public bool IsSuccessful => State == LogicResultState.Ok;

        public static LogicResult<T> Ok() {
            return new LogicResult<T>() {
                State = LogicResultState.Ok
            };
        }

        public static LogicResult<T> Ok(T result) {
            return new LogicResult<T>() {
                State = LogicResultState.Ok,
                Data = result
            };
        }

        public static LogicResult<T> BadRequest() {
            return new LogicResult<T>() {
                State = LogicResultState.BadRequest
            };
        }

        public static LogicResult<T> BadRequest(string message) {
            return new LogicResult<T>() {
                State = LogicResultState.BadRequest,
                Message = message
            };
        }

        public static LogicResult<T> Forbidden() {
            return new LogicResult<T>() {
                State = LogicResultState.Forbidden
            };
        }

        public static LogicResult<T> Forbidden(string message) {
            return new LogicResult<T>() {
                State = LogicResultState.Forbidden,
                Message = message
            };
        }

        public static LogicResult<T> NotFound() {
            return new LogicResult<T>() {
                State = LogicResultState.NotFound
            };
        }

        public static LogicResult<T> NotFound(string message) {
            return new LogicResult<T>() {
                State = LogicResultState.NotFound,
                Message = message
            };
        }

        public static LogicResult<T> Conflict() {
            return new LogicResult<T>() {
                State = LogicResultState.Conflict
            };
        }

        public static LogicResult<T> Conflict(string message) {
            return new LogicResult<T>() {
                State = LogicResultState.Conflict,
                Message = message
            };
        }

        public static LogicResult<T> Forward(ILogicResult result) {
            return new LogicResult<T>() {
                State = result.State,
                Message = result.Message
            };
        }

        public static LogicResult<T> Forward<T2>(ILogicResult<T2> result) {
            return new LogicResult<T>() {
                State = result.State,
                Message = result.Message
            };
        }
    }
}