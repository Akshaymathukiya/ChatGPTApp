var v1 = false;
var v2 = false;
var v3 = false;
var v4 = false;
var v5 = false;
var v6 = false;
function user_register() {
    verifyUser();
    if (v1 && v2 && v3 && v4 && v5 && v6) {
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
}

function verifyUser() {
  
    $(".error").text("");
    var firstName = $("#firstName").val();
    var lastName = $("#lastName").val();
    var email = $("#email").val();
    var mobileNumber = $("#phoneNumber").val();
    var password = $("#password").val();
    var confirmPassword = $("#confirmPassword").val();

    var nameRegex = /^[a-zA-Z ]+$/;
    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    var mobileRegex = /^\d{10}$/;
    var passwordRegex = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$/;

    // Validate firstName
    if (firstName === "") {
        $("#firstNameError").text("First Name is required.");
        v1 = false;
    } else if (!nameRegex.test(firstName)) {
        $("#firstNameError").text("Invalid First Name.");
        v1 = false;
    } else {
        v1 = true;
    }

    // Validate lastName
    if (lastName === "") {
        $("#lastNameError").text("Last Name is required.");
        v2 = false;
    } else if (!nameRegex.test(lastName)) {
        $("#lastNameError").text("Invalid Last Name.");
        v2 = false;
    } else {
        v2 = true;
    }

    // Validate email
    if (email === "") {
        $("#emailError").text("Email is required.");
        v3 = false;
    } else if (!emailRegex.test(email)) {
        $("#emailError").text("Invalid Email.");
        v3 = false;
    } else {
        v3 = true;
    }

    // Validate mobileNumber
    if (mobileNumber === "") {
        $("#mobileNumberError").text("Mobile Number is required.");
        v4 = false;
    } else if (!mobileRegex.test(mobileNumber)) {
        $("#mobileNumberError").text("Invalid Mobile Number.");
        v4 = false;
    } else {
        v4 = true;
    }

    // Validate password
    if (password === "") {
        $("#passwordError").text("Password is required.");
        v5 = false;
    } else if (!passwordRegex.test(password)) {
        $("#passwordError").text("Password must be at least 8 characters long and contain at least one letter and one digit.");
        v5 = false;
    } else {
        v5 = true;
    }

    // Validate confirmPassword
    if (confirmPassword === "") {
        $("#confirmPasswordError").text("Confirm Password is required.");
        v6 = false;
    } else if (confirmPassword !== password) {
        $("#confirmPasswordError").text("Passwords do not match.");
        v6 = false;
    } else{
        v6 = true;
    }
}