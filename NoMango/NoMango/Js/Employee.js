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
        cty:"Robotcom FA"
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
    }

    btnAddOnClick() {
        $('.dialog-model').show();
    }

    btncancel() {
        $('.dialog-model').hide();
    }

    edit() {

    }

    delete() {

    }
}
