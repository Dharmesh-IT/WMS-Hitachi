

//$(function () {
//    var docType = $("#doctype").val();
//    $("#grn").select2({
//        theme: 'bootstrap4',
//        /* minimumInputLength: 2,*/
//        /*templateResult: formatState, //this is for append country flag.*/
//        ajax: {
//            url: '/PickSlip/PoList?docType=' + docType,
//            dataType: 'json',
//            type: "GET",
//            //data: function (term) {
//            //    return {
//            //        term: term
//            //    };
//            //},
//            processResults: function (data) {
//                /*console.log(data);*/
//                return {
//                    results: $.map(data, function (item) {
//                        console.log(item);
//                        return {
//                            text: item.GRNId,
//                            id: item.GRNId,
//                        }
//                    })
//                };
//            }

//        }
//    });


//});
var selectedListItemArray = [];
$(document).ready(function () {
    var sn = 1;
    var totalItem = 0;
    $("#doctype").change(function () {
        console.log("doctype change");
        PoNumber($(this).val());
    });
    $("#grn").change(function () {
        itemList($(this).val(), $("#doctype").val());
    });
    $("#items").change(function () {
        console.log($("#items option:selected").data('areaid'));
        area($("#items option:selected").data('areaid'));
    });
    $("#save").click(function () {
        var items = $('#pick-grid').data().kendoGrid.dataSource.data();
        console.log(items);
       
        if (items.length > 0) {
            if (items.length == totalItem) {
                var list = [];
                for (var i = 0; i < items.length; i++) {
                    var location = {};
                    location["Amount"] = items[i].Amount;
                    location["AreaId"] = items[i].AreaId;
                    location["Id"] = items[i].Id;
                    location["InventoryId"] = items[i].InventoryId;
                    location["ItemCode"] = items[i].ItemCode;
                    location["Location"] = items[i].Location;
                    location["MaterialDescription"] = items[i].MaterialDescription;
                    location["POId"] = items[i].POId;
                    location["Qty"] = items[i].Qty;
                    location["SubItemCode"] = items[i].SubItemCode;
                    location["SubItemName"] = items[i].SubItemName;
                    location["invoiceNumber"] = items[i].invoiceNumber;
                    location["Address1"] = items[i].Address1;
                    location["Unit"] = items[i].Unit;
                    location["DockType"] = $("#doctype").val();
                    list.push(location);
                    console.log(items[i].AreaId);
                }
                console.log(list);
                var settings = {
                    "url": "/PickSlip/Save",
                    "method": "POST",
                    "timeout": 0,
                    "headers": {
                        "Content-Type": "application/json"
                    },
                    "data": JSON.stringify(list),
                };

                $.ajax(settings).done(function (response) {
                    hideloading();
                    window.location = "/PickSlip/PickSlipList";
                }).fail(function () {

                    hideloading();
                    alert("something went wrong please try again.");
                });
            }
            else {
                toastr.error('Selected Items are not equal to Sales Order Item. Please try again.');
            }
        }
        else {
            alert("Please Add item in list.");
            return false;
        }
    });
    $("#add-new").click(function () {
        var grnid = $("#grn").val();
        var category = $("#doctype").val();
        $.ajax({
            type: "GET",
            url: "/PickSlip/GetPoProduct?id=" + grnid + "&docType=" + category,
            data: "{}",
            success: function (data) {
                console.log(data);
                if (data.length > 0) {
                    var grid = $("#pick-grid").data("kendoGrid");
                    for (var i = 0; i < data.length; i++) {
                        grid.dataSource.add(data[i]);
                    }
                    totalItem = data.length;
                    toastr.success('Item  successfully added into list.');
                }
                else {
                    toastr.error('No records');
                }
                
            }
        });

    });
    var initialLoad = true;
    $("#pick-grid").kendoGrid({


        dataSource: {

            transport: {
                read: {
                    url: "/PickSlip/List",
                    type: "POST",
                    dataType: "json",
                    data: additionalData,
                    complete: function (result) {
                        console.log("Remote built-in transport", result);
                        if (result.status == 401) {
                            /* document.location.href = "@Html.Raw(Url.Action("Index", "AccessDenied"))";*/
                        }
                    }

                }
            },
            schema: {
                total: "Total",
                errors: "Errors",
                data: "Data",
                model: {
                    id: "Id",
                    field: {
                        POId: { editable: false },
                        SubItemCode: { editable: false },
                        SubItemName: { editable: false },
                        MaterialDescription: { editable: false },
                        line_itemId: { editable: false },
                        AreaId: { editable: false },
                        Qty: { editable: false },
                        ChangeLocation: {editable: false}
                        //Location: {
                        //    defaultValue: {
                        //        AreaId: 0,
                        //        Location: ""
                        //    }
                        //}
                    }
                },

            },
            error: function (e) {
                //display_kendoui_grid_error(e);
                // Cancel the changes
                console.log(e)
                this.cancelChanges();
            },
            pageSize: 20,
            sortable: true,
            serverPaging: true,
            serverSorting: true,
            requestStart: function () {
                if (initialLoad) //<-- if it's the initial load, manually start the spinner
                    kendo.ui.progress($("#booking-grid"), true);
            },
            requestEnd: function () {
                if (initialLoad)
                    kendo.ui.progress($("#booking-grid"), false);
                initialLoad = false; //<-- make sure the spinner doesn't fire again (that would produce two spinners instead of one)

            },

        },
        //selectable: 'raw',
        //change: onChange,
        height: 350,
        pageable: {
            refresh: true,
            pageSizes: true
        },
        editable: "incell",
        scrollable: false,
        columns: [
            {
            hidden: true,
            field: "Id",
           // width: 50
        },
            {
            hidden : true,
            field: "POId",
           // width: 100
        },
            {
                hidden: true,
                field: "invoiceNumber",
            },
            {
                hidden: true,
                field: "Address1",
            },
        {
            field: "SubItemCode",
            title: "SubItemCode",
            width: 100
        },

            {
                field: "line_itemId",
                title: "Line Item Id",
                width: 100

            },

            {
                field: "MaterialDescription",
                title: "Material Description",
                width: 100

            }
            ,
        {
            field: "SubItemName",
            title: "SubItemName",
            width: 120
        },
        {
            field: "AreaId",
            title: "AreaId",
        },
        {
            field: "Location",
            title: "Location",
            width: 300,
           /* template: "<input type='button' value='Add Location' onclick='OpenAddLocation()' />"*/
            },
            {
                title: "Remove Sku From Bin",
                width: 300,
                template: clientLocationDialog
            },
        {
            field: "Qty",
            title: "Qty",
            encoded: false,
            width: 120
        },
        {
            field: "InventoryId",
            title: "InventoryId",

        },
        { command: "destroy", title: "&nbsp;", width: 120 }
            //{
            //    field: "Unit",
            //    title: "Unit",
            //    width: 100
            //},


            //{
            //    field: "Amt",
            //    title: "Amt",
            //    width: 100
            //},


        ],

    });
    //$(document).on('click', '.btnToAddLocation', function (e) {
    //    var grid = $("#pick-grid").data("kendoGrid");
    //    var dataItem = grid.dataItem($(this).closest('tr'));
    //    console.log("dataItem" + dataItem);
    //    var ID = dataItem.Id;
    //    console.log("id" + ID);
    //    selectedListItemArray.push(dataItem);
    //});
   
   
});
var dialog = $('#dialog');
dialog.kendoDialog({
    title: 'Get sku removed from BIN',
    height: '50%',
    width: '50%',
    position: ['top', 70],
    draggable: false,
    show: 'blind',
    hide: 'blind',
    modal: true,
    resizable: false,
    visible: false,
    //content: "<p>A new version of <strong>Kendo UI</strong> is available. Would you like to download and install it now?<p>",
    open: function () {
        var initialLoadDialog = true;
        $("#gridForLocation").kendoGrid({
            dataSource: {
                transport: {
                    read: {
                        type: "GET",
                        url: "/PickSlip/GetLocationForGrid?SubItemCode=" + selectedSubItemCode,
                        dataType: "json",
                        complete: function (result) {
                            console.log("Remote built-in transport", result);
                            if (result.status == 401) {
                                /* document.location.href = "@Html.Raw(Url.Action("Index", "AccessDenied"))";*/
                            }
                        }

                    }
                },
                schema: {
                    total: "Total",
                    errors: "Errors",
                    data: "Data",
                    model: {
                        field: {
                            Location: { editable: false },
                            AreaId: { editable: false },
                            InventoryId: { editable: false }
                        }
                    },

                },
                error: function (e) {
                    //display_kendoui_grid_error(e);
                    // Cancel the changes
                    console.log(e)
                    this.cancelChanges();
                },
                pageSize: 20,
                sortable: true,
                serverPaging: true,
                serverSorting: true,
                requestStart: function () {
                    if (initialLoadDialog) //<-- if it's the initial load, manually start the spinner
                        kendo.ui.progress($("#booking-grid"), true);
                },
                requestEnd: function () {
                    if (initialLoadDialog)
                        kendo.ui.progress($("#booking-grid"), false);
                    initialLoadDialog = false; //<-- make sure the spinner doesn't fire again (that would produce two spinners instead of one)

                },

            },
            //selectable: 'raw',
            //change: onChange,
            height: 600,
            pageable: {
                refresh: true,
                pageSizes: true
            },
            editable: "incell",
            scrollable: false,
            columns: [
                {
                    field: "Location",
                    title: "Location",
                    width: 120
                },
                {
                    field: "Fifo",
                    title: "Fifo",
                    width: 120
                },
                {
                    field: "ReceivedDate",
                    title: "Received Date",
                    width: 120
                },

                {
                    hidden: true,
                    field: "AreaId",
                    title: "AreaId",
                },
                {
                    hidden: true,
                    field: "InventoryId",
                    title: "InventoryId",

                }, {
                    field: "ChangeLocation",
                    title: "Change Location",
                    width: 300,
                    template: returnSelecthtml
                },
                //{ command: "destroy", title: "&nbsp;", width: 120 }
                //{
                //    field: "Unit",
                //    title: "Unit",
                //    width: 100
                //},


                //{
                //    field: "Amt",
                //    title: "Amt",
                //    width: 100
                //},


            ],

        });

    },
    close: function (event, ui) {
        $("#gridForLocation").empty();
        selectedSubItemCode = '';
        gridRowUid = '';
    },
    //actions: [
    //    { text: 'Skip this version' },
    //    { text: 'Remind me later' },
    //    { text: 'Install update', primary: true }
    //]
});
var selectedSubItemCode = '';
var gridRowUid = '';
function clientLocationDialog(container) {
    var subItemCode = container.SubItemCode;
    console.log(container.id);
    var buttonText = '<input type="button" value="Remove Sku" class="btnToAddLocation" onclick="openDialog(event,\'' + subItemCode + '\',\'' + container.uid + '\')">';
    return buttonText;
}
function returnSelecthtml(container) {
    var areaId = container.AreaId;
    var location = container.Location;
    var inventoryId = container.InventoryId;
    var buttonText = '<input type="button" value="Select" onclick="selectAreaAndLocation(\'' + location + '\',\'' + areaId + '\',\'' + inventoryId + '\')">';
    return buttonText;
}
function selectAreaAndLocation(location, areaId, inventoryId) {
    console.log("location " + location + " areaId : " + areaId + " inverntoryId : " + inventoryId);
    var dataItems = $("#pick-grid").data("kendoGrid").dataSource.data();
    let dataItem = null;
    $.each(dataItems, function (index, item) {
        if (item.uid === gridRowUid) {
            dataItem = item;
        }
    });
    if (dataItem != null) {
        dataItem.set("Location", location);
        dataItem.set("AreaId", areaId);
        dataItem.set("InventoryId", inventoryId);
    }
    
    $('#dialog').data("kendoDialog").close();
}
function clientLocationEditor(container, options) {

    $('<input required name="' + options.field + '">')
        .appendTo(container)
        .kendoDropDownList({
            dataTextField: "Location",
            dataValueField: "Location",
            //template: "<div class='dropdown-country-wrap'><img src='../content/web/country-flags/#:CountryNameShort#.png' alt='#: CountryNameLong#' title='#: CountryNameLong#' width='30' /><span>#:CountryNameLong #</span></div>",
            change: function (e) {
                //var ddl = this;
                //var value = ddl.dataItem().value;
                //var editedRow = ddl.element.closest("tr");
                var grid = $('#grid').data('kendoGrid');
                //var model = grid.editable.options.model;
                console.log(e.sender.dataItem().AreaId);
                options.model.set("AreaId", e.sender.dataItem().AreaId);
                options.model.set("InventoryId", e.sender.dataItem().InventoryId);

                //model.set("AreaId", value);
            },
            dataSource: {
                dataValueField: "Location",
                transport: {
                    read: {
                        url: "/PickSlip/GetLocation?SubItemCode=" + options.model["SubItemCode"],
                        dataType: "json"
                    }
                }
            },
            autoWidth: true
        });
}
function openDialog(e, subItemCode, rowId) {
    selectedSubItemCode = subItemCode;
    gridRowUid = rowId;
    e.preventDefault();
    dialog.data("kendoDialog").open();
}
function additionalData() {
    var data = {


    }
    addAntiForgeryToken(data);
    return data;
}



