using Application.Services;
using Application.Services.Master;
using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using Domain.Model;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WMS.Data;
using WMSWebApp.HitachiProvider;
using WMSWebApp.ViewModels;
using WMSWebApp.Wrapper;

namespace WMSWebApp.Controllers
{
    public class IntrasitController : Controller
    {
        private readonly IIntrasitHelper _IntrasitHelper;
        private readonly IMapper _mapper;
        private readonly IWorkContext _workContext;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserProfileService _userProfileService;
        private readonly IMemoryCacheWrapper _memoryCacheWrapper;
        private readonly IHitachiConnection _hitachiConnection;

        private string inbound = "4e57534b5240313233";
        private string userName = "NWSKR";
        private string password = "NWSKR@23";
        public IntrasitController(IIntrasitHelper intrasitHelper, IMapper mapper, IWorkContext workContext, UserManager<ApplicationUser> userManager,
                              IUserProfileService userProfileService, IMemoryCacheWrapper memoryCache, IHitachiConnection hitachiConnection)
        {
            _IntrasitHelper = intrasitHelper;
            _mapper = mapper;
            _workContext = workContext;
            _userManager = userManager;
            _userProfileService = userProfileService;
            _memoryCacheWrapper = memoryCache;
            _hitachiConnection = hitachiConnection;
        }

        [HttpGet]
        public string testM()
        {
            //var value =  EncryptDecryptV1.Encrypt("test",inbound,inbound);

            // System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
            // binding.MaxBufferSize = int.MaxValue;
            // binding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
            // binding.MaxReceivedMessageSize = int.MaxValue;
            // binding.AllowCookies = true;
            // binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            // binding.TransferMode = System.ServiceModel.TransferMode.Buffered;
            // string uRL = "";
            // //binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            // var endpoint = new EndpointAddress(uRL + "/AirService");
            // ////AirLowFareSearchPortTypeClient client = new AirLowFareSearchPortTypeClient(AirLowFareSearchPortTypeClient.EndpointConfiguration.AirLowFareSearchPort, uRL + "/AirService");
            // //AirLowFareSearchPortTypeClient client = new AirLowFareSearchPortTypeClient(binding, endpoint);

            // DeliverySerializationcl_UserAuthentication deliverySerializationcl_UserAuthentication = new DeliverySerializationcl_UserAuthentication();
            // deliverySerializationcl_UserAuthentication.userauthenticationdata = "\"{\"username\":\"" + userName + "\",\"password\":\"" + password + "\"}";

            // DeliveryDetailsClient client = new DeliveryDetailsClient(binding, endpoint);
            // var data = client.getUserAuthenticateAsync(deliverySerializationcl_UserAuthentication).Result;
            return "";
        }
        public IActionResult Index()
        {
            List<Intrasitc> intrasitcs = new List<Intrasitc>();
            try
            {
                var data = _IntrasitHelper.GetAllIntrasit();
                for (int i = 0; i < data.Count; i++)
                {
                    Intrasitc objDate = new Intrasitc();
                    objDate.Id = data[i].Id;
                    objDate.PurchaseOrder = data[i].PurchaseOrder;
                    objDate.Branch = data[i].Sender_Branch;
                    objDate.ItemCode = data[i].Item_Code;
                    objDate.SubItemCode = data[i].SubItem_Code;
                    objDate.SubItemName = data[i].SubItem_Name;
                    objDate.MaterialDescription = data[i].Material_Description;
                    objDate.Qty = data[i].Qty;
                    objDate.Unit = data[i].Unit;
                    objDate.Amt = data[i].Amt;
                    objDate.Recv_Date = data[i].Recv_Date;
                    objDate.Way_Bill_Number = data[i].Way_Bill_Number;
                    objDate.Line_Item_id = data[i].Line_Item_id;
                    objDate.Source_Number = data[i].Source_Number;
                    objDate.Bucket = data[i].Bucket;
                    intrasitcs.Add(objDate);
                }
            }
            catch (Exception ex)
            {

            }
            return View(intrasitcs);

        }

