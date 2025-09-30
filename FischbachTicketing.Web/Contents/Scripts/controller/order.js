$(document).ready(function () {
    $.noConflict();
    GetEmployee($('#hdnEmployeeID').val());
    $('#cmbOrderTemplate').on('change', function (event) {
        if ($('#cmbOrderTemplate').val() != '') {
            clearForm();
            $('#divOrder').show();            
            if ($('#txtWorkOrder').val() <= 0) {
                $('#divItemList').hide();
            }
            else {
                $('#divItemList').show();
            }           
            $('#txtWorkOrder').focus();
            $('#txtWorkOrder').select();
        }
        else {
            $('#divOrder').hide();
        }
    });

    $('#txtWorkOrder').on('keydown', function (event) {
        if (event.which == 13) {

            $('#txtWorkOrder').removeClass('is-invalid');
            if ($('#txtWorkOrder').val() <= 0) {
                $('#txtWorkOrder').addClass('is-invalid');
                $('#txtWorkOrder').focus();
                $('#txtWorkOrder').select();
                return false;
            }

            $('.spinner').css('display', 'block');
            $.ajax({
                type: "POST",
                url: '/Order/GetWorkOrderData/',
                data: { workOrderID: $('#txtWorkOrder').val() },
                cache: false,
                success: function (response) {
                    if (response.status == true) {
                        $('.spinner').css('display', 'none');
                        $('.input-number').val(0);
                        workOrder = response.workOrder;
                        $('#txtItemNo').val(workOrder.ITEMNO);
                        $('#txtPrintLine').val(workOrder.WORKCENTER_DESCRIPTION);
                        $('#txtComments').val('');
                        $('#divItemList').show();
                        if ($('#cmbOrderTemplate').val() == 'OI') {
                            $('#btnAddItem').show();
                        }

                        if ($('#cmbOrderTemplate').val() == 'OS') {
                            $('#btnAddItem').hide();
                        }
                        LoadItems();
                    }
                    else {
                        $('.spinner').css('display', 'none');
                        toastr.error(response.message);                        
                    }
                },
                failure: function (response) {
                    $('.spinner').css('display', 'none');
                    toastr.error(response.message);
                },
                error: function (response) {
                    $('.spinner').css('display', 'none');
                    toastr.error(response.message);
                }
            });

        }
    });

    $('#btnAddItem').on('click', function () {
        $('#gridItemList').jqGrid('GridUnload');
        $("#gridItemList").jqGrid
            ({
                url: "/Order/GetItems/",
                datatype: 'json',
                mtype: 'Get',
                autoencode: false,
                pageable: false,
                height: 100,
                gridview: true,
                shrinkToFit: false,
                scrollOffset: 1,
                colNames: ['Item No', 'Item Description', 'ARINVT_ID'],
                colModel: [
                    {
                        key: false,
                        name: 'ITEMNO',
                        index: 'ITEMNO',
                        editable: false,
                        sortable: false,
                        width: 400,
                        align: "left",
                        search: true
                    },
                    {
                        key: false,
                        name: 'DESCRIPTION',
                        index: 'DESCRIPTION',
                        editable: false,
                        sortable: false,
                        width: 500,
                        align: "left",
                        search: true
                    },                    
                    {
                        key: false,
                        name: 'ID',
                        index: 'ID',
                        editable: false,
                        sortable: false,
                        width: 130,
                        hidden: true
                    },
                ],
                pager: jQuery('#pagerItemList'),
                rowNum: 100,
                rowList: [100, 150, 200, 250, 300],
                viewrecords: true,
                emptyrecords: 'No records to display',
                jsonReader:
                {
                    root: "rows",
                    page: "page",
                    total: "total",
                    records: "records",
                    repeatitems: false,
                    Id: "0"
                },
                autowidth: true,
                multiselect: true,
                scroll: false,
                gridComplete: function () {
                    var grid = $("#gridItemList");
                    var rows = grid.getDataIDs();
                    for (var i = 0; i < rows.length; i++) {
                        grid.jqGrid('editRow', rows[i], { keys: true });
                    }

                    $(".editable").keypress(function (e) {
                        var keyCode = e.which ? e.which : e.keyCode
                        if (!(keyCode >= 48 && keyCode <= 57 || keyCode == 46)) {
                            return false;
                        }
                    });
                },
                loadComplete: function (data) {                    
                }
            }).navGrid('#gridItemList',
                {
                    edit: false,
                    add: false,
                    del: false,
                    search: false,
                    refresh: false

                });

        $("#gridItemList").jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: true, ignoreCase: true
        });
        $('.spinner').css('display', 'none');       
        $('#ItemModal').modal({ backdrop: 'static', keyboard: false });
    });

    $('#btnModalAddItems').on('click', function () {
        $('.spinner').css('display', 'block');
        var selRowId = $('#gridItemList').jqGrid("getGridParam", "selrow");
        var itemData = $('#gridItemList').jqGrid("getRowData", selRowId);

        var rowKey = $('#gridItemList').jqGrid("getGridParam", "selrow");
        var detail = new Array();

        if (!rowKey) {
            alert("Please select Items any of them");
            $('.spinner').css('display', 'none');
            return;
        }
        else {
            var selectedIDs = $('#gridItemList').getGridParam("selarrrow");            
            var grid = $("#gridItems");
            var rows = grid.getDataIDs();

            var lastRowID;
            
            for (var i = 0; i < rows.length; i++) {                
                lastRowID = rows[i];
            }

            if (rows.length <= 0) {
                lastRowID = 1;
            }

            for (var i = 0; i < selectedIDs.length; i++) {
                var selectWorkOrderList = {};
                var itemList = $('#gridItemList').jqGrid("getRowData", selectedIDs[i]);
                var itemRow = { ID: itemList.ID, ITEMNO: itemList.ITEMNO, DESCRIPTION: itemList.DESCRIPTION };
                lastRowID++;
                grid.addRowData(lastRowID, itemRow);
                grid.jqGrid('setSelection', lastRowID, true);
            }

           
            $('.spinner').css('display', 'none');
            $('#ItemModal').modal('hide');
            $('.modal-backdrop').hide();
        }
    });

    $('#btnSubmit').on('click', function () {

        $('#txtWorkOrder').removeClass('is-invalid');
        if ($('#txtWorkOrder').val() <= 0) {
            $('#txtWorkOrder').addClass('is-invalid');
            $('#txtWorkOrder').focus();
            $('#txtWorkOrder').select();
            return false;
        }

        $('.spinner').css('display', 'block');

        var selRowId = $('#gridItems').jqGrid("getGridParam", "selrow");
        var itemData = $('#gridItems').jqGrid("getRowData", selRowId);

        var rowKey = $('#gridItems').jqGrid("getGridParam", "selrow");
        var detail = new Array();

        if (!rowKey) {           
            toastr.error("Please select Item any of them");
            $('.spinner').css('display', 'none');
            return;
        }
        else {
            var selectedIDs = $('#gridItems').getGridParam("selarrrow");
            var ticket = {};
            var itemNo;
            itemNo = '';
            for (var i = 0; i < selectedIDs.length; i++) {                
                var itemRow = $('#gridItems').jqGrid("getRowData", selectedIDs[i]);
                itemNo += itemRow.ITEMNO + "\n";
            }
            ticket.WORKORDER_ID = $('#txtWorkOrder').val();
            ticket.WORKCENTER_DESCRIPTION = $('#txtPrintLine').val();
            if ($('#chkUrgent').is(":checked")) {
                ticket.URGENT = true;
            }
            else {
                ticket.URGENT = false;
            }
            ticket.NOTES = itemNo + "\n" + $('#txtComments').val();
            ticket.ORDER_TYPE = $('#cmbOrderTemplate').val();
            ticket.ITEMNO = workOrder.ITEMNO;
        }

        $.ajax({
            type: "POST",
            url: "/Order/SaveTicket/",
            data: { ticket: ticket },
            cache: false,
            success: function (response) {
                if (response.status == true) {                                    
                    $('.spinner').css('display', 'none');                             
                    toastr.success(response.message);
                    $('#hdnTicketID').val(response.ticketID);
                    pop('PrintLabel.aspx', 100, 100);
                    clearForm();
                    $('#divOrder').show();
                }
                else {
                    $('.spinner').css('display', 'none');                    
                    toastr.error(response.message);
                }
            },
            failure: function (response) {
                $('.spinner').css('display', 'none');                
            },
            error: function (response) {
                $('.spinner').css('display', 'none');                
            }
        });
    });

    var intervalMilliseconds = 300000;
    setInterval(function () {
        $.ajax({
            url: '/Order/KeepSessionAlive/',
            type: "POST",
            datatype: "JSON",
            data: {},
            success: function (result) {
                if (result.status == true) {
                }
                else {
                }
            },
            error: function (response) {
            }
        });
    }, intervalMilliseconds);
    
});

