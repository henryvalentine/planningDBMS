
//----------------------------------------- Department ----------------------------------------------------------------------------

var xcrt = 2;
var fxCol = [];
var xmail = '';
var xChkBr = '';
var xBtnPrBr = '';
var xErrDisp = '';
var container = '';
var originalWidth = '';
var originalHeight = '';

function addDeptrow()
{
    $($('#tblMoreDept tbody:last').append($('<tr id="trNw' + xcrt + '" class ="ggxv"><td  class="form-group" style="vertical-align: bottom; width: 100%" colspan="2">'
      + '<div><i style="width: auto">Name:</i><span style="color: red">*</span> <span id="sptxtName' + xcrt + '" style="color: red; font-size: 8pt; display: none">Required </span><span id="ErrtxtName' + xcrt + '" style="color: red; font-size: 8pt; display: none"></span><img src="../../Images/Metro-white/cancel.png" id="imgNewRw' + xcrt + '" class="addNewFieldRow" onclick="remDeptrow(this.id)" title="Remove Item"/></div>'
      + '<input name = "Name" style="width:100%" id="txtName' + xcrt + '" class="form-control" onchange="checkDups(this)"/><div style="margin-top: 2%">'
      + '&nbsp;<label style="float: right; padding-left: 1%">Active?</label> <input  name = "ActiveStatus" id="chkDepartment' + xcrt + '" class="customNewCheckbox" value="Active?" type="checkbox" style="padding-top: 25px; float: right"/>'
      + '</div> </td></tr>')));
    $('#trMoreDept').show(300);
    xcrt++;
    setValidators();
}

function remDeptrow(jxk) {
    var dsx = parseInt(jxk.replace('imgNewRw', ''));
    if (dsx < 1) {
        alert('Item could not be removed. Please refresh the page and try again.');
        return;
    }

    $('#tblMoreDept #trNw' + dsx).remove();

    if ($('#tblMoreDept tr.ggxv').length < 1)
    {
        $('#trMoreDept').hide('fast');
    }

}

$(window).load(function ()
{
    $('#tblDepartments tr:odd').addClass('gridItem1');
    $('#lblDepartmentErr').addClass('errx');

});

function contentScroll()
{
   
    //var winHeight = $(window).scrollTop() + $(window).height();
      
    //    if (winHeight > 100) //$('.footer').offset().top)
    //    {
    //        if ($("#btnScrollTop").css('display') == 'inline-block')
    //        {
    //            $("#btnScrollTop").css('margin-top', winHeight);
    //        }
    //        else
    //        {
    //            $("#btnScrollTop").css('margin-top', winHeight);
    //            $("#btnScrollTop").fadeIn('fast');
    //        }
    //    }
    //    else
    //    {
    //        $("#btnScrollTop").fadeOut('fast');
    //    }
    //});
    
    //$(this)[0].scrollHeight

    var scrll = $('#fsDeptList');
    var lftMg = scrll.width() + 100;
    //var sdc = parseInt(scrll.css('max-height').replace('px', ''));
   // var th = scrll.height();
    var tw = $('#fsDeptList').scrollTop() + 100;
   // var winHeight = th + tw;

    if (tw > 100)
        {
            if ($("#dvScrll").css('display') == 'inline-block' || $("#dvScrll").css('display') == 'block')
            {
                $("#dvScrll").css({ 'z-index': '100000', 'left': lftMg + 'px', 'top': tw + 'px' });
            }
            else 
            {
                $("#dvScrll").css({ 'z-index': '100000', 'left': lftMg + 'px', 'top': tw + 'px' });
                $("#dvScrll").fadeIn('fast');
             
            }
    }
    
    if (tw < 230)
    {
        if ($("#dvScrll").css('display') == 'inline-block' || $("#dvScrll").css('display') == 'block')
        {
            $("#dvScrll").fadeOut('fast');
            $("#dvScrll").css({ 'z-index': ''});
        }
    }
}

function scrollTop()
{
    $("#btnScrollTop").click(function ()
    {
        $('#fsDeptList').animate({
            scrollTop: $("#btnAddNewDept").offset().top
        }, 400);
    });
}

function checkDups(xcid) {
    checkDuplicates(xcid, 'fsDepartment', 'This Department has already been enterd.');
}

function setValidators()
{
    toggleValidators('fsDepartment');
}

function cncFeedbackDiv() {
    closePopModal($('#dvErr'));
    $('#tblReport').find("tr:gt(0)").remove();
}

$(document).ready(function ()
{
    $('#fsDeptList').on('scroll', contentScroll);
    $("#btnScrollTop").on('click', scrollTop);
    container = $('#fsDeptAll');
    xErrDisp = $('#divErrDepartments');
    xmail = $('#txtName');
    xChkBr = $('#chkDepartment');
    xBtnPrBr = $('#btnProcessDepartment');
    originalWidth = container.children('.activeFieldSet').width();
    originalHeight = container.children('.activeFieldSet').height();
    container.children('fieldset').each(function () {
        var theFieldSet = $(this);

        if (!theFieldSet.hasClass('activeFieldSet')) {
            theFieldSet.addClass('inActiveFieldSet');
            container.data({
                width: parseInt(theFieldSet.css("width")),
                height: theFieldSet.height()
            });
        }

    });
});

function validateInput() {
    if (xmail.val().replace(' ', '').length < 1) {
        return;
    }
    setDeptWrpperSize();
}

function CleanFieldset() {
    xErrDisp.addClass("inActiveFieldSet");
    $('#lgTitle').text("New Department(s)");
    refreshForm('fsDepartment', '');
    $('#lblDepartmentErr').text('');
    $('#divErrDepartments').hide();
    $('#fsDepartment [id^="ErrtxtName"]').each(function () {
        $(this).text('');
        $(this).hide();
    });
    removeDepts();
    $("#spDeptSuccess").text('');
    $("#dvDeptSuccess").hide();
    setDeptWrpperSize();
}


function removeDepts() {
    $('#tblMoreDept tr.ggxv').each(function () {
        $(this).remove();
    });
    $('#trMoreDept').hide();
}

function retoggleBtnValue() {
    setDeptWrpperSize();
}

function setDeptWrpperSize() {
    var txd = container.children('.activeFieldSet').attr("id");
    var inActiveFieldset = '';
    var activeFieldset = '';
    if (txd === $('#fsDeptList').attr("id")) {
        inActiveFieldset = $('#fsDepartment');
        activeFieldset = $('#fsDeptList');
    }
    else {
        if (txd === $('#fsDepartment').attr("id")) {
            inActiveFieldset = $('#fsDeptList');
            activeFieldset = $('#fsDepartment');
        }
    }
    activeFieldset.fadeOut("slow", function () {
        container.css({ "background": "white" });

        activeFieldset.removeClass('activeFieldSet');

        if (inActiveFieldset.attr("id") === $('#fsDeptList').attr("id")) {
            container.stop()
                .animate(
                    {
                        width: originalWidth + 'px',
                        height: originalHeight + 'px'

                    }, 500, function () {
                        inActiveFieldset.addClass('activeFieldSet');
                        activeFieldset.addClass('inActiveFieldSet');
                        inActiveFieldset.fadeIn("slow");
                        container.css({ "background": "whitesmoke" });
                    });
        }
        else {
            container.stop()
                .animate(
                    {
                        width: parseInt(inActiveFieldset.css("width").replace('px', '')) + 'px',
                        height: inActiveFieldset.height() + 'px'

                    }, 500, function () {
                        inActiveFieldset.addClass('activeFieldSet');
                        activeFieldset.addClass('inActiveFieldSet');
                        inActiveFieldset.width((container.width()) + 'px');
                        inActiveFieldset.fadeIn("slow");
                        container.css({ "background": "whitesmoke" });


                    });
        }

    });

}

function edtxg(r) {
    $('#spDeptError').text('');
    $('#dvDeptError').hide(250);
    $('#spDeptSuccess').text('');
    $('#dvDeptSuccess').hide(250);
    $('#lblDepartmentErr').text('');
    $('#divErrDepartments').fadeOut(250);

    var cn = parseInt(r.replace('edCInf', ''));
    if (cn < 1) {
        alert('Invalid Selection!');
        return;
    }

    $('.dsx' + cn).hide('slow');
    $('.rtx' + cn).show('slow');
    fxCol.push(cn);
    if (fxCol.length > 1) {
        $('#btnupdateAll').show('slow');
    }
}

function csx(r) {
    $('#spDeptError').text('');
    $('#dvDeptError').hide(250);
    $('#spDeptSuccess').text('');
    $('#dvDeptSuccess').hide(250);
    $('#lblDepartmentErr').text('');
    $('#divErrDepartments').fadeOut(250);

    var cn = parseInt(r.replace('cnc', ''));
    if (cn < 1) {
        alert('Invalid Selection!');
        return;
    }

    var tm = $('#txt' + cn);
    var rtx = $('.rtx' + cn);

    if ($('#spChck' + cn).text() == 'Active') {
        rtx.prop('checked', true);
    }
    else {
        rtx.prop('checked', false);
    }

    $(tm).removeClass('duplicateErr');
    tm.removeClass('req');
    tm.attr('title', '');
    $(tm).val($('#spContent' + cn).text());
    $('.rtx' + cn).hide('slow');
    $('.dsx' + cn).show('slow');
    fxCol.splice($.inArray(cn, fxCol), 1);
    if (fxCol.length < 2) {
        $('#btnupdateAll').hide('slow');
    }
}