        // GET: CompaniesController/Create
        public ActionResult Create()
        {
            IntransitViewModel intransitViewModel = new IntransitViewModel();
            List<Intrasitc> intrasitcs = new List<Intrasitc>();
            intransitViewModel.intrasitcList = intrasitcs;
            intransitViewModel.intrasitc = new Intrasitc();
            try
            {
                //var data = _IntrasitHelper.GetAllIntrasit();
                //for (int i = 0; i < data.Count; i++)
                //{
                //    Intrasitc objDate = new Intrasitc();
                //    objDate.Id = data[i].Id;
                //    objDate.Amt = data[i].Amt;
                //    objDate.Qty = data[i].Qty;
                //    objDate.ETA = data[i].ETA;
                //    objDate.MaterialDescription = data[i].MaterialDescription;
                //    objDate.Branch = data[i].Branch;
                //    objDate.ItemCode = data[i].Item_Code;
                //    objDate.SubItemName = data[i].SubItem_Name;
                //    objDate.SubItemCode = data[i].SubItem_Code;
                //    objDate.PurchaseOrder = data[i].PurchaseOrder;
                //    intrasitcs.Add(objDate);
                //}
                var listBranch = _IntrasitHelper.GetAllBranches();
                var listCompany = _IntrasitHelper.GetAllCompany();
                var listItem = _IntrasitHelper.GetAllItem();
                listCompany.Insert(0, new CompanyDb { Id = 0, CompanyName = "Select" });
                listBranch.Insert(0, new Branch { Id = 0, BranchName = "Select" });
                listItem.Insert(0, new ItemDb { Id = 0, ItemName = "Select" });
                ViewBag.ListofCompany = listCompany;
                ViewBag.listBranch = listBranch;
                ViewBag.listItem = listItem;
                //intransitViewModel.listSenderBranch = listBranch.ToList();
                //intransitViewModel.listSenderCompany = listCompany.ToList();
                // intrasitcs = _mapper.Map<List<Intrasitc>>(data);
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView(intransitViewModel);
            //return View();
        }

        // POST: CompaniesController/Create
        [HttpPost]
        public JsonResult Create([FromBody] IntransitViewModel intransitViewModel)
        {
            try
            {
                // var intrasit = _mapper.Map<IntrasitDb>(intrasitc);
                var branch = _workContext.GetCurrentBranch().Result;
                foreach (var item in intransitViewModel.intrasitcList)
                {
                    IntrasitDb intrasitDb = new IntrasitDb();
                    intrasitDb.Sender_Branch = item.Branch;
                    intrasitDb.PurchaseOrder = item.PurchaseOrder;
                    intrasitDb.Sender_Company = item.SenderCompany;
                    intrasitDb.SubItem_Name = item.SubItemName;
                    intrasitDb.SubItem_Code = item.subItemCodeVal;
                    intrasitDb.Material_Description = item.MaterialDescription;
                    intrasitDb.Unit = item.Unit;
                    intrasitDb.Amt = item.Amt;
                    intrasitDb.Qty = item.Qty;
                    intrasitDb.Item_Code = item.ItemCode;
                    intrasitDb.Login_Branch = branch.BranchCode;
                    _IntrasitHelper.CreateNewIntrasit(intrasitDb);
                }
                return Json(new { success = true, message = "Saved Successfully" });
                // return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Not Saved Successfully" });
                //return View(intrasitc);
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostMyUploader(IFormFile importFile)
        {
            if (importFile == null) return Json(new { Status = 0, Message = "No File Selected" });
            
            try
            {

                //Getting FileName
                var fileName = Path.GetFileName(importFile.FileName);
                //Getting file Extension
                var fileExtension = Path.GetExtension(fileName);
                // concatenating  FileName + FileExtension
                var newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);
                DataSet ds = new DataSet();
                using (var target = new MemoryStream())
                {
                    importFile.CopyTo(target);
                    using (var reader = ExcelReaderFactory.CreateReader(target))
                    {
                        ds = reader.AsDataSet();
                    }
                    //var fileData = GetDataFromCSVFile(ds);
                   
                }

              var status =  await GetDataFromCSVFile(ds);
                if(!status.status)
                {
                    return Json(new { Status = 0, Message = status.message });
                }
                //var dtEmployee = fileData.ToDataTable();
                //var tblEmployeeParameter = new SqlParameter("tblEmployeeTableType", SqlDbType.Structured)
                //{
                //    TypeName = "dbo.tblTypeEmployee",
                //    Value = dtEmployee
                //};
                //await _dbContext.Database.ExecuteSqlCommandAsync("EXEC spBulkImportEmployee @tblEmployeeTableType", tblEmployeeParameter);
                return Json(new { Status = 1, Message = "File Imported Successfully " });
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, Message = ex.Message });
            }
        }
        private async Task<(bool status,string message)> GetDataFromCSVFile(DataSet ds)
        {
            bool status = true;
            try
            {
                DataTable dt = new DataTable();

                dt = AddColumnsToReadFromExcelSheet(dt);
                dt = CopyDataFromExcel(ds, dt);

                var listOfExcelItem = CopyDataFromExcel(dt);
                var listOfSubItem = listOfExcelItem.Select(x => x.SubItemName).ToList();
                var items = _IntrasitHelper.GetItemSubItemDetails();
                StringBuilder sb = new StringBuilder();
                bool intialStatus = true;
               foreach(var subItem in listOfSubItem.Distinct())
                {
                    if(!items.Any(x=>x.SubItemName == subItem.Trim()))
                    {
                        intialStatus = false;
                        sb.AppendLine(subItem + " - Sub Item Name Not Found.");
                    }
                }
               if(!intialStatus)
                {
                    sb.AppendLine("Please verify excel and reupload again.");
                    return (intialStatus, sb.ToString());
                }
                DataTable dtSerialMapping = new DataTable();
                dtSerialMapping = CopySerialMappingDataToAnotherTable(dtSerialMapping, dt);
                List<ItemWiseQty> lstItemWiseQty = GetQtyGroupBySubItem(dt);

                

                DataTable dtFinalTable = new DataTable();

                dt = RemoveUnwantedColumns(dt);
                dt = RemoveDuplicateRows(dt);

                var branch = _workContext.GetCurrentBranch().Result;
                var companyName = _IntrasitHelper.GetAllCompany().Find(x => x.Id == branch.CompanyId).CompanyName;

                dtFinalTable = PrepareFinalData(dtFinalTable, dt, lstItemWiseQty, companyName);


                //var email = User.Identity.Name;
                // var loggedinUser = await _userManager.FindByEmailAsync(email);
                // var userProfile = _userProfileService.GetByUserId(loggedinUser.Id);



                _IntrasitHelper.blukUpload(dtFinalTable, dtSerialMapping, recvDate: DateTime.Now, loginBranch: branch.BranchName, companyName);

                // _IntrasitHelper.blukUpload(dtFinalTable, dtSerialMapping, recvDate: DateTime.Now, loginBranch: "Test", "1025");

              var list =  await PrepareDataForHitachi(dtFinalTable, lstItemWiseQty);

                foreach (var listItem in list)
                {
                    var result = await _hitachiConnection.SubmitGrnNotification(JsonConvert.SerializeObject(listItem));
                    if (result.Status.ToLower() == "failure")
                    {
                        status = false;
                    }
                }


            }
            catch (Exception ex)
            {
                // throw;
                return (false, ex.Message); 
            }
            return (status,"Something is wrong please try again.");
        }

