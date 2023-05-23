function user_register() {
    
    var user_data = new FormData();
    user_data.append('FirstName', $('#firstName').val());
    user_data.append('LastName', $('#lastName').val());
    user_data.append('Email', $('#email').val());
    user_data.append('MobileNumber', $('#phoneNumber').val());
    user_data.append('Password', $('#password').val());
    
    $.ajax({
        url: '/Home/AddUsers',
        method: 'POST',
        data: user_data,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success) {
                window.location.href = response.redirectUrl;
                toastr.success("Registered successfully");
            }
        },
        error: function (request, error) {
            console.log(error);
        }
    })
}