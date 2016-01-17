using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.IO;
using EmpikTest.Models;

namespace EmpikTest.Controllers
{
    public class DirectoryController : ApiController
    {
        [HttpGet]
        public async Task<IEnumerable<ModelViewDirectoryInfo>> GetList(string path="\\")
        {
            try
            {
                return await DirectoryUtils.BrowsePath(path);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(e.Message, System.Text.Encoding.UTF8, "text/plain"),
                }
                
                );
            }
        }
        [HttpGet]
        public async Task<long[]> GetAmounts(string path="\\")
        {
            try
            {
                return await DirectoryUtils.CountFiles(path);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(e.Message, System.Text.Encoding.UTF8, "text/plain"),
                });
            }
        }
    }
}