function clearForm() {   
    $('#txtWorkOrder').val('');
    $('#txtItemNo').val('');
    $('#txtPrintLine').val('');
    $('#txtComments').val('');
    $('#chkUrgent').prop('checked', false);
    $('#divOrder').hide();
    $('#divItemList').hide();    
}

function GetEmployee(employeeID)
{
    $('.spinner').css('display', 'block');
    $.ajax({
        type: "POST",
        url: '/Order/GetEmployeeID/',
        data: { employeeID: employeeID },
        cache: false,
        success: function (response) {
            if (response.status == true) {
                $('.spinner').css('display', 'none');
                $('.input-number').val(0);
                employee = response.employee;
                $('#txtEmployeeName').val(employee.FirstName + ' ' + employee.LastName);
            }
            else {
                $('.spinner').css('display', 'none');
                toastr.error(response.message);

                setInterval(logout, 3500);
            }
        },
        failure: function (response) {
            $('.spinner').css('display', 'none');
            toastr.error(postFailLiteral);
        },
        error: function (response) {
            $('.spinner').css('display', 'none');
            toastr.error(postFailLiteral);
        }
    });
}

function LoadItems() {
    var ItemNoLiteral;
    if ($('#cmbOrderTemplate').val() == 'OI') {
        ItemNoLiteral = 'INK COLOR'
    }

    if ($('#cmbOrderTemplate').val() == 'OS') {
        ItemNoLiteral = 'SCREEN'
    }
    $('#gridItems').jqGrid('GridUnload');
    $("#gridItems").jqGrid
        ({
            url: "/Order/GetWorkOrderItemData/" + $('#txtWorkOrder').val(),
            datatype: 'json',
            mtype: 'Get',
            autoencode: false,
            pageable: true,
            gridview: true,
            shrinkToFit: false,
            scrollOffset: 1,
            colNames: [ItemNoLiteral ,'ITEM DESCRIPTION', 'ARINVT_ID'],
            colModel: [
                {
                    key: false,
                    name: 'ITEMNO',
                    index: 'ITEMNO',
                    editable: false,
                    sortable: false,
                    width: 700,
                    align: "left"
                },
                {
                    key: false,
                    name: 'DESCRIPTION',
                    index: 'DESCRIPTION',
                    editable: false,
                    sortable: false,
                    width: 400,
                    align: "left",
                    hidden: true
                },                
                {
                    key: false,
                    name: 'ID',
                    index: 'ID',
                    editable: false,
                    sortable: false,
                    width: 130,
                    hidden: true
                },                
            ],
            pager: jQuery('#pagerItems'),
            rowNum: 20,
            rowList: [10, 20, 30, 40],
            viewrecords: true,
            emptyrecords: 'No records to display',
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false,
                Id: "0"
            },
            autowidth: false,
            multiselect: true,
            scroll: false,
            gridComplete: function () {
                var grid = $("#gridItems");
                var rows = grid.getDataIDs();
                for (var i = 0; i < rows.length; i++) {
                    grid.jqGrid('editRow', rows[i], { keys: true });                         
                }

                $(".editable").keypress(function (e) {
                    var keyCode = e.which ? e.which : e.keyCode
                    if (!(keyCode >= 48 && keyCode <= 57 || keyCode == 46)) {
                        return false;
                    }
                });

                if (parseInt($('#txtWorkOrder').val()) <= 0) {
                    $('#divItemList').hide();
                }
                else {
                    $('#divItemList').show();
                }
                $('#controlPanel').show();                
            },
            loadComplete: function (data) {
                var grid = $("#gridItems");
                var rows = grid.getDataIDs();
                for (var i = 0; i < rows.length; i++) {                   
                    grid.jqGrid('setSelection', rows[i], true);
                }
            }
        }).navGrid('#pagerItems',
            {
                edit: false,
                add: false,
                del: false,
                search: false,
                refresh: false

            });
    $('.spinner').css('display', 'none');    
}

function logout() {
    var url = '/Login/Index';
    window.location.href = url;
}

function pop(url, w, h) {
    n = window.open(url, '_blank', 'toolbar=0,location=0,directories=0,status=1,menubar=0,titlebar=0,scrollbars=1,resizable=1,width=' + w + ',height=' + h);
    if (n == null) {
        return true;
    }
    return false;
}