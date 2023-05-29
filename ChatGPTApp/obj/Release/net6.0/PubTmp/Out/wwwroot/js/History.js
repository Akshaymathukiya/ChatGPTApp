
function delete_history(id) {
    console.log(id);
    $.ajax({
        url: '/Home/delete_history',
        method: 'POST',
        data: {
            id: id
        },
        success: function (data) {
            console.log(data);
            if (data) {
                toastr.success("Data deleted successfully");
                $("#history_container").html(data);
            } else {
                toastr.error("Data not deleted");
            }
        },
        error: function (request, error) {
            console.log(error);
        }
    })
}