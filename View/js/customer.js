$(document).ready(function(){
    loadData(3);
})
loadData(1);
function loadData(msg){
$.each(data, function(index, item)
{
    var trHTML = $(`<tr>
        <td>`+item.EmployeeCode+`</td>
        <td>`+item.EmpooyeeName+`</td>
        <td>`+item.Mobile+`</td>  
        <td>`+item.Email+`</td>
        <td>`+item.Address+`</td>
    </tr>`);
    $('.grid tbody').append(trHTML);
});


}


var data=[
    {
        EmployeeCode:"Nv0002",
        EmpooyeeName:"Le Van Tung",
        Email:"Tung:V1997@",
        Mobile:"0978366106",
        Address:"Ha Noi"
    },
    {
        EmployeeCode:"Nv0005",
        EmpooyeeName:"Bùi thị Thảo",
        Email:"Tung:V1997@",
        Mobile:"0978366106",
        Address:"Ha Noi"
    },
    {
        EmployeeCode:"Nv0006",
        EmpooyeeName:"Le Van ANH",
        Email:"Tung:V1997@",
        Mobile:"0978366106",
        Address:"Ha Noi"
    }
]

