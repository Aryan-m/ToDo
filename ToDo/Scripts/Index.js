$(document).ready(
    // makes an ajax request to get all to do items for current user
    function () {
        $.ajax
            (
            {
                url: 'ToDoes/ToDoList',
                error: function () {
                    $('#ToDoTable').empty();
                    $("#hideMe").html("Could Not Load...");
                    alert("Cannot get data from server!");
                }
            }
            ).done(function (data) {
                $("#hideMe").empty();
                $('#ToDoTable').html(data);
            }
            );
    }
);