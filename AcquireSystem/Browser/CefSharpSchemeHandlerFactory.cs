using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;

namespace AcquireSystem.Browser
{
    public class CefSharpSchemeHandlerFactory:ISchemeHandlerFactory
    {
        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
