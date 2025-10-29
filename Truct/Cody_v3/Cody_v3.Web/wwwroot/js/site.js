// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function showLoading() {
    $(':input[type="submit"]').prop('disabled', true);
    $("#fullscreenloading")?.show();

    $(document).ready(function () {
        var connection = new signalR.HubConnectionBuilder().withUrl("/ExcelHandlingHub").build();

        //document.getElementById("sendButton").disabled = true;

        connection.on("ReceiveMessage", function (message, percent) {
            if (message) {
                $("#LoadingMessage").html(message);
                $("#ProgressSub").width((percent | 0) + '%');
                $("#ProgressSub").html(percent + '%')
                console.log('=================\n connection.on("ReceiveMessage", function (message) ', (percent | 0)+'%');
            }
            //var li = document.createElement("li");
            //document.getElementById("messagesList").appendChild(li);
            //// We can assign user-supplied strings to an element's textContent because it
            //// is not interpreted as markup. If you're assigning in any other way, you
            //// should be aware of possible script injection concerns.
            //li.textContent = `${user} says ${message}`;
        });

        connection.start().then(function () {
            console.log("=================\n connection.start()");
            $("#ProgressMain").show();
            //document.getElementById("sendButton").disabled = false;
        }).catch(function (err) {
            return console.error(err.toString());
        });


        //document.getElementById("sendButton").addEventListener("click", function (event) {
        //    var user = document.getElementById("userInput").value;
        //    var message = document.getElementById("messageInput").value;
        //    connection.invoke("SendMessage", user, message).catch(function (err) {
        //        return console.error(err.toString());
        //    });
        //    event.preventDefault();
        //});

    });

}
function hideLoading() {
    $(':input[type="submit"]').prop('disabled', false);
    $("#fullscreenloading")?.hide();
}

function InitMainTable() {
    $('#mainTable').DataTable({
        dom: 'Bfrtip',
        buttons: [
            'copy', 'csv', 'excel', 'pdf', 'print'
        ],
        'autoWidth': true,
        'pageLength': 20,
        'autoHeight': true,
        //"bPaginate": false,
        //info: false,
        searchHighlight: true,
    });
}