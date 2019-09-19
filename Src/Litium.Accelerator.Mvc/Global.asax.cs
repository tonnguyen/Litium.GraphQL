using System;
using System.Web;
using System.Web.SessionState;

namespace Litium.Accelerator.Mvc
{
    public class MvcApplication : HttpApplication
    {
        public override void Init()
        {
            // enable Session for GraphQL controller
            PostAuthenticateRequest += MvcApplication_PostAuthenticateRequest;
            base.Init();
        }

        void MvcApplication_PostAuthenticateRequest(object sender, EventArgs e)
        {
            // force the session to be enabled for GraphQL requests only, not every requests
            if (IsGraphQLRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
                HttpContext.Current.Items["VAT"] = true;
            }
        }

        private static bool IsGraphQLRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api/graphql");
        }
    }
}