        #region classes need to move



        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Extras
        {
            public string invoice_num { get; set; }
            public string waybill_num { get; set; }
            public string received_date { get; set; }
        }

        public class Product
        {
            public string product_sku { get; set; }
            public int quantity { get; set; }
            public string bucket { get; set; }
            public string line_item_id { get; set; }
        }

        public class GrnNotificationData
        {
            public string fulfillment_center { get; set; }
            public string source_number { get; set; }
            public List<Product> products { get; set; }
            public Extras extras { get; set; }
        }

        public class OrderNotificationData
        {
            public List<Order> orders { get; set; }
        }

        public class Order
        {
            public string order_number { get; set; }
            public string fulfillment_center { get; set; }
            public string status { get; set; }
            public List<Shipments> shipments { get; set; }
        }

        public class Shipments
        {
            public string shipment_number { get; set; }
            public string waybill { get; set; }
            public DateTime created_at { get; set; }
            public string courier { get; set; }
            public DateTime dispathed_at { get; set; }
            public DateTime delivered_at { get; set; }

            public Extras extras { get; set; }

            public List<OrderLine> order_lines { get; set; }

        }

        public class OrderLine
        {
            public string order_line_number { get; set; }
            public List<Products> products { get; set; }

        }

        public class Products
        {
            public string prod_sku { get; set; }
            public int prod_qty { get; set; }
            public Extras extras { get; set; }
        }

