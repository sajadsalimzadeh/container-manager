using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos
{
    public class OperationResult
    {
        public bool Success { get; set; } = true;
        public int Code { get; set; } = 0;
        public string Message { get; set; }

        public OperationResult()
        {

        }

        public OperationResult(bool success)
        {
            Success = success;
        }

        public OperationResult(bool success, string message) : this(success)
        {
            Message = message;
        }

        public static OperationResult Succeed()
        {
            return new OperationResult();
        }

        public static OperationResult Failed(string message)
        {
            return new OperationResult()
            {
                Success = false,
                Message = message
            };
        }
    }
    public class OperationResult<T> : OperationResult
    {
        public T Data { get; set; }

        public OperationResult()
        {

        }
        public OperationResult(bool success) : base(success)
        {
        }

        public OperationResult(bool success, string message) : base(success, message)
        {
        }

        public OperationResult(T data)
        {
            Data = data;
        }
        public static OperationResult<T> Succeed(T data)
        {
            return new OperationResult<T>(data);
        }
        public new static OperationResult<T> Succeed()
        {
            return new OperationResult<T>();
        }
        public new static OperationResult<T> Failed(string message)
        {
            return new OperationResult<T>()
            {
                Success = false,
                Message = message
            };
        }
    }
}