function updateAll()
{
    $('#spDeptError').text('');
    $('#dvDeptError').hide(250);
    $('#spDeptSuccess').text('');
    $('#dvDeptSuccess').hide(250);
    $('#lblDepartmentErr').text('');
    $('#divErrDepartments').fadeOut(250);


    var updtCol = [];
    var errList = [];
    if (fxCol.length < 1)
    {
        alert('The update list could not be retrieved. Please try again.');
        return;
    }

    for (var n = 0; n < fxCol.length; n++) {
        var vx = fxCol[n];

        var tm = $('#txt' + vx);
        var rtx = $('.rtx' + vx);

        if (tm.hasClass('duplicateErr')) {
            errList.push(tm);
        }
        else {
            if (tm.val().trim().length < 1) {
                tm.addClass('req');
                tm.attr('title', 'Required');
                return;
            }

            else {
                tm.removeClass('req');
                tm.attr('title', '');
            }

            var dxf = {};

            dxf['Name'] = tm.val();

            dxf['DepartmentId'] = vx;

            if (rtx.is(':checked')) {
                dxf['ActiveStatus'] = 'true';
            }
            else {
                dxf['ActiveStatus'] = 'false';
            }

            updtCol.push(dxf);
        }
    }

    if (errList.length > 0) {
        $('#spDeptError').text('Please resolve the duplicate(s) on the page and try again.');
        $('#dvDeptError').fadeIn(300);
        return;
    }

    if (updtCol.length < 1)
    {
        alert('The update list could not be compiled. Please try again.');
        return;
    }

    var sdx = JSON.stringify({ myViewObjList: updtCol });

    $.ajax({

        url: "/Department/EditMultipleDepartment",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: sdx,
        type: 'POST',
        success: function (zxz) {
            if (zxz === 2) {
                deptFeedBackStatus(0, 'The selected items could not be updated. Please try again or contact the Administarator.');
                return;
            }

            if (zxz.SuccessList.length > 0 && zxz.ErrorList.length < 1) {

                for (var y = 0; y < zxz.SuccessList.length; y++) {
                    var cvx = zxz.SuccessList[y].DepartmentId;
                    var txm = $('#txt' + cvx);
                    var rctx = $('.rtx' + cvx);
                    var tx = $('#spContent' + cvx);
                    var rcx = $('#spChck' + cvx);

                    tx.text(txm.val());

                    if (rctx.is(':checked')) {
                        rcx.text('Active');
                    }
                    else {
                        rcx.text('Inactive');
                    }

                    $('.rtx' + cvx).hide('slow');
                    $('.dsx' + cvx).show('slow');
                }

                $('#btnupdateAll').hide('slow');
                deptFeedBackStatus(1, 'The selected items were successfully updated.');
                fxCol = [];
                return;
            }

            if (zxz.SuccessList.length > 0 && zxz.ErrorList.length > 0) {
                for (var q = 0; q < zxz.SuccessList.length; q++) {
                    var cx = zxz.SuccessList[q].DepartmentId;
                    var txfm = $('#txt' + cx);
                    var rx = $('.rtx' + cx);
                    var txf = $('#spContent' + cx);
                    var xr = $('#spChck' + cx);

                    txf.text(txfm.val());

                    if (rx.is(':checked')) {
                        xr.text('Active');
                    }

                    else {
                        xr.text('Inactive');
                    }

                    $('.rtx' + cx).hide('slow');
                    $('.dsx' + cx).show('slow');
                    fxCol.splice($.inArray(cx, fxCol), 1);
                }

                if (zxz.ErrorList.length > 0) {
                    if (zxz.ErrorList.length > 1) {
                        $('#btnupdateAll').show('slow');
                        $('#tblReport').find("tr:gt(0)").remove();

                        for (var g = 0; g < zxz.ErrorList.length; g++) {
                            var tmb = zxz.ErrorList[g];
                            $($('#tblReport tbody:last').append($('<tr><td  style="vertical-align: top; width: 40%">'
                                + tmb.Name + '</td>'
                                + '<td style="vertical-align: top; width: 60%"><span style="color: red; font-size: 11pt;">' + tmb.Error + ' </span>'
                                + '</td></tr>')));
                        }
                        deptFeedBackStatus(0, 'Some of the selected items could not be updated. Please try again or contact the Administarator.');
                        setModal($('#dvErr'));
                    }

                    else {
                        $('#btnupdateAll').hide('slow');
                        $('#txt' + zxz.ErrorList[0].DepartmentId).addClass('req');
                        deptFeedBackStatus(0, 'The operation returned with an error. The Item(s) could not be updated');
                    }

                }
                return;
            }


            if (zxz.SuccessList.length < 1 && zxz.ErrorList.length > 0) {
                if (zxz.ErrorList.length > 1) {
                    $('#btnupdateAll').show('slow');
                    for (var a = 0; a < zxz.ErrorList.length; a++) {
                        var txmb = zxz.ErrorList[a];
                        $($('#tblReport tbody:last').append($('<tr><td  style="vertical-align: top; width: 40%">'
                            + txmb.Name + '</td>'
                            + '<td style="vertical-align: top; width: 60%"><span style="color: red; font-size: 11pt;">' + txmb.Error + ' </span>'
                            + '</td></tr>')));
                    }

                    deptFeedBackStatus(0, 'None of the selected items could be updated. Please try again or contact the Administarator.');
                    setModal($('#dvErr'));
                }

                if (zxz.ErrorList.length === 1) {
                    $('#btnupdateAll').hide('slow');
                    $('#txt' + zxz.ErrorList[0].DepartmentId).addClass('req');
                    deptFeedBackStatus(0, 'The operation returned with an error. The Item(s) could not be updated');
                }
                return;
            }

        }
    });
}

function uptxg(y) {
    $('#spDeptError').text('');
    $('#dvDeptError').hide(250);
    $('#spDeptSuccess').text('');
    $('#dvDeptSuccess').hide(250);
    $('#lblDepartmentErr').text('');
    $('#divErrDepartments').fadeOut(250);


    var txxv = parseInt(y.replace('upCInf', ''));

    if (txxv < 1 || txxv === NaN || txxv == 'NaN')
    {
        alert('Invalid Selection!');
        return;
    }

    var tm = $('#txt' + txxv);

    if (tm.hasClass('duplicateErr'))
    {
        $('#spDeptError').text('Please resolve the duplicate(s) on the page and try again.');
        $('#dvDeptError').fadeIn(300);
        return;
    }
    
    if (tm.val().trim().toLowerCase().replace(/ /g, '') < 1)
    {
        tm.addClass('req');
        $('#spDeptError').text('Please enter Department Name.');
        tm.attr('title', 'Department Name is required.');
        $('#dvDeptError').fadeIn(300);
        return;
    }
    else
    {
        tm.attr('title', '');
        tm.removeClass('req');
    }


    if (tm.hasClass('req'))
    {
        $('#spDeptError').text('Please enter Department Name.');
        $('#dvDeptError').fadeIn(300);
        return;
    }
    
    fxCol.splice($.inArray(txxv, fxCol), 1);

    var rtx = $('.rtx' + txxv);

    if (tm.val().trim().length < 1) {
        tm.css({ 'border-color': 'red' });
        return;
    }
    else {
        tm.css({ 'border-color': '#CCC' });
    }

    var dxf = {};

    dxf['Name'] = tm.val();

    dxf['DepartmentId'] = txxv;

    if (rtx.is(':checked')) {
        dxf['ActiveStatus'] = 'true';
    }
    else {
        dxf['ActiveStatus'] = 'false';
    }

    var sdx = JSON.stringify({ myViewObj: dxf });

    $.ajax({

        url: "/Department/EditDepartment",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: sdx,
        type: 'POST',
        success: function (dTx) {
            var i = dTx;

            if (i.ErrorCode < 1) {
                ///Database operation failed/Data could not be deleted
                deptFeedBackStatus(i.ErrorCode, i.Error);
                return;
            }

            if (i.ActiveStatus === true) {
                $('#spChck' + txxv).text('Active');
            }
            else {
                $('#spChck' + txxv).text('Inactive');
            }
            $('#spContent' + txxv).text(i.Name);

            $('.rtx' + txxv).hide('slow');
            $('.dsx' + txxv).show('slow');
            deptFeedBackStatus(i.ErrorCode, i.Success);
        }
    });
}

function deptFeedBackStatus(x, z) {
    var dx = parseInt(x);

    if (dx === 0) {
        $("#spDeptError").text(z);
        $("#dvDeptError").fadeIn();
    }
    //
    if (dx === 1) {
        $("#spDeptSuccess").text(z);
        $("#dvDeptSuccess").fadeIn();
    }

}

function checkToggle(t) {
    var gdx = $('#' + t);
    if (gdx.val().trim().length > 0) {
        gdx.removeClass('req');
        gdx.attr('title', '');
        checkDuplicates(gdx, 'tblDepartments', 'This Department has already been enterd.');
        return;
    }

    else {
        $('#' + t).addClass('req');
        $('#' + t).attr('title', 'Field is Required.');
        return;
    }

}

function delTCmp(fx) {
    $('#spDeptError').text('');
    $('#dvDeptError').hide(250);
    $('#spDeptSuccess').text('');
    $('#dvDeptSuccess').hide(250);
    $('#lblDepartmentErr').text('');
    $('#divErrDepartments').fadeOut(250);


    var txxv = parseInt(fx.replace('del', ''));

    if (txxv < 1) {
        alert('Invalid Selection!');
        return;
    }

    $.ajax({
        beforeSend: function () {
            return confirm('Are you sure you want to delete this item?');
        },
        url: "/Department/DeleteDepartment?id=" + txxv,
        type: 'POST',
        success: function (i) {
            if (i.ErrorCode < 1) {
                ///Database operation failed/Data could not be deleted
                deptFeedBackStatus(i.ErrorCode, i.Error);
                return;
            }

            $('#tblDepartments #rw' + txxv).remove();

            $('#tblDepartments tr.edrow').each(function (r) {
                $(this).children('.edTd').text(r + 1);
            });

            //$('#tblDepartments tr:odd').addClass('gridItem1');

            deptFeedBackStatus(i.ErrorCode, i.Success);
        }

    });
}