        public class ItemWiseQty
        {
            public string SubItemName { get; set; }
            public int Qty { get; set; }
            public string InvoiceNumber { get; set; }
        }
        #endregion
        private async void UpdateHitachiForInTrasit()
        {
            // URL to which you want to send the data
            string apiUrl = "https://example.com/api/endpoint";

            // Data to be sent in the request body (you can replace this with your actual data)
            string postData = "{\"key1\":\"value1\",\"key2\":\"value2\"}";

            // Create an instance of HttpClient
            using (HttpClient client = new HttpClient())
            {
                // Set the content type of the request body
                StringContent content = new StringContent(postData, Encoding.UTF8, "application/json");

                // Send the POST request
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                // Check if the request was successful (status code 2xx)
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Request was successful!");
                    // Optionally, you can read and display the response content
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response Content: {responseContent}");
                }
                else
                {
                    Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                }
            }
        }

        private DataTable PrepareFinalData(DataTable dtFinalTable, DataTable Copy, List<ItemWiseQty> lstItemWiseQty, string CompanyName)
        {
            dtFinalTable = CreateFinalTable();

            var items = _IntrasitHelper.GetItemSubItemDetails();


            foreach (DataRow dr in Copy.Rows)
            {
                DataRow drFinal = dtFinalTable.NewRow();
                drFinal["Branch"] = dr["FulfillmentCenter"];
                drFinal["Sender_Company"] = CompanyName;
                drFinal["PurchaseOrder"] = dr["SourceNumber"];
                drFinal["SubItem_Name"] = dr["SubItemName"];
                //drFinal["SerialNumber"] = dr["SerialNumber"];
                drFinal["Way_Bill_Number"] = dr["WayBillNo"];
                drFinal["Source_Number"] = dr["SourceNumber"];
                drFinal["Bucket"] = dr["Bucket"];
                drFinal["Line_Item_Id"] = dr["LineItemId"];
                var itemSubItem = items.Find(x => x.SubItemName == dr["SubItemName"].ToString());
                drFinal["Item_Code"] = itemSubItem.ItemCode;
                drFinal["SubItem_Code"] = itemSubItem.SubItemCode;
                drFinal["Material_Description"] = itemSubItem.MaterialDescription;
                var qty = lstItemWiseQty.Find(x => x.SubItemName == dr["SubItemName"].ToString() && x.InvoiceNumber == dr["InvoiceNumber"].ToString()).Qty;
                if (itemSubItem.ItemCode == "SPLIT AC")
                {
                    drFinal["Qty"] = qty / 2;
                }
                else
                {
                    drFinal["Qty"] = qty;
                }
                drFinal["Unit"] = qty;
                dtFinalTable.Rows.Add(drFinal);
            }
            return dtFinalTable;
        }


        private async Task<List<GrnNotificationData>> PrepareDataForHitachi(DataTable dt, List<ItemWiseQty> lstItemWiseQties)
        {
            var groupedData = from row in dt.AsEnumerable()
                              group row by new
                              {
                                  SubItemName = row.Field<string>("SubItem_Name"),
                                  WayBillNo = row.Field<string>("Way_Bill_Number"),
                                  FulfillmentCenter = row.Field<string>("Branch"),
                                  SourceNumber = row.Field<string>("Source_Number"),
                                  InvoiceNumber = row.Field<string>("PurchaseOrder"),
                              }
                           into grouped
                              select new
                              {
                                  GroupKey = grouped.Key,
                                  Count = grouped.Count()
                              };
            List<GrnNotificationData> lstGrnNotificationData = new List<GrnNotificationData>();
            // Displaying the results
            foreach (var group in groupedData)
            {
                //get data against that invoice number and get only subitemname,qty,bucket,lineitemid,

                DataTable dtTemp = dt.DefaultView.ToTable(false, "SubItem_Name", "Bucket", "Line_Item_Id", "PurchaseOrder");


                List<Product> lstProducts = new List<Product>();

                foreach (DataRow dr in dtTemp.Rows)
                {
                    Product p = new Product
                    {
                        bucket = dr["Bucket"].ToString(),
                        product_sku = dr["SubItem_Name"].ToString(),
                        line_item_id = dr["Line_Item_Id"].ToString(),
                        quantity = lstItemWiseQties.Find(x => x.SubItemName == dr["SubItem_Name"].ToString()).Qty
                    };
                    lstProducts.Add(p);
                }

                GrnNotificationData grnData = new GrnNotificationData
                {
                    fulfillment_center = group.GroupKey.FulfillmentCenter,
                    source_number = group.GroupKey.SourceNumber,
                    extras = new Extras { invoice_num = group.GroupKey.InvoiceNumber, received_date = DateTime.Today.ToString(), waybill_num = group.GroupKey.WayBillNo },
                    products = lstProducts

                };

                lstGrnNotificationData.Add(grnData);


                // Add other columns here if needed

                var jsonValue = System.Text.Json.JsonSerializer.Serialize(lstGrnNotificationData[0]);
               // await _hitachiConnection.SendGRNToHitachi("");
              
            }

            return lstGrnNotificationData;
        }

