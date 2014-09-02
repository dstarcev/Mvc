using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using MvcSample.Web.Models;

namespace MvcSample.Web.RandomNameSpace
{
    public class BindingController
    {
        public int Count { get; set; }

        public string Bind([FromHeader("Accept")] string i)
        {
            return Count + "_" + i.ToString();
        }
    }
}