function Addx() {
    $('#spDeptError').text('');
    $('#dvDeptError').hide(250);
    $('#spDeptSuccess').text('');
    $('#dvDeptSuccess').hide(250);
    $('#lblDepartmentErr').text('');
    $('#divErrDepartments').fadeOut(250);

    if (!validateTemplate('fsDepartment')) {
        return;
    }

    var addArray = [];
    var errList = [];
    var addList = $('#fsDepartment input[id^="txtName"]');

    addList.each(function ()
    {
        var vx = $(this);

        if (vx.hasClass('duplicateErr'))
        {
            errList.push(vx);
        }
        else {
            var rtx = $('#chkDepartment' + vx.attr('id').replace('txtName', ''));

            if (vx.val().trim().length > 0) {
                var dxf = {};

                dxf[vx.prop('name')] = vx.val();

                if (rtx.is(':checked')) {
                    dxf[rtx.prop('name')] = 'true';
                }
                else {
                    dxf[rtx.prop('name')] = 'false';
                }

                addArray.push(dxf);
            }
        }

    });

    if (errList.length > 0) {
        $('#lblDepartmentErr').text('Please resolve the duplicate(s) on the page and try again.');
        $('#divErrDepartments').fadeIn(300);
        return;
    }
    errList = [];
    if (addArray.length < 1) {
        alert('The Item list could not be compiled. Please try again.');
        return;
    }

    var sdx = JSON.stringify({ myViewObjList: addArray });

    $.ajax({
        url: '/Department/AddDepartments',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: sdx,
        type: 'POST',
        success: function (zxz) {

            if (zxz === 2) {
                zxz = '';
                addArray = [];
                $('#lblDepartmentErr').text('The Request action could not be executed. Please try again or contact the Administarator.');
                $('#divErrDepartments').fadeIn(100);
                return;
            }

            if (zxz.SuccessList.length > 0 && zxz.ErrorList.length < 1) {
                for (var y = 0; y < zxz.SuccessList.length; y++) {
                    var i = zxz.SuccessList[y];
                    addRows(i);
                }

                if (zxz.SuccessList.length === 1) {
                    deptFeedBackStatus(1, 'Department Information was successfully Submitted.');
                }

                if (addArray.length > 1) {
                    deptFeedBackStatus(1, 'The Department list was successfully Submitted.');
                }

                $('#tblDepartments tr:odd').addClass('gridItem1');
                setDeptWrpperSize();
                zxz = '';
                addList = [];
                addArray = [];
                return;
            }

            if (zxz.SuccessList.length < 1 && zxz.ErrorList.length > 0) {
                for (var m = 0; m < zxz.ErrorList.length; m++) {
                    var j = zxz.ErrorList[m];
                    var jx = j.Name.trim().toLowerCase().replace(/ /g, '');

                    for (var h = 0; h < addList.length; h++) {
                        var dhi = addList[h];
                        var dxi = $(dhi).val().replace(/ /g, '').trim().toLowerCase();

                        if (jx === dxi) {
                            var xdi = $(dhi).attr('id').replace('txtName', '');
                            $('#fsDepartment #ErrtxtName' + xdi).text(j.Error);
                            $('#fsDepartment #ErrtxtName' + xdi).show('fast');
                        }
                    }

                }

                $('#lblDepartmentErr').text('The item(s) could not be Submitted. Please Check the error message(s) and try again.');
                $('#divErrDepartments').fadeIn(100);
                $('#fsDepartment #ErrtxtName' + xdi).text(j.Error);
                zxz = '';
                addList = [];
                addArray = [];
                return;
            }

            if (zxz.SuccessList.length > 0 && zxz.ErrorList.length > 0) {
                $.each(zxz.SuccessList, function (c, g) {
                    var u = g;
                    var txcg = u.Name.trim().toLowerCase().replace(/ /g, '');

                    addList.each(function () {
                        var dfh = $(this);
                        var tdxf = dfh.val().replace(/ /g, '').trim().toLowerCase();

                        var xzd = dfh.attr('id').replace('txtName', '');

                        if (txcg === tdxf) {
                            $('#tblMoreDept #trNw' + xzd).remove();
                        }

                        if (xzd === 1) {
                            $('#tblMoreDept #txtName' + xzd).val('');

                        }
                    });

                    addRows(g);
                });

                $('#tblDepartments tr:odd').addClass('gridItem1');


                $.each(zxz.ErrorList, function (c, g) {
                    var p = g;
                    var xcg = p.Name.trim().toLowerCase().replace(/ /g, '');

                    addList.each(function () {
                        var dh = $(this);
                        var dx = dh.val().replace(/ /g, '').trim().toLowerCase();

                        if (xcg === dx) {
                            var xd = dh.attr('id').replace('txtName', '');
                            var yxu = $('#fsDepartment #ErrtxtName' + xd);
                            yxu.text(p.Error);
                            yxu.show('fast');
                        }
                    });

                    addRows(p);
                });

                $('#lblDepartmentErr').text('Some of the items could not be Submitted. Please Check the error messages.');
                $('#divErrDepartments').fadeIn(100);
                zxz = '';
                addList = [];
                addArray = [];
                return;
            }
        }
    });
}

function addRows(i) {
    var status = '';
    var check = '';
    if (i.ActiveStatus === true) {
        status = 'Active';
        check = '&nbsp;<input id="chkDepartment" checked="true" class="customNewCheckbox rtx' + i.DepartmentId + '" value="Active?" type="checkbox" />';
    }
    else {
        status = 'Inactive';
        check = '&nbsp;<input id="chkDepartment" checked="false" class="customNewCheckbox rtx' + i.DepartmentId + '" value="Active?" type="checkbox" />';
    }

    var bv = $('#tblDepartments tr').length - 1;

    $($('#tblDepartments tbody:last').append($('<tr class="edrow" id="rw' + i.DepartmentId + '"><td id="td' + i.DepartmentId + '" class="edTd bxcf ssd">' + (bv + 1) + '</td><td class="bxcf fxxv"><span class="dsx' + i.DepartmentId + '" id="spContent' + i.DepartmentId + '" style="float: left">' + i.Name + '</span>'
     + '<input type="text" name="Name" id="txt' + i.DepartmentId + '" value="' + i.Name + '" class="form-control xxvz rtx' + i.DepartmentId + '" onchange="checkToggle(this.id)" style=" display: none; float: left;"/>'
     + '</td><td class="bxcf kxxd"><span class="dsx' + i.DepartmentId + '" id="spChck' + i.DepartmentId + '" style="float: left">' + status + '</span>'
     + '<div style="display: none; float: left" class="rtx' + i.DepartmentId + '">Active? &nbsp;' + check + '</div>'
     + '</td><td class="bxcf kxxd" ><a class="edrTx dsx' + i.DepartmentId + '" title="Edit" id="edCInf' + i.DepartmentId + '" onclick="edtxg(this.id)"><img src="/Images/Metro-black/edit.png" alt="Edit" style="width: 20px; height: 20px" /></a>'
     + '<a class="rtx' + i.DepartmentId + '" title="Update" style="display: none; cursor:pointer" id="upCInf' + i.DepartmentId + '" onclick="uptxg(this.id)"><img src="/Images/Metro-black/refresh.png" alt="Update" style="width: 20px; height: 20px" /></a>&nbsp;'
     + '<a class="dnTx rtx' + i.DepartmentId + '" onclick="csx(this.id)" title="Cancel" id="cnc' + i.DepartmentId + '" style="cursor:pointer; display: none"><img src="/Images/Metro-black/cancel.png" alt="Cancel" style="width: 20px; height: 20px" /></a>'
     + '<a class="delTx dsx' + i.DepartmentId + '" title="Delete" id="del' + i.DepartmentId + '" onclick="delTCmp(this.id)"><img src="/Images/Metro-black/delete.gif" alt="Delete" style="width: 20px; height: 20px" /></a>'
     + '</td></tr>')));
}


// ---------------------------------------- End of Department




// ---------------------------------------- Educational Qualification ------------------------------------------------------------------------

var xEdqualcrt = 2;
var edQualcontainer = '';
var originalEdQualWidth = '';
var originalEdQualHeight = '';
var xEdQual = '';
var xChkEdQualBr = '';
var xBtnPrEdQualBr = '';
var xEdQualErrDisp = '';
var fxEdQualCol = [];

function addEdQualrow() {
    $($('#tblMoreEdQual tbody:last').append($('<tr id="trNw' + xEdqualcrt + '" class ="ggxv"><td  class="form-group" style="vertical-align: bottom; width: 100%" colspan="2">'
      + '<div><i style="width: auto">Name:</i><span style="color: red">*</span> <span id="sptxtEdQualName' + xEdqualcrt + '" style="color: red; font-size: 8pt; display: none">Required </span><span id="ErrtxtEdQualName' + xEdqualcrt + '" style="color: red; font-size: 8pt; display: none"></span><img src="../../Images/Metro-white/cancel.png" id="imgEdQualNewRw' + xEdqualcrt + '" class="addNewFieldRow" onclick="remEdQualrow(this.id)" title="Remove Item"/></div>'
      + '<input name = "Name" style="width:100%" id="txtEdQualName' + xEdqualcrt + '" class="form-control" onchange="checkEdQualDups(this)"/><div style="margin-top: 2%">'
      + '&nbsp;<label style="float: right; padding-left: 1%">Active?</label> <input  name = "ActiveStatus" id="chkEdQual' + xEdqualcrt + '" class="customNewCheckbox" value="Active?" type="checkbox" style="padding-top: 25px; float: right"/>'
      + '</div> </td></tr>')));
    $('#trMoreEdQual').show(300);
    xEdqualcrt++;
    setEdQualValidators();
}

function remEdQualrow(jxk) {
    var dsx = parseInt(jxk.replace('imgEdQualNewRw', ''));
    if (dsx < 1) {
        alert('Item could not be removed. Please refresh the page and try again.');
        return;
    }

    $('#tblMoreEdQual #trNw' + dsx).remove();

    if ($('#tblMoreEdQual tr.ggxv').length < 1) {
        $('#trMoreEdQual').hide('fast');
    }

}

$(window).load(function () {
    $('#tblEdQuals tr:odd').addClass('gridItem1');
    $('#lblEducationErr').addClass('errx');

});

$(function () {
    $(window).scroll(function ()
    {
        var winHeight = $(window).scrollTop() + $(window).height();
        var scrll = $('#fsEdQualList');
        var lftMg = scrll.width() + 100;
        //var sdc = parseInt(scrll.css('max-height').replace('px', ''));
        // var th = scrll.height();
        var tw = $('#fsEdQualList').scrollTop() + 100;


        if (winHeight > 100)
        {
            if ($("#dvEdQualScrll").css('display') == 'block')
            {
                $("#dvEdQualScrll").css({ 'z-index': '100000', 'left': lftMg + 'px', 'top': tw + 'px' });
            }
            else
            {
                $("#dvEdQualScrll").css({ 'z-index': '100000', 'left': lftMg + 'px', 'top': tw + 'px' });
                $("#dvEdQualScrll").fadeIn('fast');
            }
        }
        else
        {
            $("#dvEdQualScrll").fadeOut('fast');
            $("#dvEdQualScrll").css({ 'z-index': '' });
        }
    });

    $("#btnEdQualScrollTop").click(function ()
    {
        $('html, body').animate({
            scrollTop: $("#btnAddNewEdQual").offset().top - 20
        }, 400);
    });

});

function checkEdQualDups(xcid) {
    checkDuplicates(xcid, 'fsEducational', 'This Educational Qualification has already been enterd.');
}

function setEdQualValidators() {
    toggleValidators('fsEducational');
}

function cncEdQualFeedbackDiv() {
    closePopModal($('#dvErrEdQual'));
    $('#tblEdQualReport').find("tr:gt(0)").remove();
}

$(document).ready(function ()
{
    edQualcontainer = $('#fsEducationalAll');
    xBtnPrEdQualBr = $('#btnProcessEdQual');
    xEdQualErrDisp = $('#divErrEducation');
    xEdQual = $('#txtEdQualName');
    xChkEdQualBr = $('#chkEdQual');
    originalEdQualWidth = edQualcontainer.children('.activeFieldSet').width();
    originalEdQualHeight = edQualcontainer.children('.activeFieldSet').height();
    edQualcontainer.children('fieldset').each(function () {
        var theFieldSet = $(this);

        if (!theFieldSet.hasClass('activeFieldSet')) {
            theFieldSet.addClass('inActiveFieldSet');
            edQualcontainer.data({
                width: parseInt(theFieldSet.css("width")),
                height: theFieldSet.height()
            });
        }

    });
});

