

$(window).load(function ()
    {
        
        
    });
  
    $(document).ready(function ()
    {
        
    });
    
    var tableToExcel = (function () {
        var uri = 'data:application/vnd.ms-excel;base64,', template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table>{table}</table></body></html>', base64 = function (s) {
            return window.btoa(unescape(encodeURIComponent(s)));
        },
            format = function (s, c) {
                return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; });
            };
        return function (table, name) {
            if (!table.nodeType) {
                table = document.getElementById(table);
            }
            var ctx =
            {
                worksheet: name || 'Worksheet',
                table: table.innerHTML
            };
            window.location.href = uri + base64(format(template, ctx));
        };
    })();

    function printReportData(id, id2)
    {
        //$('#' + id).after($('<div id="repView" style="border: none"> </div>'));
        //$('#dvItemsView').remove();
        //var tempTarget = $('#' + id).after($('#dvItemsView'));

        var stylStr = '<style type="text/css">.newTable {width: 100%; color: #333;font-family: "Segoe UI",Verdana,Helvetica,Sans-Serif;padding: 0.25em 2em 0.25em 0em;}body {color: #333;font-size: 0.85em;font-family: "Segoe UI",Verdana,Helvetica,Sans-Serif;}'
            + ' .newTable th {width: auto; text-align: left;font-size: 10px;color: #008000;}.newTable tr {color: black;}fieldset {margin: 0px;padding: 0px;border: 1px solid #808080;border-radius: 3px;}.allcontent {margin-left: auto;margin-right: auto;width: 100%;height: auto;min-height: 60%;top: 30%;}'
            + '.tr2{ font-size: 10pt;background-color:green;color:#f9f9f9;}.newTable td {width: auto; text-align: left;font-size: 10px;}.customlegend22 {width: 100%;position: relative;font-weight: bold;font-size: 7pt;padding: 7px 0px 6px 7px;background-color: #008000;color: #F9F9F9;}html {background-color: #E2E2E2;margin: 0px;padding: 0px;}*, *:before, *:after {box-sizing: border-box;}</style> ';

        var xxt = '<script type="text/javascript"> $(document).ready(function (){$("#" + id2).remove();});</script>';

        $('<div id= "ftx" style="width:100%"></div>');
       
        var winpops = window.open('', "Transaction Voucher", "fullscreen=no,toolbar=no,status=yes, " +
                                   "menubar=no,scrollbars=yes,resizable=yes,directories=yes,location=no, " +
                                 "width=1100,height=500,left=100,top=100,screenX=100,screenY=100");
        winpops.document.write(stylStr + $('#' + id).html() + xxt);
        
        winpops.print();
        //$('#dvItemsView').remove();
    }
    


