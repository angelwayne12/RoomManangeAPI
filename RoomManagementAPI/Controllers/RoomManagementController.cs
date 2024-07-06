using Microsoft.AspNetCore.Mvc;
using RoomManage;

namespace RoomManagementAPI.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomManagementController : ControllerBase
    {
        RoomGetServices getServices;
        RoomTransactionService transactionService;

        public RoomManagementController()
        {
            getServices = new RoomGetServices();
            transactionService = new RoomTransactionService();
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
    }
}
