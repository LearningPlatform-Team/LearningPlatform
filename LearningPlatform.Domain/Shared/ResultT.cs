using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningPlatform.Domain.Shared
{
    public class ResultT<T> : Result
    {
        public T? Data { get; set; }

        public ResultT(bool isSuccess, T? data = default, ErrorType errorType = ErrorType.None, string errorMessage = "") : base(isSuccess, errorType, errorMessage)
        {
            Data = data;
        }
        public static ResultT<T> Success(T data) => new ResultT<T>(true, data);
        public static new ResultT<T> Failure(string errorMessage, ErrorType errorType = ErrorType.Failure) => new ResultT<T>(false, default, errorType, errorMessage);
    
    }
}