function itemList(id, doctype) {
    $.ajax({
        type: "GET",
        url: "/PickSlip/GetPoProduct?id=" + id + "&docType=" + doctype,
        data: "{}",
        success: function (data) {
            var s = '';
            for (var i = 0; i < data.length; i++) {
                s += '<option value="' + data[i].Id + '" data-code=' + data[i].SubItemCode + ' data-itemname=' + data[i].SubItemName +
                    ' data-areaid=' + data[i].AreaId + ' >' + data[i].SubItemName + '</option>';
            }
            $("#items").html(s);
            $("#items").trigger("change");
        }
    });

    /*area($("#items option:selected").data('areaid'));*/
}
function area(areaId) {
    console.log(areaId);
    $.ajax({
        type: "GET",
        url: "/PickSlip/GetLocation?areaId=" + areaId,
        data: "{}",
        success: function (data) {
            console.log(data);
            $("#location").val(data);

        }
    });

}

function PoNumber(docType) {

    $.ajax({
        type: "GET",
        url: "/PickSlip/PoList?docType=" + docType,
        data: "{}",
        success: function (data) {
            console.log(data);
            var s = '';
            for (var i = 0; i < data.length; i++) {
                s += '<option value="' + data[i].PoNumber + '" >' + data[i].PoNumber + '</option>';
            }
            $("#grn").html(s);
            $("#grn").trigger("change");
        }
    });
}
function pleaseWait() {
    $('#pleaseWait').modal({
        backdrop: 'static',
        keyboard: false
    })
}

function hideloading() {
    $('#pleaseWait').on('shown.bs.modal', function (e) {
        $(this).hide();
        $('.modal').modal('hide');
    })
}