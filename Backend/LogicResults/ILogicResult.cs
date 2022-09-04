namespace Backend.LogicResults {
    public interface ILogicResult {
        LogicResultState State { get; set; }

        string Message { get; set; }

        bool IsSuccessful { get; }
    }

    public interface ILogicResult<T> {
        LogicResultState State { get; set; }

        T Data { get; set; }

        string Message { get; set; }

        bool IsSuccessful { get; }
    }
}