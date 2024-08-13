$(document).ready(function () {

    var custumerJS = new CustumerJS();
    custumerJS.loadData('mas');
})



var data = [
    {
        EmployeeCode: "Nv0002",
        EmpooyeeName: "Le Van Tung",
        Email: "Tung:V1997@",
        Mobile: "0978366106",
        Address: "Ha Noi"
    },
    {
        EmployeeCode: "Nv0005",
        EmpooyeeName: "Bùi thị Thảo",
        Email: "Tung:V1997@",
        Mobile: "0978366106",
        Address: "Ha Noi"
    },
    {
        EmployeeCode: "Nv0006",
        EmpooyeeName: "Le Van ANH",
        Email: "Tung:V1997@",
        Mobile: "0978366106",
        Address: "Ha Noi"
    },
    {
        EmployeeCode: "Nv0006",
        EmpooyeeName: "Le Van ANH",
        Email: "Tung:V1997@",
        Mobile: "0978366106",
        Address: "Viet Nam"
    }
]



class CustumerJS {
    constructor() { // Thực thi ngay khi đối tượng được khởi tạo
        //this.loadData("xin chao");
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

    add() {

    }

    edit() {

    }

    delete() {

    }

}
