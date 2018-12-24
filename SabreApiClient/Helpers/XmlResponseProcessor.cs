using Domain.Models;
using System;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SabreApiClient.Helpers
{
    internal class XmlResponseProcessor
    {
        public void CheckErrors(object request, dynamic response)
        {
            try
            {
                if (request == null || response == null)
                {
                    return;
                }

                // Sabre Web Services version 1.x
                var t = (response as object).GetType().GetProperties().FirstOrDefault(x => x.Name == "Errors");
                if ((response as object).GetType().GetProperties().FirstOrDefault(x => x.Name == "Errors") != null)
                {
                    if (response.Errors != null)
                    {
                        var responseXml = this.ToXmlString(response);
                        var requestXml = this.ToXmlString(request);
                        throw new SabreException(response.Errors.Error.ErrorInfo.Message)
                        {
                            SabreRequest = requestXml,
                            SabreResponse = responseXml
                        };
                    }
                }

                // Sabre Web Services version 2.x
                if ((response as object).GetType().GetProperties().FirstOrDefault(x => x.Name == "ApplicationResults") != null)
                {
                    if (response.ApplicationResults.Error != null)
                    {
                        var responseXml = this.ToXmlString(response);
                        var requestXml = this.ToXmlString(request);

                        var errors = response.ApplicationResults.Error as dynamic[];
                        if (errors == null || errors.Length == 0)
                        {
                            return;
                        }

                        var sb = new StringBuilder();
                        foreach (var error in errors)
                        {
                            foreach (var result in error.SystemSpecificResults)
                            {
                                foreach (var message in result.Message)
                                {
                                    var messageContent = message.Value as string;
                                    sb.AppendLine(messageContent);
                                }
                            }
                        }
                        throw new SabreException(sb.ToString())
                        {
                            SabreRequest = requestXml,
                            SabreResponse = responseXml
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string ToXmlString(object obj)
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(obj.GetType());
            serializer.Serialize(stringwriter, obj);
            return stringwriter.ToString();
        }
    }
}
