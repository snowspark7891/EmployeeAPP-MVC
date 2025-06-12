
using EmployeeeApp.Data;
using EmployeeeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
            ClientView client = new ClientView();
            return View(client);
        }

        [HttpPost]
        public IActionResult Create(ClientView client)
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
            return View(client);
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            ClientView client = _clientData.GetById(Id);
            return View(client);
        }

        [HttpPost]
        public IActionResult Edit(ClientView client)
        {
            if (ModelState.IsValid)
            {
                if (_clientData.UpdateClient(client))
                {
                    TempData["SuccessMessage"] = "Client updated successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update client. Please try again.");
                }
            }
            return View(client);
        }

        public IActionResult EditList([FromBody] AddAddressRequest request)
        {
            if (request == null || request.NewAddress == null || string.IsNullOrWhiteSpace(request.NewAddress.CityName))
            {
                return Json(new { success = false, message = "Invalid address data provided." });
            }

            try
            {
                request.NewAddress.ClientId = request.ClientId;

                int newAddressId = _clientData.AddAddress(request.ClientId, request.NewAddress);

                if (newAddressId > 0)
                {
                    return Json(new { success = true, addressId = newAddressId, cityName = request.NewAddress.CityName });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to add address to the database. No ID returned." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        public class AddAddressRequest
        {
            [JsonProperty("clientId")]
            public int ClientId { get; set; }

            [JsonProperty("newAddress")]
            public ClientDetails NewAddress { get; set; }
        }

        public IActionResult DeleteList(int addressId)
        {
            if (addressId <= 0)
            {
                return Json(new { success = false, message = "Invalid address ID provided for deletion." });
            }

            try
            {
                ClientDetails addressToDelete = _clientData.GetClientDetailsById(addressId);
                int clientId = addressToDelete?.ClientId ?? 0;

                bool result = _clientData.DeleteAddress(addressId, clientId);

                if (result)
                {
                    return Json(new { success = true, message = "Address deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to delete address from the database." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        public ActionResult AddAddress(int clientId, string city)
        {
            if (clientId <= 0 || string.IsNullOrWhiteSpace(city))
            {
                return Json(new { success = false, message = "Invalid client ID or city provided." });
            }
            try
            {
                ClientDetails newAddress = new ClientDetails
                {
                    ClientId = clientId,
                    CityName = city
                };
                int newAddressId = _clientData.AddAddress(clientId, newAddress);
                if (newAddressId > 0)
                {
                    return Json(new { success = true, addressId = newAddressId, cityName = city });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to add address to the database. No ID returned." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        public IActionResult Delete(int Id)
        {
            ClientView client = _clientData.GetById(Id);
            return View(client);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int Id)
        {
            if (_clientData.DeleteClient(Id))
            {
                TempData["SuccessMessage"] = "Client deleted successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Failed to delete client. Please try again.");
            }
            return View("Delete", _clientData.GetById(Id));
        }
    }
}











//using EmployeeeApp.Data;
//using EmployeeeApp.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using Newtonsoft.Json;
//using System.Runtime.InteropServices.Marshalling;

//namespace EmployeeeApp.Controllers
//{
//    public class ClientController : Controller
//    {

//        public ClientData _clientData;

//        public ClientController()
//        {
//            _clientData = new ClientData();
//        }


//        public IActionResult Index()
//        {
//            List<ClientView> clients = _clientData.GetALL();
//            return View(clients);
//        }

//        public IActionResult Create()
//        {
//            ClientView client = new ClientView();

//            return View(client);
//        }
//        [HttpPost]
//        public IActionResult Create(ClientView client)
//        {

//            if (_clientData.AddClient(client))
//            {
//                TempData["SuccessMessage"] = "Client added successfully!";
//                return RedirectToAction("Index");
//            }
//            else
//            {
//                ModelState.AddModelError("", "Failed to add client. Please try again.");
//            }

//            return View(client);
//        }

//        [HttpGet]
//        public IActionResult Edit(int Id)
//        {
//            ClientView client = _clientData.GetById(Id);
//            return View(client);
//        }


//        [HttpPost]
//        public IActionResult Edit(ClientView client)
//        {
//            if (ModelState.IsValid)
//            {
//                if (_clientData.UpdateClient(client))
//                {
//                    TempData["SuccessMessage"] = "Client updated successfully!";
//                    return RedirectToAction("Index");
//                }
//                else
//                {
//                    ModelState.AddModelError("", "Failed to update client. Please try again.");
//                }
//            }
//            return View(client);
//        }
//        // DeleteList

//        //public IActionResult EditList(int OrderId,string Address)
//        //{
//        //   ClientDetails clientDetails = new ClientDetails
//        //   {
//        //       AddressofClient = Address,
//        //       ClientId = OrderId
//        //   };
//        //    ClientView client = _clientData.GetById(clientDetails.ClientId ?? 0);

//        //    if (clientDetails == null)
//        //    {
//        //        if(_clientData.AddAddress(clientDetails)) ///insert requires the value too  it require the value from the Form 
//        //        {
//        //            TempData["SuccessMessage"] = "Client details updated successfully!";
//        //            return RedirectToAction("Edit",client);
//        //        }
//        //        else
//        //        {
//        //            ModelState.AddModelError("", "Failed to update client details. Please try again.");
//        //        }
//        //    }
//        //    return View(clientDetails);
//        //}


//        //public IActionResult DeleteList(int OrderId)
//        //{
//        //    ClientDetails clientDetails = _clientData.GetClientDetailsById(OrderId);
//        //    ClientView client = _clientData.GetById(clientDetails.ClientId ?? 0);
//        //    if (_clientData.DeleteAddress(OrderId, clientDetails.ClientId ?? 0))
//        //    {
//        //        TempData["SuccessMessage"] = "Client details deleted successfully!";
//        //        return RedirectToAction("Edit", client);
//        //    }
//        //    else
//        //    {
//        //        ModelState.AddModelError("", "Failed to delete client details. Please try again.");
//        //    }

//        //    return View(clientDetails);
//        //}



//        //public IActionResult Index(int Page = 1, int PageNumber = 10)
//        //{
//        //    List<Client> clients = _clientData.GetAll(Page, PageNumber);
//        //    int totalEmployees = _clientData.GetClientTotalCount();

//        //    ViewBag.TotalPages = (int)Math.Ceiling((double)totalEmployees / PageNumber);
//        //    ViewBag.CurrentPage = Page;

//        //    return View(clients);
//        //}





//        //public IActionResult InsertAll()
//        //{
//        //    return View(new ClientViewID { Addresses = new List<ClientDetails>() });
//        //}

//        //[HttpPost]
//        //[ValidateAntiForgeryToken]
//        //public IActionResult InsertAll(ClientViewID model)
//        //{


//        //    if (ModelState.IsValid)
//        //    {
//        //        if (_clientData.InsertAll(model))
//        //        {
//        //            TempData["SuccessMessage"] = "Client and details saved successfully!";
//        //            model.Addresses.RemoveAll(cd => string.IsNullOrWhiteSpace(cd.Address));
//        //            return RedirectToAction("Index");
//        //        }
//        //        else
//        //        {
//        //            ModelState.AddModelError("", "Failed to save client due to a data access error. Please try again.");
//        //        }
//        //    }
//        //    else
//        //    {
//        //        ModelState.AddModelError("", "Please correct the errors in the form before saving.");
//        //    }

//        //    return View(model);
//        //}



//        ////main client oprations

//        //[HttpGet]
//        //public IActionResult EditPage(int Id)
//        //{
//        //    ClientViewID client = _clientData.GetViewBYId(Id);
//        //    if (client == null)
//        //    {
//        //        return NotFound();
//        //    }
//        //    return View(client);
//        //}

//        //[HttpPost]
//        //public IActionResult EditClient(ClientViewID client)
//        //{

//        //    Client client1 = new Client
//        //    {
//        //        Id = client.Id,
//        //        Name = client.Name,
//        //        Role = client.Role,
//        //        Email = client.Email
//        //    };
//        //    if (_clientData.UpdateClient(client1))
//        //    {
//        //        return RedirectToAction("Index");
//        //    }
//        //    ClientViewID cli = _clientData.GetViewBYId(client.Id);
//        //    return View("EditPage", cli);

//        //}

//        //[HttpGet]
//        //public IActionResult DeletePage(int Id)
//        //{
//        //    ClientViewID client = _clientData.GetViewBYId(Id);
//        //    if (client == null)
//        //    {
//        //        return NotFound();
//        //    }
//        //    return View(client);
//        //}

//        //[HttpPost]
//        //public IActionResult DeleteClient(int Id)
//        //{
//        //    if (_clientData.DeleteClient(Id))
//        //    {
//        //        return RedirectToAction("Index");
//        //    }
//        //    ClientViewID cli = _clientData.GetViewBYId(Id);
//        //    return View("DeletePage", cli);
//        //}


//        ////address table operations

//        //[HttpGet]
//        //public IActionResult EditLIst(int Id, int OrderID)
//        //{
//        //    ClientDetails client = _clientData.EditDetails(Id, OrderID);


//        //    if (client == null)
//        //    {
//        //        return NotFound();
//        //    }

//        //    return View(client);
//        //}

//        //[HttpPost]
//        //public IActionResult EditLIst(ClientDetails client)
//        //{
//        //    if (_clientData.AddAddress(client))
//        //    {
//        //        int Id = client.ClientId;
//        //        ClientViewID cli = _clientData.GetViewBYId(Id);
//        //        return View("EditPage", cli);
//        //    }
//        //    return View("EditList", client);

//        //}

//        //[HttpGet]
//        //public IActionResult DeleteList(int Id, int OrderId)
//        //{
//        //    ClientDetails client = _clientData.EditDetails(Id, OrderId);
//        //    if (client == null)
//        //    {
//        //        return NotFound();
//        //    }

//        //    return View(client);
//        //}
//        //[HttpPost]
//        //public IActionResult DeleteList(ClientDetails client)
//        //{
//        //    int Id = client.ClientId;
//        //    int OrderId = client.OrderId;
//        //    if (_clientData.DeleteAddress(Id, OrderId))
//        //    {

//        //        ClientViewID cli = _clientData.GetViewBYId(Id);
//        //        return View("EditPage", cli);
//        //    }
//        //    return View("DeleteList", client);
//        //}




//        //[HttpGet]
//        //public IActionResult AddAddressList(int Id)
//        //{
//        //    ClientDetails client = new ClientDetails
//        //    {
//        //        ClientId = Id
//        //    };
//        //    if (client == null)
//        //    {
//        //        return NotFound();
//        //    }
//        //    return View(client);
//        //}
//        //[HttpPost]
//        //public IActionResult AddAddresslist(int Id, string Address)
//        //{
//        //    if (_clientData.AddAddress(Id, Address))
//        //    {
//        //        ClientViewID cli = _clientData.GetViewBYId(Id);
//        //        return View("EditPage", cli);
//        //    }
//        //    else
//        //    {
//        //        return NotFound();
//        //    }
//        //}

//        public IActionResult EditList([FromBody] AddAddressRequest request)
//        {
//            if (request == null || request.NewAddress == null || string.IsNullOrWhiteSpace(request.NewAddress.AddressofClient))
//            {
//                return Json(new { success = false, message = "Invalid address data provided." });
//            }

//            try
//            {
//                request.NewAddress.ClientId = request.ClientId;

//                int newAddressId = _clientData.AddAddress(request.ClientId, request.NewAddress);

//                if (newAddressId > 0)
//                {
//                    return Json(new { success = true, addressId = newAddressId, addressofClient = request.NewAddress.AddressofClient });
//                }
//                else
//                {
//                    return Json(new { success = false, message = "Failed to add address to the database. No ID returned." });
//                }
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
//            }
//        }

//        public class AddAddressRequest
//        {
//            [JsonProperty("clientId")]
//            public int ClientId { get; set; }

//            [JsonProperty("newAddress")]
//            public ClientDetails NewAddress { get; set; }
//        }


//        public IActionResult DeleteList(int addressId)
//        {
//            if (addressId <= 0)
//            {
//                return Json(new { success = false, message = "Invalid address ID provided for deletion." });
//            }

//            try
//            {
//                ClientDetails addressToDelete = _clientData.GetClientDetailsById(addressId);
//                int clientId = addressToDelete?.ClientId ?? 0;

//                bool result = _clientData.DeleteAddress(addressId, clientId);

//                if (result)
//                {
//                    return Json(new { success = true, message = "Address deleted successfully." });
//                }
//                else
//                {
//                    return Json(new { success = false, message = "Failed to delete address from the database." });
//                }
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
//            }
//        }



//        public ActionResult AddAddress(int clientId, string address)
//        {
//            if (clientId <= 0 || string.IsNullOrWhiteSpace(address))
//            {
//                return Json(new { success = false, message = "Invalid client ID or address provided." });
//            }
//            try
//            {
//                ClientDetails newAddress = new ClientDetails
//                {
//                    ClientId = clientId,
//                    AddressofClient = address
//                };
//                int newAddressId = _clientData.AddAddress(clientId, newAddress);
//                if (newAddressId > 0)
//                {
//                    return Json(new { success = true, addressId = newAddressId, addressofClient = address });
//                }
//                else
//                {
//                    return Json(new { success = false, message = "Failed to add address to the database. No ID returned." });
//                }
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
//            }

//        }


//        public IActionResult Delete(int Id)
//        {
//            ClientView client = _clientData.GetById(Id);
//            return View(client);
//        }


//        [HttpPost]
//        public IActionResult DeleteConfirmed(int Id)
//        {
//            if (_clientData.DeleteClient(Id))
//            {
//                TempData["SuccessMessage"] = "Client deleted successfully!";
//                return RedirectToAction("Index");
//            }
//            else
//            {
//                ModelState.AddModelError("", "Failed to delete client. Please try again.");
//            }
//            return View("Delete", _clientData.GetById(Id));
//        }


//    }
//}







////[HttpGet("Edit/{Id}")]
////public IActionResult Edit(int Id)
////{
////    EditId viewClientModel = _clientData.GetById(Id);

////    if (viewClientModel == null)
////    {
////        return NotFound();
////    }
////    return View(viewClientModel);
////}

////[HttpPost]
////public IActionResult Edit(Client client)
////{
////    if (ModelState.IsValid)
////    {
////        if (_clientData.Update(client))
////        {
////            return RedirectToAction("Index");
////        }
////        else
////        {
////            ModelState.AddModelError("", "Failed to update client. Please try again.");
////        }
////    }
////    return View("Edit", client);
////}


////[HttpGet]
////public IActionResult Delete(int Id)
////{
////    EditId client = _clientData.GetById(Id);
////    if (client == null)
////    {
////        return NotFound();
////    }
////    return View(client);
////}


////[HttpPost, ActionName("Delete")]
////public IActionResult DeleteConfirmed(int Id)
////{
////    if (_clientData.Delete(Id))
////    {
////        return RedirectToAction("Index");
////    }
////    else
////    {
////        ModelState.AddModelError("", "Failed to delete client. Please try again.");
////        return View();
////    }
////}//public IActionResult Single()
////{
////    var lin = new ViewModel
////    {
////        newClient = new Client(),
////        clients = _clientData.GetAll(1, 10),
////        //clientDetails = new ClientDetails(),
////        Addresses = _clientData.getList()
////    };

////    return View(lin);

////}



////public IActionResult EditLIst(int Id,int OrderID)
////{
////    ClientViewID client = _clientData.GetViewBYID(Id , OrderID);
////    if(client == null)
////    {
////        return NotFound();
////    }

////    return View(client);
////}


////[HttpPost]
////public IActionResult UpdateAll(ClientViewID client,int id)
////{
////    if (_clientData.UpdateAlll(client))
////    {
////        int Id = client.Id;

////        return RedirectToAction("Edit",Id);
////    }
////    return View("EditLIst", client);
////}



////[HttpGet]
////public IActionResult DeleteFromList(int Id,int OrderId)
////{
////    ClientViewID df = _clientData.GetViewBYID(Id, OrderId);
////    if (df != null)
////    {
////        return View(df);
////    }
////    return NotFound();
////}



////[HttpPost]
////public IActionResult DeleteFromListAct(int OrderId,int Id)
////{
////    if (_clientData.DeleteFromList(OrderId))
////    {
////        return RedirectToAction("Edit", new { Id = Id });
////    }
////    return NotFound();
////}



////public IActionResult AddLIst(int Id, int OrderId)
////{
////    ClientViewID li = _clientData.GetViewBYID(Id,OrderId);
////    return View(li);
////}



////[HttpPost]
////public IActionResult AddLIst(ClientViewID li)
////{
////    int Id = li.Id;
////    if (_clientData.AddADDRL(li))
////    {
////        return RedirectToAction("Edit",Id);
////    }
////    else
////    {
////        return NotFound();
////    }
////}



////[HttpPost]
////public IActionResult AddClient(ViewModel lin, String addr)
////{
////    if (!string.IsNullOrEmpty(lin.newClient.Name) && !string.IsNullOrWhiteSpace(addr))
////    {
////        lin.clients.Add(new Client
////        {
////            Name = lin.newClient.Name,
////            Role = lin.newClient.Role,
////            Email = lin.newClient.Email
////        });
////        lin.Addresses = _clientData.AddList(addr);
////        lin.newClient = new Client();
////        lin.Addresses = _clientData.getList();
////        lin.addr = string.Empty;




////    }

////    return View("Single", lin);
////}





////[HttpPost]
////public IActionResult DeleteAddress(ViewModel lin, int index)
////{
////    if (index >= 0)
////    {
////        if (_clientData.DeleteAdd(index))
////        {
////            return RedirectToAction("Single");
////        }
////        else
////        {
////            ModelState.AddModelError("", "Failed to Delete Address. Please try again.");
////        }

////    }
////    return View("Single", lin);
////}

////[HttpPost]
////public IActionResult AddClientTODB(ViewModel lin)
////{
////    if (ModelState.IsValid)
////    {
////        if (_clientData.ListToDB(lin.clients))
////        {
////            lin.newClient = new Client();
////            lin.clients = _clientData.GetAll(1, 10);
////            lin.Addresses = _clientData.getList();
////            return RedirectToAction("Single");
////        }
////        else
////        {
////            ModelState.AddModelError("", "Failed to add client to database. Please try again.");
////        }
////    }
////    return View("Single", lin);


////}