//******************************* Well Reports ************************************************
    function generateReports()
    {
        //tm.val().trim().toLowerCase().replace(/ /g, '');
        
        var cId1 = parseInt($('#ddlWellDynamicType').val());
        var cId2 = parseInt($('#ddlWellDynamicClass').val());
        var cId3 = parseInt($('#ddlWellDynamicField').val());
        var cId4 = parseInt($('#ddlWellDynamicTerrain').val());
        var cId5 = parseInt($('#ddlWellDynamicZone').val());
        var cId6 = $('#wellStartDate').val().trim();
        var cId7 = parseInt($('#ddlWellDynamicComp').val());
        var cId8 = parseInt($('#ddlWellCompDynamicBlock').val());
        var cId9 = $('#wellEndDate').val().trim();
         

        if (cId1 < 1 && cId2 < 1 && cId3 < 1 && cId4 < 1 && cId5 < 1 && cId6.length < 1 && cId7 < 1 && cId8 < 1 && cId9.length < 1)
        {
            alert('Please select at least one report Field.');
            $('#frmSaveWellQuery').hide('fast');
            return;
        }
        
        if (cId6.length < 1 && cId9.length > 1)
        {
            alert('Please select a starte Date.');
            $('#frmSaveWellCompletionQuery').hide('fast');
            return;
        }

        var tdc = {};

        tdc['WellTypeId'] = cId1;
        tdc['CompanyId'] = cId7; 
        tdc['FieldId'] = cId3; 
        tdc['WellClassId'] = cId2;
        tdc['TerrainId'] = cId4;
        tdc['BlockId'] = cId8;
        tdc['ZoneId'] = cId5; 
        tdc['StartDate'] = cId6;
        tdc['EndDate'] = cId9;
        
        var sdx = JSON.stringify({ queryBuilder: tdc });
        
        closeWellRepFields();
        $('#frmSaveWellQuery').fadeOut('fast');
        wellSaverAncHide();
        setModal2($('#imgWellRepoSpin'));
        
        var url = '/WellReport/GetWells';

            $.ajax({
                async: true,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: sdx,
                type: "POST",
                url: url,
                success: function (r)
                {
                    statusQuery = 0;
                    buildReport(r);
                }
                ,
                error:function() {
                    closePopModal($('#imgWellRepoSpin'));
                    $('#frmSaveWellQuery').hide('fast');
                }
            });
    }
    
    function generateWellReports2() {

        var cId = parseInt($('#ddlwellPreviousQueries').val().trim());

      if (cId < 1)
      {
            alert('Please select a valid query.');
            return;
        }
        
      var sdx = JSON.stringify({ queryId: cId });

        closeWellRepFields();
        setModal2($('#imgWellRepoSpin'));

        var url = '/WellReport/GetWells2';
        
        $.ajax({
            async: true,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: sdx,
            type: "POST",
            url: url,
            success: function (r) {
                statusQuery = 1;
                buildReport(r);
            }
        });

    }

    var statusQuery = 0;

    function prepWellRepFiledsTemplate()
    {
       setModal($('#dvSelectReportFields'));
    }

        function closeWellRepFields () 
        {
            closePopModal($('#dvSelectReportFields'));
        }
        
        function closeWellQuery () 
        {
            closePopModal($('#frmSaveWellQuery'));
        }
        
        function wellSaverAncHide()
        {
            $('#ancSaveWellQuery').fadeOut('slow');
        }
        
        function wellSaverAncShow()
        {
            $('#ancSaveWellQuery').fadeIn('slow');
        }
        
        function showWellQuerySaver()
        {
            $('#txtWellQueryName').val('');
            setModal($('#frmSaveWellQuery'));
        }
        
        function saveWellReportQuery() {
            var query = $('#txtWellQueryName').val().trim();
            if (query.length < 1)
            {
                alert('Please kindly provide a name for the query.');
                return;
            }
        
            var sdx = JSON.stringify({ queryName: query });
            var url = '/WellReport/SaveSuccessfulQuery';
        
            $.ajax({
                async: true,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: sdx,
                type: "POST",
                url: url,
                success: function (r)
                {
                    if (r.Code < 1)
                    {
                        alert(r.Error);
                        return;
                    }

                    if (r.Code > 0)
                    {
                        refreshPreviousWellQueryDropdown(r.Code);
                        alert(r.Error);
                        closeWellQuery();
                        wellSaverAncHide();
                        return;
                    }
                }
            });

        }
        
        function refreshPreviousWellQueryDropdown(k)
        {
            if (k < 1) {
                alert('An error was encountered. Previous Query list could not be refreshed.');
                return;
            }
            
            $('#ddlwellPreviousQueries option[value = 0]').text('-- Select Previous Query --');
            $('#ddlwellPreviousQueries').append($("<option />").val(k).text($('#txtWellQueryName').val().trim()));
           
        }
       
    function buildReport(data)
    {
        if (data.length < 1)
        {
            closePopModal($('#imgWellRepoSpin'));
            alert('No record found');
            return;
        }
       
        $('#tbWellReports').remove(); 
        
        var headerStr = '<table class="newTable allcontent" id="tbWellReports">' +
            '<tr class="customGridHeader"><th>S/N</th><th>Company</th><th >Well Name</th><th >Well Type</th><th >Well Class</th><th >Block</th>'
        + '<th >Field</th><th >Terrain</th>'
        + '<th >Zone</th><th >Total Depth(FT)</th><th >SPUd Date</th></tr>';
        var rowStr = '';
        
        var bv = 0;
        
        $.each(data, function (i, r) {
            bv += 1;
            rowStr += '<tr ><td style="width: auto; text-align: left">' + bv + '</td><td style="width: auto; text-align: left">'
                + r.CompanyName
                    + '</td><td style="width: auto; text-align: left">' + r.Name
                + '</td><td style="width: auto; text-align: left">' + r.WellTypeName
                + '</td>'
             + '<td style="width: auto; text-align: left">' + r.WellClassName + '</td>'
            + '<td style="width: auto; text-align: left">' + r.BlockName + '</td>'
            + '<td style="width: auto; text-align: left">' + r.FieldName + '</td>'
                + '<td style="width: auto; text-align: left">' 
                + r.TerrainName
                + '</td><td style="width: auto; text-align: left">' + r.ZoneName
                + '</td><td style="width: auto; text-align: left">' + r.TotalDept
                + '</td><td style="width: auto; text-align: left" >' + r.Date
                    + '</td></tr>';
            
        });
        
        $("#dvWellRep").append($(headerStr + rowStr + '</table>'));
        closePopModal($('#imgWellRepoSpin'));
        retrivedWellReports = data;
        IsWellQueryExisting();
    }
    
    function IsWellQueryExisting()
    {
        var url = '/WellReport/IsWellQueryExisting';

        $.ajax({
            async: true,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: "POST",
            url: url,
            success: function (r)
            {
                if (r.Code < 1)
                {
                    if (r.Code === -5)
                    {
                        if (statusQuery < 1)
                        {
                            wellSaverAncShow();
                        }
                        if (statusQuery > 0)
                        {
                            wellSaverAncHide();
                        }
                    }
                    if (r.Code === -2) {
                        wellSaverAncHide();
                    }
                    return;
                }
             
                if (r.Code > 0)
                {
                    wellSaverAncHide();
                    return;
                }
            }
        });
    }

    var retrivedWellReports = [];
