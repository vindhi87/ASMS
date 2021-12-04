//----------------------Item---------------

$("#pusort_des").keyup(function () {
    var name = $('#pusort_des').val();

    $.ajax({
        url: "https://localhost:44339/invoice/getbynameitem/",
        method: "POST",
        data: { ItemName: name },
        dataType: "json",
        success: function (data) {

            $('#multiple').find('option').remove();
            $("#multiple").width(250);
            $.each(data, function (key, value) {
                $("#multiple").append(new Option(value.ItemName, value.ItemID));
            });

        },
        error: function () {
            console.log('Error');
        }
    });//end of ajax
});
//secound function to load data to table 
$("#multiple").change(function () {
    var ID = $('#multiple').val();

    $.ajax({
        //url: vbase_url + "inventory/item_details/" + ID,
        url: "https://localhost:44339/invoice/getbyiditem/",
        method: "POST",
        data: { ItemID: ID },
        dataType: "json",
        success: function (data) {

            $.each(data, function (key, value) {
                $('#type').val("i");
                $('#itemid').val(value.ItemID);
                $('#itemname').val(value.ItemName);
                $('#itemprice').val(value.ItemPrice);
                $('#itemunit').val(value.UnitID);
                $('#quantity').val(1);
                $('#total').val(value.ItemPrice);
            });

        },
        error: function () {
            console.log('Error');
        }
    });//end of ajax
});


//----------------------Service--------------

$("#pusort_des1").keyup(function () {
    var name = $('#pusort_des1').val();

    $.ajax({
        url: "https://localhost:44339/invoice/getbynameservice/",
        method: "POST",
        data: { ServiceName: name },
        dataType: "json",
        success: function (data) {

            $('#multiple1').find('option').remove();
            $("#multiple1").width(250);

            $.each(data, function (key, value) {
                $("#multiple1").append(new Option(value.ServiceName, value.ServiceID));
            });

        },
        error: function () {
            console.log('Error');
        }
    });//end of ajax
});
//secound function to load data to table 
$("#multiple1").change(function () {
    var ID = $('#multiple1').val();

    $.ajax({
        //url: vbase_url + "inventory/item_details/" + ID,
        url: "https://localhost:44339/invoice/getbyidservice/",
        method: "POST",
        data: { ServiceID: ID },
        dataType: "json",
        success: function (data) {

            $.each(data, function (key, value) {
                $('#type').val("s");
                $('#itemid').val(value.ServiceID);
                $('#itemname').val(value.ServiceName);
                $('#itemprice').val(value.ServicePrice);
                $('#itemunit').val("1");
                $('#quantity').val(1);
                $('#total').val(value.ServicePrice);
            });

        },
        error: function () {
            console.log('Error');
        }
    });//end of ajax
});

    //------------------Vehicle-------------------------

 $("#pusort_des2").keyup(function () {
            var name = $('#pusort_des2').val();

            $.ajax({
                url: "https://localhost:44339/invoice/getbynamevehicle/",
                method: "POST",
                data: { VehicleNumber: name },
                dataType: "json",
                success: function (data) {

                    $('#multiple2').find('option').remove();
                    $("#multiple2").width(250);

                    $.each(data, function (key, value) {
                        $("#multiple2").append(new Option(value.VehicleNumber, value.VehicleID));
                    });

                },
                error: function () {
                    console.log('Error');
                }
            });//end of ajax
   });

//secound function to load data to table 
$("#multiple2").change(function () {
    var ID = $('#multiple2').val();

    $.ajax({
        //url: vbase_url + "inventory/item_details/" + ID,
        url: "https://localhost:44339/invoice/getbyidvehicle/",
        method: "POST",
        data: { VehicleID: ID },
        dataType: "json",
        success: function (data) {

            // $.each(data, function (key, value) {
            $('#custname').val(data[0].Myuser.FirstName);
            $('#number').val(data[0].Myvehicle.VehicleNumber);
            $('#cid').val(data[0].Myuser.UserID);
            $('#vehiid').val(data[0].Myvehicle.VehicleID);
               
           // });

        },
        error: function () {
            console.log('Error');
        }
    });//end of ajax
});

//test for quantity
$("#quantity").keyup(function () {

    var num1 = $('#quantity').val();
    var num2 = $('#itemprice').val();
    $('#total').val(num1 * num2)
    });
//test quantity end
