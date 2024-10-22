using ApiInterface.InternalModels;
using ApiInterface.Models;
using Entities;
using QueryProcessor;

namespace ApiInterface.Processors
{
    internal class SQLSentenceProcessor(Request request) : IProcessor 
    {
        public Request Request { get; } = request;

        public Response Process()
        {
            try
            {
                var sentence = this.Request.RequestBody;
                var result = SQLQueryProcessor.Execute(sentence);
                var response = this.ConvertToResponse(result);
                return response;
            }
            catch (Exception ex)
            {
                // Capturamos la excepción y la enviamos como parte del ResponseBody
                return new Response
                {
                    Status = OperationStatus.Error,
                    Request = this.Request,
                    ResponseBody = "Error processing SQL: " + ex.Message
                };
            }
        }

        private Response ConvertToResponse(OperationStatus result)
        {
            // Verificamos si la operación fue exitosa y retornamos un mensaje o el resultado de la consulta
            string responseBody;

            if (result == OperationStatus.Success)
            {

                responseBody = "Consulta ejecutada exitosamente"; // Mensaje genérico de éxito

            }
            else
            {
                responseBody = "Error al ejecutar la consulta";
            }

            return new Response
            {
                Status = result,
                Request = this.Request,
                ResponseBody = responseBody
            };
        }
    }
}
