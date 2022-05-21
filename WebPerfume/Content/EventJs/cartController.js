var cart = {
    init: function () {
        cart.registerEvent();
    },
    registerEvent: function () {                

        $('#btnDeleteAll').off('click').on('click', function () {
            $.ajax({
                url: '/Cart/DeleteAll',
                dataType: 'json',
                type: 'POST',
                success: function (res) {
                    if (res.status == true) {
                        window.location.href = "/Cart";
                    }

                }
            })
        });
    }
}
cart.init();