        private List<ItemWiseQty> GetQtyGroupBySubItem(DataTable dt)
        {
            var groupedData = from row in dt.AsEnumerable()
                              group row by new
                              {
                                  SubItemName = row.Field<string>("SubItemName"),
                                  // ItemCode = row.Field<string>("ItemCode"),
                                  InvoiceNumber = row.Field<string>("InvoiceNumber")
                                  // Add other columns you want to group by
                              }
                           into grouped
                              select new
                              {
                                  GroupKey = grouped.Key,
                                  Count = grouped.Count()
                              };
            List<ItemWiseQty> lstItemsWiseQty = new List<ItemWiseQty>();
            // Displaying the results
            foreach (var group in groupedData)
            {
                ItemWiseQty itemWiseQty = new ItemWiseQty
                {
                    SubItemName = group.GroupKey.SubItemName,
                    InvoiceNumber = group.GroupKey.InvoiceNumber,
                    Qty = group.Count
                };

                lstItemsWiseQty.Add(itemWiseQty);


                // Add other columns here if needed
            }

            return lstItemsWiseQty;
        }

        private DataTable CreateFinalTable()
        {
            DataTable dtFinalTable = new DataTable();
            dtFinalTable.Columns.Add("Sender_Company", typeof(string));//fulfilment center
            dtFinalTable.Columns.Add("Branch", typeof(string));
            dtFinalTable.Columns.Add("PurchaseOrder", typeof(string));//invoice number
            dtFinalTable.Columns.Add("SubItem_Name", typeof(string));
            dtFinalTable.Columns.Add("Qty", typeof(string));
            dtFinalTable.Columns.Add("Way_Bill_Number", typeof(string));
            dtFinalTable.Columns.Add("Source_Number", typeof(string));
            dtFinalTable.Columns.Add("Line_Item_Id", typeof(string));
            dtFinalTable.Columns.Add("Bucket", typeof(string));
            dtFinalTable.Columns.Add("Item_Code", typeof(string));
            dtFinalTable.Columns.Add("SubItem_Code", typeof(string));
            dtFinalTable.Columns.Add("Material_Description", typeof(string));
            dtFinalTable.Columns.Add("Unit", typeof(string));

            return dtFinalTable;
        }
        private DataTable CopySerialMappingDataToAnotherTable(DataTable dtSerialMapping, DataTable dt)
        {


            dtSerialMapping.Columns.Add("SubItemName", typeof(string));
            dtSerialMapping.Columns.Add("SerialNumber", typeof(string));
            dtSerialMapping.Columns.Add("Fifo", typeof(string));


            dtSerialMapping = dt.DefaultView.ToTable(false, "SubItemName", "SerialNumber", "Fifo");
            dtSerialMapping.Columns["SubItemName"].ColumnName = "SubItem_Name";
            dtSerialMapping.Columns["SerialNumber"].ColumnName = "Serial_Number";
            dtSerialMapping.Columns.Add("Id", typeof(int));
            return dtSerialMapping;

            //dt.AsEnumerable().All(row => { dtSerialMapping.Rows.Add(); return true; });
        }
        private DataTable CopyDataFromExcel(DataSet ds, DataTable dt)
        {
            DataTable dt2 = ds.Tables["Sheet1"];

            foreach (DataRow dr in dt2.Rows)
            {
                dt.Rows.Add(dr.ItemArray);
            }


            dt.Rows[0].Delete();
            return dt;
        }

