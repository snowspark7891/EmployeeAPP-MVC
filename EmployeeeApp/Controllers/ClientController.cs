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


        public IActionResult Index()
        {
           List<ClientView> clients = _clientData.GetALL();
            return View(clients);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ClientView client)
        {
            if (ModelState.IsValid)
            {
                if (_clientData.AddClient(client))
                {
                    TempData["SuccessMessage"] = "Client added successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to add client. Please try again.");
                }
            }
            return View(client);
        }













        //public IActionResult Index(int Page = 1, int PageNumber = 10)
        //{
        //    List<Client> clients = _clientData.GetAll(Page, PageNumber);
        //    int totalEmployees = _clientData.GetClientTotalCount();

        //    ViewBag.TotalPages = (int)Math.Ceiling((double)totalEmployees / PageNumber);
        //    ViewBag.CurrentPage = Page;

        //    return View(clients);
        //}





        //public IActionResult InsertAll()
        //{
        //    return View(new ClientViewID { Addresses = new List<ClientDetails>() });
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult InsertAll(ClientViewID model)
        //{


        //    if (ModelState.IsValid)
        //    {
        //        if (_clientData.InsertAll(model))
        //        {
        //            TempData["SuccessMessage"] = "Client and details saved successfully!";
        //            model.Addresses.RemoveAll(cd => string.IsNullOrWhiteSpace(cd.Address));
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Failed to save client due to a data access error. Please try again.");
        //        }
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "Please correct the errors in the form before saving.");
        //    }

        //    return View(model);
        //}



        ////main client oprations

        //[HttpGet]
        //public IActionResult EditPage(int Id)
        //{
        //    ClientViewID client = _clientData.GetViewBYId(Id);
        //    if (client == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(client);
        //}

        //[HttpPost]
        //public IActionResult EditClient(ClientViewID client)
        //{

        //    Client client1 = new Client
        //    {
        //        Id = client.Id,
        //        Name = client.Name,
        //        Role = client.Role,
        //        Email = client.Email
        //    };
        //    if (_clientData.UpdateClient(client1))
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    ClientViewID cli = _clientData.GetViewBYId(client.Id);
        //    return View("EditPage", cli);

        //}

        //[HttpGet]
        //public IActionResult DeletePage(int Id)
        //{
        //    ClientViewID client = _clientData.GetViewBYId(Id);
        //    if (client == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(client);
        //}

        //[HttpPost]
        //public IActionResult DeleteClient(int Id)
        //{
        //    if (_clientData.DeleteClient(Id))
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    ClientViewID cli = _clientData.GetViewBYId(Id);
        //    return View("DeletePage", cli);
        //}


        ////address table operations

        //[HttpGet]
        //public IActionResult EditLIst(int Id, int OrderID)
        //{
        //    ClientDetails client = _clientData.EditDetails(Id, OrderID);


        //    if (client == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(client);
        //}

        //[HttpPost]
        //public IActionResult EditLIst(ClientDetails client)
        //{
        //    if (_clientData.AddAddress(client))
        //    {
        //        int Id = client.ClientId;
        //        ClientViewID cli = _clientData.GetViewBYId(Id);
        //        return View("EditPage", cli);
        //    }
        //    return View("EditList", client);

        //}

        //[HttpGet]
        //public IActionResult DeleteList(int Id, int OrderId)
        //{
        //    ClientDetails client = _clientData.EditDetails(Id, OrderId);
        //    if (client == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(client);
        //}
        //[HttpPost]
        //public IActionResult DeleteList(ClientDetails client)
        //{
        //    int Id = client.ClientId;
        //    int OrderId = client.OrderId;
        //    if (_clientData.DeleteAddress(Id, OrderId))
        //    {

        //        ClientViewID cli = _clientData.GetViewBYId(Id);
        //        return View("EditPage", cli);
        //    }
        //    return View("DeleteList", client);
        //}




        //[HttpGet]
        //public IActionResult AddAddressList(int Id)
        //{
        //    ClientDetails client = new ClientDetails
        //    {
        //        ClientId = Id
        //    };
        //    if (client == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(client);
        //}
        //[HttpPost]
        //public IActionResult AddAddresslist(int Id, string Address)
        //{
        //    if (_clientData.AddAddress(Id, Address))
        //    {
        //        ClientViewID cli = _clientData.GetViewBYId(Id);
        //        return View("EditPage", cli);
        //    }
        //    else
        //    {
        //        return NotFound();
        //    }
        //}













    }
}







//[HttpGet("Edit/{Id}")]
//public IActionResult Edit(int Id)
//{
//    EditId viewClientModel = _clientData.GetById(Id);

//    if (viewClientModel == null)
//    {
//        return NotFound();
//    }
//    return View(viewClientModel);
//}

//[HttpPost]
//public IActionResult Edit(Client client)
//{
//    if (ModelState.IsValid)
//    {
//        if (_clientData.Update(client))
//        {
//            return RedirectToAction("Index");
//        }
//        else
//        {
//            ModelState.AddModelError("", "Failed to update client. Please try again.");
//        }
//    }
//    return View("Edit", client);
//}


//[HttpGet]
//public IActionResult Delete(int Id)
//{
//    EditId client = _clientData.GetById(Id);
//    if (client == null)
//    {
//        return NotFound();
//    }
//    return View(client);
//}


//[HttpPost, ActionName("Delete")]
//public IActionResult DeleteConfirmed(int Id)
//{
//    if (_clientData.Delete(Id))
//    {
//        return RedirectToAction("Index");
//    }
//    else
//    {
//        ModelState.AddModelError("", "Failed to delete client. Please try again.");
//        return View();
//    }
//}//public IActionResult Single()
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



//public IActionResult EditLIst(int Id,int OrderID)
//{
//    ClientViewID client = _clientData.GetViewBYID(Id , OrderID);
//    if(client == null)
//    {
//        return NotFound();
//    }

//    return View(client);
//}


//[HttpPost]
//public IActionResult UpdateAll(ClientViewID client,int id)
//{
//    if (_clientData.UpdateAlll(client))
//    {
//        int Id = client.Id;

//        return RedirectToAction("Edit",Id);
//    }
//    return View("EditLIst", client);
//}



//[HttpGet]
//public IActionResult DeleteFromList(int Id,int OrderId)
//{
//    ClientViewID df = _clientData.GetViewBYID(Id, OrderId);
//    if (df != null)
//    {
//        return View(df);
//    }
//    return NotFound();
//}



//[HttpPost]
//public IActionResult DeleteFromListAct(int OrderId,int Id)
//{
//    if (_clientData.DeleteFromList(OrderId))
//    {
//        return RedirectToAction("Edit", new { Id = Id });
//    }
//    return NotFound();
//}



//public IActionResult AddLIst(int Id, int OrderId)
//{
//    ClientViewID li = _clientData.GetViewBYID(Id,OrderId);
//    return View(li);
//}



//[HttpPost]
//public IActionResult AddLIst(ClientViewID li)
//{
//    int Id = li.Id;
//    if (_clientData.AddADDRL(li))
//    {
//        return RedirectToAction("Edit",Id);
//    }
//    else
//    {
//        return NotFound();
//    }
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