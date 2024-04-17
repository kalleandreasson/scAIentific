using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatGPTAPI.Services
{
    public class OperationResult<T>
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public T Data { get; set; }

    public static OperationResult<T> CreateSuccessful(T data) => new OperationResult<T> { Success = true, Data = data };
    public static OperationResult<T> CreateFailure(string errorMessage) => new OperationResult<T> { Success = false, ErrorMessage = errorMessage };
}

}