//******************************* End of Well Reports ************************************************


//******************************* Production Reports ************************************************
   
     function printProductionReport() {
         if ($('#tbProductionReports').html() == null)
         {

             alert('Nothing to Print');
             return;
         }
         $('#ancSaveProductionQuery').hide();
         printReportData('dvProductionRep');
     }    

//GetFieldsByBlock(int blockId) GetBlocksByCompany(long companyId) 
//ddlProductionDynamicBlock ddlProductionDynamicField

     $('#ddlProductionDynamicComp').on('change', function ()
     {
         //ddlProductionDynamicField
         var tss = parseInt($(this).val());
         if (tss < 1) {
             return;
         } else {
             var url = '/Well/GetBlocksByCompany?companyId=' + tss;
            
             $.get(url, function (retVal) {
                 populateBlockDropdown(retVal);
             });
         }
     });
     
     function populateCompFieldDropdown(data)
     {
         if (data == null || data.length < 1)
         {
             $('#ddlProductionDynamicField').empty();
             $('#ddlProductionDynamicField').html('<option value="0"> -- List is empty -- </option>');
             $('#ddlProductionDynamicField').prop('disabled', true);
             return;
         }
         $('#ddlProductionDynamicField').empty();
         $('#ddlProductionDynamicField').html('<option value="0"> -- Select Field -- </option>');
         $.each(data, function (i, v) {
             if (v.FieldId < 1) {
                 $('#ddlProductionDynamicField').empty();
                 $('#ddlProductionDynamicField').html('<option value="0"> -- List is empty -- </option>');
                 $('#ddlProductionDynamicField').prop('disabled', 'disabled');
                 return;
             }

             $('#ddlProductionDynamicField').append($("<option />").val(v.FieldId).text(v.Name));
         });

         $('#ddlProductionDynamicField').prop('disabled', false);
     }
     

     $('#ddlProductionDynamicBlock').on('change', function () {
         var rxv = parseInt($(this).val());

         if (rxv < 1) {
             alert('Invalid selection');
             return;
         }

         var url = '/Well/GetFieldsByBlock?blockId=' + rxv;

         $.get(url, function (retVal)
         {
             populateCompFieldDropdown(retVal);
         });

     });
     
     //GetBlocksByCompany(long companyId) GetFieldsByBlock(int blockId)

     function populateBlockDropdown(data) {
         if (data == null || data.length < 1) {

             $('#ddlProductionDynamicBlock').html('<option value="0"> -- List is empty -- </option>');
             $('#ddlProductionDynamicBlock').prop('disabled', true);
             return;
         }
         $('#ddlProductionDynamicBlock').empty();
         $('#ddlProductionDynamicBlock').html('<option value="0"> -- Select Block -- </option>');
         $.each(data, function (i, v) {
             if (v.BlockId < 1) {
                 $('#ddlProductionDynamicBlock').empty();
                 $('#ddlProductionDynamicBlock').html('<option value="0"> -- List is empty -- </option>');
                 $('#ddlProductionDynamicBlock').prop('disabled', 'disabled');
                 return;
             }

             $('#ddlProductionDynamicBlock').append($("<option />").val(v.BlockId).text(v.Name));
         });

         $('#ddlProductionDynamicBlock').prop('disabled', false);
     }




     function generateProductionReports()
     {
        //tm.val().trim().toLowerCase().replace(/ /g, '');

        var cId1 = parseInt($('#ddlProductionDynamicBlock').val());
        var cId2 = parseInt($('#ddlProductionDynamicProduct').val());
        var cId3 = parseInt($('#ddlProductionDynamicField').val());
        var cId6 = $('#StartDate').val();
        var cId7 = parseInt($('#ddlProductionDynamicComp').val());
        var cId8 = $('#endDate').val();
         
        if (cId1 < 1 && cId2 < 1 && cId3 < 1 && cId6.length < 1 && cId7 < 1 && cId8.length < 1)
        {
            alert('Please select at least one query criteria.');
            $('#frmSaveProductionQuery').hide('fast');
            return;
        }
         
        if (cId6.length < 1 && cId8.length > 1)
        {
            alert('Please select a starte Date.');
            $('#frmSaveProductionQuery').hide('fast');
            return;
        }


        var tdc = {};
       
        tdc['BlockId'] = cId1;
        tdc['ProductId'] = cId2;
        tdc['FieldId'] = cId3;
        tdc['StartDate'] = cId6;
        tdc['CompanyId'] = cId7;
        tdc['EndDate'] = cId8;

        var sdx = JSON.stringify({ queryBuilder: tdc });

        closeProductionRepFields();
        $('#frmSaveProductionQuery').fadeOut('fast');
        productionSaverAncHide();
        setModal2($('#imgProductionRepoSpin'));

        var url = '/ProductionReport/GetProductions';

        $.ajax({
            async: true,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: sdx,
            type: "POST",
            url: url,
            success: function (r) {
                productionQueryStatus = 0;
                buildProductionReport(r);
            }
            ,
            error: function () {
                closePopModal($('#imgProductionRepoSpin'));
                $('#frmSaveProductionQuery').hide('fast');
            }
        });
    }

    function generateProductionReports2() {

        var cId = parseInt($('#ddlproductionPreviousQueries').val().trim());

        if (cId < 1) {
            alert('Please select a valid query.');
            return;
        }

        var sdx = JSON.stringify({ queryId: cId });

        closeProductionRepFields();
        setModal2($('#imgProductionRepoSpin'));

        var url = '/ProductionReport/GetProductions2';

        $.ajax({
            async: true,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: sdx,
            type: "POST",
            url: url,
            success: function (r) {
                productionQueryStatus = 1;
                buildProductionReport(r);
            }
        });

    }

    var productionQueryStatus = 0;

    function prepProductionRepFiledsTemplate() {
        setModal($('#dvSelectReportFields'));
    }

    function closeProductionRepFields() {
        closePopModal($('#dvSelectReportFields'));
    }

    function closeProductionQuery() {
        closePopModal($('#frmSaveProductionQuery'));
    }

    function productionSaverAncHide() {
        $('#ancSaveProductionQuery').fadeOut('slow');
    }

    function productionSaverAncShow() {
        $('#ancSaveProductionQuery').fadeIn('slow');
    }

    function showProductionQuerySaver() {
        $('#txtProductionQueryName').val('');
        setModal($('#frmSaveProductionQuery'));
    }

    function saveProductionReportQuery() {
        var query = $('#txtProductionQueryName').val().trim();
        if (query.length < 1) {
            alert('Please kindly provide a name for the query.');
            return;
        }

        var sdx = JSON.stringify({ queryName: query });
        var url = '/ProductionReport/SaveSuccessfulQuery';

        $.ajax({
            async: true,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: sdx,
            type: "POST",
            url: url,
            success: function (r) {
                if (r.Code < 1) {
                    alert(r.Error);
                    return;
                }

                if (r.Code > 0) {
                    refreshPreviousProductionQueryDropdown(r.Code);
                    alert(r.Error);
                    closeProductionQuery();
                    productionSaverAncHide();
                    return;
                }
            }
        });

    }

    function refreshPreviousProductionQueryDropdown(k) {
        if (k < 1) {
            alert('An error was encountered. Previous Query list could not be refreshed.');
            return;
        }

        //ddlproductionPreviousQueries
        //.html('<option value="0"> -- Select Field -- </option>');
        $('#ddlproductionPreviousQueries option[value = 0]').text('-- Select Previous Query --');

        $('#ddlproductionPreviousQueries').append($("<option />").val(k).text($('#txtProductionQueryName').val().trim()));

        //$('#ddlFields').prop('selectedIndex', 0);
        //$('#ddlFields').prop('disabled', false);
    }

    function buildProductionReport(data) {
        if (data.length < 1) {
            closePopModal($('#imgProductionRepoSpin'));
            alert('No record found');
            return;
        }

        $('#tbProductionReports').remove();

        var headerStr = '<table class="newTable allcontent" id="tbProductionReports">' +
            '<tr class="customGridHeader"><th>S/N</th><th>Company</th>' +
            '<th >Product</th><th >Quantity(Barrels)</th>'
        + ' <th >Block</th><th >Field</th><th >Terrain</th>' //
        + '<th >Zone</th><th >API Gravity</th><th >Period</th><th >Remark</th></tr>'; 
        var rowStr = '';
        var bv = 0;
        $.each(data, function (i, r) {
            bv += 1;
            var remark = " ";
            var apiGravity = " ";
            if (r.APIGravity === null || r.APIGravity.trim().length < 1) {
                apiGravity = " ";
            } else {
                apiGravity = r.APIGravity;
            }
            if (r.Remark === null || r.Remark.trim().length < 1)
            {
                remark = " ";
            } else
            {
                remark = r.Remark;
            }
            rowStr += '<tr ><td style="width: auto; text-align: left">' + bv + '</td><td style="width: auto; text-align: left">' + r.CompanyName
                    + '</td><td style="width: auto; text-align: left">' + r.ProductName
                + '</td><td style="width: auto; text-align: left">' + r.Quantity
                + '</td>'
                + '<td style="width: auto; text-align: left">' + r.BlockName + '</td>'
                + '<td style="width: auto; text-align: left">' + r.FieldName + '</td>'
                + '<td style="width: auto; text-align: left">' + r.TerrainName + '</td>'
                + '<td style="width: auto; text-align: left">' + r.ZoneName
                + '</td><td style="width: auto; text-align: left">' + apiGravity
                + '</td><td style="width: auto; text-align: left" >' + r.Period + '</td>'
                + '</td><td style="width: auto; text-align: left" >' + remark
                + '</td></tr>';

        });
        
        $("#dvProductionRep").append($(headerStr + rowStr + '</table>'));
        closePopModal($('#imgProductionRepoSpin'));
        retrivedProductionReports = data;
        IsProductionQueryExisting();
    }

    function IsProductionQueryExisting() {
        var url = '/ProductionReport/IsProductionQueryExisting';

        $.ajax({
            async: true,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: "POST",
            url: url,
            success: function (r) {
                if (r.Code < 1)
                {
                    if (r.Code < 1)
                    {
                        if (r.Code === -5)
                        {
                            if (productionQueryStatus < 1)
                            {
                                productionSaverAncShow();
                            }
                            if (productionQueryStatus > 0) {
                                wellSaverAncHide();
                            }
                        }
                        if (r.Code === -2) {
                            productionSaverAncHide();
                        }
                        return;
                    }
                    
                }

                if (r.Code > 0) {
                    productionSaverAncHide();
                    return;
                }
            }
        });
    }

    var retrivedProductionReports = [];
   