        private List<ExcelData> CopyDataFromExcel(DataTable dt)
        {
            List<ExcelData> list = new();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add(new ExcelData
                {
                    FulfillmentCenter = Convert.ToString(dt.Rows[i][0]),
                    InvoiceNumber = Convert.ToString(dt.Rows[i][1]),
                    SubItemName = Convert.ToString(dt.Rows[i][2]),
                    SerialNumber = Convert.ToString(dt.Rows[i][3]),
                    Fifo = Convert.ToString(dt.Rows[i][4]),
                    WayBillNo = Convert.ToString(dt.Rows[i][5]),
                    SourceNumber = Convert.ToString(dt.Rows[i][6]),
                    Bucket = Convert.ToString(dt.Rows[i][7])
                });
            }

            return list;
        }
        private DataTable AddColumnsToReadFromExcelSheet(DataTable dt)
        {
            dt.Columns.Add("FulfillmentCenter", typeof(string));//fulfilment center is the sender branch
            dt.Columns.Add("InvoiceNumber", typeof(string));//invoice number
            dt.Columns.Add("SubItemName", typeof(string));
            dt.Columns.Add("SerialNumber", typeof(string));
            dt.Columns.Add("Fifo", typeof(string));
            dt.Columns.Add("WayBillNo", typeof(string));
            dt.Columns.Add("SourceNumber", typeof(string));
            dt.Columns.Add("Bucket", typeof(string));
            dt.Columns.Add("LineItemId", typeof(string));


            return dt;
        }

        private DataTable RemoveUnwantedColumns(DataTable dt)
        {
            dt.Columns.Remove("SerialNumber");
            dt.Columns.Remove("Fifo");
            return dt;
        }

        private DataTable RemoveDuplicateRows(DataTable dt)
        {
            // Identify duplicate rows based on all columns
            var duplicateRows = dt.AsEnumerable()
                .GroupBy(row => string.Join(",", row.ItemArray))
                .Where(group => group.Count() > 1)
                .SelectMany(group => group.Skip(1));

            // Remove duplicate rows
            foreach (var row in duplicateRows.ToList())
            {
                dt.Rows.Remove(row);
            }

            return dt;
        }
        [HttpGet]
        public JsonResult GetSubItem(int subItemId)
        {
            var data = _IntrasitHelper.GetSubItem(subItemId);
            return Json(data);
        }//GetMaterialDesc

        [HttpGet]
        public JsonResult GetMaterialDesc(string subItemId)
        {
            var data = _IntrasitHelper.GetSubItemTitle(subItemId);
            return Json(data);
        }


        public IActionResult DownloadFile()
        {
            //var memory = DownloadSingleFile("IntransitTemplate.xlsx", "wwwroot\\template");
            //return File(memory.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "IntransitTemplate.xlsx");
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "IntransitTemplate.xlsx";

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet =
                workbook.Worksheets.Add("Sheet1");
                worksheet.Cell(1, 1).Value = "LoginBranchId";
                worksheet.Cell(1, 2).Value = "SenderCompanyId";
                worksheet.Cell(1, 3).Value = "Branch";
                worksheet.Cell(1, 4).Value = "PurchaseOrder";
                worksheet.Cell(1, 5).Value = "ItemCode";
                worksheet.Cell(1, 6).Value = "SubItemCode";
                worksheet.Cell(1, 7).Value = "SubItemName";
                worksheet.Cell(1, 8).Value = "MaterialDescription";
                worksheet.Cell(1, 9).Value = "Qty";
                worksheet.Cell(1, 10).Value = "Unit";
                worksheet.Cell(1, 11).Value = "Amt";
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
            }

        }

        private MemoryStream DownloadSingleFile(string filename, string uploadPath)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), uploadPath, filename);
            var memory = new MemoryStream();
            if (System.IO.File.Exists(path))
            {
                var net = new System.Net.WebClient();
                var data = net.DownloadData(path);
                var content = new System.IO.MemoryStream();
                memory = content;
            }
            memory.Position = 0;
            return memory;
        }

        public ActionResult Delete(int id)
        {
            var data = _IntrasitHelper.DeleteIntrasitById(id);
            //_context.Employees.Remove(data);
            //_context.SaveChanges();
            //ViewBag.Messsage = "Record Delete Successfully";
            return RedirectToAction("index");
        }
    }

    public  class ExcelData
    {
        public string FulfillmentCenter { get; set; }
        public string InvoiceNumber { get; set; }
        public string SubItemName { get; set; }
        public string SerialNumber { get; set; }
        public string Fifo { get; set; }
        public string WayBillNo { get; set; }
        public string SourceNumber { get; set; }
        public string Bucket { get; set; }

    }
}
