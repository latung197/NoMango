using Worker.Application.Constants;

namespace Worker.Application.CustomModels
{
    public class ServiceResult
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
    public class ServiceResultSuccess : ServiceResult
    {
        public ServiceResultSuccess()
        {
            Code = CommonConstant.SUCCESS;
        }
        public ServiceResultSuccess(string msg)
        {
            Code = CommonConstant.SUCCESS;
            Message = msg;
        }
        public ServiceResultSuccess(string msg, object obj)
        {
            Code = CommonConstant.SUCCESS;
            Message = msg;
            Data = obj;
        }
    }
    public class ServiceResultWarning : ServiceResult
    {
        public ServiceResultWarning()
        {
            Code = CommonConstant.WARNING;
        }
        public ServiceResultWarning(string msg)
        {
            Code = CommonConstant.WARNING;
            Message = msg;
        }
        public ServiceResultWarning(string msg, object obj)
        {
            Code = CommonConstant.WARNING;
            Message = msg;
            Data = obj;
        }
    }
    public class ServiceResultError : ServiceResult
    {
        public ServiceResultError()
        {
            Code = CommonConstant.ERROR;
        }
        public ServiceResultError(string msg)
        {
            Code = CommonConstant.ERROR;
            Message = msg;
        }
        public ServiceResultError(string msg, object obj)
        {
            Code = CommonConstant.ERROR;
            Message = msg;
            Data = obj;
        }
    }
}
