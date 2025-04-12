namespace BambooExhangeRateService.Helpers
{
    public class GenericResponseModel<TResult>
    {
        public int StatusCode = 500;
        public TResult Data { get; set; }
        public List<ErrorModel> Errors { get; set; }

        public void Success(TResult data, int statusCode = 200)
        {
            Data = data;
            Errors = null;
            StatusCode = statusCode;
        }

        public void InternalError()
        {
            Error("Internal error occured!", 500);
        }

        public void Error(ErrorModel errorModel, int statusCode)
        {
            if (Errors == null)
            {
                Errors = new List<ErrorModel>();
            }

            Errors.Add(errorModel);
            Data = default(TResult);
            StatusCode = statusCode;
        }

        public void Error(string message, int statusCode)
        {
            ErrorModel errorModel = new ErrorModel(message);
            Error(errorModel, statusCode);
        }

        public void Error(string message, string description, int statusCode)
        {
            ErrorModel errorModel = new ErrorModel(message, description);
            Error(errorModel, statusCode);
        }

        public void Error(string message, IEnumerable<string> descriptions, int statusCode)
        {
            ErrorModel errorModel = new ErrorModel(message, descriptions);
            Error(errorModel, statusCode);
        }
    }
    public class ErrorModel
    {
        public string Message { get; set; }

        public List<string> Descriptions { get; set; }

        public ErrorModel(string message)
        {
            Message = message;
        }

        public ErrorModel(string message, string description)
        {
            Message = message;
            Descriptions = new List<string> { description };
        }

        public ErrorModel(string message, IEnumerable<string> descriptions)
        {
            Message = message;
            Descriptions = descriptions.ToList();
        }

        public void AddDescription(string description)
        {
            if (Descriptions == null)
            {
                Descriptions = new List<string>();
            }

            Descriptions.Add(description);
        }

        public void AddDescription(IEnumerable<string> descriptions)
        {
            if (Descriptions == null)
            {
                Descriptions = new List<string>();
            }

            Descriptions.AddRange(descriptions);
        }
    }
}
