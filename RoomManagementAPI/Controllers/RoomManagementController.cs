using Microsoft.AspNetCore.Mvc;
using RoomManage;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using System;
using Amazon.S3.Transfer;
using Amazon.Runtime;

namespace RoomManagementAPI.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomManagementController : ControllerBase
    {
        //RoomGetServices getServices;
        //RoomTransactionService transactionService;

        //public RoomManagementController()
        //{
        //    getServices = new RoomGetServices();
        //    transactionService = new RoomTransactionService();
        //}
        private readonly RoomGetServices getServices;
        private readonly RoomTransactionService transactionService;
        private readonly IAmazonS3 s3Client;
        private readonly string _accessKey = "";
        private readonly string _secretKey = "";
        private readonly string bucketName = "awtorralba";

        public RoomManagementController()
        {
            
            var credentials = new BasicAWSCredentials(_accessKey, _secretKey);
            this.s3Client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.USEast1);

            this.getServices = new RoomGetServices();
            this.transactionService = new RoomTransactionService();
        }


        [HttpGet]
        public IEnumerable<RoomManagementAPI.Room> GetRoom()
        {
            var room = getServices.GetAllRooms();

            List<RoomManagementAPI.Room> cont = new List<RoomManagementAPI.Room>();

            foreach (var rooms in room)
            {
                cont.Add(new RoomManagementAPI.Room { Roomnum = rooms.Roomnum, Name = rooms.Name });
            }
            return cont;
        }
        [HttpPost]
        public JsonResult AddRoom(Room request)
        {
            var result = transactionService.CreateRoom(request.Roomnum, request.Name);

            return new JsonResult(result);

        }

        [HttpPatch]
        public JsonResult UpdateRoom(Room request)
        {
            var result = transactionService.UpdateRoom(request.Roomnum, request.Name);

            return new JsonResult(result);
        }

        [HttpDelete]
        public JsonResult DeleteRoom(RoomManagementAPI.Room request)
        {

            var roomToDelete = new RoomManageModel.Room
            {   
                Roomnum = request.Roomnum

            };

            var result = transactionService.DeleteRoom(roomToDelete);

            return new JsonResult(result);
        }


        [HttpPost("upload")]
        public async Task<IActionResult> UploadFileToS3(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                var fileTransferUtility = new TransferUtility(s3Client);

                // Generate a unique file name to avoid name conflicts
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // Upload the file to S3
                using (var stream = file.OpenReadStream())
                {
                    await fileTransferUtility.UploadAsync(stream, bucketName, fileName);
                }

                return Ok(new { message = "File uploaded successfully", fileName });
            }
            catch (AmazonS3Exception e)
            {
                return StatusCode(500, new { message = $"Error uploading file: {e.Message}" });
            }
        }
    }
}
