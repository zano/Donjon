namespace Donjon {
    class Result {
        public bool IsAction { get; set; }
        public string Message { get; set; }
        public bool EndsTurn { get; set; }

        private Result(string message, bool isAction, bool endsTurn = false) {
            IsAction = isAction;
            Message = message;
            EndsTurn = endsTurn;
        }

        public static Result NoAction(string message = null) => new Result(message, isAction: false);
        public static Result Action(string message = null) => new Result(message, isAction: true);
        public static Result LastAction(string message = null) => new Result(message, isAction: true, endsTurn: true);
    }
}