function validateEdQualInput() {
    if (xEdQual.val().replace(' ', '').length < 1) {
        return;
    }
    setEdQualWrapperSize();
}

function CleanEdQualForm() {
    xEdQualErrDisp.addClass("inActiveFieldSet");
    $('#lgEdQualTitle').text("New Educational Qualification(s)");
    refreshForm('fsEducational', '');
    $('#lblEducationErr').text('');
    $('#divErrEducation').hide();
    $('#fsEducational [id^="ErrtxtEdQualName"]').each(function () {
        $(this).text('');
        $(this).hide();
    });
    setEdQualWrapperSize();
    removeEdQuals();
    $("#spEdQualSuccess").text('');
    $("#dvEdQualSuccess").hide();
}

function removeEdQuals() {
    $('#tblMoreEdQual tr.ggxv').each(function () {
        $(this).remove();
    });
    $('#trMoreEdQual').hide();
}

function retoggleEdQualBtnValue() {
    setEdQualWrapperSize();
}

function setEdQualWrapperSize() {
    var txd = edQualcontainer.find('.activeFieldSet').attr("id");
    var inActiveFieldset = '';
    var activeFieldset = '';
    if (txd === $('#fsEdQualList').attr("id")) {
        inActiveFieldset = $('#fsEducational');
        activeFieldset = $('#fsEdQualList');
    }
    else {
        if (txd === $('#fsEducational').attr("id")) {
            inActiveFieldset = $('#fsEdQualList');
            activeFieldset = $('#fsEducational');
        }
    }
    activeFieldset.fadeOut("slow", function () {
        edQualcontainer.css({ "background": "white" });

        activeFieldset.removeClass('activeFieldSet');

        if (inActiveFieldset.attr("id") === $('#fsEdQualList').attr("id")) {
            edQualcontainer.stop()
                .animate(
                    {
                        width: originalEdQualWidth + 'px',
                        height: originalEdQualHeight + 'px'

                    }, 500, function () {
                        inActiveFieldset.addClass('activeFieldSet');
                        activeFieldset.addClass('inActiveFieldSet');
                        inActiveFieldset.fadeIn("slow");
                        edQualcontainer.css({ "background": "whitesmoke" });
                    });
        }
        else {
            edQualcontainer.stop()
                .animate(
                    {
                        width: parseInt(inActiveFieldset.css("width").replace('px', '')) + 'px',
                        height: inActiveFieldset.height() + 'px'

                    }, 500, function () {
                        inActiveFieldset.addClass('activeFieldSet');
                        activeFieldset.addClass('inActiveFieldSet');
                        inActiveFieldset.width((edQualcontainer.width()) + 'px');
                        inActiveFieldset.fadeIn("slow");
                        edQualcontainer.css({ "background": "whitesmoke" });


                    });
        }

    });

}

function edEdQualXg(r) {
    $('#spEdQualError').text('');
    $('#dvEdQualError').hide(250);
    $('#spEdQualSuccess').text('');
    $('#dvEdQualSuccess').hide(250);
    $('#lblEducationErr').text('');
    $('#divErrEducation').fadeOut(250);

    var cn = parseInt(r.replace('edEdQualInf', ''));
    if (cn < 1) {
        alert('Invalid Selection!');
        return;
    }

    $('.dsx' + cn).hide('slow');
    $('.rtx' + cn).show('slow');
    fxEdQualCol.push(cn);
    if (fxEdQualCol.length > 1) {
        $('#btnUpdateAllEdQual').show('slow');
    }
}

function cEdQualSx(r) {
    $('#spEdQualError').text('');
    $('#dvEdQualError').hide(250);
    $('#spEdQualSuccess').text('');
    $('#dvEdQualSuccess').hide(250);
    $('#lblEducationErr').text('');
    $('#divErrEducation').fadeOut(250);

    var cn = parseInt(r.replace('cnc', ''));
    if (cn < 1) {
        alert('Invalid Selection!');
        return;
    }

    var tm = $('#txtTblEdQual' + cn);
    var rtx = $('.rtx' + cn);

    if ($('#spEdQualChck' + cn).text() == 'Active') {
        rtx.prop('checked', true);
    }
    else {
        rtx.prop('checked', false);
    }

    $(tm).removeClass('duplicateErr');
    tm.removeClass('req');
    tm.attr('title', '');
    $(tm).val($('#spEdQualContent' + cn).text());
    $('.rtx' + cn).hide('slow');
    $('.dsx' + cn).show('slow');
    fxEdQualCol.splice($.inArray(cn, fxEdQualCol), 1);
    if (fxEdQualCol.length < 2) {
        $('#btnUpdateAllEdQual').hide('slow');
    }
}

function updateAllEdQual() {
    $('#spEdQualError').text('');
    $('#dvEdQualError').hide(250);
    $('#spEdQualSuccess').text('');
    $('#dvEdQualSuccess').hide(250);
    $('#lblEducationErr').text('');
    $('#divErrEducation').fadeOut(250);


    var updtEdQualCol = [];
    var errList = [];
    if (fxEdQualCol.length < 1) {
        alert('The update list could not be retrieved. Please try again.');
        return;
    }

    for (var n = 0; n < fxEdQualCol.length; n++) {
        var vx = fxEdQualCol[n];

        var tm = $('#txtTblEdQual' + vx);
        var rtx = $('.rtx' + vx);

        if (tm.hasClass('duplicateErr')) {
            errList.push(tm);
        }
        else {
            if (tm.val().trim().length < 1) {
                tm.addClass('req');
                tm.attr('title', 'Required');
                return;
            }

            else {
                tm.removeClass('req');
                tm.attr('title', '');
            }

            var dxf = {};

            dxf['Name'] = tm.val();

            dxf['EducationalQualificationId'] = vx;

            if (rtx.is(':checked')) {
                dxf['ActiveStatus'] = 'true';
            }
            else {
                dxf['ActiveStatus'] = 'false';
            }

            updtEdQualCol.push(dxf);
        }
    }

    if (errList.length > 0) {
        $('#spEdQualError').text('Please resolve the duplicate(s) on the page and try again.');
        $('#dvEdQualError').fadeIn(300);
        return;
    }

    if (updtEdQualCol.length < 1)
    {
        alert('The update list could not be compiled. Please try again.');
        return;
    }

    var sdx = JSON.stringify({ myViewObjList: updtEdQualCol });

    $.ajax({

        url: "/EducationalQualification/EditMultipleEducationalQualification",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: sdx,
        type: 'POST',
        success: function (zxz) {
            if (zxz === 2) {
                edQualFeedBackStatus(0, 'The selected items could not be updated. Please try again or contact the Administarator.');
                return;
            }

            if (zxz.SuccessList.length > 0 && zxz.ErrorList.length < 1) {

                for (var y = 0; y < zxz.SuccessList.length; y++) {
                    var cvx = zxz.SuccessList[y].EducationalQualificationId;
                    var txm = $('#txtTblEdQual' + cvx);
                    var rctx = $('.rtx' + cvx);
                    var tx = $('#spEdQualContent' + cvx);
                    var rcx = $('#spEdQualChck' + cvx);

                    tx.text(txm.val());

                    if (rctx.is(':checked')) {
                        rcx.text('Active');
                    }
                    else {
                        rcx.text('Inactive');
                    }

                    $('.rtx' + cvx).hide('slow');
                    $('.dsx' + cvx).show('slow');
                }

                $('#btnUpdateAllEdQual').hide('slow');
                edQualFeedBackStatus(1, 'The selected items were successfully updated.');
                fxEdQualCol = [];
                return;
            }

            if (zxz.SuccessList.length > 0 && zxz.ErrorList.length > 0) {
                for (var q = 0; q < zxz.SuccessList.length; q++) {
                    var cx = zxz.SuccessList[q].EducationalQualificationId;
                    var txfm = $('#txtTblEdQual' + cx);
                    var rx = $('.rtx' + cx);
                    var txf = $('#spEdQualContent' + cx);
                    var xr = $('#spEdQualChck' + cx);

                    txf.text(txfm.val());

                    if (rx.is(':checked')) {
                        xr.text('Active');
                    }

                    else {
                        xr.text('Inactive');
                    }

                    $('.rtx' + cx).hide('slow');
                    $('.dsx' + cx).show('slow');
                    fxEdQualCol.splice($.inArray(cx, fxEdQualCol), 1);
                }

                if (zxz.ErrorList.length > 0) {
                    if (zxz.ErrorList.length > 1) {
                        $('#btnUpdateAllEdQual').show('slow');
                        $('#tblEdQualReport').find("tr:gt(0)").remove();

                        for (var g = 0; g < zxz.ErrorList.length; g++) {
                            var tmb = zxz.ErrorList[g];
                            $($('#tblEdQualReport tbody:last').append($('<tr><td  style="vertical-align: top; width: 40%">'
                                + tmb.Name + '</td>'
                                + '<td style="vertical-align: top; width: 60%"><span style="color: red; font-size: 11pt;">' + tmb.Error + ' </span>'
                                + '</td></tr>')));
                        }
                        edQualFeedBackStatus(0, 'Some of the selected items could not be updated. Please try again or contact the Administarator.');
                        setModal($('#dvErrEdQual'));
                    }

                    else {
                        $('#btnUpdateAllEdQual').hide('slow');
                        $('#txtTblEdQual' + zxz.ErrorList[0].EducationalQualificationId).addClass('req');
                        edQualFeedBackStatus(0, 'The operation returned with an error. The Item(s) could not be updated');
                    }

                }
                return;
            }


            if (zxz.SuccessList.length < 1 && zxz.ErrorList.length > 0) {
                if (zxz.ErrorList.length > 1) {
                    $('#btnUpdateAllEdQual').show('slow');
                    for (var a = 0; a < zxz.ErrorList.length; a++) {
                        var txmb = zxz.ErrorList[a];
                        $($('#tblEdQualReport tbody:last').append($('<tr><td  style="vertical-align: top; width: 40%">'
                            + txmb.Name + '</td>'
                            + '<td style="vertical-align: top; width: 60%"><span style="color: red; font-size: 11pt;">' + txmb.Error + ' </span>'
                            + '</td></tr>')));
                    }

                    edQualFeedBackStatus(0, 'None of the selected items could be updated. Please try again or contact the Administarator.');
                    setModal($('#dvErrEdQual'));
                }

                if (zxz.ErrorList.length === 1) {
                    $('#btnUpdateAllEdQual').hide('slow');
                    $('#txtTblEdQual' + zxz.ErrorList[0].EducationalQualificationId).addClass('req');
                    edQualFeedBackStatus(0, 'The operation returned with an error. The Item(s) could not be updated');
                }
                return;
            }

        }
    });
}

