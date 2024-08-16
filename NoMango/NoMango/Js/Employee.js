$(document).ready(function () {
    var custumerJS = new CustumerJS();
})
loadData(1);
function loadData(msg) {
    $.each(data, function (index, item) {
        var trHTML = $(`<tr>
            <td>`+ item.EmployeeCode + `</td>
            <td>`+ item.EmpooyeeName + `</td>
            <td>`+ item.Mobile + `</td>  
            <td>`+ item.Email + `</td>
            <td>`+ item.Address + `</td>
            <td>`+ item.cty + `</td>
        
        </tr>`);
        $('.grid tbody').append(trHTML);
    });


}


var data = [
    {
        EmployeeCode: "Nv0002",
        EmpooyeeName: "Le Van Tung",
        Email: "Tung:V1997@",
        Mobile: "0978366106",
        Address: "Ha Noi",
        cty: "Robotcom FA"
    },
    {
        EmployeeCode: "Nv0005",
        EmpooyeeName: "Bùi thị Thảo",
        Email: "Tung:V1997@",
        Mobile: "0978366106",
        Address: "Ha Noi",
        cty: "Robotcom FA"
    },
    {
        EmployeeCode: "Nv0006",
        EmpooyeeName: "Le Van ANH",
        Email: "Tung:V1997@",
        Mobile: "0978366106",
        Address: "Ha Noi",
        cty: "Robotcom FA"
    }
]

class CustumerJS {
    constructor() { // Thực thi ngay khi đối tượng được khởi tạo
        this.loadData("xin chao");
        this.initEvents();
    }
    loadData(msg) {
        $.each(data, function (index, item) {
            var trHTML = $(`<tr>
            <td>`+ item.EmployeeCode + `</td>
            <td>`+ item.EmpooyeeName + `</td>
            <td>`+ item.Mobile + `</td>  
            <td>`+ item.Email + `</td>
            <td>`+ item.Address + `</td>
        </tr>`);
            $('.grid tbody').append(trHTML);
        });


    }

    initEvents() {
        $('#btnAdd').click(this.btnAddOnClick);
        $('#btncancel').click(this.btncancel);
        $('#btncancel').click(this.btncancel);
        $('.dialog-button-close').click(this.btncancel);
        $('#btnSave').click(this.btnSave);
        $('#txtMa_nv').blur(this.checkReQuired)
    }

    checkReQuired() {
        $('.requied').addClass('requirederror');
    }

    btnSave() {
        var employeCode = $('#txtMa_nv').val();
        var employeName = $('#txtTen_nv').val();

        if (!employeCode) {
            //$('#txtMa_nv').addClass('requirederror');
            $('#txtMa_nv').focus();
            $('#txtMa_nv').attr("title", "Bạn cần nhập thông tin này!");
            return;
        }
        else {
            
        }

        if (!employeName) {
            alert("Bạn phải nhập tên nhân viên!");
            return;
        }

        var employee = {};
        debugger;

        employee.EmployeeCode = $('#txtMa_nv').val();
        employee.EmpooyeeName = $('#txtTen_nv').val();
        employee.Email = $('#txtEmail').val();
        employee.Mobile = $('#txtMobile').val();
        employee.cty = $('#txtCongTy').val();

        data.push(employee);
        this.this.loadData("xin chao");


    }

    
    btnAddOnClick() {
        $('.dialog-model').show();
        $('.dialog').show();
        $('#txtMa_nv').focus();

    }

    btncancel() {
        $('.dialog-model').hide();
        $('.dialog').show();
    }

    edit() {

    }

    delete() {

    }


}

