using System;
namespace coreMVCproject1.Models.Dto
{
    public class AjaxReturnDto<T>
    {
        public bool success{get;set;}
        public T data{get;set;}
        public string msg{get;set;}
        public AjaxReturnDto(bool Success,T Data,string message="")
        {
            success = Success;
            data = Data;
            msg = message;
        }
    }
    public class AjaxReturnDto
    {
        public bool success{get;set;}
        public string msg{get;set;}
    }
}