function upEdQualXg(y) {
    $('#spEdQualError').text('');
    $('#dvEdQualError').hide(250);
    $('#spEdQualSuccess').text('');
    $('#dvEdQualSuccess').hide(250);
    $('#lblEducationErr').text('');
    $('#divErrEducation').fadeOut(250);


    var txxv = parseInt(y.replace('upEdQualInf', ''));

    if (txxv < 1 || txxv === NaN || txxv == 'NaN') {
        alert('Invalid Selection!');
        return;
    }

    var tm = $('#txtTblEdQual' + txxv);

    if (tm.hasClass('duplicateErr')) {
        $('#spEdQualError').text('Please resolve the duplicate(s) on the page and try again.');
        $('#dvEdQualError').fadeIn(300);
        return;
    }

    if (tm.val().trim().toLowerCase().replace(/ /g, '') < 1) {
        tm.addClass('req');
        $('#spDeptError').text('Please enter Department Name.');
        tm.attr('title', 'Department Name is required.');
        $('#dvDeptError').fadeIn(300);
        return;
    }
    else {
        tm.attr('title', '');
        tm.removeClass('req');
    }


    if (tm.hasClass('req')) {
        $('#spDeptError').text('Please enter Department Name.');
        $('#dvDeptError').fadeIn(300);
        return;
    }

    fxCol.splice($.inArray(txxv, fxCol), 1);
    if (fxEdQualCol.length < 2) {
        $('#btnUpdateAllEdQual').hide('slow');
    }

    var rtx = $('.rtx' + txxv);

    if (tm.val().trim().length < 1) {
        tm.css({ 'border-color': 'red' });
        return;
    }
    else {
        tm.css({ 'border-color': '#CCC' });
    }

    var dxf = {};

    dxf['Name'] = tm.val();

    dxf['EducationalQualificationId'] = txxv;

    if (rtx.is(':checked')) {
        dxf['ActiveStatus'] = 'true';
    }
    else {
        dxf['ActiveStatus'] = 'false';
    }

    var sdx = JSON.stringify({ myViewObj: dxf });

    $.ajax({

        url: "/EducationalQualification/EditEducationalQualification",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: sdx,
        type: 'POST',
        success: function (dTx) {
            var i = dTx;

            if (i.ErrorCode < 1) {
                ///Database operation failed/Data could not be deleted
                edQualFeedBackStatus(i.ErrorCode, i.Error);
                return;
            }

            if (i.ActiveStatus === true) {
                $('#spEdQualChck' + txxv).text('Active');
            }
            else {
                $('#spEdQualChck' + txxv).text('Inactive');
            }
            $('#spEdQualContent' + txxv).text(i.Name);

            $('.rtx' + txxv).hide('slow');
            $('.dsx' + txxv).show('slow');
            edQualFeedBackStatus(i.ErrorCode, i.Success);
        }
    });
}

function edQualFeedBackStatus(x, z) {
    var dx = parseInt(x);

    if (dx === 0) {
        $("#spEdQualError").text(z);
        $("#dvEdQualError").fadeIn();
    }
    //
    if (dx === 1) {
        $("#spEdQualSuccess").text(z);
        $("#dvEdQualSuccess").fadeIn();
    }

}

function checkToggleEdQual(t) {
    var gdx = $('#' + t);
    if (gdx.val().trim().length > 0) {
        gdx.removeClass('req');
        gdx.attr('title', '');
        checkDuplicates(gdx, 'tblEdQuals', 'This Educational Qualification has already been enterd.');
        return;
    }

    else {
        $('#' + t).addClass('req');
        $('#' + t).attr('title', 'Field is Required.');
        return;
    }

}

function delEdQualTCmp(fx) {
    $('#spEdQualError').text('');
    $('#dvEdQualError').hide(250);
    $('#spEdQualSuccess').text('');
    $('#dvEdQualSuccess').hide(250);
    $('#lblEducationErr').text('');
    $('#divErrEducation').fadeOut(250);


    var txxv = parseInt(fx.replace('delEdQual', ''));

    if (txxv < 1) {
        alert('Invalid Selection!');
        return;
    }

    $.ajax({
        beforeSend: function () {
            return confirm('Are you sure you want to delete this item?');
        },
        url: "/EducationalQualification/DeleteEducationalQualification?id=" + txxv,
        type: 'POST',
        success: function (i) {
            if (i.ErrorCode < 1) {
                ///Database operation failed/Data could not be deleted
                edQualFeedBackStatus(i.ErrorCode, i.Error);
                return;
            }

            $('#tblEdQuals #rw' + txxv).remove();

            $('#tblEdQuals tr.edrow').each(function (r) {
                $(this).children('.edTd').text(r + 1);
            });

            //$('#tblEdQuals tr:odd').addClass('gridItem1');

            edQualFeedBackStatus(i.ErrorCode, i.Success);
        }

    });
}

function AddEdQualx() {
    $('#spEdQualError').text('');
    $('#dvEdQualError').hide(250);
    $('#spEdQualSuccess').text('');
    $('#dvEdQualSuccess').hide(250);
    $('#lblEducationErr').text('');
    $('#divErrEducation').fadeOut(250);

    if (!validateTemplate('fsEducational')) {
        return;
    }

    var addArray = [];
    var errList = [];
    var addList = $('#fsEducational input[id^="txtEdQualName"]');

    addList.each(function () {
        var vx = $(this);

        if (vx.hasClass('duplicateErr')) {
            errList.push(vx);
        }
        else {
            var rtx = $('#chkEdQual' + vx.attr('id').replace('txtEdQualName', ''));

            if (vx.val().trim().length > 0) {
                var dxf = {};

                dxf[vx.prop('name')] = vx.val();

                if (rtx.is(':checked')) {
                    dxf[rtx.prop('name')] = 'true';
                }
                else {
                    dxf[rtx.prop('name')] = 'false';
                }

                addArray.push(dxf);
            }
        }

    });

    if (errList.length > 0) {
        $('#lblEducationErr').text('Please resolve the duplicate(s) on the page and try again.');
        $('#divErrEducation').fadeIn(300);
        return;
    }
    errList = [];
    if (addArray.length < 1) {
        alert('The Item list could not be compiled. Please try again.');
        return;
    }

    var sdx = JSON.stringify({ myViewObjList: addArray });

    $.ajax({
        url: '/EducationalQualification/AddEducationalQualifications',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: sdx,
        type: 'POST',
        success: function (zxz) {
            if (zxz === 2) {
                zxz = '';
                addArray = [];
                $('#lblEducationErr').text('The Request action could not be executed. Please try again or contact the Administarator.');
                $('#divErrEducation').fadeIn(100);
                return;
            }

            if (zxz.SuccessList.length > 0 && zxz.ErrorList.length < 1) {
                for (var y = 0; y < zxz.SuccessList.length; y++) {
                    var i = zxz.SuccessList[y];
                    addEdQualRows(i);
                }

                if (zxz.SuccessList.length === 1) {
                    edQualFeedBackStatus(1, 'Educational Qualification Information was successfully Submitted.');
                }

                if (addArray.length > 1) {
                    edQualFeedBackStatus(1, 'The Educational Qualification list was successfully Submitted.');
                }

                $('#tblEdQuals tr:odd').addClass('gridItem1');
                setEdQualWrapperSize();
                zxz = '';
                addList = [];
                addArray = [];
                return;
            }

            if (zxz.SuccessList.length < 1 && zxz.ErrorList.length > 0) {
                for (var m = 0; m < zxz.ErrorList.length; m++) {
                    var j = zxz.ErrorList[m];
                    var jx = j.Name.trim().toLowerCase().replace(/ /g, '');

                    for (var h = 0; h < addList.length; h++) {
                        var dhi = addList[h];
                        var dxi = $(dhi).val().replace(/ /g, '').trim().toLowerCase();

                        if (jx === dxi) {
                            var xdi = $(dhi).attr('id').replace('txtEdQualName', '');
                            $('#fsEducational #ErrtxtEdQualName' + xdi).text(j.Error);
                            $('#fsEducational #ErrtxtEdQualName' + xdi).show('fast');
                        }
                    }

                }

                $('#lblEducationErr').text('The item(s) could not be Submitted. Please Check the error message(s) and try again.');
                $('#divErrEducation').fadeIn(100);
                $('#fsEducational #ErrtxtEdQualName' + xdi).text(j.Error);
                zxz = '';
                addList = [];
                addArray = [];
                return;
            }

            if (zxz.SuccessList.length > 0 && zxz.ErrorList.length > 0) {
                $.each(zxz.SuccessList, function (c, g) {
                    var u = g;
                    var txcg = u.Name.trim().toLowerCase().replace(/ /g, '');

                    addList.each(function () {
                        var dfh = $(this);
                        var tdxf = dfh.val().replace(/ /g, '').trim().toLowerCase();

                        var xzd = dfh.attr('id').replace('txtEdQualName', '');

                        if (txcg === tdxf) {
                            $('#tblMoreEdQual #trNw' + xzd).remove();
                        }

                        if (xzd === 1) {
                            $('#tblMoreEdQual #txtEdQualName' + xzd).val('');

                        }
                    });

                    addEdQualRows(g);
                });

                $('#tblEdQuals tr:odd').addClass('gridItem1');


                $.each(zxz.ErrorList, function (c, g) {
                    var p = g;
                    var xcg = p.Name.trim().toLowerCase().replace(/ /g, '');

                    addList.each(function () {
                        var dh = $(this);
                        var dx = dh.val().replace(/ /g, '').trim().toLowerCase();

                        if (xcg === dx) {
                            var xd = dh.attr('id').replace('txtEdQualName', '');
                            var yxu = $('#fsEducational #ErrtxtEdQualName' + xd);
                            yxu.text(p.Error);
                            yxu.show('fast');
                        }
                    });

                    addEdQualRows(p);
                });

                $('#lblEducationErr').text('Some of the items could not be Submitted. Please Check the error messages.');
                $('#divErrEducation').fadeIn(100);
                zxz = '';
                addList = [];
                addArray = [];
                return;
            }
        }
    });
}