//******************************* End of Production Reports ************************************************
    

//****************************** Field Report **************************************************************

    function printFieldReport() {
        if ($('#tbFieldReports').html() == null)
        {
            alert('Nothing to Print');
            return;
        }
        $('#ancSaveFieldQuery').css({ 'display': 'none' });
        printReportData('dvFieldRep');
    }
    
    function generateFieldReports()
    {
        //tm.val().trim().toLowerCase().replace(/ /g, '');

        var cId1 = parseInt($('#ddlFieldDynamicTerrain').val());
        var cId2 = parseInt($('#ddlFieldDynamicBlock').val());
        var cId3 = parseInt($('#ddlFieldDynamicZone').val());
        var cId4 = parseInt($('#ddlFieldDynamicComp').val());

        if (cId1 < 1 && cId2 < 1 && cId3 < 1 && cId4 < 1)
        {
            alert('Please select at least one report Criteria.');
            $('#frmSaveFieldQuery').hide('fast');
            return;
        }

        var tdc = {};

        tdc['TerrainId'] = cId1;
        tdc['BlockId'] = cId2;
        tdc['ZoneId'] = cId3;
        tdc['CompanyId'] = cId4;

        var sdx = JSON.stringify({queryBuilder: tdc });

        closeFieldRepFields();
        
        $('#frmSaveFieldQuery').fadeOut('fast');
        fieldSaverAncHide();
        setModal2($('#imgFieldRepoSpin'));

        var url = '/FieldReport/GetFields';

        $.ajax({
            async: true,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: sdx,
            type: "POST",
            url: url,
            success: function (r) {
                fieldQueryStatus = 0;
                buildFieldReport(r);
            }
            ,
            error: function () {
                closePopModal($('#imgFieldRepoSpin'));
                $('#frmSaveFieldQuery').hide('fast');
            }
        });
    }

    function generateFieldReports2() {

        var cId = parseInt($('#ddlfieldPreviousQueries').val().trim());

        if (cId < 1) {
            alert('Please select a valid query.');
            return;
        }

        var sdx = JSON.stringify({ queryId: cId });

        closeFieldRepFields();
        setModal2($('#imgFieldRepoSpin'));

        var url = '/FieldReport/GetFields2';

        $.ajax({
            async: true,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: sdx,
            type: "POST",
            url: url,
            success: function (r) {
                fieldQueryStatus = 1;
                buildFieldReport(r);
            }
        });

    }

    var fieldQueryStatus = 0;

    function prepFieldRepFiledsTemplate() {
        setModal($('#dvSelectFieldReportItems'));
    }

    function closeFieldRepFields() {
        closePopModal($('#dvSelectFieldReportItems'));
    }

    function closeFieldQuery() {
        closePopModal($('#frmSaveFieldQuery'));
    }

    function fieldSaverAncHide() {
        $('#ancSaveFieldQuery').fadeOut('slow');
    }

    function fieldSaverAncShow() {
        $('#ancSaveFieldQuery').fadeIn('slow');
    }

    function showFieldQuerySaver() {
        $('#txtFieldQueryName').val('');
        setModal($('#frmSaveFieldQuery'));
    }

    function saveFieldReportQuery() {
        var query = $('#txtFieldQueryName').val().trim();
        if (query.length < 1) {
            alert('Please kindly provide a name for the query.');
            return;
        }

        var sdx = JSON.stringify({ queryName: query });
        var url = '/FieldReport/SaveSuccessfulQuery';

        $.ajax({
            async: true,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: sdx,
            type: "POST",
            url: url,
            success: function (r) {
                if (r.Code < 1) {
                    alert(r.Error);
                    return;
                }

                if (r.Code > 0) {
                    refreshPreviousFieldQueryDropdown(r.Code);
                    alert(r.Error);
                    closeFieldQuery();
                    fieldSaverAncHide();
                    return;
                }
            }
        });

    }

    function refreshPreviousFieldQueryDropdown(k) {
        if (k < 1) {
            alert('An error was encountered. Previous Query list could not be refreshed.');
            return;
        }
        
        $('#ddlfieldPreviousQueries option[value = 0]').text('-- Select Previous Query --');

        $('#ddlfieldPreviousQueries').append($("<option />").val(k).text($('#txtFieldQueryName').val().trim()));
        
    }

    function buildFieldReport(data)
    {
        if (data.length < 1) {
            closePopModal($('#imgFieldRepoSpin'));
            alert('No record found');
            return;
        }

        $('#tbFieldReports').remove();

        var headerStr = '<table class="newTable allcontent" id="tbFieldReports">' +
            '<tr class="customGridHeader"><th>S/N</th><th>Company</th>' +
            '<th >Field Name</th><th >Block Name</th><th >Terrain</th>'
        + '<th >Zone</th>'
        + '<th >Technical Allowable</th></tr>';
        var rowStr = '';
        var bv = 0;
        $.each(data, function (i, r) {
            bv += 1;
            var area = " ";
            var technicalAllowable = " ";
            if (r.TechnicalAllowable === null || r.TechnicalAllowable < 1) {
                technicalAllowable = " ";
            } else {
                technicalAllowable = r.TechnicalAllowable;
            }
            rowStr += '<tr ><td style="width: auto; text-align: left">' + bv + '</td><td style="width: auto; text-align: left">' + r.CompanyName + '</td>'
                + '<td style="width: auto; text-align: left">' + r.FieldName + '</td>'
                + '<td style="width: auto; text-align: left">' + r.BlockName + '</td>'
                + '<td style="width: auto; text-align: left">' + r.TerrainName
                + '<td style="width: auto; text-align: left">' + r.ZoneName
                + '</td><td style="width: auto; text-align: left">' + technicalAllowable + '</td></tr>';

        });

        $("#dvFieldRep").append($(headerStr + rowStr + '</table>'));
        closePopModal($('#imgFieldRepoSpin'));
        retrivedFieldReports = data;
        IsFieldQueryExisting();
    }

    function IsFieldQueryExisting()
    {
        var url = '/FieldReport/IsFieldQueryExisting';

        $.ajax({
            async: true,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: "POST",
            url: url,
            success: function (r)
            {
                if (r.Code < 1)
                {
                    if (r.Code === -5)
                    {
                        if (fieldQueryStatus < 1)
                        {
                            fieldSaverAncShow();
                        }
                        if (fieldQueryStatus > 0)
                        {
                            fieldSaverAncHide();
                        }
                    }
                    
                    if (r.Code === -2)
                    {
                        productionSaverAncHide();
                    }
                    
                    return;
                }

                if (r.Code > 0) {
                    fieldSaverAncHide();
                    return;
                }
            }
        });
    }

    var retrivedFieldReports = [];
   
