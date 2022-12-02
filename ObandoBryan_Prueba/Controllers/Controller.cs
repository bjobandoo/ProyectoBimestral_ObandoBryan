using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ObandoBryan_Prueba.OCR;
using ObandoBryan_Prueba.Description;
using System.Text;

namespace ObandoBryan_Prueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Controller : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<string>> PostImagen(string imgRoute)
        {
            //------------------------------------------------------------------------------------
            var api = new System.Net.WebClient();
            api.Headers.Add("Content_Type", "application/octec-stream");
            api.Headers.Add("Content_Type", "application/json");
            api.Headers.Add("Ocp-Apim-Subscription-Key", "1ece5a2bffaf4e32a19f4899e002daf1");
            var qs = "language=es&language=true&model-version=latest";
            var url = "https://eastus.api.cognitive.microsoft.com/vision/v3.2/ocr";

            var resp = api.UploadFile(url + "?" + qs, "POST", imgRoute);
            var json = Encoding.UTF8.GetString(resp);
            var text1 = Newtonsoft.Json.JsonConvert.DeserializeObject<ocr_response>(json);
            //------------------------------------------------------------------------------------

            var qsO = "maxCandidates=1&language=es&model-version=latest";
            var urlO = "https://eastus.api.cognitive.microsoft.com/vision/v3.2/describe";

            var respO = api.UploadFile(urlO + "?" + qsO, "POST", imgRoute);
            var jsonO = Encoding.UTF8.GetString(respO);
            var textO =  Newtonsoft.Json.JsonConvert.DeserializeObject<describe_response>(jsonO);

            return textOcr(text1) + "\n" +textDescribe(textO);
        }
        private static string textOcr(ocr_response resp)
        {
            var txt = "<b>Texto:</b><br/>";
            foreach (var region in resp.regions)
            {
                txt += "<p>";
                foreach (var line in region.lines)
                {
                    foreach (var word in line.words)
                    {
                        txt += word.text + " ";
                    }
                    txt += "<br/>";
                }
                txt += "</p>";
            }
            return txt;

        }

        private static string textDescribe(describe_response resp)
        {
            var txt = "<b>Descripcion de la imagen:</b><br/>";
            foreach (var caption in resp.description.captions)
            {
                txt = txt + caption.text + "<br/>";
            }
            return txt;

        }

    }
}