function addEdQualRows(i) {
    var status = '';
    var check = '';
    if (i.ActiveStatus === true) {
        status = 'Active';
        check = '&nbsp;<input id="chkEdQual" checked="true" class="customNewCheckbox rtx' + i.EducationalQualificationId + '" value="Active?" type="checkbox" />';
    }
    else {
        status = 'Inactive';
        check = '&nbsp;<input id="chkEdQual" checked="false" class="customNewCheckbox rtx' + i.EducationalQualificationId + '" value="Active?" type="checkbox" />';
    }

    var bv = $('#tblEdQuals tr').length - 1;

    $($('#tblEdQuals tbody:last').append($('<tr class="edrow" id="rw' + i.EducationalQualificationId + '"><td id="td' + i.EducationalQualificationId + '" class="edTd bxcf ssd">' + (bv + 1) + '</td><td class="bxcf fxxv"><span class="dsx' + i.EducationalQualificationId + '" id="spEdQualContent' + i.EducationalQualificationId + '" style="float: left">' + i.Name + '</span>'
     + '<input type="text" name="Name" id="txtTblEdQual' + i.EducationalQualificationId + '" value="' + i.Name + '" class="form-control xxvz rtx' + i.EducationalQualificationId + '" onchange="checkToggleEdQual(this.id)" style=" display: none; float: left;"/>'
     + '</td><td class="bxcf kxxd"><span class="dsx' + i.EducationalQualificationId + '" id="spEdQualChck' + i.EducationalQualificationId + '" style="float: left">' + status + '</span>'
     + '<div style="display: none; float: left" class="rtx' + i.EducationalQualificationId + '">Active? &nbsp;' + check + '</div>'
     + '</td><td class="bxcf kxxd" ><a class="edrTx dsx' + i.EducationalQualificationId + '" title="Edit" id="edEdQualInf' + i.EducationalQualificationId + '" onclick="edEdQualXg(this.id)"><img src="/Images/Metro-black/edit.png" alt="Edit" style="width: 20px; height: 20px" /></a>'
     + '<a class="rtx' + i.EducationalQualificationId + '" title="Update" style="display: none; cursor:pointer" id="upEdQualInf' + i.EducationalQualificationId + '" onclick="upEdQualXg(this.id)"><img src="/Images/Metro-black/refresh.png" alt="Update" style="width: 20px; height: 20px" /></a>&nbsp;'
     + '<a class="dnTx rtx' + i.EducationalQualificationId + '" onclick="cEdQualSx(this.id)" title="Cancel" id="cnc' + i.EducationalQualificationId + '" style="cursor:pointer; display: none"><img src="/Images/Metro-black/cancel.png" alt="Cancel" style="width: 20px; height: 20px" /></a>'
     + '<a class="delTx dsx' + i.EducationalQualificationId + '" title="Delete" id="delEdQual' + i.EducationalQualificationId + '" onclick="delEdQualTCmp(this.id)"><img src="/Images/Metro-black/delete.gif" alt="Delete" style="width: 20px; height: 20px" /></a>'
     + '</td></tr>')));
}


// ---------------------------------------- End of Educational Qualification 




//----------------------------------------- Education Type ----------------------------------------------------------------------------

var xcrtEdtp = 2;
var fxColEdTp = [];
var xmailEdTp = '';
var xChkEdTpBr = '';
var xBtnEdTpPrBr = '';
var xErrEdTpDisp = '';
var edTpContainer = '';
var edTpOriginalWidth = '';
var edTpOriginalHeight = '';

function addEdTypeRow() {
    $($('#tblMoreEdType tbody:last').append($('<tr id="trNw' + xcrtEdtp + '" class ="ggxv"><td  class="form-group" style="vertical-align: bottom; width: 100%" colspan="2">'
      + '<div><i style="width: auto">Name:</i><span style="color: red">*</span> <span id="sptxtEdTypeName' + xcrtEdtp + '" style="color: red; font-size: 8pt; display: none">Required </span><span id="ErrEdTypetxtEdTypeName' + xcrtEdtp + '" style="color: red; font-size: 8pt; display: none"></span><img src="../../Images/Metro-white/cancel.png" id="imgEdTpNewRw' + xcrtEdtp + '" class="addNewFieldRow" onclick="remEdTpRow(this.id)" title="Remove Item"/></div>'
      + '<input name = "Name" style="width:100%" id="txtEdTypeName' + xcrtEdtp + '" class="form-control" onchange="checkEdTypeDups(this)"/><div style="margin-top: 2%">'
      + '&nbsp;<label style="float: right; padding-left: 1%">Active?</label> <input  name = "ActiveStatus" id="chkEdType' + xcrtEdtp + '" class="customNewCheckbox" value="Active?" type="checkbox" style="padding-top: 25px; float: right"/>'
      + '</div> </td></tr>')));
    $('#trMoreEdType').show(300);
    xcrtEdtp++;
    setValidators();
}

function remEdTpRow(jxk) {
    var dsx = parseInt(jxk.replace('imgEdTpNewRw', ''));
    if (dsx < 1) {
        alert('Item could not be removed. Please refresh the page and try again.');
        return;
    }

    $('#tblMoreEdType #trNw' + dsx).remove();

    if ($('#tblMoreEdType tr.ggxv').length < 1) {
        $('#trMoreEdType').hide('fast');
    }

}

$(window).load(function () {
    $('#tblEdTypes tr:odd').addClass('gridItem1');
    $('#lblEdTypeErr').addClass('errx');

});

function contentScroll() {

    //var winHeight = $(window).scrollTop() + $(window).height();

    //    if (winHeight > 100) //$('.footer').offset().top)
    //    {
    //        if ($("#btnEdTpScrollTop").css('display') == 'inline-block')
    //        {
    //            $("#btnEdTpScrollTop").css('margin-top', winHeight);
    //        }
    //        else
    //        {
    //            $("#btnEdTpScrollTop").css('margin-top', winHeight);
    //            $("#btnEdTpScrollTop").fadeIn('fast');
    //        }
    //    }
    //    else
    //    {
    //        $("#btnEdTpScrollTop").fadeOut('fast');
    //    }
    //});

    //$(this)[0].scrollHeight

    var scrll = $('#fsEdTypeList');
    var lftMg = scrll.width() + 100;
    //var sdc = parseInt(scrll.css('max-height').replace('px', ''));
    // var th = scrll.height();
    var tw = $('#fsEdTypeList').scrollTop() + 100;
    // var winHeight = th + tw;

    if (tw > 100) {
        if ($("#dvEdTpScrll").css('display') == 'inline-block' || $("#dvEdTpScrll").css('display') == 'block') {
            $("#dvEdTpScrll").css({ 'z-index': '100000', 'left': lftMg + 'px', 'top': tw + 'px' });
        }
        else {
            $("#dvEdTpScrll").css({ 'z-index': '100000', 'left': lftMg + 'px', 'top': tw + 'px' });
            $("#dvEdTpScrll").fadeIn('fast');

        }
    }

    if (tw < 230) {
        if ($("#dvEdTpScrll").css('display') == 'inline-block' || $("#dvEdTpScrll").css('display') == 'block') {
            $("#dvEdTpScrll").fadeOut('fast');
            $("#dvEdTpScrll").css({ 'z-index': '' });
        }
    }
}

function scrollTop() {
    $("#btnEdTpScrollTop").click(function () {
        $('#fsEdTypeList').animate({
            scrollTop: $("#btnAddNewEdType").offset().top
        }, 400);
    });
}

function checkEdTypeDups(xcid) {
    checkDuplicates(xcid, 'fsEdType', 'This Department has already been enterd.');
}

function setValidators() {
    toggleValidators('fsEdType');
}

function cncEdTpFeedbackDiv() {
    closePopModal($('#dvEdTpErr'));
    $('#tblEdTpReport').find("tr:gt(0)").remove();
}

$(document).ready(function () {
    $('#fsEdTypeList').on('scroll', contentScroll);
    $("#btnEdTpScrollTop").on('click', scrollTop);
    edTpContainer = $('#fsEdTypeAll');
    xErrEdTpDisp = $('#divErrEdTypes');
    xmailEdTp = $('#txtEdTypeName');
    xChkEdTpBr = $('#chkEdType');
    xBtnEdTpPrBr = $('#btnProcessEdType');
    edTpOriginalWidth = edTpContainer.children('.activeFieldSet').width();
    edTpOriginalHeight = edTpContainer.children('.activeFieldSet').height();
    edTpContainer.children('fieldset').each(function () {
        var theFieldSet = $(this);

        if (!theFieldSet.hasClass('activeFieldSet')) {
            theFieldSet.addClass('inActiveFieldSet');
            edTpContainer.data({
                width: parseInt(theFieldSet.css("width")),
                height: theFieldSet.height()
            });
        }

    });
});

function validateInput() {
    if (xmailEdTp.val().replace(' ', '').length < 1) {
        return;
    }
    setEdWrpperSize();
}

function CleanEdTypeFieldset() {
    xErrEdTpDisp.addClass("inActiveFieldSet");
    $('#fsEdTypeTitle').text("New Education Type(s)");
    refreshForm('fsEdType', '');
    $('#lblEdTypeErr').text('');
    $('#divErrEdTypes').hide();
    $('#fsEdType [id^="ErrEdTypetxtEdTypeName"]').each(function () {
        $(this).text('');
        $(this).hide();
    });
    setEdWrpperSize();
    removeEdTp();
    $("#spEdTypeSuccess").text('');
    $("#dvEdTypeSuccess").hide();
}

function removeEdTp() {
    $('#tblMoreEdType tr.ggxv').each(function () {
        $(this).remove();
    });
    $('#trMoreEdType').hide();
}

function retoggleBtnValue() {
    setEdWrpperSize();
}

function setEdWrpperSize() {
    var txd = edTpContainer.children('.activeFieldSet').attr("id");
    var inActiveFieldset = '';
    var activeFieldset = '';
    if (txd === $('#fsEdTypeList').attr("id")) {
        inActiveFieldset = $('#fsEdType');
        activeFieldset = $('#fsEdTypeList');
    }
    else {
        if (txd === $('#fsEdType').attr("id")) {
            inActiveFieldset = $('#fsEdTypeList');
            activeFieldset = $('#fsEdType');
        }
    }
    activeFieldset.fadeOut("slow", function () {
        edTpContainer.css({ "background": "white" });

        activeFieldset.removeClass('activeFieldSet');

        if (inActiveFieldset.attr("id") === $('#fsEdTypeList').attr("id")) {
            edTpContainer.stop()
                .animate(
                    {
                        width: edTpOriginalWidth + 'px',
                        height: edTpOriginalHeight + 'px'

                    }, 500, function () {
                        inActiveFieldset.addClass('activeFieldSet');
                        activeFieldset.addClass('inActiveFieldSet');
                        inActiveFieldset.fadeIn("slow");
                        edTpContainer.css({ "background": "whitesmoke" });
                    });
        }
        else {
            edTpContainer.stop()
                .animate(
                    {
                        width: parseInt(inActiveFieldset.css("width").replace('px', '')) + 'px',
                        height: inActiveFieldset.height() + 'px'

                    }, 500, function () {
                        inActiveFieldset.addClass('activeFieldSet');
                        activeFieldset.addClass('inActiveFieldSet');
                        inActiveFieldset.width((edTpContainer.width()) + 'px');
                        inActiveFieldset.fadeIn("slow");
                        edTpContainer.css({ "background": "whitesmoke" });


                    });
        }

    });

}

