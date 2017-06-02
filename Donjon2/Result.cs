namespace Donjon {
    class Result {
        public bool IsAction { get; set; }
        public string Message { get; set; }

        private Result(bool isAction, string message) {
            IsAction = isAction;
            Message = message;
        }

        public static Result Action(string message = null) => new Result(true, message);
        public static Result NoAction(string message = null) => new Result(false, message);
    }
}