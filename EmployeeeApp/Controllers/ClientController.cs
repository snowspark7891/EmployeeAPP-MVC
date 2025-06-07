using EmployeeeApp.Data;
using EmployeeeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.InteropServices.Marshalling;

namespace EmployeeeApp.Controllers
{
    public class ClientController : Controller
    {

        public ClientData _clientData;

        public ClientController()
        {
            _clientData = new ClientData();
        }


        public IActionResult Index(int Page = 1, int PageNumber = 10)
        {
            List<Client> clients = _clientData.GetAll(Page, PageNumber);
            int totalEmployees = _clientData.GetClientTotalCount();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalEmployees / PageNumber);
            ViewBag.CurrentPage = Page;

            return View(clients);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Client client)
        {
            if (ModelState.IsValid)
            {
                if (_clientData.Insert(client))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create client. Please try again.");
                }
            }
            return View(client);
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            Client viewClientModel = _clientData.GetById(Id);

            if (viewClientModel == null)
            {
                return NotFound();
            }
            return View(viewClientModel);
        }

        [HttpPost]
        public IActionResult Edit(Client client)
        {
            if (ModelState.IsValid)
            {
                if (_clientData.Update(client))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update client. Please try again.");
                }
            }
            return View("Edit", client);
        }


        [HttpGet]
        public IActionResult Delete(int Id)
        {
            Client client = _clientData.GetById(Id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int Id)
        {
            if (_clientData.Delete(Id))
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Failed to delete client. Please try again.");
                return View();
            }
        }


        public IActionResult InsertAll()
        {
            return View();
        }

        [HttpPost]
        public IActionResult InsertAll(Creatmodel b)
        {
            if (ModelState.IsValid)
            {
                if (_clientData.InsertAll(b))
                {
                    return RedirectToAction("GetTableClient");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to insert all clients. Please try again.");
                }
            }
            return View("Single", b);
        }

        [HttpGet]
        public IActionResult GetTableClient()
        {
            List<ClientViewID> client = _clientData.GettNewView();

            return View(client);
        } 

        public IActionResult EditLIst(int Id,int OrderID)
        {
            ClientViewID client = _clientData.GetViewBYID(Id , OrderID);
            if(client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        [HttpPost]
        public IActionResult UpdateAll(ClientViewID client)
        {
            if (_clientData.UpdateAlll(client))
            {
              
                return RedirectToAction("GetTableClient");
            }
            return View("EditLIst", client);
        }

        [HttpGet]
        public IActionResult DeleteFromList(int Id,int OrderId)
        {
            ClientViewID df = _clientData.GetViewBYID(Id, OrderId);
            if (df != null)
            {
                return View(df);
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult DeleteFromList(int OrderId)
        {
            if (_clientData.DeleteFromList(OrderId))
            {
                return RedirectToAction("GetTableClient");
            }
            return NotFound();
        }
        public IActionResult AddLIst(int Id, int OrderId)
        {
            ClientViewID li = _clientData.GetViewBYID(Id,OrderId);
            return View(li);
        }
        [HttpPost]
        public IActionResult AddLIst(ClientViewID li)
        {
            if (_clientData.AddADDRL(li))
            {
                return RedirectToAction("GetTableClient");
            }
            else
            {
                return NotFound();
            }
        }
    }
}






//public IActionResult Single()
//{
//    var lin = new ViewModel
//    {
//        newClient = new Client(),
//        clients = _clientData.GetAll(1, 10),
//        //clientDetails = new ClientDetails(),
//        Addresses = _clientData.getList()
//    };

//    return View(lin);

//}







//[HttpPost]
//public IActionResult AddClient(ViewModel lin, String addr)
//{
//    if (!string.IsNullOrEmpty(lin.newClient.Name) && !string.IsNullOrWhiteSpace(addr))
//    {
//        lin.clients.Add(new Client
//        {
//            Name = lin.newClient.Name,
//            Role = lin.newClient.Role,
//            Email = lin.newClient.Email
//        });
//        lin.Addresses = _clientData.AddList(addr);
//        lin.newClient = new Client();
//        lin.Addresses = _clientData.getList();
//        lin.addr = string.Empty;




//    }

//    return View("Single", lin);
//}





//[HttpPost]
//public IActionResult DeleteAddress(ViewModel lin, int index)
//{
//    if (index >= 0)
//    {
//        if (_clientData.DeleteAdd(index))
//        {
//            return RedirectToAction("Single");
//        }
//        else
//        {
//            ModelState.AddModelError("", "Failed to Delete Address. Please try again.");
//        }

//    }
//    return View("Single", lin);
//}

//[HttpPost]
//public IActionResult AddClientTODB(ViewModel lin)
//{
//    if (ModelState.IsValid)
//    {
//        if (_clientData.ListToDB(lin.clients))
//        {
//            lin.newClient = new Client();
//            lin.clients = _clientData.GetAll(1, 10);
//            lin.Addresses = _clientData.getList();
//            return RedirectToAction("Single");
//        }
//        else
//        {
//            ModelState.AddModelError("", "Failed to add client to database. Please try again.");
//        }
//    }
//    return View("Single", lin);


//}