function edEdTypeTxg(r) {
    $('#spEdTypeError').text('');
    $('#dvEdTypeError').hide(250);
    $('#spEdTypeSuccess').text('');
    $('#dvEdTypeSuccess').hide(250);
    $('#lblEdTypeErr').text('');
    $('#divErrEdTypes').fadeOut(250);

    var cn = parseInt(r.replace('edCInfEdType', ''));
    if (cn < 1) {
        alert('Invalid Selection!');
        return;
    }

    $('.dsx' + cn).hide('slow');
    $('.rtx' + cn).show('slow');
    fxColEdTp.push(cn);
    if (fxColEdTp.length > 1) {
        $('#btnUpdateAllEdType').show('slow');
    }
}

function csxEdTp(r) {
    $('#spEdTypeError').text('');
    $('#dvEdTypeError').hide(250);
    $('#spEdTypeSuccess').text('');
    $('#dvEdTypeSuccess').hide(250);
    $('#lblEdTypeErr').text('');
    $('#divErrEdTypes').fadeOut(250);

    var cn = parseInt(r.replace('cncEdTp', ''));
    if (cn < 1) {
        alert('Invalid Selection!');
        return;
    }

    var tm = $('#txtEdTp' + cn);
    var rtx = $('.rtx' + cn);

    if ($('#spChckEdType' + cn).text() == 'Active') {
        rtx.prop('checked', true);
    }
    else {
        rtx.prop('checked', false);
    }

    $(tm).removeClass('duplicateErr');
    tm.removeClass('req');
    tm.attr('title', '');
    $(tm).val($('#spEdTypeContent' + cn).text());
    $('.rtx' + cn).hide('slow');
    $('.dsx' + cn).show('slow');
    fxColEdTp.splice($.inArray(cn, fxColEdTp), 1);
    if (fxColEdTp.length < 2) {
        $('#btnUpdateAllEdType').hide('slow');
    }
}

function updateAllEdType() {
    $('#spEdTypeError').text('');
    $('#dvEdTypeError').hide(250);
    $('#spEdTypeSuccess').text('');
    $('#dvEdTypeSuccess').hide(250);
    $('#lblEdTypeErr').text('');
    $('#divErrEdTypes').fadeOut(250);


    var updtCol = [];
    var errList = [];
    if (fxColEdTp.length < 1) {
        alert('The update list could not be retrieved. Please try again.');
        return;
    }

    for (var n = 0; n < fxColEdTp.length; n++) {
        var vx = fxColEdTp[n];

        var tm = $('#txtEdTp' + vx);
        var rtx = $('.rtx' + vx);

        if (tm.hasClass('duplicateErr')) {
            errList.push(tm);
        }
        else {
            if (tm.val().trim().length < 1) {
                tm.addClass('req');
                tm.attr('title', 'Required');
                return;
            }

            else {
                tm.removeClass('req');
                tm.attr('title', '');
            }

            var dxf = {};

            dxf['Name'] = tm.val();

            dxf['EducationTypeId'] = vx;

            if (rtx.is(':checked')) {
                dxf['ActiveStatus'] = 'true';
            }
            else {
                dxf['ActiveStatus'] = 'false';
            }

            updtCol.push(dxf);
        }
    }

    if (errList.length > 0) {
        $('#spEdTypeError').text('Please resolve the duplicate(s) on the page and try again.');
        $('#dvEdTypeError').fadeIn(300);
        return;
    }

    if (updtCol.length < 1) {
        alert('The update list could not be compiled. Please try again.');
        return;
    }

    var sdx = JSON.stringify({ myViewObjList: updtCol });

    $.ajax({

        url: "/EducationType/EditMultipleEducationType",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: sdx,
        type: 'POST',
        success: function (zxz) {
            if (zxz === 2) {
                deptFeedBackStatus(0, 'The selected items could not be updated. Please try again or contact the Administarator.');
                return;
            }

            if (zxz.SuccessList.length > 0 && zxz.ErrorList.length < 1) {

                for (var y = 0; y < zxz.SuccessList.length; y++) {
                    var cvx = zxz.SuccessList[y].EducationTypeId;
                    var txm = $('#txtEdTp' + cvx);
                    var rctx = $('.rtx' + cvx);
                    var tx = $('#spEdTypeContent' + cvx);
                    var rcx = $('#spChckEdType' + cvx);

                    tx.text(txm.val());

                    if (rctx.is(':checked')) {
                        rcx.text('Active');
                    }
                    else {
                        rcx.text('Inactive');
                    }

                    $('.rtx' + cvx).hide('slow');
                    $('.dsx' + cvx).show('slow');
                }

                $('#btnUpdateAllEdType').hide('slow');
                deptFeedBackStatus(1, 'The selected items were successfully updated.');
                fxColEdTp = [];
                return;
            }

            if (zxz.SuccessList.length > 0 && zxz.ErrorList.length > 0) {
                for (var q = 0; q < zxz.SuccessList.length; q++) {
                    var cx = zxz.SuccessList[q].EducationTypeId;
                    var txfm = $('#txtEdTp' + cx);
                    var rx = $('.rtx' + cx);
                    var txf = $('#spEdTypeContent' + cx);
                    var xr = $('#spChckEdType' + cx);

                    txf.text(txfm.val());

                    if (rx.is(':checked')) {
                        xr.text('Active');
                    }

                    else {
                        xr.text('Inactive');
                    }

                    $('.rtx' + cx).hide('slow');
                    $('.dsx' + cx).show('slow');
                    fxColEdTp.splice($.inArray(cx, fxColEdTp), 1);
                }

                if (zxz.ErrorList.length > 0) {
                    if (zxz.ErrorList.length > 1) {
                        $('#btnUpdateAllEdType').show('slow');
                        $('#tblEdTpReport').find("tr:gt(0)").remove();

                        for (var g = 0; g < zxz.ErrorList.length; g++) {
                            var tmb = zxz.ErrorList[g];
                            $($('#tblEdTpReport tbody:last').append($('<tr><td  style="vertical-align: top; width: 40%">'
                                + tmb.Name + '</td>'
                                + '<td style="vertical-align: top; width: 60%"><span style="color: red; font-size: 11pt;">' + tmb.Error + ' </span>'
                                + '</td></tr>')));
                        }
                        deptFeedBackStatus(0, 'Some of the selected items could not be updated. Please try again or contact the Administarator.');
                        setModal($('#dvEdTpErr'));
                    }

                    else {
                        $('#btnUpdateAllEdType').hide('slow');
                        $('#txtEdTp' + zxz.ErrorList[0].EducationTypeId).addClass('req');
                        deptFeedBackStatus(0, 'The operation returned with an error. The Item(s) could not be updated');
                    }

                }
                return;
            }


            if (zxz.SuccessList.length < 1 && zxz.ErrorList.length > 0) {
                if (zxz.ErrorList.length > 1) {
                    $('#btnUpdateAllEdType').show('slow');
                    for (var a = 0; a < zxz.ErrorList.length; a++) {
                        var txmb = zxz.ErrorList[a];
                        $($('#tblEdTpReport tbody:last').append($('<tr><td  style="vertical-align: top; width: 40%">'
                            + txmb.Name + '</td>'
                            + '<td style="vertical-align: top; width: 60%"><span style="color: red; font-size: 11pt;">' + txmb.Error + ' </span>'
                            + '</td></tr>')));
                    }

                    deptFeedBackStatus(0, 'None of the selected items could be updated. Please try again or contact the Administarator.');
                    setModal($('#dvEdTpErr'));
                }

                if (zxz.ErrorList.length === 1) {
                    $('#btnUpdateAllEdType').hide('slow');
                    $('#txtEdTp' + zxz.ErrorList[0].EducationTypeId).addClass('req');
                    deptFeedBackStatus(0, 'The operation returned with an error. The Item(s) could not be updated');
                }
                return;
            }

        }
    });
}

function upEdTypeTxg(y) {
    $('#spEdTypeError').text('');
    $('#dvEdTypeError').hide(250);
    $('#spEdTypeSuccess').text('');
    $('#dvEdTypeSuccess').hide(250);
    $('#lblEdTypeErr').text('');
    $('#divErrEdTypes').fadeOut(250);


    var txxv = parseInt(y.replace('upCInfEdType', ''));

    if (txxv < 1 || txxv === NaN || txxv == 'NaN') {
        alert('Invalid Selection!');
        return;
    }

    var tm = $('#txtEdTp' + txxv);

    if (tm.hasClass('duplicateErr')) {
        $('#spEdTypeError').text('Please resolve the duplicate(s) on the page and try again.');
        $('#dvEdTypeError').fadeIn(300);
        return;
    }

    if (tm.val().trim().toLowerCase().replace(/ /g, '') < 1) {
        tm.addClass('req');
        $('#spEdTypeError').text('Please enter Education Type.');
        tm.attr('title', 'Education Type is required.');
        $('#dvEdTypeError').fadeIn(300);
        return;
    }
    else {
        tm.attr('title', '');
        tm.removeClass('req');
    }


    if (tm.hasClass('req')) {
        $('#spEdTypeError').text('Please enter Education Type.');
        $('#dvEdTypeError').fadeIn(300);
        return;
    }

    fxColEdTp.splice($.inArray(txxv, fxColEdTp), 1);

    var rtx = $('.rtx' + txxv);

    if (tm.val().trim().length < 1) {
        tm.css({ 'border-color': 'red' });
        return;
    }
    else {
        tm.css({ 'border-color': '#CCC' });
    }

    var dxf = {};

    dxf['Name'] = tm.val();

    dxf['EducationTypeId'] = txxv;

    if (rtx.is(':checked')) {
        dxf['ActiveStatus'] = 'true';
    }
    else {
        dxf['ActiveStatus'] = 'false';
    }

    var sdx = JSON.stringify({ myViewObj: dxf });

    $.ajax({

        url: "/EducationType/EditEducationType",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: sdx,
        type: 'POST',
        success: function (dTx) {
            var i = dTx;

            if (i.ErrorCode < 1) {
                ///Database operation failed/Data could not be deleted
                deptFeedBackStatus(i.ErrorCode, i.Error);
                return;
            }

            if (i.ActiveStatus === true) {
                $('#spChckEdType' + txxv).text('Active');
            }
            else {
                $('#spChckEdType' + txxv).text('Inactive');
            }
            $('#spEdTypeContent' + txxv).text(i.Name);

            $('.rtx' + txxv).hide('slow');
            $('.dsx' + txxv).show('slow');
            deptFeedBackStatus(i.ErrorCode, i.Success);
        }
    });
}

