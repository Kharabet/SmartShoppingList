$(function () {
    $(".bind-rfid").on("click", function (e) {
        e.preventDefault();
        var userProductId = $(e.currentTarget).data("user-product-id");
        var data = {
            userProductId: userProductId,

        };
        $.post("api/bind-unassigned-rfid-to-user-product", data).done(function (response) {
            if (response.success === true) {
                $.alert(response.message);
                document.location.reload(true);
            } else {
                $.alert(response.message);
            }

        });
    });
});