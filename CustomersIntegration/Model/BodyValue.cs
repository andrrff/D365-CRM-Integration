using Microsoft.AspNetCore.Http;
using System;

namespace CustomersIntegration.Model
{
    class BodyValue
    {
        public static String getName(HttpRequest _req, dynamic _data)
        {
            string name = _req.Query["name"];

            return name ?? _data?.name;
        }

        public static String getEmail(HttpRequest _req, dynamic _data)
        {
            string email = _req.Query["email"];

            return email ?? _data?.email;
        }

        public static String getDocument(HttpRequest _req, dynamic _data)
        {
            string document = _req.Query["document"];

            return document ?? _data?.document;
        }

        public static String getClientType(HttpRequest _req, dynamic _data)
        {
            string clientType = _req.Query["clientType"];

            return clientType ?? _data?.clientType;
        }

        public static Boolean getHasAcceptedPrivacyPolicy(HttpRequest _req, dynamic _data)
        {
            string hasAcceptedPrivacyPolicy = _req.Query["hasAcceptedPrivacyPolicy"];

            return hasAcceptedPrivacyPolicy ?? _data?.hasAcceptedPrivacyPolicy;
        }

        public static Boolean getHasAcceptedEmailNewsletter(HttpRequest _req, dynamic _data)
        {
            string hasAcceptedEmailNewslette = _req.Query["hasAcceptedEmailNewslette"];

            return hasAcceptedEmailNewslette ?? _data?.hasAcceptedEmailNewslette;
        }
    }
}
