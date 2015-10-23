using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models
{
    public class RequireRequestValueAttribute : ActionMethodSelectorAttribute
    {
        public RequireRequestValueAttribute(string[] valueNames)
        {
            ValueNames = valueNames;
        }
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            bool contains = false;
            foreach (var value in ValueNames)
            {
                contains = (controllerContext.HttpContext.Request[value] != null);
                if (!contains) break;

            }
            return contains;
        }
        public string[] ValueNames { get; private set; }
    }
}