//******************************** End of Field Report *****************************************************


//******************************* Incident Reports ************************************************
    //function generateIncidentReports() {
    //    //tm.val().trim().toLowerCase().replace(/ /g, '');
    //    var chkIncidentIncidentList = $('#ulIncidentReports :checkbox[id^="chkIncident"]');
    //    var productionfielCollection = [];
    //    $.each(chkIncidentIncidentList, function (i, k) {
    //        if ($(k).is(':checked')) {
    //            var chkIncidentId = parseInt($(k).attr('id').replace('chkIncident', ''));
    //            if (chkIncidentId > 0) {
    //                productionfielCollection.push(chkIncidentId);
    //            }

    //        }
    //    });

    //    if (productionfielCollection.length < 1) {
    //        alert('Please select the Report Fields to be displayed.');
    //        return;
    //    }

    //    if (retrivedIncidentReports.length > 0) {
    //        buildIncidentReport(retrivedIncidentReports);
    //        return;
    //    }
    //    else {
    //        var url = '/IncidentReport/GetIncidents';

    //        $.ajax({
    //            async: true,
    //            type: "POST",
    //            url: url,
    //            success: function (r) {
    //                buildIncidentReport(r);
    //            }
    //        });
    //    }
    //}

    ////    

    //function prepIncidentRepFiledsTemplate() {
    //    getIncidentList();
    //    setModal($('#dvSelectIncidentReportFields'));
    //}

    //function closeIncidentRepFields() {
    //    closePopModal($('#dvSelectIncidentReportFields'));
    //}

    //function populateIncidentFields(data) {
    //    if (data == null || data.length < 1) {
    //        $('#brFeedbackIncidentReport').text("Field list for the selected Company is empty");
    //        $('#brFeedbackIncidentReport').fadeIn('slow');
    //        return;
    //    }

    //    $('#ulIncidentReports').empty();

    //    $('#ulIncidentReports').append($('<li id="0" style="font-weight: bold"><input type="checkbox" id="chkIncident0" value="0" onclick = "productionChkAll()"/>&nbsp; < Select All > </li>'));

    //    $.each(data, function (i, v) {
    //        if (v.Id < 1) {
    //            $('#ulIncidentReports').empty();
    //            $('#ulIncidentReports').prop('disabled', 'disabled');
    //            alert('An unknown error was encountered. Process has been terminated');
    //            return;
    //        }

    //        //$('#ddlReportFields').append($("<option />").val(v.FieldId).text(v.Name));
    //        $('#ulIncidentReports').append($('<li id="' + v.DocId + '"><input type="checkbox" id ="chkIncident' + v.DocId + '" value="' + v.DocId + '"/>&nbsp;' + v.DocName + '</li>'));
    //    });

    //    $('#ulIncidentReports').prop('disabled', false);
    //    $('#chkIncident1').prop('checked', true);
    //    $('#ulIncidentReports #chkIncident1').prop('disabled', true);

    //    $('#chkIncident2').prop('checked', true);
    //    $('#ulIncidentReports #chkIncident2').prop('disabled', true);
    //}

    //function productionChkAll() {
    //    var chkIncidentIncidentAll = $('#chkIncident0');
    //    var chkIncidentIncidentList = $('#ulIncidentReports :checkbox[id^="chkIncident"]');
    //    if (chkIncidentIncidentAll.is(':checked')) {
    //        $.each(chkIncidentIncidentList, function (i, k) {
    //            if ($(k).attr('id') === 'chkIncident1' || $(k).attr('id') === 'chkIncident2') {

    //            } else {
    //                $(k).prop('checked', true);
    //            }
    //        });
    //    }
    //    else {
    //        $.each(chkIncidentIncidentList, function (i, k) {
    //            if ($(k).attr('id') === 'chkIncident1' || $(k).attr('id') === 'chkIncident2') {

    //            } else {
    //                $(k).prop('checked', false);
    //            }
    //        });
    //    }

    //}

    //function getIncidentList() {
    //    var url = '/IncidentReport/GetIncidentReportFields';

    //    $.get(url, function (data) {
    //        populateIncidentFields(data);
    //    });

    //}

    //function buildIncidentReport(data) 
    //{
    //    if (data.length < 1) 
    //    {
    //        alert('The report details could not be retrieved. Please try again');
    //        return;
    //    }

    //    $('#tbIncidentReports').remove();

    //    var incidentHeaderStr = '<table class="newTable" id="tbIncidentReports"><tr class="customGridHeader"><th >Incident</th><th >Type</th>';
    //    var incidentRowStr = '';

    //    if ($('#chkIncident3').is(':checked')) {
    //        incidentHeaderStr += '<th >Location</th>';

    //    }


    //    if ($('#chkIncident4').is(':checked')) {
    //        incidentHeaderStr += '<th >Description</th>';
    //    }

    //    if ($('#chkIncident5').is(':checked')) {
    //        incidentHeaderStr += '<th >Date</th>';
    //    }

    //    if ($('#chkIncident6').is(':checked'))
    //    {
    //        incidentHeaderStr += '<th >Company</th>';
    //    }

    //    if ($('#chkIncident7').is(':checked')) {
    //        incidentHeaderStr += '<th >Reported By</th>';
    //    }
        
    //    incidentHeaderStr += '</tr>';

    //    $.each(data, function (i, r)
    //    {
    //        incidentRowStr += '<tr ><td style="width: auto; text-align: left">' + r.Title
    //            + '</td><td style="width: auto; text-align: left">' + r.IncidentTypeName
    //            + '</td>';

    //        if ($('#chkIncident3').is(':checked')) {
    //            incidentRowStr += '<td style="width: auto; text-align: left">' + r.Location
    //                + '</td>';

    //        }

    //        if ($('#chkIncident4').is(':checked')) {
    //            incidentRowStr += '<td style="width: auto; text-align: left">' + r.Description
    //                + '</td>';

    //        }

    //        if ($('#chkIncident5').is(':checked')) {
    //            incidentRowStr += '<td style="width: auto; text-align: left">' + r.Date
    //                + '</td>';

    //        }

    //        if ($('#chkIncident6').is(':checked')) {
    //            incidentRowStr += '<td style="width: auto; text-align: left" id="tdCompany">' + r.CompanyName
    //                + '</td>';

    //        }

    //        if ($('#chkIncident7').is(':checked')) {
    //            incidentRowStr += '<td style="width: auto; text-align: left" id="tdCompany">' + r.ReportedBy
    //                + '</td>';
    //        }

    //        incidentRowStr += '</tr>';

    //    });

    //    $("#fsIncidentReport").append($(incidentHeaderStr + incidentRowStr + '</table>'));
    //    closeIncidentRepFields();
    //    retrivedIncidentReports = data;
    //}

    //var retrivedIncidentReports = [];
    //******************************* End of Incident Reports ************************************************