function deptFeedBackStatus(x, z) {
    var dx = parseInt(x);

    if (dx === 0) {
        $("#spEdTypeError").text(z);
        $("#dvEdTypeError").fadeIn();
    }
    //
    if (dx === 1) {
        $("#spEdTypeSuccess").text(z);
        $("#dvEdTypeSuccess").fadeIn();
    }

}

function checkToggleEdType(t) {
    var gdx = $('#' + t);
    if (gdx.val().trim().length > 0) {
        gdx.removeClass('req');
        gdx.attr('title', '');
        checkDuplicates(gdx, 'tblEdTypes', 'This Department has already been enterd.');
        return;
    }

    else {
        $('#' + t).addClass('req');
        $('#' + t).attr('title', 'Field is Required.');
        return;
    }

}

function delEdTpTCmp(fx) {
    $('#spEdTypeError').text('');
    $('#dvEdTypeError').hide(250);
    $('#spEdTypeSuccess').text('');
    $('#dvEdTypeSuccess').hide(250);
    $('#lblEdTypeErr').text('');
    $('#divErrEdTypes').fadeOut(250);


    var txxv = parseInt(fx.replace('delEdTp', ''));

    if (txxv < 1) {
        alert('Invalid Selection!');
        return;
    }

    $.ajax({
        beforeSend: function () {
            return confirm('Are you sure you want to delete this item?');
        },
        url: "/EducationType/DeleteEducationType?id=" + txxv,
        type: 'POST',
        success: function (i) {
            if (i.ErrorCode < 1) {
                ///Database operation failed/Data could not be deleted
                deptFeedBackStatus(i.ErrorCode, i.Error);
                return;
            }

            $('#tblEdTypes #rw' + txxv).remove();

            $('#tblEdTypes tr.edrow').each(function (r) {
                $(this).children('.edTd').text(r + 1);
            });

            //$('#tblEdTypes tr:odd').addClass('gridItem1');

            deptFeedBackStatus(i.ErrorCode, i.Success);
        }

    });
}

function addEdTypXg() {
    $('#spEdTypeError').text('');
    $('#dvEdTypeError').hide(250);
    $('#spEdTypeSuccess').text('');
    $('#dvEdTypeSuccess').hide(250);
    $('#lblEdTypeErr').text('');
    $('#divErrEdTypes').fadeOut(250);

    if (!validateTemplate('fsEdType')) {
        return;
    }

    var addArray = [];
    var errList = [];
    var addList = $('#fsEdType input[id^="txtEdTypeName"]');

    addList.each(function () {
        var vx = $(this);

        if (vx.hasClass('duplicateErr')) {
            errList.push(vx);
        }
        else {
            var rtx = $('#chkEdType' + vx.attr('id').replace('txtEdTypeName', ''));

            if (vx.val().trim().length > 0) {
                var dxf = {};

                dxf[vx.prop('name')] = vx.val();

                if (rtx.is(':checked')) {
                    dxf[rtx.prop('name')] = 'true';
                }
                else {
                    dxf[rtx.prop('name')] = 'false';
                }

                addArray.push(dxf);
            }
        }

    });

    if (errList.length > 0) {
        $('#lblEdTypeErr').text('Please resolve the duplicate(s) on the page and try again.');
        $('#divErrEdTypes').fadeIn(300);
        return;
    }
    errList = [];
    if (addArray.length < 1) {
        alert('The Item list could not be compiled. Please try again.');
        return;
    }

    var sdx = JSON.stringify({ myViewObjList: addArray });

    $.ajax({
        url: '/EducationType/AddEducationTypes',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: sdx,
        type: 'POST',
        success: function (zxz) {

            if (zxz === 2) {
                zxz = '';
                addArray = [];
                $('#lblEdTypeErr').text('The Request action could not be executed. Please try again or contact the Administarator.');
                $('#divErrEdTypes').fadeIn(100);
                return;
            }

            if (zxz.SuccessList.length > 0 && zxz.ErrorList.length < 1) {
                for (var y = 0; y < zxz.SuccessList.length; y++) {
                    var i = zxz.SuccessList[y];
                    addRows(i);
                }

                if (zxz.SuccessList.length === 1) {
                    deptFeedBackStatus(1, 'Education Type Information was successfully Submitted.');
                }

                if (addArray.length > 1) {
                    deptFeedBackStatus(1, 'The Education Type list was successfully Submitted.');
                }

                $('#tblEdTypes tr:odd').addClass('gridItem1');
                setEdWrpperSize();
                zxz = '';
                addList = [];
                addArray = [];
                return;
            }

            if (zxz.SuccessList.length < 1 && zxz.ErrorList.length > 0) {
                for (var m = 0; m < zxz.ErrorList.length; m++) {
                    var j = zxz.ErrorList[m];
                    var jx = j.Name.trim().toLowerCase().replace(/ /g, '');

                    for (var h = 0; h < addList.length; h++) {
                        var dhi = addList[h];
                        var dxi = $(dhi).val().replace(/ /g, '').trim().toLowerCase();

                        if (jx === dxi) {
                            var xdi = $(dhi).attr('id').replace('txtEdTypeName', '');
                            $('#fsEdType #ErrEdTypetxtEdTypeName' + xdi).text(j.Error);
                            $('#fsEdType #ErrEdTypetxtEdTypeName' + xdi).show('fast');
                        }
                    }

                }

                $('#lblEdTypeErr').text('The item(s) could not be Submitted. Please Check the error message(s) and try again.');
                $('#divErrEdTypes').fadeIn(100);
                $('#fsEdType #ErrEdTypetxtEdTypeName' + xdi).text(j.Error);
                zxz = '';
                addList = [];
                addArray = [];
                return;
            }

            if (zxz.SuccessList.length > 0 && zxz.ErrorList.length > 0) {
                $.each(zxz.SuccessList, function (c, g) {
                    var u = g;
                    var txcg = u.Name.trim().toLowerCase().replace(/ /g, '');

                    addList.each(function () {
                        var dfh = $(this);
                        var tdxf = dfh.val().replace(/ /g, '').trim().toLowerCase();

                        var xzd = dfh.attr('id').replace('txtEdTypeName', '');

                        if (txcg === tdxf) {
                            $('#tblMoreEdType #trNw' + xzd).remove();
                        }

                        if (xzd === 1) {
                            $('#tblMoreEdType #txtEdTypeName' + xzd).val('');

                        }
                    });

                    addRows(g);
                });

                $('#tblEdTypes tr:odd').addClass('gridItem1');


                $.each(zxz.ErrorList, function (c, g) {
                    var p = g;
                    var xcg = p.Name.trim().toLowerCase().replace(/ /g, '');

                    addList.each(function () {
                        var dh = $(this);
                        var dx = dh.val().replace(/ /g, '').trim().toLowerCase();

                        if (xcg === dx) {
                            var xd = dh.attr('id').replace('txtEdTypeName', '');
                            var yxu = $('#fsEdType #ErrEdTypetxtEdTypeName' + xd);
                            yxu.text(p.Error);
                            yxu.show('fast');
                        }
                    });

                    addRows(p);
                });

                $('#lblEdTypeErr').text('Some of the items could not be Submitted. Please Check the error messages.');
                $('#divErrEdTypes').fadeIn(100);
                zxz = '';
                addList = [];
                addArray = [];
                return;
            }
        }
    });
}

function addRows(i) {
    var status = '';
    var check = '';
    if (i.ActiveStatus === true) {
        status = 'Active';
        check = '&nbsp;<input id="chkEdType" checked="true" class="customNewCheckbox rtx' + i.EducationTypeId + '" value="Active?" type="checkbox" />';
    }
    else {
        status = 'Inactive';
        check = '&nbsp;<input id="chkEdType" checked="false" class="customNewCheckbox rtx' + i.EducationTypeId + '" value="Active?" type="checkbox" />';
    }

    var bv = $('#tblEdTypes tr').length - 1;

    $($('#tblEdTypes tbody:last').append($('<tr class="edrow" id="rw' + i.EducationTypeId + '"><td id="td' + i.EducationTypeId + '" class="edTd bxcf ssd">' + (bv + 1) + '</td><td class="bxcf fxxv"><span class="dsx' + i.EducationTypeId + '" id="spEdTypeContent' + i.EducationTypeId + '" style="float: left">' + i.Name + '</span>'
     + '<input type="text" name="Name" id="txtEdTp' + i.EducationTypeId + '" value="' + i.Name + '" class="form-control xxvz rtx' + i.EducationTypeId + '" onchange="checkToggleEdType(this.id)" style=" display: none; float: left;"/>'
     + '</td><td class="bxcf kxxd"><span class="dsx' + i.EducationTypeId + '" id="spChckEdType' + i.EducationTypeId + '" style="float: left">' + status + '</span>'
     + '<div style="display: none; float: left" class="rtx' + i.EducationTypeId + '">Active? &nbsp;' + check + '</div>'
     + '</td><td class="bxcf kxxd" ><a class="edrTx dsx' + i.EducationTypeId + '" title="Edit" id="edCInfEdType' + i.EducationTypeId + '" onclick="edEdTypeTxg(this.id)"><img src="/Images/Metro-black/edit.png" alt="Edit" style="width: 20px; height: 20px" /></a>'
     + '<a class="rtx' + i.EducationTypeId + '" title="Update" style="display: none; cursor:pointer" id="upCInfEdType' + i.EducationTypeId + '" onclick="upEdTypeTxg(this.id)"><img src="/Images/Metro-black/refresh.png" alt="Update" style="width: 20px; height: 20px" /></a>&nbsp;'
     + '<a class="dnTx rtx' + i.EducationTypeId + '" onclick="csxEdTp(this.id)" title="Cancel" id="cncEdTp' + i.EducationTypeId + '" style="cursor:pointer; display: none"><img src="/Images/Metro-black/cancel.png" alt="Cancel" style="width: 20px; height: 20px" /></a>'
     + '<a class="delTx dsx' + i.EducationTypeId + '" title="Delete" id="delEdTp' + i.EducationTypeId + '" onclick="delEdTpTCmp(this.id)"><img src="/Images/Metro-black/delete.gif" alt="Delete" style="width: 20px; height: 20px" /></a>'
     + '</td></tr>')));
}


// ---------------------------------------- End of Education Type

