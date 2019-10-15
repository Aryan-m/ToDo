$(document).ready(
    function () {
        // Set up antiforgery token
        var token = $('[name="__RequestVerificationToken"]').val();

        // Ajax request to change 'Checked' field of a to do item
        $('#editor').val('');
        $(".Is_Done").change(function () {
            var id = $(this).attr('id');
            var val = $(this).prop('checked');
            $.ajax
                (
                {
                    url: 'ToDoes/Edit_Checked',
                        data: { __RequestVerificationToken: token, id: id, val: val },
                    type: 'POST',
                    error: function () {
                        alert('Cannot get data from server!');
                    }
                }
                ).done(function (data) {
                    $("#hideMe").empty();
                    $('#ToDoTable').html(data);
                }
                );
        }
        );

        // Ajax request to change 'Important' field of a to do item
        $(".importantIcon").click(function () {
            var id = $(this).attr('data-importantID');
            var val = ($(this).attr('data-important') == 'True') ? false : true;
            function Important_Ajax() {
                $.ajax
                    (
                    {
                        url: 'ToDoes/Edit_Important',
                            data: { __RequestVerificationToken: token, id: id, val: val },
                        type: 'POST',
                        error: function () {
                            alert('Cannot get data from server!');
                        }
                    }
                    ).done(function (data) {
                        $("#hideMe").empty();
                        $('#ToDoTable').html(data);
                    }
                    );
            }

            // Animations for background color change if item is marked important
            if ($(this).attr('data-important') == 'True') {
                $('.ToDoRow[data-taskid=' + id + ']').animate({ backgroundColor: "white" }, "fast",
                    Important_Ajax);
            }
            else {
                $('.ToDoRow[data-taskid=' + id + ']').animate({ backgroundColor: "yellow" }, "fast",
                    Important_Ajax);
            }
        }
        );

        // delete a to do item with a fade animation and make AJAX request to controller
        $('.trash').click(function () {
            var id = $(this).attr('data-taskid'); // id of the selected item in database
            $('.ToDoRow[data-taskid=' + id + ']').animate({ opacity: 0, backgroundColor: "#aa0000" }, "fast",
                function () {
                    $.ajax
                        ({
                            url: 'ToDoes/Delete',
                            data: { __RequestVerificationToken: token, id: id },
                            type: 'POST',
                            error: function () {
                                alert('Cannot access the server!');
                            }
                        }).done(function (data) {
                            $("#hideMe").empty();
                            $('#ToDoTable').html(data);
                        });
                });
        }
        );

        // jQuery UI functionality to drag rows around and make an AJAX request to save changes to database
        $('tbody').sortable(
            {
                update: function (event, id) {
                    var id_list = '';
                    $('tbody').find('.ToDoRow').each(function () {
                        id_list = id_list + $(this).attr('data-taskid') + ',';
                    });
                    $.ajax
                        ({
                            url: 'ToDoes/Edit_Order',
                            data: { __RequestVerificationToken: token, id_list: id_list },
                            type: 'POST',
                            error: function () {
                                alert('Cannot get data from server!');
                            }
                        }).done(function (data) {
                            $("#hideMe").empty();
                            $('#ToDoTable').html(data);
                        });
                }
            }
        );

        // add proper style class based on the isDone field of a to do list
        $('.ToDoRow').each(function () {
            if ($(this).attr('data-isDone') == 'True') {
                $(this).addClass('doneRow');
            }
            else if ($(this).attr('data-isImportant') == 'True') {
                $(this).addClass('importantRow');
            }
        }
        );
    }
);